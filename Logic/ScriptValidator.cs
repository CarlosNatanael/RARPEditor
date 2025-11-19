#nullable enable
using RARPEditor.Models;
using RARPEditor.Parsers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RARPEditor.Logic
{
    public enum ValidationType
    {
        Error,
        Warning,
        Info,
        Success
    }

    public class ValidationResult
    {
        public ValidationType Type { get; set; }
        public string Message { get; set; } = "";
    }

    public static class ScriptValidator
    {
        private static readonly HashSet<string> ComparisonOperators = new() { "=", "!=", "<", "<=", ">", ">=" };
        private static readonly HashSet<string> ArithmeticFlags = new() { "Add Address", "Add Source", "Sub Source", "Remember" };
        private static readonly HashSet<string> ChainFlags = new() { "Add Address", "Add Source", "Sub Source" };

        // Types that represent reading from memory (and thus impose a size limit)
        private static readonly HashSet<string> MemoryTypes = new() { "Mem", "Delta", "Prior", "BCD", "Inverted" };

        public static List<ValidationResult> Validate(RichPresenceLookup lookup, RichPresenceScript script)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(lookup.Name) || lookup.Name.Contains(" "))
            {
                results.Add(new ValidationResult { Type = ValidationType.Error, Message = "Name should not be empty or contain spaces." });
            }

            var caseCollisions = script.Lookups
                .Where(l => l != lookup &&
                            l.Name.Equals(lookup.Name, System.StringComparison.OrdinalIgnoreCase) &&
                            !l.Name.Equals(lookup.Name, System.StringComparison.Ordinal))
                .ToList();

            if (caseCollisions.Any())
            {
                string collisionNames = string.Join(", ", caseCollisions.Select(c => $"'{c.Name}'"));
                results.Add(new ValidationResult { Type = ValidationType.Warning, Message = $"Potential issue: Another lookup/formatter exists with a similar name but different casing: {collisionNames}. Macro names are case-sensitive." });
            }

            var entries = lookup.Entries;
            for (int i = 0; i < entries.Count; i++)
            {
                var entryA = entries[i];
                uint startA = entryA.KeyValue;
                uint endA = entryA.KeyValueEnd ?? startA;

                for (int j = i + 1; j < entries.Count; j++)
                {
                    var entryB = entries[j];
                    uint startB = entryB.KeyValue;
                    uint endB = entryB.KeyValueEnd ?? startB;

                    if (startA <= endB && endA >= startB)
                    {
                        results.Add(new ValidationResult
                        {
                            Type = ValidationType.Warning,
                            Message = $"Key '{entryA.KeyString}' overlaps with key '{entryB.KeyString}'. The first entry in the list will take precedence."
                        });
                    }
                }
            }

            // Add validation to check for duplicate formatter types.
            bool isFormatter = !lookup.Entries.Any() && lookup.Default == null;
            if (isFormatter)
            {
                var duplicateFormatters = script.Lookups
                    .Where(l => l != lookup &&
                                !l.Entries.Any() && l.Default == null &&
                                l.Format.Equals(lookup.Format, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (duplicateFormatters.Any())
                {
                    string formatterNames = string.Join(", ", duplicateFormatters.Select(f => $"'{f.Name}'"));
                    results.Add(new ValidationResult
                    {
                        Type = ValidationType.Warning,
                        Message = $"The format type '{lookup.Format}' is already used by another formatter: {formatterNames}. This may be redundant."
                    });
                }
            }

            // Validate that a Lookup (Format=VALUE with explicit Default) is not empty.
            if (lookup.Format == "VALUE" && lookup.Default != null)
            {
                if (!lookup.Entries.Any() && string.IsNullOrEmpty(lookup.Default))
                {
                    results.Add(new ValidationResult
                    {
                        Type = ValidationType.Error,
                        Message = "Lookup is empty. A Lookup must have at least one entry or a non-empty default value."
                    });
                }
            }

            // Check if the lookup/formatter is unused in any display string.
            bool isUsed = script.DisplayStrings
                .SelectMany(ds => ds.Parts)
                .Any(p => p.IsMacro && p.Text.Equals(lookup.Name, System.StringComparison.Ordinal));

            if (!isUsed)
            {
                results.Add(new ValidationResult
                {
                    Type = ValidationType.Info,
                    Message = "This lookup is not currently used in any display string."
                });
            }

            return results;
        }

        public static List<ValidationResult> Validate(RichPresenceDisplayString displayString, RichPresenceScript script)
        {
            var results = new List<ValidationResult>();

            if (!displayString.IsDefault)
            {
                if (string.IsNullOrWhiteSpace(displayString.Condition))
                {
                    results.Add(new ValidationResult { Type = ValidationType.Error, Message = "Conditional string should have a condition." });
                }
                else
                {
                    var conditionGroups = AchievementParser.ParseAchievementTrigger(displayString.Condition, 'S');
                    var conditions = conditionGroups.SelectMany(g => g.Conditions).ToList();

                    ValidateRedundantReads(conditions, results, "the display condition");
                    ValidateConditionValues(conditions, results, "the display condition");

                    var warningFlags = new HashSet<string> { "Measured", "Measured%", "MeasuredIf", "Trigger" };

                    foreach (var condition in conditions)
                    {
                        // Check for unnecessary flags that don't contribute to a true/false condition (Warning).
                        if (warningFlags.Contains(condition.Flag))
                        {
                            results.Add(new ValidationResult { Type = ValidationType.Warning, Message = $"The flag '{condition.Flag}' on line {condition.ID} is not typically used in a display condition and may have no effect." });
                        }

                        // Check for missing comparison operators on lines that require them (Error).
                        if (!ArithmeticFlags.Contains(condition.Flag))
                        {
                            if (!ComparisonOperators.Contains(condition.Operator))
                            {
                                results.Add(new ValidationResult { Type = ValidationType.Error, Message = $"Condition line {condition.ID} must have a comparison operator (e.g., '=', '!=', '<')." });
                            }
                        }
                    }
                }
            }

            foreach (var part in displayString.Parts.Where(p => p.IsMacro))
            {
                bool exactMatchExists = script.Lookups.Any(l => l.Name.Equals(part.Text, System.StringComparison.Ordinal));

                if (!exactMatchExists)
                {
                    var caseInsensitiveMatch = script.Lookups.FirstOrDefault(l => l.Name.Equals(part.Text, System.StringComparison.OrdinalIgnoreCase));
                    if (caseInsensitiveMatch != null)
                    {
                        results.Add(new ValidationResult { Type = ValidationType.Error, Message = $"Macro '{{{part.Text}}}' is invalid. Did you mean '{{{caseInsensitiveMatch.Name}}}'? Macro names are case-sensitive." });
                    }
                    else
                    {
                        results.Add(new ValidationResult { Type = ValidationType.Error, Message = $"Macro '{{{part.Text}}}' does not point to any defined Lookup or Formatter." });
                    }
                }

                if (string.IsNullOrWhiteSpace(part.Parameter))
                {
                    results.Add(new ValidationResult { Type = ValidationType.Error, Message = $"Macro '{{{part.Text}}}' has no logic defined. Please select it in the dropdown and add logic." });
                }
                else
                {
                    var paramConditionGroups = AchievementParser.ParseAchievementTrigger(part.Parameter, '$');

                    for (int i = 0; i < paramConditionGroups.Count; i++)
                    {
                        var group = paramConditionGroups[i];
                        var paramConditions = group.Conditions;
                        string groupIdentifier = $"Value Group {i + 1}";

                        ValidateRedundantReads(paramConditions, results, $"macro '{{{part.Text}}}' ({groupIdentifier})");
                        ValidateConditionValues(paramConditions, results, $"macro '{{{part.Text}}}' ({groupIdentifier})");
                        ValidateMeasuredFlagRequirement(paramConditions, results, part, groupIdentifier);

                        bool isChain = paramConditions.Any(c => ArithmeticFlags.Contains(c.Flag));

                        var conditionalLines = paramConditions.Where(c => ComparisonOperators.Contains(c.Operator)).ToList();
                        var valueProviderLines = paramConditions.Where(c => (c.Flag == "Measured" || c.Flag == "Measured%") && !ComparisonOperators.Contains(c.Operator)).ToList();

                        foreach (var condition in paramConditions.Where(c => (c.Flag == "Measured" || c.Flag == "Measured%") && ComparisonOperators.Contains(c.Operator)))
                        {
                            results.Add(new ValidationResult { Type = ValidationType.Error, Message = $"In macro '{{{part.Text}}}' ({groupIdentifier}), 'Measured' (M:) on line {condition.ID} cannot be combined with a comparison operator." });
                        }

                        if (conditionalLines.Any() && !valueProviderLines.Any())
                        {
                            results.Add(new ValidationResult { Type = ValidationType.Error, Message = $"In macro '{{{part.Text}}}' ({groupIdentifier}), conditional logic exists but is missing a 'Measured' (M:) or 'Measured%' (G:) flag to specify the return value." });
                        }

                        if (valueProviderLines.Any())
                        {
                            foreach (var condition in paramConditions)
                            {
                                if (valueProviderLines.Contains(condition) || ArithmeticFlags.Contains(condition.Flag))
                                    continue;

                                if (!ComparisonOperators.Contains(condition.Operator))
                                {
                                    results.Add(new ValidationResult
                                    {
                                        Type = ValidationType.Error,
                                        Message = $"In macro '{{{part.Text}}}' ({groupIdentifier}), line {condition.ID} is treated as a condition because a 'Measured' value exists. Therefore, it must have a comparison operator (e.g., '=', '!=', '<')."
                                    });
                                }
                            }
                        }

                        if (isChain)
                        {
                            var lastCondition = paramConditions.LastOrDefault();
                            if (lastCondition != null && lastCondition.Flag != "Measured" && lastCondition.Flag != "Measured%")
                            {
                                results.Add(new ValidationResult { Type = ValidationType.Error, Message = $"In macro '{{{part.Text}}}' ({groupIdentifier}), the arithmetic chain does not end with a 'Measured' (M:) or 'Measured%' (G:) flag." });
                            }
                        }

                        foreach (var condition in paramConditions.Where(c => c.Flag == "Trigger"))
                        {
                            results.Add(new ValidationResult { Type = ValidationType.Warning, Message = $"In macro '{{{part.Text}}}' ({groupIdentifier}), the 'Trigger' (T:) flag on line {condition.ID} has no effect here." });
                        }

                        if (valueProviderLines.Count > 1)
                        {
                            results.Add(new ValidationResult { Type = ValidationType.Error, Message = $"In macro '{{{part.Text}}}' ({groupIdentifier}), there cannot be more than one 'Measured' (M: or G:) value." });
                        }

                        bool hasParameterErrors = results.Any(r => r.Type == ValidationType.Error && r.Message.Contains($"'{part.Text}'") && r.Message.Contains(groupIdentifier));
                        if (!hasParameterErrors)
                        {
                            bool hasMeasured = valueProviderLines.Any(c => c.Flag == "Measured");
                            bool hasMeasuredIf = conditionalLines.Any(c => c.Flag == "MeasuredIf");
                            bool hasEmptyFlagCondition = paramConditions.Any(c => string.IsNullOrEmpty(c.Flag) && ComparisonOperators.Contains(c.Operator));

                            if (hasMeasured && hasEmptyFlagCondition && !hasMeasuredIf)
                            {
                                results.Add(new ValidationResult { Type = ValidationType.Info, Message = $"For macro '{{{part.Text}}}' ({groupIdentifier}), consider using 'MeasuredIf' (Q:) for conditions to clarify intent." });
                            }
                        }
                    }
                }
            }

            return results;
        }

        private static void ValidateConditionValues(List<AchievementCondition> conditions, List<ValidationResult> results, string context)
        {
            foreach (var cond in conditions)
            {
                if (cond.LeftOperand.Type == "Float" || cond.RightOperand.Type == "Float")
                    continue;

                if (!ComparisonOperators.Contains(cond.Operator))
                    continue;

                // Case 1: Left is Mem/Delta/etc, Right is Value.
                if (MemoryTypes.Contains(cond.LeftOperand.Type) && cond.RightOperand.Type == "Value")
                {
                    ulong? limit = GetMaxSizeForSize(cond.LeftOperand.Size);
                    if (limit.HasValue && TryParseOperandValue(cond.RightOperand.Value, out ulong rightVal))
                    {
                        if (rightVal > limit.Value)
                        {
                            results.Add(new ValidationResult
                            {
                                Type = ValidationType.Warning,
                                Message = $"In {context} (line {cond.ID}): Comparison value '{cond.RightOperand.Value}' ({rightVal}) exceeds the maximum value for {cond.LeftOperand.Size} ({limit.Value}). This comparison will unlikely behave as expected."
                            });
                        }
                    }
                }
                // Case 2: Right is Mem/Delta/etc, Left is Value.
                else if (MemoryTypes.Contains(cond.RightOperand.Type) && cond.LeftOperand.Type == "Value")
                {
                    ulong? limit = GetMaxSizeForSize(cond.RightOperand.Size);
                    if (limit.HasValue && TryParseOperandValue(cond.LeftOperand.Value, out ulong leftVal))
                    {
                        if (leftVal > limit.Value)
                        {
                            results.Add(new ValidationResult
                            {
                                Type = ValidationType.Warning,
                                Message = $"In {context} (line {cond.ID}): Comparison value '{cond.LeftOperand.Value}' ({leftVal}) exceeds the maximum value for {cond.RightOperand.Size} ({limit.Value}). This comparison will unlikely behave as expected."
                            });
                        }
                    }
                }
            }
        }

        private static bool TryParseOperandValue(string valueStr, out ulong result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(valueStr)) return false;

            // Handle Hex
            if (valueStr.StartsWith("0x", System.StringComparison.OrdinalIgnoreCase))
            {
                string hex = valueStr.Substring(2);
                return ulong.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
            }
            // Handle Decimal
            return ulong.TryParse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        private static ulong? GetMaxSizeForSize(string size)
        {
            switch (size)
            {
                case "Bit0":
                case "Bit1":
                case "Bit2":
                case "Bit3":
                case "Bit4":
                case "Bit5":
                case "Bit6":
                case "Bit7":
                    return 1;

                case "Lower4":
                case "Upper4":
                    return 15; // 0xF

                case "8-bit":
                    return 255; // 0xFF

                case "16-bit":
                case "16-bit BE":
                    return 65535; // 0xFFFF

                case "24-bit":
                case "24-bit BE":
                    return 16777215; // 0xFFFFFF

                case "32-bit":
                case "32-bit BE":
                    return uint.MaxValue; // 0xFFFFFFFF

                case "BitCount":
                    return 8; // 8 bits in a byte

                default:
                    // For Float, MBF32, or empty/unknown sizes, we don't enforce integer limits
                    return null;
            }
        }

        private static void ValidateRedundantReads(List<AchievementCondition> conditions, List<ValidationResult> results, string context)
        {
            var seenBlocks = new Dictionary<string, int>();

            for (int i = 0; i < conditions.Count; i++)
            {
                var currentCondition = conditions[i];
                var block = new List<AchievementCondition>();
                int endOfBlockIndex = i;

                // Treat "Add Address", "Add Source", and "Sub Source" as part of the same logical chain.
                if (ChainFlags.Contains(currentCondition.Flag))
                {
                    block.Add(currentCondition);
                    int j = i + 1;

                    while (j < conditions.Count)
                    {
                        var nextCondition = conditions[j];
                        block.Add(nextCondition);
                        j++;

                        if (!ChainFlags.Contains(nextCondition.Flag))
                        {
                            break;
                        }
                    }
                    endOfBlockIndex = j - 1;
                }
                else
                {
                    block.Add(currentCondition);
                }

                string blockKey = string.Join(";", block.Select(c =>
                    $"{c.Flag}|{c.LeftOperand.Type}|{c.LeftOperand.Size}|{c.LeftOperand.Value}|" +
                    $"{c.Operator}|{c.RightOperand.Type}|{c.RightOperand.Size}|{c.RightOperand.Value}"
                ));

                int startLine = block.First().ID;

                if (seenBlocks.TryGetValue(blockKey, out int firstStartLine))
                {
                    string currentBlockDescription;
                    if (block.Count > 1 && ChainFlags.Contains(block.First().Flag))
                    {
                        currentBlockDescription = $"the logical block from line {startLine} to {block.Last().ID}";
                    }
                    else
                    {
                        currentBlockDescription = $"the condition on line {startLine}";
                    }

                    results.Add(new ValidationResult
                    {
                        Type = ValidationType.Warning,
                        Message = $"In {context}, {currentBlockDescription} is a redundant read of an identical block starting at line {firstStartLine}."
                    });
                }
                else
                {
                    seenBlocks[blockKey] = startLine;
                }

                i = endOfBlockIndex;
            }
        }

        private static void ValidateMeasuredFlagRequirement(List<AchievementCondition> conditions, List<ValidationResult> results, RichPresenceDisplayPart part, string groupIdentifier)
        {
            // A "singleton" is a condition without a comparison operator, which means it produces a value.
            var allSingletons = conditions.Where(c => string.IsNullOrEmpty(c.Operator)).ToList();
            if (allSingletons.Count <= 1)
            {
                // No ambiguity if there's 0 or 1 value-producing line.
                return;
            }

            // A "true endpoint" is a value-producing line that isn't just an intermediate step in a calculation.
            // We filter out arithmetic flags, as they are part of a chain.
            var trueEndpoints = allSingletons.Where(c => !ArithmeticFlags.Contains(c.Flag)).ToList();

            // If the code is just one long arithmetic chain (e.g., I...A...A...), there are no "true" endpoints found above.
            // In this case, the actual endpoint is the very last line of that chain.
            if (!trueEndpoints.Any() && allSingletons.Any())
            {
                trueEndpoints.Add(allSingletons.Last());
            }

            // If there's still more than one endpoint, there is ambiguity.
            if (trueEndpoints.Count > 1)
            {
                bool hasMeasured = trueEndpoints.Any(c => c.Flag == "Measured" || c.Flag == "Measured%");

                if (!hasMeasured)
                {
                    results.Add(new ValidationResult
                    {
                        Type = ValidationType.Error,
                        Message = $"In macro '{{{part.Text}}}' ({groupIdentifier}), multiple memory addresses are provided without a comparison. One must be marked with a 'Measured' (M:) or 'Measured%' (G:) flag to specify the return value."
                    });
                }
            }
        }
    }
}