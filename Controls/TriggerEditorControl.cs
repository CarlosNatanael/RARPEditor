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
            SetupGrid();
            CreateEditingContextMenus();
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

            // Fix: Explicitly set columns that use context menus for editing to be NOT ReadOnly.
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
                var rowsToAdd = new DataGridViewRow[_conditions.Count];
                for (int i = 0; i < _conditions.Count; i++)
                {
                    var cond = _conditions[i];
                    var row = new DataGridViewRow();
                    row.CreateCells(triggerGrid);
                    cond.ID = i + 1;
                    SetRowValuesFromCondition(row, cond);
                    row.Tag = cond;
                    rowsToAdd[i] = row;
                }
                triggerGrid.Rows.AddRange(rowsToAdd);
                triggerGrid.ClearSelection();
                foreach (var index in selectedIndices)
                {
                    if (index < triggerGrid.Rows.Count)
                        triggerGrid.Rows[index].Selected = true;
                }

                if (firstDisplayedRow < triggerGrid.Rows.Count)
                {
                    try
                    {
                        triggerGrid.FirstDisplayedScrollingRowIndex = firstDisplayedRow;
                    }
                    catch { /* Can fail if grid is not visible */ }
                }
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
            row.Cells[ColLValue.Index].Value = FormatDisplayValue(cond.LeftOperand);
            row.Cells[ColCmp.Index].Value = cond.Operator;
            row.Cells[ColRType.Index].Value = cond.RightOperand.Type;
            row.Cells[ColRSize.Index].Value = cond.RightOperand.Size;
            row.Cells[ColRValue.Index].Value = FormatDisplayValue(cond.RightOperand, cond.LeftOperand.Size);

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
            }
            else
            {
                rTypeCell.ReadOnly = false;
                rTypeCell.Style.BackColor = SystemColors.Window;
                rValueCell.ReadOnly = false;
                rValueCell.Style.BackColor = SystemColors.Window;

                if (cond.RightOperand.Type is "Float" or "Value")
                {
                    rSizeCell.ReadOnly = true;
                    rSizeCell.Style.BackColor = SystemColors.ControlLight;
                }
                else
                {
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

        private string FormatDisplayValue(Operand operand, string sizeReference = "")
        {
            if (string.IsNullOrEmpty(operand.Value)) return "";

            if (long.TryParse(operand.Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long val))
            {
                if (operand.Type == "Value")
                {
                    if (_showDecimal)
                    {
                        return val.ToString();
                    }
                    else
                    {
                        int padding = GetPaddingForSize(sizeReference);
                        long mask = GetMaskForSize(sizeReference);
                        return "0x" + (val & mask).ToString($"X{padding}");
                    }
                }
                else if (operand.Type != "Float") // Mem, Delta, Prior etc.
                {
                    int padding = GetPaddingForSize(operand.Size);
                    return "0x" + val.ToString($"X{padding}");
                }
            }

            if (operand.Type == "Float")
            {
                if (decimal.TryParse(operand.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decVal) && decVal % 1 == 0 && !operand.Value.Contains('.'))
                {
                    return operand.Value + ".0";
                }
                return operand.Value;
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
                // If user enters a float literal, convert it to hex for storage.
                if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatVal))
                {
                    byte[] bytes = BitConverter.GetBytes(floatVal);
                    if (size.EndsWith("BE"))
                    {
                        Array.Reverse(bytes);
                    }
                    uint hexVal = BitConverter.ToUInt32(bytes, 0);
                    return "0x" + hexVal.ToString("X8");
                }
                // If user enters a hex value, validate and format it.
                if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    if (uint.TryParse(input.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint hexVal))
                    {
                        return "0x" + hexVal.ToString("X8");
                    }
                }
                return input; // Invalid format, return as-is for user to correct.
            }

            if (operandType == "Float")
            {
                return input;
            }

            long val;
            int padding;

            if (operandType == "Mem")
            {
                string hexInput = input.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? input.Substring(2) : input;
                if (!long.TryParse(hexInput, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out val))
                    return input;
                padding = 8;
            }
            else if (operandType == "Value")
            {
                if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    if (!long.TryParse(input.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out val))
                        return input;
                }
                else
                {
                    if (!long.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out val))
                        return input;
                }
                padding = GetPaddingForSize(size);
            }
            else
            {
                return input;
            }

            return "0x" + val.ToString($"X{padding}");
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
                cond.RightOperand.Value = ParseAndFormatValue(value, cond.RightOperand.Type, cond.LeftOperand.Size);
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
            // Fix: Reverted to checking the cell's style, as the ReadOnly property was causing cascading issues.
            // This correctly blocks interaction with visually disabled cells without breaking others.
            if (triggerGrid.Rows[rowIndex].Cells[columnIndex].Style.BackColor == SystemColors.ControlLight)
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
                    case var _ when _currentEditColumnIndex == ColLType.Index: HandleTypeChange(cond.LeftOperand, newValue, true); break;
                    case var _ when _currentEditColumnIndex == ColLSize.Index: HandleSizeChange(cond.LeftOperand, newValue); break;
                    case var _ when _currentEditColumnIndex == ColCmp.Index:
                        string oldOperator = cond.Operator;
                        cond.Operator = newValue;
                        HandleOperatorChange(cond, oldOperator);
                        break;
                    case var _ when _currentEditColumnIndex == ColRType.Index: HandleTypeChange(cond.RightOperand, newValue, false); break;
                    case var _ when _currentEditColumnIndex == ColRSize.Index: HandleSizeChange(cond.RightOperand, newValue); break;
                }

                ApplyConditionLogicRules(cond);
            }

            PopulateGridFromConditions();
            BuildTriggerAndNotify();
        }

        private void HandleTypeChange(Operand op, string newType, bool isLeftOperand)
        {
            string oldType = op.Type;
            if (oldType == newType) return;

            // --- Step 1: Handle special cases that reset everything ---
            if (newType == "Recall" && isLeftOperand)
            {
                op.Value = "";
                op.Type = newType;
                return;
            }

            if (string.IsNullOrEmpty(op.Value) || (oldType == "Recall" && isLeftOperand))
            {
                op.Value = (newType == "Float") ? "1.0" : "0x1";
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
                cond.RightOperand = new Operand();
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
            else if (e.Control && e.KeyCode == Keys.C)
            {
                CopySelectedRows();
                e.Handled = true;
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.C)
            {
                CopySelectedLogic();
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
            Clipboard.SetText(_triggerText);
            StatusUpdateRequested?.Invoke(this, "Logic copied to clipboard.");
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

            if (!clipboardText.Contains("0x") && !clipboardText.Contains(':') && !clipboardText.Contains("_"))
            {
                return;
            }

            var pastedConditions = AchievementParser.ParseAchievementTrigger(clipboardText)
                .SelectMany(g => g.Conditions)
                .ToList();

            if (!pastedConditions.Any()) return;

            int insertionIndex = _conditions.Count;
            if (triggerGrid.SelectedRows.Count > 0)
            {
                insertionIndex = triggerGrid.SelectedRows.Cast<DataGridViewRow>().Max(r => r.Index) + 1;
            }

            _conditions.InsertRange(insertionIndex, pastedConditions);

            PopulateGridFromConditions();

            if (insertionIndex < triggerGrid.Rows.Count)
            {
                triggerGrid.ClearSelection();
                triggerGrid.Rows[insertionIndex].Selected = true;
                triggerGrid.FirstDisplayedScrollingRowIndex = insertionIndex;
            }

            BuildTriggerAndNotify();
            StatusUpdateRequested?.Invoke(this, "Pasted logic from clipboard.");
        }

        private void copyRowToolStripMenuItem_Click(object sender, EventArgs e) => CopySelectedRows();
        private void CopySelectedRows()
        {
            if (triggerGrid.SelectedRows.Count == 0) return;
            var rows = triggerGrid.SelectedRows.Cast<DataGridViewRow>().ToList();
            var sb = new StringBuilder();

            foreach (var row in rows.OrderBy(r => r.Index))
            {
                var cellValues = row.Cells.Cast<DataGridViewCell>().Select(cell => cell.FormattedValue?.ToString() ?? "");
                sb.AppendLine(string.Join("\t", cellValues));
            }
            Clipboard.SetText(sb.ToString());
            StatusUpdateRequested?.Invoke(this, $"Copied {rows.Count} row(s) as text.");
        }

        private void copyLogicToolStripMenuItem_Click(object sender, EventArgs e) => CopySelectedLogic();
        private void CopySelectedLogic()
        {
            if (triggerGrid.SelectedRows.Count == 0) return;

            var sb = new StringBuilder();
            var selectedConditions = triggerGrid.SelectedRows
                .Cast<DataGridViewRow>()
                .OrderBy(r => r.Index)
                .Select(r => (AchievementCondition)r.Tag!);

            foreach (var cond in selectedConditions)
            {
                if (sb.Length > 0) sb.Append('_');
                sb.Append(ConditionToString(cond));
            }
            Clipboard.SetText(sb.ToString());
            StatusUpdateRequested?.Invoke(this, $"Copied logic for {selectedConditions.Count()} line(s).");
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

        private void MoveSelectedItems(int distance)
        {
            if (triggerGrid.SelectedRows.Count == 0) return;
            var selectedRows = triggerGrid.SelectedRows.Cast<DataGridViewRow>().ToList();
            var selectedIndices = selectedRows.Select(r => r.Index).ToList();
            var newSelectedIndices = new List<int>();
            int minIndex = selectedIndices.Min();
            int maxIndex = selectedIndices.Max();
            distance = Math.Max(distance, -minIndex);
            distance = Math.Min(distance, _conditions.Count - 1 - maxIndex);
            if (distance == 0) return;
            selectedIndices.Sort();
            if (distance > 0) selectedIndices.Reverse();
            foreach (int index in selectedIndices)
            {
                var item = _conditions[index];
                _conditions.RemoveAt(index);
                _conditions.Insert(index + distance, item);
                newSelectedIndices.Add(index + distance);
            }
            PopulateGridFromConditions();
            BuildTriggerAndNotify();
            StatusUpdateRequested?.Invoke(this, $"Moved {selectedRows.Count} line(s).");
            triggerGrid.ClearSelection();
            foreach (int newIndex in newSelectedIndices)
            {
                if (newIndex >= 0 && newIndex < triggerGrid.Rows.Count)
                    triggerGrid.Rows[newIndex].Selected = true;
            }
        }

        private void DeleteSelectedItems()
        {
            if (triggerGrid.SelectedRows.Count == 0) return;
            var selectedRows = triggerGrid.SelectedRows.Cast<DataGridViewRow>().ToList();
            var selectedIndices = selectedRows.Select(r => r.Index).OrderByDescending(i => i).ToList();

            foreach (var index in selectedIndices)
            {
                _conditions.RemoveAt(index);
            }

            PopulateGridFromConditions();
            BuildTriggerAndNotify();
            StatusUpdateRequested?.Invoke(this, $"Deleted {selectedRows.Count} line(s).");
        }

        private void DuplicateSelectedItems()
        {
            if (triggerGrid.SelectedRows.Count == 0) return;

            var selectedRows = triggerGrid.SelectedRows.Cast<DataGridViewRow>()
                .OrderBy(r => r.Index)
                .ToList();

            int insertionIndex = selectedRows.Max(r => r.Index) + 1;

            var clonedConditions = selectedRows
                .Select(row => (AchievementCondition)row.Tag!)
                .Select(cond => cond.Clone())
                .ToList();

            _conditions.InsertRange(insertionIndex, clonedConditions);

            PopulateGridFromConditions();
            BuildTriggerAndNotify();
            StatusUpdateRequested?.Invoke(this, $"Duplicated {selectedRows.Count} line(s).");

            // Select the newly created rows.
            triggerGrid.ClearSelection();
            for (int i = 0; i < clonedConditions.Count; i++)
            {
                triggerGrid.Rows[insertionIndex + i].Selected = true;
            }
        }
        #endregion
    }
}