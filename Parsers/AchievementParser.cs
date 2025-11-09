using RARPEditor.Models;
using System.Globalization;
using System.Text.RegularExpressions;

#nullable enable

namespace RARPEditor.Parsers
{
    public static class AchievementParser
    {
        // Maps single-character flags from the trigger string to their full names.
        private static readonly Dictionary<char, string> FlagMap = new()
        {
            {'R', "Reset If"}, {'P', "Pause If"}, {'A', "Add Source"},
            {'B', "Sub Source"}, {'C', "Add Hits"}, {'D', "Sub Hits"},
            {'I', "Add Address"}, {'N', "AndNext"}, {'O', "OrNext"},
            {'M', "Measured"}, {'Q', "MeasuredIf"}, {'G', "Measured%"},
            {'T', "Trigger"}, {'Z', "ResetNextIf"}, {'K', "Remember"}
        };

        // Maps memory size identifiers to their human-readable format.
        private static readonly Dictionary<char, string> SizeMap = new()
        {
            {' ', "16-bit"},
            {'H', "8-bit"}, {'X', "32-bit"}, {'W', "24-bit"},
            {'M', "Bit0"}, {'N', "Bit1"}, {'O', "Bit2"}, {'P', "Bit3"},
            {'Q', "Bit4"}, {'R', "Bit5"}, {'S', "Bit6"}, {'T', "Bit7"},
            {'L', "Lower4"}, {'U', "Upper4"}, {'K', "BitCount"},
            {'G', "32-bit BE"}, {'I', "16-bit BE"}, {'J', "24-bit BE"}
        };

        private static readonly Dictionary<char, string> FloatSizeMap = new()
        {
            {'F', "Float"}, {'B', "Float BE"}, {'H', "Double32"},
            {'I', "Double32 BE"}, {'M', "MBF32"}, {'L', "MBF32 LE"}
        };

        public static List<AchievementConditionGroup> ParseAchievementTrigger(string trigger, char separator = 'S')
        {
            var conditionSet = new List<AchievementConditionGroup>();
            if (string.IsNullOrEmpty(trigger)) return conditionSet;

            string[] groupStrings;
            if (separator == 'S')
            {
                // 'S' is used for both "Bit6" size (0xS...) and as a separator for Alt groups.
                // To fix this, we temporarily replace legitimate uses of 'S' as a size modifier,
                // perform the split, and then restore it. Case-insensitivity is important.
                string safeTrigger = trigger.Replace("0xS", "0x\u0001").Replace("0xs", "0x\u0001");
                groupStrings = safeTrigger.Split(separator);
            }
            else
            {
                // '$' is not a hex character, so no special handling is needed.
                groupStrings = trigger.Split(separator);
            }


            for (int i = 0; i < groupStrings.Length; i++)
            {
                if (string.IsNullOrEmpty(groupStrings[i])) continue;

                string restoredGroupString = groupStrings[i];
                if (separator == 'S')
                {
                    restoredGroupString = groupStrings[i].Replace('\u0001', 'S');
                }

                var group = new AchievementConditionGroup
                {
                    GroupName = (separator == 'S')
                        ? ((i == 0) ? "Core" : $"Alt {i}")
                        : $"Value Group {i + 1}"
                };

                ParseConditionsIntoGroup(group, restoredGroupString);
                conditionSet.Add(group);
            }
            return conditionSet;
        }

        private static void ParseConditionsIntoGroup(AchievementConditionGroup group, string logic)
        {
            var conditionStrings = logic.Split('_');
            int conditionId = 1;
            foreach (var conditionString in conditionStrings)
            {
                if (!string.IsNullOrEmpty(conditionString))
                {
                    var condition = ParseCondition(conditionString);
                    condition.ID = conditionId++;
                    group.Conditions.Add(condition);
                }
            }
        }

