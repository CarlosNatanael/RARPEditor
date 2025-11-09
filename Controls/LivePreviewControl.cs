using RARPEditor.Models;
using RARPEditor.Parsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RARPEditor.Controls
{
    public partial class LivePreviewControl : UserControl
    {
        private RichPresenceScript? _currentScript;
        private RichPresenceDisplayString? _currentDisplayString;
        private readonly System.Windows.Forms.Timer _previewTimer = new System.Windows.Forms.Timer();
        private readonly Random _random = new Random();

        // Simulation state variables
        private long _simulatedTimeValue = 0;
        // Fix: Added a dedicated simulation value for score-type formatters.
        private long _simulatedScoreValue = 0;
        private readonly Dictionary<string, uint> _simulatedLookupValues = new Dictionary<string, uint>();
        private readonly Dictionary<string, (long current, long max)> _progressValues = new Dictionary<string, (long, long)>();

        private uint? _manualLookupOverrideKey = null;

        public LivePreviewControl()
        {
            InitializeComponent();
            _previewTimer.Interval = 1000;
            _previewTimer.Tick += PreviewTimer_Tick;
        }

        public void LoadDisplayString(RichPresenceDisplayString displayString, RichPresenceScript script)
        {
            _currentDisplayString = displayString;
            _currentScript = script;

            ResetSimulation();
            UpdatePreview(false);

            if (!_previewTimer.Enabled)
            {
                _previewTimer.Start();
            }
        }

        public void ClearPreview()
        {
            _previewTimer.Stop();
            _currentDisplayString = null;
            _currentScript = null;
            lookupPreviewGrid.Visible = false;
            lookupSelectorComboBox.Visible = false;
            previewLabel.Text = "Live Preview Panel";
        }

        private void ResetSimulation()
        {
            _simulatedTimeValue = 0;
            // Fix: Initialize the score to a random starting value for a more dynamic feel.
            _simulatedScoreValue = _random.Next(0, 10000);
            _simulatedLookupValues.Clear();
            _progressValues.Clear();
            _manualLookupOverrideKey = null;
        }

        private void PreviewTimer_Tick(object? sender, EventArgs e)
        {
            _simulatedTimeValue++;

            // Fix: Update the simulated score with a "random walk" to make it feel more dynamic.
            _simulatedScoreValue += _random.Next(-50, 51);
            if (_simulatedScoreValue < 0) _simulatedScoreValue = 0;


            if (_currentDisplayString != null)
            {
                UpdatePreview(true);
            }
            else
            {
                _previewTimer.Stop();
            }
        }

        private void UpdatePreview(bool isTick)
        {
            if (_currentDisplayString == null || _currentScript == null) return;

            var sb = new StringBuilder();
            var lookupsUsed = new List<RichPresenceLookup>();

            if (!isTick)
            {
                _manualLookupOverrideKey = null;
                _progressValues.Clear();
            }

            for (int i = 0; i < _currentDisplayString.Parts.Count; i++)
            {
                var part = _currentDisplayString.Parts[i];
                bool handledAsProgress = false;

                if (part.IsMacro)
                {
                    // Pattern 1: @{Macro1}/@{Macro2}
                    if (i + 2 < _currentDisplayString.Parts.Count &&
                        !_currentDisplayString.Parts[i + 1].IsMacro && _currentDisplayString.Parts[i + 1].Text == "/" &&
                        _currentDisplayString.Parts[i + 2].IsMacro)
                    {
                        var leftPart = part;
                        var rightPart = _currentDisplayString.Parts[i + 2];
                        string key = $"macro/{leftPart.Text}({leftPart.Parameter})/{rightPart.Text}({rightPart.Parameter})";

                        if (!_progressValues.ContainsKey(key))
                        {
                            var operand = AchievementParser.ParseOperand(rightPart.Parameter);
                            long maxValue = GetMaxValueFromSize(operand.Size);
                            long randomMax = _random.Next(0, (int)Math.Min(maxValue, 1000) + 1);
                            _progressValues[key] = (0, randomMax);
                        }

                        if (_progressValues.TryGetValue(key, out var progress))
                        {
                            sb.Append($"{progress.current}/{progress.max}");
                            if (isTick && progress.current < progress.max)
                            {
                                _progressValues[key] = (progress.current + 1, progress.max);
                            }
                        }

                        i += 2; // Skip the "/" and the right macro part
                        handledAsProgress = true;
                    }
                    // Pattern 2: @{Macro}/StaticNumber
                    else if (i + 1 < _currentDisplayString.Parts.Count && !_currentDisplayString.Parts[i + 1].IsMacro)
                    {
                        var nextPart = _currentDisplayString.Parts[i + 1];
                        var match = Regex.Match(nextPart.Text, @"^/(\d+)(.*)");
                        if (match.Success)
                        {
                            long staticMax = long.Parse(match.Groups[1].Value);
                            string remainingText = match.Groups[2].Value;
                            string key = $"static/{part.Text}({part.Parameter})/{staticMax}";

                            if (!_progressValues.ContainsKey(key))
                            {
                                _progressValues[key] = (0, staticMax);
                            }

                            if (_progressValues.TryGetValue(key, out var progress))
                            {
                                long newCurrent = progress.current;
                                if (isTick && newCurrent < progress.max)
                                {
                                    newCurrent++;
                                }
                                _progressValues[key] = (newCurrent, progress.max);
                                sb.Append($"{newCurrent}/{progress.max}");
                            }

                            sb.Append(remainingText);
                            i += 1; // Skip the text part we just processed
                            handledAsProgress = true;
                        }
                    }
                }

                if (handledAsProgress) continue;

                // --- Fallback to original logic if not a progress bar ---
                if (part.IsMacro)
                {
                    var lookup = _currentScript.Lookups.FirstOrDefault(l => l.Name.Equals(part.Text, StringComparison.OrdinalIgnoreCase));
                    if (lookup != null)
                    {
                        if (lookup.Entries.Any())
                        {
                            if (!lookupsUsed.Contains(lookup)) lookupsUsed.Add(lookup);

                            uint keyToTest;
                            var selectedLookupName = lookupSelectorComboBox.SelectedItem?.ToString();

                            if (_manualLookupOverrideKey.HasValue && lookup.Name == selectedLookupName)
                            {
                                keyToTest = _manualLookupOverrideKey.Value;
                            }
                            else
                            {
                                if (!isTick || !_simulatedLookupValues.ContainsKey(lookup.Name))
                                {
                                    int index = _random.Next(lookup.Entries.Count);
                                    _simulatedLookupValues[lookup.Name] = lookup.Keys.ElementAt(index);
                                }
                                keyToTest = _simulatedLookupValues[lookup.Name];
                            }
                            sb.Append(lookup.TryGetValue(keyToTest, out var val) ? val : (lookup.Default ?? ""));
                        }
                        else
                        {
                            sb.Append(FormatValue(_simulatedTimeValue, lookup.Format, part.Parameter));
                        }
                    }
                    else
                    {
                        sb.Append(FormatValue(_simulatedTimeValue, "VALUE", part.Parameter));
                    }
                }
                else
                {
                    sb.Append(part.Text);
                }
            }


            previewLabel.Text = sb.ToString();

            if (!isTick)
            {
                lookupSelectorComboBox.DataSource = lookupsUsed.Select(l => l.Name).ToList();
                lookupSelectorComboBox.Visible = lookupsUsed.Any();
                PopulateLookupGrid();
            }
        }

        private long GetMaxValueFromSize(string size)
        {
            return size switch
            {
                "8-bit" or "Bit0" or "Bit1" or "Bit2" or "Bit3" or "Bit4" or "Bit5" or "Bit6" or "Bit7" or "Lower4" or "Upper4" or "BitCount" => 0xFF,
                "16-bit" or "16-bit BE" => 0xFFFF,
                "24-bit" or "24-bit BE" => 0xFFFFFF,
                _ => 0xFFFFFFFF // Default to 32-bit max for safety on unknown/larger types
            };
        }

        private string FormatValue(long value, string format, string parameter)
        {
            // Check for percentage logic
            if (parameter.Contains('*') && parameter.Contains('/'))
            {
                long current = _random.Next(0, 101);
                long max = 100;
                // Try to find a progress tracker if it exists
                foreach (var kvp in _progressValues)
                {
                    if (parameter.Contains(kvp.Key.Split('/')[0]) && parameter.Contains(kvp.Key.Split('/')[1]))
                    {
                        current = kvp.Value.current;
                        max = kvp.Value.max;
                        break;
                    }
                }
                long percentage = max > 0 ? (current * 100) / max : 0;
                return percentage.ToString();
            }

            long displayValue = value;
            if (!string.IsNullOrEmpty(parameter))
            {
                string operandString = parameter;
                if (operandString.Length > 2 && operandString[1] == ':')
                {
                    operandString = operandString.Substring(2);
                }

                var operand = AchievementParser.ParseOperand(operandString);
                long maxValue = GetMaxValueFromSize(operand.Size);
                if (maxValue > 0)
                {
                    displayValue = value % (maxValue + 1);
                }
            }

            // Fix: Use the dedicated simulated score value for the SCORE format type.
            return format.ToUpper() switch
            {
                "SCORE" => _simulatedScoreValue.ToString("D6"),
                "FRAMES" => $"{displayValue / 3600}:{(displayValue / 60) % 60:D2}.{((displayValue * 100) / 60) % 100:D2}",
                "MILLISECS" => $"{displayValue / 6000}:{(displayValue / 100) % 60:D2}.{displayValue % 100:D2}",
                "SECS" => TimeSpan.FromSeconds(displayValue).ToString(@"m\:ss"),
                "MINUTES" => $"{(displayValue / 60)}h{displayValue % 60:D2}",
                "SECS_AS_MINS" => $"{(displayValue / 3600)}h{(displayValue / 60) % 60:D2}:{displayValue % 60:D2}",
                "UNSIGNED" => ((uint)displayValue).ToString(),
                "TENS" => (displayValue * 10).ToString(),
                "HUNDREDS" => (displayValue * 100).ToString(),
                "THOUSANDS" => (displayValue * 1000).ToString(),
                "FIXED1" => (displayValue / 10.0).ToString("F1"),
                "FIXED2" => (displayValue / 100.0).ToString("F2"),
                "FIXED3" => (displayValue / 1000.0).ToString("F3"),
                "FLOAT1" => ((float)_random.NextDouble() * 1000).ToString("F1"),
                "FLOAT2" => ((float)_random.NextDouble() * 1000).ToString("F2"),
                "FLOAT3" => ((float)_random.NextDouble() * 1000).ToString("F3"),
                "FLOAT4" => ((float)_random.NextDouble() * 1000).ToString("F4"),
                "FLOAT5" => ((float)_random.NextDouble() * 1000).ToString("F5"),
                "FLOAT6" => ((float)_random.NextDouble() * 1000).ToString("F6"),
                "VALUE" or _ => displayValue.ToString(),
            };
        }

        private void PopulateLookupGrid()
        {
            lookupPreviewGrid.Visible = false;
            lookupPreviewGrid.Rows.Clear();

            if (_currentScript == null || lookupSelectorComboBox.SelectedItem == null) return;

            string? selectedLookupName = lookupSelectorComboBox.SelectedItem.ToString();
            if (selectedLookupName != null)
            {
                var lookup = _currentScript.Lookups.FirstOrDefault(l => l.Name.Equals(selectedLookupName, StringComparison.OrdinalIgnoreCase));
                if (lookup != null && lookup.Entries.Any())
                {
                    lookupPreviewGrid.Visible = true;
                    foreach (var entry in lookup.Entries.OrderBy(e => e.KeyValue))
                    {
                        string displayValue = string.IsNullOrEmpty(entry.Value) ? "(empty)" : entry.Value;
                        lookupPreviewGrid.Rows.Add(entry.KeyString, displayValue);
                    }
                }
            }
        }

        private void lookupPreviewGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var keyStr = lookupPreviewGrid.Rows[e.RowIndex].Cells[0].Value?.ToString();
            if (keyStr != null && RichPresenceParser.TryParseUInt(keyStr, out uint key))
            {
                _manualLookupOverrideKey = key;
                if (_currentDisplayString != null)
                {
                    UpdatePreview(true);
                }
            }
        }

        private void lookupSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _manualLookupOverrideKey = null;
            PopulateLookupGrid();
        }
    }
}