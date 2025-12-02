using System.Globalization;
using System.Text.RegularExpressions;
using RARPEditor.Models;

#nullable enable

namespace RARPEditor.Parsers
{
    public static class RichPresenceParser
    {
        public static bool TryParseUInt(string input, out uint result)
        {
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return uint.TryParse(input.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
            }
            return uint.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        public static bool ParseKeyString(string keyString, out uint startValue, out uint? endValue)
        {
            startValue = 0;
            endValue = null;

            var rangeParts = keyString.Split('-');
            if (rangeParts.Length == 2)
            {
                if (TryParseUInt(rangeParts[0], out uint start) && TryParseUInt(rangeParts[1], out uint end) && start <= end)
                {
                    startValue = start;
                    endValue = end;
                    return true;
                }
            }
            else if (TryParseUInt(keyString, out uint keyValue))
            {
                startValue = keyValue;
                endValue = null;
                return true;
            }

            return false;
        }

        public static RichPresenceScript Parse(string script)
        {
            var parsedScript = new RichPresenceScript();
            if (string.IsNullOrEmpty(script))
                return parsedScript;

            var lines = script.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string? currentSection = null;
            RichPresenceLookup? currentLookup = null;
            string? currentComment = null;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

                if (trimmedLine.StartsWith("//"))
                {
                    currentComment = trimmedLine.Substring(2).Trim();
                    continue;
                }

                if (trimmedLine.StartsWith("Lookup:"))
                {
                    currentSection = "Lookup";
                    var name = trimmedLine.Substring(7).Trim();
                    currentLookup = new RichPresenceLookup { Name = name };
                    parsedScript.Lookups.Add(currentLookup);
                    currentComment = null; // Reset comment for new lookup
                    continue;
                }
                if (trimmedLine.StartsWith("Format:"))
                {
                    currentSection = "Format";
                    var name = trimmedLine.Substring(7).Trim();
                    currentLookup = parsedScript.Lookups.FirstOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (currentLookup == null)
                    {
                        currentLookup = new RichPresenceLookup { Name = name };
                        parsedScript.Lookups.Add(currentLookup);
                    }
                    currentComment = null;
                    continue;
                }
                if (trimmedLine.StartsWith("Display:"))
                {
                    currentSection = "Display";
                    currentLookup = null;
                    currentComment = null;
                    continue;
                }

                switch (currentSection)
                {
                    case "Lookup":
                        if (currentLookup != null)
                        {
                            var parts = trimmedLine.Split(new[] { '=' }, 2);
                            if (parts.Length == 2)
                            {
                                if (parts[0].Trim() == "*")
                                {
                                    currentLookup.Default = parts[1].Trim();
                                }
                                else
                                {
                                    string combinedKeyString = parts[0].Trim();
                                    string valueString = parts[1].Trim();

                                    // Handle comma-separated keys for the same value (e.g., "0x1,0x2=Value").
                                    // This ensures that Lookups using this syntax are parsed as Lookups (with entries)
                                    // rather than falling back to being classified as Formatters.
                                    var keys = combinedKeyString.Split(',');

                                    foreach (var rawKey in keys)
                                    {
                                        string keyString = rawKey.Trim();
                                        if (ParseKeyString(keyString, out uint start, out uint? end))
                                        {
                                            currentLookup.Entries.Add(new LookupEntry
                                            {
                                                KeyString = keyString,
                                                Value = valueString,
                                                Comment = currentComment,
                                                KeyValue = start,
                                                KeyValueEnd = end
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case "Format":
                        if (currentLookup != null && trimmedLine.StartsWith("FormatType="))
                        {
                            currentLookup.Format = trimmedLine.Substring(11).Trim();
                        }
                        break;

                    case "Display":
                        var displayString = new RichPresenceDisplayString();
                        var displayContent = trimmedLine;

                        if (trimmedLine.StartsWith("?"))
                        {
                            var qParts = trimmedLine.Split(new[] { '?' }, 3);
                            if (qParts.Length == 3)
                            {
                                displayString.Condition = qParts[1];
                                displayContent = qParts[2];
                            }
                            else
                            {
                                displayString.Condition = qParts[1];
                                displayContent = "";
                            }
                        }

                        var regex = new Regex(@"(@([^()]+)\(([^)]*)\))");
                        var matches = regex.Matches(displayContent);
                        int lastIndex = 0;

                        foreach (Match match in matches.Cast<Match>())
                        {
                            if (match.Index > lastIndex)
                            {
                                displayString.Parts.Add(new RichPresenceDisplayPart { IsMacro = false, Text = displayContent.Substring(lastIndex, match.Index - lastIndex) });
                            }
                            displayString.Parts.Add(new RichPresenceDisplayPart
                            {
                                IsMacro = true,
                                Text = match.Groups[2].Value,
                                Parameter = match.Groups[3].Value
                            });
                            lastIndex = match.Index + match.Length;
                        }

                        if (lastIndex < displayContent.Length)
                        {
                            displayString.Parts.Add(new RichPresenceDisplayPart { IsMacro = false, Text = displayContent.Substring(lastIndex) });
                        }

                        parsedScript.DisplayStrings.Add(displayString);
                        break;
                }
            }

            return parsedScript;
        }

        public static string Serialize(RichPresenceScript script)
        {
            var sb = new System.Text.StringBuilder();

            // Correctly separate lookups (with entries) from formatters (without entries).
            var formatLookups = script.Lookups.Where(l => !l.Entries.Any() && l.Default == null).ToList();
            var standardLookups = script.Lookups.Except(formatLookups).ToList();

            foreach (var lookup in standardLookups)
            {
                sb.AppendLine($"Lookup:{lookup.Name}");

                var groupedEntries = lookup.Entries
                                           .GroupBy(e => e.Comment)
                                           .OrderBy(g => g.Key == null) // Uncategorized last
                                           .ThenBy(g => g.Key);

                foreach (var group in groupedEntries)
                {
                    if (!string.IsNullOrEmpty(group.Key))
                    {
                        sb.AppendLine($"// {group.Key}");
                    }
                    // Order the entries within each group by their key value for consistent serialization.
                    foreach (var entry in group.OrderBy(e => e.KeyValue))
                    {
                        sb.AppendLine($"{entry.KeyString}={entry.Value}");
                    }
                }

                if (lookup.Default != null)
                {
                    sb.AppendLine($"*={lookup.Default}");
                }
                sb.AppendLine();
            }

            foreach (var lookup in formatLookups)
            {
                // Skip serialization if the formatter name is a built-in macro name.
                // These are reserved and handled natively by RA integration.
                // We check the Name (e.g. "Number") against the Keys of BuiltInMacros.
                if (RichPresenceLookup.BuiltInMacros.ContainsKey(lookup.Name))
                {
                    continue;
                }

                sb.AppendLine($"Format:{lookup.Name}");
                sb.AppendLine($"FormatType={lookup.Format}");
                sb.AppendLine();
            }

            sb.AppendLine("Display:");
            foreach (var displayString in script.DisplayStrings)
            {
                string line = "";
                if (!displayString.IsDefault && !string.IsNullOrEmpty(displayString.Condition))
                {
                    line += $"?{displayString.Condition}?";
                }

                foreach (var part in displayString.Parts)
                {
                    if (part.IsMacro)
                    {
                        line += $"@{part.Text}({part.Parameter})";
                    }
                    else
                    {
                        line += part.Text;
                    }
                }
                sb.AppendLine(line);
            }

            return sb.ToString();
        }
    }
}
#nullable disable