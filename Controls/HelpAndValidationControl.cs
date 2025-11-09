using RARPEditor.Models;
using RARPEditor.Logic;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using RARPEditor.Parsers;

namespace RARPEditor.Controls
{
    public partial class HelpAndValidationControl : UserControl
    {
        public HelpAndValidationControl()
        {
            InitializeComponent();
        }

        public void ClearView()
        {
            helpRichTextBox.Text = "Select an item in the Project Explorer to see help and validation information.";
        }

        public bool UpdateView(object? selectedObject, RichPresenceScript script)
        {
            if (selectedObject == null)
            {
                ClearView();
                return false;
            }

            var sb = new StringBuilder();
            List<ValidationResult> results;

            sb.AppendLine(@"{\rtf1\ansi\deff0
{\fonttbl{\f0 Segoe UI;}}
{\colortbl ;\red220\green53\blue69;\red255\green193\blue7;\red13\green110\blue253;}
\pard\sa200\sl276\slmult1\f0\fs22");

            switch (selectedObject)
            {
                case RichPresenceLookup lookup:
                    results = ScriptValidator.Validate(lookup, script);
                    BuildLookupHelp(sb, lookup, script, results);
                    break;
                case RichPresenceDisplayString displayString:
                    results = ScriptValidator.Validate(displayString, script);
                    BuildDisplayStringHelp(sb, displayString, script, results);
                    break;
                case TreeNode node:
                    results = new List<ValidationResult>(); // No validation for parent nodes
                    BuildTreeNodeHelp(sb, node);
                    break;
                default:
                    results = new List<ValidationResult>();
                    ClearView();
                    break;
            }

            sb.AppendLine(@"\pard}");
            helpRichTextBox.Rtf = sb.ToString();
            return results.Any(r => r.Type == ValidationType.Error);
        }

        private void AppendHeader(StringBuilder sb, string text)
        {
            sb.AppendLine($@"\pard\sa200\sl276\slmult1\b\fs24 {text}\b0\fs22\par");
        }

        // Fix: Ensure every line resets its color to default (\cf0) to prevent color bleed.
        private void AppendLine(StringBuilder sb, string text, bool isBold = false, string bullet = "")
        {
            sb.Append(@"\pard\sa100\sl276\slmult1");
            if (!string.IsNullOrEmpty(bullet))
            {
                sb.Append($@"\fi-360\li720 {bullet} \tab");
            }
            if (isBold) sb.Append(@"\b");
            text = text.Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}");
            sb.Append($@" {text}");
            if (isBold) sb.Append(@"\b0");
            sb.AppendLine(@"\cf0\par"); // Reset to default color (color table index 0)
        }

        private void AppendSubtext(StringBuilder sb, string text)
        {
            sb.Append(@"\pard\sa100\sl276\slmult1\i");
            text = text.Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}");
            sb.Append($@" {text}");
            sb.AppendLine(@"\i0\cf0\par");
        }


        private void BuildTreeNodeHelp(StringBuilder sb, TreeNode node)
        {
            switch (node.Name)
            {
                case "lookupsNode":
                    AppendHeader(sb, "Lookups");
                    AppendLine(sb, "Lookups are dictionaries that map a memory value to a specific string. For example, mapping a value of 1 to the text \"World 1-1\".");
                    break;
                case "formattersNode":
                    AppendHeader(sb, "Formatters");
                    AppendLine(sb, "Formatters define how a raw memory value should be displayed. For example, formatting a value of 3600 as a time: \"1:00:00\".");
                    break;
                case "displayLogicNode":
                    AppendHeader(sb, "Display Logic");
                    AppendLine(sb, "This section contains the conditional strings that will be displayed as the Rich Presence. The conditions are checked from top to bottom, and the first one that is true will be displayed.");
                    break;
            }
        }

        // Reorder sections to show Validation at the top.
        private void BuildLookupHelp(StringBuilder sb, RichPresenceLookup lookup, RichPresenceScript script, List<ValidationResult> results)
        {
            AppendHeader(sb, "Validation");
            RenderValidationResults(sb, results);

            bool isFormatter = !lookup.Entries.Any() && lookup.Default == null;
            AppendHeader(sb, isFormatter ? $"Formatter: {lookup.Name}" : $"Lookup: {lookup.Name}");
            AppendLine(sb, isFormatter
                ? "A formatter defines how a raw memory value should be displayed."
                : "A lookup maps a memory value to a specific string.");

            if (isFormatter)
            {
                AppendLine(sb, $"Format Type: {lookup.Format}", true);
                AppendLine(sb, GetFormatTypeDescription(lookup.Format), false);
            }

            AppendHeader(sb, "Used By");
            int usageCount = 0;
            foreach (var ds in script.DisplayStrings)
            {
                foreach (var part in ds.Parts.Where(p => p.IsMacro && p.Text.Equals(lookup.Name, System.StringComparison.Ordinal)))
                {
                    AppendLine(sb, ds.ToFormattedString(script).Trim(), false, @"\bullet");
                    usageCount++;
                }
            }
            if (usageCount == 0)
            {
                AppendLine(sb, "Not currently used by any display strings.", false);
            }
        }

