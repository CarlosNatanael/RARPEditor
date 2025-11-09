using RARPEditor.Models;
using RARPEditor.Parsers;
using System.Collections.Generic;
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
                    var conditionGroups = AchievementParser.ParseAchievementTrigger(displayString.Condition);
                    var conditions = conditionGroups.SelectMany(g => g.Conditions).ToList();

                    ValidateRedundantReads(conditions, results, "the display condition");

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

                if (!string.IsNullOrWhiteSpace(part.Parameter))
                {
                    var paramConditionGroups = AchievementParser.ParseAchievementTrigger(part.Parameter, '$');

                    for (int i = 0; i < paramConditionGroups.Count; i++)
                    {
                        var group = paramConditionGroups[i];
                        var paramConditions = group.Conditions;
                        string groupIdentifier = $"Value Group {i + 1}";

                        ValidateRedundantReads(paramConditions, results, $"macro '{{{part.Text}}}' ({groupIdentifier})");
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

        private static void ValidateRedundantReads(List<AchievementCondition> conditions, List<ValidationResult> results, string context)
        {
            var seenBlocks = new Dictionary<string, int>();

            for (int i = 0; i < conditions.Count; i++)
            {
                var currentCondition = conditions[i];
                var block = new List<AchievementCondition>();
                int endOfBlockIndex = i;

                // A logical block is a sequence of 'Add Address' lines followed by ONE other line that uses the resulting pointer.
                if (currentCondition.Flag == "Add Address")
                {
                    block.Add(currentCondition);
                    int j = i + 1;
                    // Consume the rest of the pointer chain offsets.
                    while (j < conditions.Count && conditions[j].Flag == "Add Address")
                    {
                        block.Add(conditions[j]);
                        j++;
                    }
                    // Now, consume the final line that USES the pointer.
                    // This could be Add Source, Sub Source, or a comparison line.
                    if (j < conditions.Count)
                    {
                        block.Add(conditions[j]);
                        endOfBlockIndex = j;
                    }
                }
                else // It's a standalone condition, so the block is just this one line.
                {
                    block.Add(currentCondition);
                }

                // Generate a unique key for the entire logical block, including flags, operands, types, and comparisons.
                string blockKey = string.Join(";", block.Select(c =>
                    $"{c.Flag}|{c.LeftOperand.Type}|{c.LeftOperand.Size}|{c.LeftOperand.Value}|" +
                    $"{c.Operator}|{c.RightOperand.Type}|{c.RightOperand.Size}|{c.RightOperand.Value}"
                ));

                int startLine = block.First().ID;

                if (seenBlocks.TryGetValue(blockKey, out int firstStartLine))
                {
                    string currentBlockDescription;
                    if (block.Count > 1 && block.First().Flag == "Add Address")
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

                // Advance the main loop past the block we just processed.
                i = endOfBlockIndex;
            }
        }

        // Fix: Logic rewritten to correctly identify calculation endpoints, ignoring intermediate arithmetic steps.
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