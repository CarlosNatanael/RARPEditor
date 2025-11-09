#nullable enable

using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace RARPEditor.Models
{
    public class LookupEntry
    {
        public uint KeyValue { get; set; }
        public uint? KeyValueEnd { get; set; }
        public string KeyString { get; set; } = "";
        public string Value { get; set; } = "";
        public string? Comment { get; set; }

        public LookupEntry Clone() => (LookupEntry)this.MemberwiseClone();
    }

    public class RichPresenceLookup
    {
        [Category("General")]
        [Description("The name of the lookup, used in macros.")]
        public string Name { get; set; } = "";

        [Browsable(false)]
        public string Format { get; set; } = "VALUE";

        [Browsable(false)]
        public List<LookupEntry> Entries { get; set; } = new List<LookupEntry>();

        [Category("General")]
        [Description("The fallback value to use if no key matches. Corresponds to the '*' entry.")]
        public string? Default { get; set; }

        public RichPresenceLookup Clone()
        {
            var clone = new RichPresenceLookup
            {
                Name = this.Name,
                Format = this.Format,
                Default = this.Default,
                Entries = new List<LookupEntry>(this.Entries.Select(e => e.Clone()))
            };
            return clone;
        }

        [Browsable(false)]
        public IEnumerable<uint> Keys => Entries.Select(e => e.KeyValue);

        public bool TryGetValue(uint key, out string? value)
        {
            foreach (var entry in Entries)
            {
                // If it's a range
                if (entry.KeyValueEnd.HasValue)
                {
                    if (key >= entry.KeyValue && key <= entry.KeyValueEnd.Value)
                    {
                        value = entry.Value;
                        return true;
                    }
                }
                // If it's a single value
                else
                {
                    if (entry.KeyValue == key)
                    {
                        value = entry.Value;
                        return true;
                    }
                }
            }
            value = null;
            return false;
        }
    }

    public class RichPresenceDisplayPart
    {
        public bool IsMacro { get; set; }
        public string Text { get; set; } = "";
        public string Parameter { get; set; } = "";

        public RichPresenceDisplayPart Clone()
        {
            return (RichPresenceDisplayPart)this.MemberwiseClone();
        }
    }

    public class RichPresenceDisplayString
    {
        [Browsable(false)]
        public Guid InternalId { get; } = Guid.NewGuid();

        [Browsable(false)]
        public bool IsDefault { get; set; }

        [Category("Logic")]
        [Description("The condition that must be true for this string to be displayed.")]
        public string Condition { get; set; } = "";

        [Browsable(false)]
        public List<RichPresenceDisplayPart> Parts { get; set; } = new List<RichPresenceDisplayPart>();

        public override string ToString()
        {
            var displayText = string.Join("", Parts.Select(p => p.IsMacro ? $"{{{p.Text}}}" : p.Text));
            if (displayText.Length > 50)
            {
                displayText = displayText.Substring(0, 47) + "...";
            }
            return displayText;
        }

        private static string FormatStaticValue(long value, string format)
        {
            return format.ToUpper() switch
            {
                "SCORE" => value.ToString("D6"),
                "FRAMES" => "0:00.00",
                "MILLISECS" => "0:00.00",
                "SECS" => "0:00",
                "MINUTES" => "0h00",
                "SECS_AS_MINS" => "0h00:00",
                "UNSIGNED" => ((uint)value).ToString(),
                "TENS" => (value * 10).ToString(),
                "HUNDREDS" => (value * 100).ToString(),
                "THOUSANDS" => (value * 1000).ToString(),
                "FIXED1" => (value / 10.0).ToString("F1"),
                "FIXED2" => (value / 100.0).ToString("F2"),
                "FIXED3" => (value / 1000.0).ToString("F3"),
                "FLOAT1" => "0.0",
                "FLOAT2" => "0.00",
                "FLOAT3" => "0.000",
                "FLOAT4" => "0.0000",
                "FLOAT5" => "0.00000",
                "FLOAT6" => "0.000000",
                "VALUE" or _ => value.ToString(),
            };
        }

        public string ToFormattedString(RichPresenceScript script)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var part in Parts)
            {
                if (part.IsMacro)
                {
                    var lookup = script.Lookups.FirstOrDefault(l => l.Name.Equals(part.Text, StringComparison.OrdinalIgnoreCase));
                    if (lookup != null)
                    {
                        if (lookup.Entries.Any())
                        {
                            sb.Append(lookup.Default ?? "");
                        }
                        else
                        {
                            sb.Append(FormatStaticValue(0, lookup.Format));
                        }
                    }
                    else
                    {
                        // Revert to using curly braces for display.
                        sb.Append($"{{{part.Text}}}");
                    }
                }
                else
                {
                    sb.Append(part.Text);
                }
            }

            // Remove newlines and trim for a clean one-line representation in the tree.
            var displayText = Regex.Replace(sb.ToString(), @"\s+", " ").Trim();

            if (displayText.Length > 50)
            {
                displayText = displayText.Substring(0, 47) + "...";
            }

            string prefix = IsDefault ? "[Default] " : "";
            return $"{prefix}{displayText}";
        }

        public RichPresenceDisplayString Clone()
        {
            // Use MemberwiseClone for a shallow copy of properties.
            var clone = (RichPresenceDisplayString)this.MemberwiseClone();

            // Manually create a deep copy of the Parts list.
            clone.Parts = new List<RichPresenceDisplayPart>();
            foreach (var part in this.Parts)
            {
                clone.Parts.Add(part.Clone());
            }
            return clone;
        }
    }

    public class RichPresenceScript
    {
        public List<RichPresenceLookup> Lookups { get; } = new List<RichPresenceLookup>();
        public List<RichPresenceDisplayString> DisplayStrings { get; } = new List<RichPresenceDisplayString>();

        public void Clear()
        {
            Lookups.Clear();
            DisplayStrings.Clear();
        }

        public RichPresenceScript Clone()
        {
            var clone = new RichPresenceScript();
            foreach (var lookup in this.Lookups)
            {
                clone.Lookups.Add(lookup.Clone());
            }
            foreach (var ds in this.DisplayStrings)
            {
                clone.DisplayStrings.Add(ds.Clone());
            }
            return clone;
        }
    }
}
#nullable disable