        // Fix: Replace confusing terminology with clearer headers and add contextual subtext.
        // Replace the raw condition string with a helpful summary of logic and macros used.
        private void BuildDisplayStringHelp(StringBuilder sb, RichPresenceDisplayString displayString, RichPresenceScript script, List<ValidationResult> results)
        {
            AppendHeader(sb, "Validation");
            RenderValidationResults(sb, results);

            AppendHeader(sb, "Display String Summary");
            if (displayString.IsDefault)
            {
                AppendLine(sb, "This is the default string that displays when no other conditions are met.");
            }
            else
            {
                AppendHeader(sb, "Display Condition");
                AppendSubtext(sb, "This string is shown when its conditions are met.");
                if (string.IsNullOrEmpty(displayString.Condition))
                {
                    AppendLine(sb, "No condition is defined.", false);
                }
                else
                {
                    var conditionGroups = AchievementParser.ParseAchievementTrigger(displayString.Condition);
                    int lineCount = conditionGroups.SelectMany(g => g.Conditions).Count();
                    int groupCount = conditionGroups.Count;
                    string lineText = lineCount == 1 ? "1 logic line" : $"{lineCount} logic lines";
                    string groupText = groupCount > 1 ? $" across {groupCount} alternate groups" : "";
                    AppendLine(sb, $"Condition is defined by {lineText}{groupText}.", false);
                }
            }

            AppendHeader(sb, "Data Fields");
            AppendSubtext(sb, "These fields pull in dynamic data from the game.");
            var macros = displayString.Parts.Where(p => p.IsMacro).ToList();
            if (macros.Any())
            {
                foreach (var macro in macros)
                {
                    string text = $"{{{macro.Text}}}";
                    if (!string.IsNullOrEmpty(macro.Parameter))
                    {
                        text += " (has parameters)";
                    }
                    AppendLine(sb, text, false, @"\bullet");
                }
            }
            else
            {
                AppendLine(sb, "No macros are used in this display string.");
            }
        }

        // Fix: Ensure every line resets its color to default (\cf0) to prevent color bleed from validation messages.
        private void RenderValidationResults(StringBuilder sb, List<ValidationResult> results)
        {
            if (results.Any())
            {
                foreach (var result in results)
                {
                    string prefix = "";
                    string colorCode = "";
                    switch (result.Type)
                    {
                        case ValidationType.Error: prefix = "Error:"; colorCode = @"\cf1"; break;
                        case ValidationType.Warning: prefix = "Warning:"; colorCode = @"\cf2"; break;
                        case ValidationType.Info: prefix = "Info:"; colorCode = @"\cf3"; break;
                    }

                    sb.Append(@"\pard\sa100\sl276\slmult1\fi-360\li720 \bullet \tab ");
                    sb.Append(colorCode);
                    sb.Append(@"\b ");
                    sb.Append(prefix);
                    sb.Append(@"\b0  ");
                    string escapedMessage = result.Message.Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}");
                    sb.Append(escapedMessage);
                    sb.AppendLine(@"\cf0\par"); // Reset to default color
                }
            }
            else
            {
                AppendLine(sb, "No issues found.", false);
            }
        }


        private string GetFormatTypeDescription(string formatType)
        {
            return formatType.ToUpper() switch
            {
                "VALUE" => "Displays the raw value.",
                "UNSIGNED" => "Displays the value as an unsigned integer.",
                "SCORE" => "Displays as a 6-digit score, padded with zeroes (e.g., 001234).",
                "SECS" => "Displays value in seconds as M:SS.",
                "SECS_AS_MINS" => "Displays value in seconds as H:MM:SS.",
                "MINUTES" => "Displays value in minutes as XhYm.",
                "FRAMES" => "Displays value in frames as M:SS.CC (assuming 60fps).",
                "MILLISECS" => "Displays value in milliseconds as M:SS.CC.",
                "FIXED1" or "FLOAT1" => "Displays as a floating-point number with 1 decimal place.",
                "FIXED2" or "FLOAT2" => "Displays as a floating-point number with 2 decimal places.",
                "FIXED3" or "FLOAT3" => "Displays as a floating-point number with 3 decimal places.",
                _ => "A numeric format.",
            };
        }
    }
}