        private static AchievementCondition ParseCondition(string conditionStr)
        {
            var condition = new AchievementCondition();
            string current = conditionStr;

            var hitsRegex = new Regex(@"\.(\d+)\.$");
            var match = hitsRegex.Match(current);
            if (match.Success)
            {
                if (uint.TryParse(match.Groups[1].Value, out uint hits))
                {
                    condition.RequiredHits = hits;
                }
                current = current.Substring(0, match.Index);
            }

            if (current.Length > 2 && current[1] == ':' && FlagMap.TryGetValue(char.ToUpper(current[0]), out var flag))
            {
                condition.Flag = flag;
                current = current.Substring(2);
            }

            string[] operators = { "!=", "<=", ">=", "=", "<", ">", "*", "/", "%", "+", "-", "&", "^" };
            string opStr = "";
            int opIndex = -1;

            foreach (var op in operators)
            {
                opIndex = current.IndexOf(op);
                if (opIndex != -1)
                {
                    opStr = op;
                    break;
                }
            }

            if (opIndex == -1)
            {
                condition.LeftOperand = ParseOperand(current);
            }
            else
            {
                condition.Operator = opStr;
                string left = current.Substring(0, opIndex);
                string right = current.Substring(opIndex + opStr.Length);
                condition.LeftOperand = ParseOperand(left);
                condition.RightOperand = ParseOperand(right);
            }

            return condition;
        }

        public static Operand ParseOperand(string operandStr)
        {
            var operand = new Operand();
            string current = operandStr;

            if (string.IsNullOrEmpty(current)) return operand;

            if (current.Equals("{recall}", StringComparison.OrdinalIgnoreCase))
            {
                operand.Type = "Recall";
                return operand;
            }

            // 1. Handle type prefixes (d, p, b, ~) which can precede anything else.
            if (current.Length > 0 && "dpb~".Contains(char.ToLower(current[0])))
            {
                switch (char.ToLower(current[0]))
                {
                    case 'd': operand.Type = "Delta"; break;
                    case 'p': operand.Type = "Prior"; break;
                    case 'b': operand.Type = "BCD"; break;
                    case '~': operand.Type = "Inverted"; break;
                }
                current = current.Substring(1).TrimStart();
            }

            if (string.IsNullOrEmpty(current)) return operand;

            char firstChar = current[0];
            string remaining = current.Length > 1 ? current.Substring(1) : "";

            // Case 1: Float memory access (e.g., fF1234, where 'F' is size)
            if (char.ToLower(firstChar) == 'f' && current.Length > 1 && FloatSizeMap.ContainsKey(char.ToUpper(remaining[0])))
            {
                if (string.IsNullOrEmpty(operand.Type)) operand.Type = "Mem";
                operand.Size = FloatSizeMap[char.ToUpper(remaining[0])];
                operand.Value = "0x" + (remaining.Length > 1 ? remaining.Substring(1).TrimStart() : ""); // The rest is a hex address
            }
            // Case 2: Float literal (e.g., f1.23)
            else if (char.ToLower(firstChar) == 'f')
            {
                operand.Type = "Float";
                operand.Value = remaining.TrimStart();
            }
            // Case 3: Int memory access with size prefix before "0x" (e.g., H0x1234)
            else if (char.IsLetter(firstChar) && SizeMap.ContainsKey(char.ToUpper(firstChar)) && remaining.TrimStart().StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(operand.Type)) operand.Type = "Mem";
                operand.Size = SizeMap[char.ToUpper(firstChar)];
                operand.Value = remaining.TrimStart();
            }
            // Case 4: Int memory access with "0x" prefix (e.g., 0xH1234 or 0x1234)
            else if (current.TrimStart().StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(operand.Type)) operand.Type = "Mem";
                // Fix: Trim whitespace immediately after stripping "0x" to handle cases like "0x 1234"
                string addressPart = current.TrimStart().Substring(2).TrimStart();
                char sizeChar = ' '; // Default to 16-bit
                if (addressPart.Length > 0 && char.IsLetter(addressPart[0]) && SizeMap.ContainsKey(char.ToUpper(addressPart[0])))
                {
                    sizeChar = addressPart[0];
                    addressPart = addressPart.Substring(1);
                }
                operand.Size = SizeMap[char.ToUpper(sizeChar)];
                operand.Value = "0x" + addressPart;
            }
            // Case 5: Integer literal
            else
            {
                if (long.TryParse(current, NumberStyles.Integer, CultureInfo.InvariantCulture, out long val))
                {
                    if (string.IsNullOrEmpty(operand.Type)) operand.Type = "Value";
                    operand.Value = $"0x{val:X}";
                }
                // If not a valid long, it's a malformed string; we'll create an empty operand.
            }

            return operand;
        }
    }
}
#nullable disable