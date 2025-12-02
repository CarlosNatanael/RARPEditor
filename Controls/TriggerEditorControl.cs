using RARPEditor.Models;
using RARPEditor.Parsers;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RARPEditor.Controls
{
    public partial class TriggerEditorControl : UserControl
    {
        private bool _isProgrammaticallyChanging;
        private string _triggerText = "";
        private List<AchievementCondition> _conditions = new();
        private bool _showDecimal = false;
        private bool _collapseChains = false; // New property backing field

        // New UI Control
        private CheckBox collapseChainsCheckBox;

        public event EventHandler? TriggerTextChanged;
        public event EventHandler<string>? StatusUpdateRequested;

        public char GroupSeparator { get; set; } = 'S';

        #region ComboBox DataSources & Mappings
        private const string NO_FLAG_TEXT = "(No Flag)";
        private const string NO_OPERATOR_TEXT = "(None)";

        private static readonly string[] Flags = { NO_FLAG_TEXT, "Reset If", "Pause If", "Add Source", "Sub Source", "Add Hits", "Sub Hits", "Add Address", "AndNext", "OrNext", "Measured", "MeasuredIf", "Measured%", "Trigger", "ResetNextIf", "Remember" };
        private static readonly string[] OperandTypes = { "Mem", "Value", "Delta", "Prior", "BCD", "Inverted", "Float", "Recall" };
        private static readonly string[] MemorySizes = { "", "8-bit", "16-bit", "24-bit", "32-bit", "Bit0", "Bit1", "Bit2", "Bit3", "Bit4", "Bit5", "Bit6", "Bit7", "Lower4", "Upper4", "BitCount", "16-bit BE", "24-bit BE", "32-bit BE", "Float", "Float BE", "Double32", "Double32 BE", "MBF32", "MBF32 LE" };
        private static readonly string[] ComparisonOperators = { NO_OPERATOR_TEXT, "=", "!=", "<", "<=", ">", ">=" };
        private static readonly string[] ArithmeticOperators = { NO_OPERATOR_TEXT, "&", "^", "+", "-", "*", "/", "%" };

        // Create more granular flag categories to handle different logic rules.
        private static readonly HashSet<string> ArithmeticFlags = new() { "Add Source", "Sub Source", "Add Address", "Remember" };
        // These flags MUST have a comparison operator.
        private static readonly HashSet<string> StrictComparisonFlags = new() { "Reset If", "Pause If", "Add Hits", "Sub Hits", "AndNext", "OrNext", "MeasuredIf", "Trigger", "ResetNextIf" };
        private static readonly HashSet<string> FlagsWithoutHits = new() { "Add Source", "Sub Source", "Add Address", "AndNext", "OrNext" };

        private static readonly Dictionary<string, char> ReverseFlagMap = new() { { "Reset If", 'R' }, { "Pause If", 'P' }, { "Add Source", 'A' }, { "Sub Source", 'B' }, { "Add Hits", 'C' }, { "Sub Hits", 'D' }, { "Add Address", 'I' }, { "AndNext", 'N' }, { "OrNext", 'O' }, { "Measured", 'M' }, { "MeasuredIf", 'Q' }, { "Measured%", 'G' }, { "Trigger", 'T' }, { "ResetNextIf", 'Z' }, { "Remember", 'K' } };
        private static readonly Dictionary<string, char> ReverseSizeMap = new() { { "", ' ' }, { "16-bit", ' ' }, { "8-bit", 'H' }, { "32-bit", 'X' }, { "24-bit", 'W' }, { "Bit0", 'M' }, { "Bit1", 'N' }, { "Bit2", 'O' }, { "Bit3", 'P' }, { "Bit4", 'Q' }, { "Bit5", 'R' }, { "Bit6", 'S' }, { "Bit7", 'T' }, { "Lower4", 'L' }, { "Upper4", 'U' }, { "BitCount", 'K' }, { "32-bit BE", 'G' }, { "16-bit BE", 'I' }, { "24-bit BE", 'J' } };

        private static readonly Dictionary<char, string> FloatSizeMap = new()
        {
            {'F', "Float"}, {'B', "Float BE"}, {'H', "Double32"},
            {'I', "Double32 BE"}, {'M', "MBF32"}, {'L', "MBF32 LE"}
        };
        private static readonly Dictionary<string, char> ReverseFloatSizeMap =
            FloatSizeMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        #endregion

        private readonly ContextMenuStrip _flagContextMenu = new();
        private readonly ContextMenuStrip _typeContextMenu = new();
        private readonly ContextMenuStrip _sizeContextMenu = new();
        private readonly ContextMenuStrip _cmpContextMenu = new();
        private readonly ContextMenuStrip _cmpArithmeticContextMenu = new();
        // Add a new context menu for flags that require a comparison, excluding the "(None)" option.
        private readonly ContextMenuStrip _cmpStrictContextMenu = new();
        private int _currentEditColumnIndex = -1;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TriggerText { get => _triggerText; set { _triggerText = value; UpdateUIFromText(); } }
        public string GroupBoxText { get => mainGroupBox.Text; set => mainGroupBox.Text = value; }

        public TriggerEditorControl()
        {
            InitializeComponent();
            InitializeCustomUI();
            SetupGrid();
            CreateEditingContextMenus();
        }

        private void InitializeCustomUI()
        {
            // Add Collapse Chains checkbox to toolbar
            collapseChainsCheckBox = new CheckBox();
            collapseChainsCheckBox.Text = "Collapse Chains";
            collapseChainsCheckBox.Appearance = Appearance.Button;
            collapseChainsCheckBox.AutoSize = true;
            collapseChainsCheckBox.Margin = new Padding(3, 5, 3, 3);
            collapseChainsCheckBox.CheckedChanged += CollapseChainsCheckBox_CheckedChanged;

            // Insert before Show Decimal (FlowDirection is usually RightToLeft for toolbars or just prepended)
            // Assuming FlowLayout: Copy [Index 2], Clear [Index 1], Show Decimal [Index 0]
            // We want it visually next to Show Decimal.
            if (toolbarPanel.Controls.Contains(showDecimalCheckBox))
            {
                int index = toolbarPanel.Controls.IndexOf(showDecimalCheckBox);
                toolbarPanel.Controls.Add(collapseChainsCheckBox);
                toolbarPanel.Controls.SetChildIndex(collapseChainsCheckBox, 0); // Becomes the first control (rightmost)
            }
            else
            {
                toolbarPanel.Controls.Add(collapseChainsCheckBox);
            }
        }

        private void CollapseChainsCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            _collapseChains = collapseChainsCheckBox.Checked;
            PopulateGridFromConditions();
        }

        private void SetupGrid()
        {
            triggerGrid.MultiSelect = true;
            triggerGrid.ReadOnly = false;
            triggerGrid.DataError += (s, e) => { e.ThrowException = false; };
            triggerGrid.CellValueChanged += triggerGrid_CellValueChanged;
            triggerGrid.CellMouseDown += triggerGrid_CellMouseDown;
            triggerGrid.CellDoubleClick += triggerGrid_CellDoubleClick;
            triggerGrid.EditingControlShowing += triggerGrid_EditingControlShowing;
            triggerGrid.CellEndEdit += triggerGrid_CellEndEdit;
            triggerGrid.KeyDown += triggerGrid_KeyDown;

            ColID.ReadOnly = true;

            // Explicitly set columns that use context menus for editing to be NOT ReadOnly.
            // This overrides any designer settings and allows cell-level ReadOnly state to be the source of truth.
            ColFlag.ReadOnly = false;
            ColLType.ReadOnly = false;
            ColLSize.ReadOnly = false;
            ColCmp.ReadOnly = false;
            ColRType.ReadOnly = false;
            ColRSize.ReadOnly = false;
        }

        private void CreateEditingContextMenus()
        {
            CreateMenu(_flagContextMenu, Flags, OnContextMenuItemClick);
            CreateMenu(_typeContextMenu, OperandTypes, OnContextMenuItemClick);
            CreateMenu(_sizeContextMenu, MemorySizes, OnContextMenuItemClick);
            CreateMenu(_cmpContextMenu, ComparisonOperators, OnContextMenuItemClick);
            CreateMenu(_cmpArithmeticContextMenu, ArithmeticOperators, OnContextMenuItemClick);

            // Create the new strict comparison menu without the "(None)" option.
            var strictComparisonOperators = ComparisonOperators.Where(op => op != NO_OPERATOR_TEXT).ToArray();
            CreateMenu(_cmpStrictContextMenu, strictComparisonOperators, OnContextMenuItemClick);
        }

        private void CreateMenu(ContextMenuStrip menu, string[] items, EventHandler clickHandler)
        {
            foreach (var item in items)
            {
                var menuItem = new ToolStripMenuItem(item.Replace("&", "&&"), null, clickHandler) { Tag = item };
                menu.Items.Add(menuItem);
            }
        }

        #region UI and State Synchronization
        private void UpdateUIFromText()
        {
            _isProgrammaticallyChanging = true;
            ParseAndDisplayTrigger(_triggerText);
            _isProgrammaticallyChanging = false;
        }

        private void ParseAndDisplayTrigger(string trigger)
        {
            // Use the GroupSeparator property to parse with the correct context ('S' or '$').
            var conditionGroups = AchievementParser.ParseAchievementTrigger(trigger, GroupSeparator);
            _conditions = conditionGroups.SelectMany(g => g.Conditions).ToList();
            PopulateGridFromConditions();
        }

        private void PopulateGridFromConditions()
        {
            _isProgrammaticallyChanging = true;
            triggerGrid.SuspendLayout();

            int firstDisplayedRow = triggerGrid.FirstDisplayedScrollingRowIndex;
            if (firstDisplayedRow < 0) firstDisplayedRow = 0;

            var selectedIndices = triggerGrid.SelectedRows.Cast<DataGridViewRow>().Select(r => r.Index).ToList();
            triggerGrid.Rows.Clear();
            if (_conditions.Any())
            {
                var rowsToAdd = new List<DataGridViewRow>();
                for (int i = 0; i < _conditions.Count; i++)
                {
                    var cond = _conditions[i];

                    // Collapse Chains Logic: Skip "Add Address" rows if enabled
                    if (_collapseChains && cond.Flag == "Add Address")
                    {
                        continue;
                    }

                    var row = new DataGridViewRow();
                    row.CreateCells(triggerGrid);
                    cond.ID = i + 1;
                    SetRowValuesFromCondition(row, cond);
                    row.Tag = cond;
                    rowsToAdd.Add(row);
                }
                triggerGrid.Rows.AddRange(rowsToAdd.ToArray());
                triggerGrid.ClearSelection();

                // Attempt to restore selection. Indices might shift if rows are hidden, 
                // so simply clearing is safer, or selecting the first valid one.
                // We leave selection cleared to prevent index out of range errors.
            }
            triggerGrid.ResumeLayout();
            _isProgrammaticallyChanging = false;
        }

        private void SetRowValuesFromCondition(DataGridViewRow row, AchievementCondition cond)
        {
            row.Cells[ColID.Index].Value = cond.ID;
            row.Cells[ColFlag.Index].Value = cond.Flag;
            row.Cells[ColLType.Index].Value = cond.LeftOperand.Type;
            row.Cells[ColLSize.Index].Value = cond.LeftOperand.Size;
            row.Cells[ColLValue.Index].Value = FormatDisplayValue(cond.LeftOperand, cond.LeftOperand.Size);
            row.Cells[ColCmp.Index].Value = cond.Operator;
            row.Cells[ColRType.Index].Value = cond.RightOperand.Type;
            row.Cells[ColRSize.Index].Value = cond.RightOperand.Size;

            // Use helper to determine correct padding for Right Operand (full 32-bit for arithmetic)
            row.Cells[ColRValue.Index].Value = FormatDisplayValue(cond.RightOperand, GetRightOperandSizeReference(cond));

            // --- Cell ReadOnly/Styling Logic ---
            var lSizeCell = row.Cells[ColLSize.Index];
            var lValueCell = row.Cells[ColLValue.Index];

            if (cond.LeftOperand.Type is "Float" or "Recall" or "Value")
            {
                lSizeCell.ReadOnly = true;
                lSizeCell.Style.BackColor = SystemColors.ControlLight;

                if (cond.LeftOperand.Type == "Recall")
                {
                    lValueCell.ReadOnly = true;
                    lValueCell.Style.BackColor = SystemColors.ControlLight;
                    // Ensure cleared if visually Recall
                    row.Cells[ColLValue.Index].Value = "";
                }
                else
                {
                    lValueCell.ReadOnly = false;
                    lValueCell.Style.BackColor = SystemColors.Window;
                }
            }
            else
            {
                lSizeCell.ReadOnly = false;
                lSizeCell.Style.BackColor = SystemColors.Window;
                lValueCell.ReadOnly = false;
                lValueCell.Style.BackColor = SystemColors.Window;
            }

            var rSizeCell = row.Cells[ColRSize.Index];
            var rValueCell = row.Cells[ColRValue.Index];
            var rTypeCell = row.Cells[ColRType.Index];

            if (string.IsNullOrEmpty(cond.Operator))
            {
                rTypeCell.ReadOnly = true;
                rTypeCell.Style.BackColor = SystemColors.ControlLight;
                rSizeCell.ReadOnly = true;
                rSizeCell.Style.BackColor = SystemColors.ControlLight;
                rValueCell.ReadOnly = true;
                rValueCell.Style.BackColor = SystemColors.ControlLight;

                // Clear visual values if operator is missing (e.g. Add Source, Add Address chains)
                row.Cells[ColRType.Index].Value = "";
                row.Cells[ColRSize.Index].Value = "";
                row.Cells[ColRValue.Index].Value = "";
            }
            else
            {
                rTypeCell.ReadOnly = false;
                rTypeCell.Style.BackColor = SystemColors.Window;

                // Handle styling for Recall type on Right Operand
                if (cond.RightOperand.Type == "Recall")
                {
                    rSizeCell.ReadOnly = true;
                    rSizeCell.Style.BackColor = SystemColors.ControlLight;
                    rValueCell.ReadOnly = true;
                    rValueCell.Style.BackColor = SystemColors.ControlLight;
                    row.Cells[ColRValue.Index].Value = ""; // Ensure visually empty
                }
                else if (cond.RightOperand.Type is "Float" or "Value")
                {
                    rValueCell.ReadOnly = false;
                    rValueCell.Style.BackColor = SystemColors.Window;
                    rSizeCell.ReadOnly = true;
                    rSizeCell.Style.BackColor = SystemColors.ControlLight;
                }
                else
                {
                    rValueCell.ReadOnly = false;
                    rValueCell.Style.BackColor = SystemColors.Window;
                    rSizeCell.ReadOnly = false;
                    rSizeCell.Style.BackColor = SystemColors.Window;
                }
            }


            if (FlagsWithoutHits.Contains(cond.Flag))
            {
                cond.RequiredHits = 0;
                row.Cells[ColHits.Index].Value = "";
                row.Cells[ColHits.Index].ReadOnly = true;
                row.Cells[ColHits.Index].Style.BackColor = SystemColors.ControlLight;
            }
            else
            {
                row.Cells[ColHits.Index].Value = cond.RequiredHits > 0 ? cond.RequiredHits.ToString() : "";
                row.Cells[ColHits.Index].ReadOnly = false;
                row.Cells[ColHits.Index].Style.BackColor = SystemColors.Window;
            }
        }

        // Helper method to determine the display size (padding) for the Right Operand.
        // If the operator is arithmetic (*, /, %, +, -, &, ^), we ignore the Left Operand's size 
        // and default to 32-bit (8-char) padding to correctly display scaling factors.
        private string GetRightOperandSizeReference(AchievementCondition cond)
        {
            if (ArithmeticOperators.Contains(cond.Operator) && cond.Operator != NO_OPERATOR_TEXT)
            {
                return "32-bit"; // Forces 8 char padding via GetPaddingForSize
            }
            // If the Right operand itself has a size, use it (e.g. Mem to Mem comparison)
            if (!string.IsNullOrEmpty(cond.RightOperand.Size))
            {
                return cond.RightOperand.Size;
            }
            // Fallback to Left Size
            return cond.LeftOperand.Size;
        }

        private string FormatDisplayValue(Operand operand, string sizeReference = "")
        {
            if (string.IsNullOrEmpty(operand.Value)) return "";

            // Handle Float immediately
            if (operand.Type == "Float")
            {
                if (decimal.TryParse(operand.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decVal) && decVal % 1 == 0 && !operand.Value.Contains('.'))
                {
                    return operand.Value + ".0";
                }
                return operand.Value;
            }

            // Parse as Hex
            if (long.TryParse(operand.Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long val))
            {
                if (operand.Type == "Value")
                {
                    // Only "Value" type respects the Decimal Toggle
                    if (_showDecimal)
                    {
                        return val.ToString();
                    }
                    else
                    {
                        int padding = GetPaddingForSize(sizeReference);
                        long mask = GetMaskForSize(sizeReference);
                        // Standardize visual output for Values (Lowercase 'x')
                        return "0x" + (val & mask).ToString($"x{padding}");
                    }
                }
                else
                {
                    // Mem, Delta, Prior, BCD, Inverted => Always Hex, Always 8 chars
                    // Lowercase 'x' for addresses
                    int padding = 8;
                    return "0x" + val.ToString($"x{padding}");
                }
            }

            return operand.Value; // Fallback for malformed data
        }

        private int GetPaddingForSize(string size) => size switch
        {
            "8-bit" or "Bit0" or "Bit1" or "Bit2" or "Bit3" or "Bit4" or "Bit5" or "Bit6" or "Bit7" or "Lower4" or "Upper4" or "BitCount" => 2,
            "16-bit" or "16-bit BE" => 4,
            "24-bit" or "24-bit BE" => 6,
            _ => 8,
        };

        private long GetMaskForSize(string size) => size switch
        {
            "8-bit" or "Bit0" or "Bit1" or "Bit2" or "Bit3" or "Bit4" or "Bit5" or "Bit6" or "Bit7" or "Lower4" or "Upper4" or "BitCount" => 0xFF,
            "16-bit" or "16-bit BE" => 0xFFFF,
            "24-bit" or "24-bit BE" => 0xFFFFFF,
            _ => -1, // Equivalent to 0xFFFFFFFFFFFFFFFF for long, resulting in no masking
        };

        private void BuildTriggerAndNotify()
        {
            var sb = new StringBuilder();
            foreach (var cond in _conditions)
            {
                if (sb.Length > 0) sb.Append('_');
                sb.Append(ConditionToString(cond));
            }

            string newTriggerText = sb.ToString();
            if (_triggerText != newTriggerText)
            {
                _triggerText = newTriggerText;
                TriggerTextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public static string ConditionToString(AchievementCondition cond)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(cond.Flag) && ReverseFlagMap.TryGetValue(cond.Flag, out char flag))
            {
                sb.Append(flag).Append(':');
            }
            sb.Append(OperandToString(cond.LeftOperand));
            sb.Append(cond.Operator);
            sb.Append(OperandToString(cond.RightOperand));
            if (cond.RequiredHits > 0)
            {
                sb.Append('.').Append(cond.RequiredHits).Append('.');
            }
            return sb.ToString();
        }

        private static string OperandToString(Operand op)
        {
            if (op == null || string.IsNullOrEmpty(op.Type)) return "";

            var sb = new StringBuilder();
            string coreType = op.Type;
            bool isPrefixed = false;

            // Handle prefixes like Delta, Prior, etc.
            switch (op.Type)
            {
                case "Delta": sb.Append('d'); isPrefixed = true; break;
                case "Prior": sb.Append('p'); isPrefixed = true; break;
                case "BCD": sb.Append('b'); isPrefixed = true; break;
                case "Inverted": sb.Append('~'); isPrefixed = true; break;
                case "Recall": return "{recall}";
                case "Float": return "f" + op.Value;
            }

            // A prefixed operand is always a memory type at its core.
            if (isPrefixed)
            {
                coreType = "Mem";
            }

            if (coreType == "Mem")
            {
                // Handle Float Memory types: fF1234, fB1234, etc.
                if (ReverseFloatSizeMap.TryGetValue(op.Size, out char floatSizeChar))
                {
                    string prefix = "f" + floatSizeChar;
                    sb.Append(prefix);
                    sb.Append(op.Value.Replace("0x", ""));
                }
                // Handle Integer Memory types: 0xH1234, 0xX1234, etc.
                else
                {
                    sb.Append("0x");
                    if (ReverseSizeMap.TryGetValue(op.Size, out char sizeChar) && sizeChar != ' ')
                    {
                        sb.Append(char.ToUpper(sizeChar));
                    }
                    sb.Append(op.Value.Replace("0x", ""));
                }
                return sb.ToString();
            }

            if (coreType == "Value")
            {
                // Value operands are serialized as decimal literals in the trigger string.
                if (op.Value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) &&
                    long.TryParse(op.Value.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long val))
                {
                    return val.ToString();
                }
                return op.Value;
            }

            // Fallback, should not be reached with valid data.
            return op.Value;
        }
        #endregion

        #region Grid Interactivity
        private void triggerGrid_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox editingControl)
            {
                editingControl.TextChanged -= EditingControl_TextChanged;
                editingControl.TextChanged += EditingControl_TextChanged;
            }
        }

        private void EditingControl_TextChanged(object? sender, EventArgs e)
        {
            var editingControl = sender as TextBox;
            if (triggerGrid.CurrentCell != null && editingControl != null)
            {
                triggerGrid.CurrentCell.Value = editingControl.Text;
            }
        }

        // Allow user to enter float literals for float memory, and convert them to hex for storage.
        private string ParseAndFormatValue(string input, string operandType, string size)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            if (operandType == "Mem" && ReverseFloatSizeMap.ContainsKey(size))
            {
                if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatVal))
                {
                    byte[] bytes = BitConverter.GetBytes(floatVal);
                    if (size.EndsWith("BE")) Array.Reverse(bytes);
                    // Lowercase 'x'
                    return "0x" + BitConverter.ToUInt32(bytes, 0).ToString("x8");
                }
                if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase) && uint.TryParse(input.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint hexVal))
                    // Lowercase 'x'
                    return "0x" + hexVal.ToString("x8");
                return input;
            }

            if (operandType == "Float") return input;

            long val;
            // Use helper to determine correct padding for user input
            int padding = GetPaddingForSize(size);

            // Memory types always force 8 padding and hex
            if (operandType == "Mem" || operandType == "Delta" || operandType == "Prior" || operandType == "BCD" || operandType == "Inverted")
            {
                padding = 8;
            }

            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                if (long.TryParse(input.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out val))
                    // Lowercase 'x'
                    return "0x" + val.ToString($"x{padding}");
            }
            else if (long.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out val))
            {
                // Lowercase 'x' (only applies if converting Decimal input -> Hex output)
                return "0x" + val.ToString($"x{padding}");
            }
            return input;
        }

        private void triggerGrid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (_isProgrammaticallyChanging || e.RowIndex < 0) return;
            var row = triggerGrid.Rows[e.RowIndex];
            if (row.Tag is not AchievementCondition cond) return;
            var cell = row.Cells[e.ColumnIndex];
            string? value = cell.Value?.ToString() ?? "";

            if (cell.OwningColumn == ColLValue)
            {
                cond.LeftOperand.Value = ParseAndFormatValue(value, cond.LeftOperand.Type, cond.LeftOperand.Size);
            }
            else if (cell.OwningColumn == ColRValue)
            {
                // Use helper to determine correct padding for user input
                cond.RightOperand.Value = ParseAndFormatValue(value, cond.RightOperand.Type, GetRightOperandSizeReference(cond));
            }
            else if (cell.OwningColumn == ColHits)
            {
                var match = Regex.Match(value, @"\d+");
                cond.RequiredHits = match.Success ? uint.Parse(match.Value) : 0;
            }
        }

        private void triggerGrid_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = triggerGrid.Rows[e.RowIndex];
            if (row.Tag is not AchievementCondition cond) return;

            SetRowValuesFromCondition(row, cond);
            BuildTriggerAndNotify();
        }

        private void triggerGrid_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || Control.ModifierKeys != Keys.Shift || e.RowIndex < 0)
                return;
            if (!triggerGrid.Rows[e.RowIndex].Selected)
                return;

            ShowEditingMenuForCell(e.ColumnIndex, e.RowIndex);
        }

        private void triggerGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            ShowEditingMenuForCell(e.ColumnIndex, e.RowIndex);
        }

        private void ShowEditingMenuForCell(int columnIndex, int rowIndex)
        {
            // Use the explicit ReadOnly property instead of the cell's background color for robustness.
            if (triggerGrid.Rows[rowIndex].Cells[columnIndex].ReadOnly)
            {
                return;
            }

            _currentEditColumnIndex = columnIndex;
            var row = triggerGrid.Rows[rowIndex];
            var col = triggerGrid.Columns[columnIndex];

            if (col == ColFlag) _flagContextMenu.Show(Cursor.Position);
            else if (col == ColLType || col == ColRType) _typeContextMenu.Show(Cursor.Position);
            else if (col == ColLSize || col == ColRSize) _sizeContextMenu.Show(Cursor.Position);
            else if (col == ColCmp)
            {
                var flag = row.Cells[ColFlag.Index].Value?.ToString() ?? "";
                if (ArithmeticFlags.Contains(flag))
                {
                    _cmpArithmeticContextMenu.Show(Cursor.Position);
                }
                else if (StrictComparisonFlags.Contains(flag))
                {
                    _cmpStrictContextMenu.Show(Cursor.Position);
                }
                else // For Blank, Measured, Measured%
                {
                    _cmpContextMenu.Show(Cursor.Position);
                }
            }
        }

        private void OnContextMenuItemClick(object? sender, EventArgs e)
        {
            if (_isProgrammaticallyChanging || sender is not ToolStripMenuItem item || item.Tag is not string newDisplayValue) return;

            string newValue = (newDisplayValue == NO_FLAG_TEXT || newDisplayValue == NO_OPERATOR_TEXT) ? "" : newDisplayValue;

            // Updated selection retrieval to include current row if selection is empty/weird
            List<DataGridViewRow> rowsToModify = new List<DataGridViewRow>();
            if (triggerGrid.SelectedRows.Count > 0)
            {
                rowsToModify.AddRange(triggerGrid.SelectedRows.Cast<DataGridViewRow>());
            }
            else if (triggerGrid.CurrentRow != null)
            {
                rowsToModify.Add(triggerGrid.CurrentRow);
            }

            foreach (DataGridViewRow row in rowsToModify)
            {
                if (row.Tag is not AchievementCondition cond) continue;

                string oldFlag = cond.Flag;

                switch (_currentEditColumnIndex)
                {
                    case var _ when _currentEditColumnIndex == ColFlag.Index:
                        cond.Flag = newValue;
                        HandleFlagChange(cond, oldFlag);
                        break;
                    case var _ when _currentEditColumnIndex == ColLType.Index: HandleTypeChange(cond.LeftOperand, newValue, true, cond.LeftOperand.Size); break;
                    case var _ when _currentEditColumnIndex == ColLSize.Index: HandleSizeChange(cond.LeftOperand, newValue); break;
                    case var _ when _currentEditColumnIndex == ColCmp.Index:
                        string oldOperator = cond.Operator;
                        cond.Operator = newValue;
                        HandleOperatorChange(cond, oldOperator);
                        break;
                    case var _ when _currentEditColumnIndex == ColRType.Index: HandleTypeChange(cond.RightOperand, newValue, false, cond.LeftOperand.Size); break;
                    case var _ when _currentEditColumnIndex == ColRSize.Index: HandleSizeChange(cond.RightOperand, newValue); break;
                }

                ApplyConditionLogicRules(cond);
            }

            PopulateGridFromConditions();
            BuildTriggerAndNotify();
        }

        private void HandleTypeChange(Operand op, string newType, bool isLeftOperand, string referenceSize)
        {
            string oldType = op.Type;
            if (oldType == newType) return;

            // --- Step 1: Handle special cases that reset everything ---

            // If switching TO Recall
            if (newType == "Recall")
            {
                op.Value = "";
                op.Type = newType;
                return;
            }

            // If switching FROM Recall, or if Value is empty
            if (string.IsNullOrEmpty(op.Value) || (oldType == "Recall"))
            {
                if (newType == "Float")
                {
                    op.Value = "1.0";
                }
                else
                {
                    // Use reference size (Left Operand Size) to determine padding for the default value of 1
                    // If this is the Left Operand itself, we use its own size (which is passed in as referenceSize)
                    int padding = GetPaddingForSize(referenceSize);
                    op.Value = "0x" + 1.ToString("X" + padding);
                }
            }

            // --- Step 2: Reinterpret the value's bit pattern if needed ---
            var integerLikeTypes = new HashSet<string> { "Mem", "Value", "Delta", "Prior", "BCD", "Inverted", "Recall" };
            bool wasIntegerLike = integerLikeTypes.Contains(oldType);
            bool isIntegerLike = integerLikeTypes.Contains(newType);

            if (wasIntegerLike && newType == "Float")
            {
                if (uint.TryParse(op.Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint hexVal))
                {
                    float floatVal = BitConverter.ToSingle(BitConverter.GetBytes(hexVal), 0);
                    op.Value = floatVal.ToString("G9", CultureInfo.InvariantCulture);
                }
            }
            else if (oldType == "Float" && isIntegerLike)
            {
                if (float.TryParse(op.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatVal))
                {
                    uint hexVal = BitConverter.ToUInt32(BitConverter.GetBytes(floatVal), 0);
                    op.Value = $"0x{hexVal:X8}";
                }
            }

            // --- Step 3: Set the new type ---
            op.Type = newType;
        }


        private void HandleSizeChange(Operand op, string newSize)
        {
            string oldSize = op.Size;
            op.Size = newSize;

            bool isNewFloat = ReverseFloatSizeMap.ContainsKey(newSize);
            bool wasOldFloat = ReverseFloatSizeMap.ContainsKey(oldSize);

            // When switching to a float-based memory size, ensure the hex value is fully padded for clarity.
            if (isNewFloat && !wasOldFloat)
            {
                if (uint.TryParse(op.Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint val))
                {
                    op.Value = "0x" + val.ToString("X8");
                }
            }
        }

        // Implement consistent behavior when switching between flag types.
        private void HandleFlagChange(AchievementCondition cond, string oldFlag)
        {
            bool wasArithmetic = ArithmeticFlags.Contains(oldFlag);
            bool isArithmetic = ArithmeticFlags.Contains(cond.Flag);

            // Switching FROM Arithmetic TO Non-Arithmetic/Blank: Auto-populate with a default comparison.
            if (wasArithmetic && !isArithmetic)
            {
                cond.Operator = "=";
                cond.RightOperand = new Operand { Type = "Value", Value = "0x1" };
            }
            // Switching FROM Non-Arithmetic/Blank TO Arithmetic: Clear comparison fields.
            else if (!wasArithmetic && isArithmetic)
            {
                cond.Operator = "";
                cond.RightOperand = new Operand(); // Explicitly clear logic properties
            }
            // All other switches (e.g., Non-Arithmetic to Non-Arithmetic) will preserve the existing values.
        }

        // Simplify operator change logic; final validation is now handled by ApplyConditionLogicRules.
        private void HandleOperatorChange(AchievementCondition cond, string oldOperator)
        {
            bool wasEmpty = string.IsNullOrEmpty(oldOperator);
            bool isEmpty = string.IsNullOrEmpty(cond.Operator);

            // If an operator was added where there was none before
            if (wasEmpty && !isEmpty)
            {
                cond.RightOperand = new Operand { Type = "Value", Value = "0x1", Size = "" };
            }
            // If an operator was removed (set to None)
            else if (!wasEmpty && isEmpty)
            {
                cond.RightOperand = new Operand();
            }
        }

        private void ApplyConditionLogicRules(AchievementCondition cond)
        {
            ApplyOperandSizeRules(cond.LeftOperand);
            ApplyOperandSizeRules(cond.RightOperand);

            // --- Relational Size Rules ---
            if (cond.RightOperand.Type == "Value")
            {
                cond.RightOperand.Size = "";
            }

            // --- Flag/Operator Rules ---
            if (StrictComparisonFlags.Contains(cond.Flag))
            {
                if (!ComparisonOperators.Contains(cond.Operator) || string.IsNullOrEmpty(cond.Operator))
                {
                    cond.Operator = "=";
                    cond.RightOperand = new Operand { Type = "Value", Value = "0x1" };
                }
            }
            else if (ArithmeticFlags.Contains(cond.Flag))
            {
                if (ComparisonOperators.Contains(cond.Operator))
                {
                    cond.Operator = "";
                    cond.RightOperand = new Operand();
                }
            }
        }

        private void ApplyOperandSizeRules(Operand op)
        {
            var memTypes = new HashSet<string> { "Mem", "Delta", "Prior", "BCD", "Inverted" };

            if (op.Type is "Value" or "Float" or "Recall")
            {
                op.Size = "";
            }
            else if (memTypes.Contains(op.Type))
            {
                if (string.IsNullOrEmpty(op.Size))
                {
                    op.Size = "32-bit";
                }
            }
        }
        #endregion

        #region Actions, Context Menu & Shortcuts

        private void triggerGrid_KeyDown(object? sender, KeyEventArgs e)
        {
            if (triggerGrid.IsCurrentCellInEditMode) return;

            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteLogic();
                e.Handled = true;
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.C)
            {
                CopySelectedRowsAligned(); // New Aligned Copy
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                CopySelectedLogic(); // New: Copy Logic string by default
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.X)
            {
                CutSelectedLogic(); // New: Cut
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.D)
            {
                DuplicateSelectedItems();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedItems();
                e.Handled = true;
            }
            else if (e.Alt && e.KeyCode == Keys.Up)
            {
                MoveSelectedItems(-1);
                e.Handled = true;
            }
            else if (e.Alt && e.KeyCode == Keys.Down)
            {
                MoveSelectedItems(1);
                e.Handled = true;
            }
        }

        // ... [Context Menu Opening, Copy/Paste Logic, Move/Delete/Duplicate - Unchanged] ...
        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            bool anySelected = triggerGrid.SelectedRows.Count > 0;
            moveUpToolStripMenuItem.Enabled = anySelected;
            moveDownToolStripMenuItem.Enabled = anySelected;
            deleteToolStripMenuItem.Enabled = anySelected;
            duplicateToolStripMenuItem.Enabled = anySelected;
            pasteToolStripMenuItem.Enabled = Clipboard.ContainsText();
            copyRowToolStripMenuItem.Enabled = anySelected;
            copyLogicToolStripMenuItem.Enabled = anySelected;

            if (anySelected)
            {
                var selectedIndices = triggerGrid.SelectedRows.Cast<DataGridViewRow>().Select(r => r.Index).ToList();
                moveUpToolStripMenuItem.Enabled = selectedIndices.Min() > 0;
                moveDownToolStripMenuItem.Enabled = selectedIndices.Max() < _conditions.Count - 1;
            }
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            // Default Copy button now copies logic string
            CopySelectedLogic();
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear this logic?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                TriggerText = "";
            }
        }
        private void showDecimalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _showDecimal = showDecimalCheckBox.Checked;
            PopulateGridFromConditions();
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) => PasteLogic();

        private void PasteLogic()
        {
            if (!Clipboard.ContainsText()) return;
            var clipboardText = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboardText)) return;

            if (!clipboardText.Contains("0x") && !clipboardText.Contains(':') && !clipboardText.Contains("_")) return;

            var pastedConditions = AchievementParser.ParseAchievementTrigger(clipboardText)
                .SelectMany(g => g.Conditions)
                .ToList();

            if (!pastedConditions.Any()) return;

            int insertionIndex = _conditions.Count;
            if (triggerGrid.SelectedRows.Count > 0)
            {
                var lastRow = triggerGrid.SelectedRows.Cast<DataGridViewRow>().OrderByDescending(r => r.Index).First();
                if (lastRow.Tag is AchievementCondition c)
                {
                    insertionIndex = _conditions.IndexOf(c) + 1;
                }
            }

            _conditions.InsertRange(insertionIndex, pastedConditions);

            PopulateGridFromConditions();

            // Try to restore selection to pasted items (safely)
            triggerGrid.ClearSelection();
            // Since PopulateGrid recreates rows, we need to find the rows corresponding to pasted conditions
            foreach (DataGridViewRow row in triggerGrid.Rows)
            {
                if (row.Tag is AchievementCondition c && pastedConditions.Contains(c))
                {
                    row.Selected = true;
                }
            }

            BuildTriggerAndNotify();
            StatusUpdateRequested?.Invoke(this, "Pasted logic from clipboard.");
        }

        private void copyRowToolStripMenuItem_Click(object sender, EventArgs e) => CopySelectedRowsAligned();

        // REPLACED with Aligned Version
        private void CopySelectedRowsAligned()
        {
            if (triggerGrid.SelectedRows.Count == 0) return;
            var rows = triggerGrid.SelectedRows.Cast<DataGridViewRow>().OrderBy(r => r.Index).ToList();
            int colCount = triggerGrid.Columns.Count;
            int[] colWidths = new int[colCount];

            // 1. Calculate Max Widths
            for (int c = 0; c < colCount; c++)
            {
                int max = 0;
                foreach (var row in rows)
                {
                    string txt = row.Cells[c].FormattedValue?.ToString() ?? "";
                    if (txt.Length > max) max = txt.Length;
                }
                colWidths[c] = max;
            }

            // 2. Build String with Padding
            var sb = new StringBuilder();
            foreach (var row in rows)
            {
                for (int c = 0; c < colCount; c++)
                {
                    string txt = row.Cells[c].FormattedValue?.ToString() ?? "";
                    // Pad all except last column
                    if (c < colCount - 1)
                        sb.Append(txt.PadRight(colWidths[c] + 2)); // +2 spaces padding
                    else
                        sb.Append(txt);
                }
                sb.AppendLine();
            }
            Clipboard.SetText(sb.ToString());
            StatusUpdateRequested?.Invoke(this, $"Copied {rows.Count} row(s) aligned.");
        }

        private void copyLogicToolStripMenuItem_Click(object sender, EventArgs e) => CopySelectedLogic();

        // IMPLEMENTED CopySelectedLogic using Effective Selection
        private void CopySelectedLogic()
        {
            var conditionsToCopy = GetEffectiveSelection();
            if (conditionsToCopy.Count == 0) return;

            var sb = new StringBuilder();
            foreach (var cond in conditionsToCopy)
            {
                if (sb.Length > 0) sb.Append('_');
                sb.Append(ConditionToString(cond));
            }
            Clipboard.SetText(sb.ToString());
            StatusUpdateRequested?.Invoke(this, $"Copied logic for {conditionsToCopy.Count} line(s).");
        }

        // IMPLEMENTED Cut
        private void CutSelectedLogic()
        {
            CopySelectedLogic();
            DeleteSelectedItems();
            StatusUpdateRequested?.Invoke(this, "Cut logic to clipboard.");
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e) => MoveSelectedItems(-1);
        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e) => MoveSelectedItems(1);
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) => DeleteSelectedItems();
        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e) => DuplicateSelectedItems();

        private void addConditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newCondition = new AchievementCondition { LeftOperand = { Type = "Mem", Value = "0x0000" }, Operator = "=", RightOperand = { Type = "Value", Value = "0" } };
            int insertionIndex = _conditions.Count;
            if (triggerGrid.SelectedRows.Count > 0)
                insertionIndex = triggerGrid.SelectedRows.Cast<DataGridViewRow>().Max(r => r.Index) + 1;

            _conditions.Insert(insertionIndex, newCondition);
            PopulateGridFromConditions();
            BuildTriggerAndNotify();
            StatusUpdateRequested?.Invoke(this, "Added new condition.");

            triggerGrid.ClearSelection();
            if (insertionIndex < triggerGrid.Rows.Count)
            {
                var newRow = triggerGrid.Rows[insertionIndex];
                newRow.Selected = true;
                triggerGrid.CurrentCell = newRow.Cells[ColLValue.Index];
                triggerGrid.BeginEdit(true);
            }
        }

        // --- NEW HELPER: GetEffectiveSelection for Collapsed Chains ---
        private List<AchievementCondition> GetEffectiveSelection()
        {
            // Start with visually selected rows
            var selected = triggerGrid.SelectedRows.Cast<DataGridViewRow>()
                .Select(r => r.Tag as AchievementCondition)
                .Where(c => c != null)
                .OrderBy(c => _conditions.IndexOf(c!))
                .Cast<AchievementCondition>()
                .ToList();

            if (!_collapseChains || selected.Count == 0) return selected;

            var expanded = new List<AchievementCondition>();
            var processed = new HashSet<AchievementCondition>();

            foreach (var cond in selected)
            {
                int idx = _conditions.IndexOf(cond);
                if (idx == -1) continue;

                // Backtrack to find hidden "Add Address" chain
                var chain = new List<AchievementCondition>();
                int scan = idx - 1;
                while (scan >= 0)
                {
                    var prev = _conditions[scan];
                    if (prev.Flag == "Add Address")
                    {
                        chain.Insert(0, prev);
                        scan--;
                    }
                    else
                    {
                        break;
                    }
                }

                // Add chain items first
                foreach (var c in chain)
                {
                    if (processed.Add(c)) expanded.Add(c);
                }

                // Add the selected item
                if (processed.Add(cond)) expanded.Add(cond);
            }

            return expanded;
        }

        // MODIFIED: Move logic to handle Units (Chains)
        private void MoveSelectedItems(int distance)
        {
            if (triggerGrid.SelectedRows.Count == 0 || distance == 0) return;

            var selectedConditions = GetEffectiveSelection();
            if (selectedConditions.Count == 0) return;

            // Strategy: Reorder the list of conditions
            // 1. Extract all conditions into a list of "Moveable Units".
            var units = new List<List<AchievementCondition>>();

            if (_collapseChains)
            {
                var currentUnit = new List<AchievementCondition>();
                foreach (var cond in _conditions)
                {
                    currentUnit.Add(cond);
                    // A unit ends if it's NOT an AddAddress (leaf) OR if it's the last item
                    if (cond.Flag != "Add Address")
                    {
                        units.Add(new List<AchievementCondition>(currentUnit));
                        currentUnit.Clear();
                    }
                }
                // Handle trailing AddAddress (incomplete chain)
                if (currentUnit.Count > 0) units.Add(currentUnit);
            }
            else
            {
                // Normal mode: Each condition is a unit
                foreach (var cond in _conditions) units.Add(new List<AchievementCondition> { cond });
            }

            // 2. Identify selected units
            var selectedSet = new HashSet<AchievementCondition>(selectedConditions);
            var selectedUnitIndices = new HashSet<int>();

            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].Any(c => selectedSet.Contains(c)))
                    selectedUnitIndices.Add(i);
            }

            var sortedIndices = selectedUnitIndices.OrderBy(i => i).ToList();

            // 3. Move Logic
            if (distance < 0 && sortedIndices.Min() == 0) return; // Top
            if (distance > 0 && sortedIndices.Max() == units.Count - 1) return; // Bottom

            var newUnits = new List<List<AchievementCondition>>(units);

            if (distance < 0) // Up
            {
                foreach (var idx in sortedIndices)
                {
                    var unit = newUnits[idx];
                    newUnits.RemoveAt(idx);
                    newUnits.Insert(idx - 1, unit);
                }
            }
            else // Down
            {
                for (int i = sortedIndices.Count - 1; i >= 0; i--)
                {
                    var idx = sortedIndices[i];
                    var unit = newUnits[idx];
                    newUnits.RemoveAt(idx);
                    newUnits.Insert(idx + 1, unit);
                }
            }

            // 4. Flatten
            _conditions.Clear();
            foreach (var unit in newUnits) _conditions.AddRange(unit);

            PopulateGridFromConditions();

            // Restore Selection
            triggerGrid.ClearSelection();
            foreach (DataGridViewRow row in triggerGrid.Rows)
            {
                if (row.Tag is AchievementCondition c && selectedSet.Contains(c))
                {
                    row.Selected = true;
                }
            }

            BuildTriggerAndNotify();
            StatusUpdateRequested?.Invoke(this, $"Moved {selectedConditions.Count} line(s).");
        }

        private void DeleteSelectedItems()
        {
            if (triggerGrid.SelectedRows.Count == 0) return;

            // Use Effective Selection to delete hidden chains too
            var toDelete = GetEffectiveSelection();
            if (toDelete.Count == 0) return;

            foreach (var cond in toDelete)
            {
                _conditions.Remove(cond);
            }

            PopulateGridFromConditions();
            BuildTriggerAndNotify();
            StatusUpdateRequested?.Invoke(this, $"Deleted {toDelete.Count} line(s).");
        }

        private void DuplicateSelectedItems()
        {
            if (triggerGrid.SelectedRows.Count == 0) return;

            // Use Effective Selection to duplicate chains properly
            var originalConditions = GetEffectiveSelection();
            if (originalConditions.Count == 0) return;

            // Insert after the last item in the effective selection
            var lastCondition = originalConditions.Last();
            int insertIdx = _conditions.IndexOf(lastCondition) + 1;

            var clones = originalConditions.Select(c => c.Clone()).ToList();

            _conditions.InsertRange(insertIdx, clones);

            PopulateGridFromConditions();

            // Select the new duplicates
            triggerGrid.ClearSelection();
            foreach (DataGridViewRow row in triggerGrid.Rows)
            {
                if (row.Tag is AchievementCondition c && clones.Contains(c))
                {
                    row.Selected = true;
                }
            }

            BuildTriggerAndNotify();
            StatusUpdateRequested?.Invoke(this, $"Duplicated {clones.Count} line(s).");
        }
        #endregion
    }
}