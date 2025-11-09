using RARPEditor.Models;
using RARPEditor.Forms;
using System.Text;
using System.Text.RegularExpressions;
using RARPEditor.Parsers;

namespace RARPEditor.Controls
{
    public class NewMacroEventArgs : EventArgs
    {
        public string Name { get; }
        public MacroType Type { get; }

        public NewMacroEventArgs(string name, MacroType type)
        {
            Name = name;
            Type = type;
        }
    }

    public partial class DisplayLogicEditorControl : UserControl
    {
        private RichPresenceScript? _currentScript;
        private RichPresenceDisplayString? _currentDisplayString;
        private Action? _dataChangedAction;
        private bool _isProgrammaticallyChanging;
        private string _originalMasterTemplate = "";

        private readonly TriggerEditorControl _conditionEditor;
        private readonly List<TriggerEditorControl> _triggerEditorPool = new();

        private readonly List<AchievementConditionGroup> _currentMacroValueGroups = new();

        private static readonly Regex MacroRegex = new(@"@([_a-zA-Z0-9]+)\(([^)]*)\)", RegexOptions.Compiled);

        public event EventHandler<NewMacroEventArgs>? NewMacroRequested;
        public event EventHandler<string>? StatusUpdateRequested;

        public DisplayLogicEditorControl()
        {
            InitializeComponent();
            _conditionEditor = new TriggerEditorControl { Dock = DockStyle.Fill };
            _conditionEditor.TriggerTextChanged += ConditionEditor_TriggerTextChanged;
            _conditionEditor.StatusUpdateRequested += (s, msg) => StatusUpdateRequested?.Invoke(this, msg);
        }

        public void LoadDisplayString(RichPresenceDisplayString displayString, RichPresenceScript script, Action dataChangedAction)
        {
            // Fix: Store the currently selected value group tab index to restore it after reload.
            int selectedValueGroupIndex = valueGroupTabControl.SelectedIndex;

            _currentDisplayString = displayString;
            _currentScript = script;
            _dataChangedAction = dataChangedAction;
            _isProgrammaticallyChanging = true;

            bool isDefault = displayString.IsDefault;
            masterDisplayTextBox.ContextMenuStrip = isDefault ? null : macroContextMenu;
            logicSelectorComboBox.Enabled = !isDefault;
            masterDisplayTextBox.Text = ReconstructMasterTemplate();
            _originalMasterTemplate = masterDisplayTextBox.Text;

            UpdateLogicSelector();
            LoadSelectedLogic();

            // Fix: Restore the selected tab index if it was stored and is still valid.
            if (selectedValueGroupIndex >= 0 && selectedValueGroupIndex < valueGroupTabControl.TabCount)
            {
                valueGroupTabControl.SelectedIndex = selectedValueGroupIndex;
            }

            _isProgrammaticallyChanging = false;
        }

        private void ConditionEditor_TriggerTextChanged(object? sender, EventArgs e)
        {
            if (_currentDisplayString != null && logicSelectorComboBox.SelectedItem?.ToString() == "Condition")
            {
                _currentDisplayString.Condition = _conditionEditor.TriggerText;
                _dataChangedAction?.Invoke();
            }
        }

        private TriggerEditorControl GetOrCreateTriggerEditor()
        {
            if (_triggerEditorPool.Any())
            {
                var editor = _triggerEditorPool.First();
                _triggerEditorPool.RemoveAt(0);
                return editor;
            }
            var newEditor = new TriggerEditorControl { Dock = DockStyle.Fill };
            newEditor.StatusUpdateRequested += (s, msg) => StatusUpdateRequested?.Invoke(this, msg);
            return newEditor;
        }

        private void RecycleAllMacroEditors()
        {
            // Also recycle the main condition editor if it's in a tab
            if (_conditionEditor.Parent != null)
            {
                _conditionEditor.Parent.Controls.Remove(_conditionEditor);
            }

            foreach (var tab in valueGroupTabControl.TabPages.Cast<TabPage>())
            {
                if (tab.Controls.Count > 0 && tab.Controls[0] is TriggerEditorControl editor && editor != _conditionEditor)
                {
                    _triggerEditorPool.Add(editor);
                    tab.Controls.Clear();
                }
            }
        }

        private string ReconstructMasterTemplate()
        {
            if (_currentDisplayString == null) return "";
            var sb = new StringBuilder();
            foreach (var part in _currentDisplayString.Parts)
            {
                sb.Append(part.IsMacro ? $"{{{part.Text}}}" : part.Text);
            }
            return sb.ToString();
        }

        private void masterDisplayTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_isProgrammaticallyChanging || _currentDisplayString == null) return;

            UpdatePartsFromMasterTemplate();

            if (_currentDisplayString.IsDefault)
            {
                if (_currentDisplayString.Parts.Any(p => p.IsMacro))
                {
                    var sanitizedText = string.Join("", _currentDisplayString.Parts.Select(p => p.Text));
                    _isProgrammaticallyChanging = true;
                    var selectionStart = masterDisplayTextBox.SelectionStart;
                    masterDisplayTextBox.Text = sanitizedText;
                    masterDisplayTextBox.SelectionStart = selectionStart > sanitizedText.Length ? sanitizedText.Length : selectionStart;
                    _currentDisplayString.Parts.Clear();
                    _currentDisplayString.Parts.Add(new RichPresenceDisplayPart { Text = sanitizedText });
                    _isProgrammaticallyChanging = false;
                }
            }
            else
            {
                UpdateLogicSelector();
            }
        }

        private void masterDisplayTextBox_Leave(object sender, EventArgs e)
        {
            if (_originalMasterTemplate != masterDisplayTextBox.Text)
            {
                _dataChangedAction?.Invoke();
                _originalMasterTemplate = masterDisplayTextBox.Text;
            }
        }

        private void UpdatePartsFromMasterTemplate()
        {
            if (_currentDisplayString == null) return;

            string template = masterDisplayTextBox.Text;

            string parsableTemplate = Regex.Replace(template, @"\{([_a-zA-Z0-9]+)\}", match =>
            {
                string macroName = match.Groups[1].Value;
                var existingPart = _currentDisplayString.Parts.FirstOrDefault(p => p.IsMacro && string.Equals(p.Text, macroName, StringComparison.OrdinalIgnoreCase));
                return $"@{macroName}({existingPart?.Parameter ?? ""})";
            });

            var newParts = new List<RichPresenceDisplayPart>();
            int lastIndex = 0;

            foreach (Match match in MacroRegex.Matches(parsableTemplate).Cast<Match>())
            {
                if (match.Index > lastIndex)
                {
                    newParts.Add(new RichPresenceDisplayPart { IsMacro = false, Text = parsableTemplate.Substring(lastIndex, match.Index - lastIndex) });
                }

                string macroName = match.Groups[1].Value;
                string macroParam = match.Groups[2].Value;

                var existingPart = _currentDisplayString.Parts.FirstOrDefault(p => p.IsMacro && string.Equals(p.Text, macroName, StringComparison.OrdinalIgnoreCase));
                if (existingPart != null)
                {
                    existingPart.Parameter = macroParam;
                    newParts.Add(existingPart);
                }
                else
                {
                    newParts.Add(new RichPresenceDisplayPart { IsMacro = true, Text = macroName, Parameter = macroParam });
                }

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < parsableTemplate.Length)
            {
                newParts.Add(new RichPresenceDisplayPart { IsMacro = false, Text = parsableTemplate.Substring(lastIndex) });
            }

            _currentDisplayString.Parts.Clear();
            _currentDisplayString.Parts.AddRange(newParts);
        }

        private void UpdateLogicSelector()
        {
            if (_currentDisplayString == null || _currentDisplayString.IsDefault)
            {
                logicSelectorComboBox.Items.Clear();
                return;
            }

            var selectedItem = logicSelectorComboBox.SelectedItem?.ToString();
            _isProgrammaticallyChanging = true;

            var macrosInText = Regex.Matches(masterDisplayTextBox.Text, @"\{([_a-zA-Z0-9]+)\}")
                                .Cast<Match>()
                                .Select(m => m.Groups[1].Value)
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToList();

            logicSelectorComboBox.Items.Clear();
            logicSelectorComboBox.Items.Add("Condition");
            if (macrosInText.Any())
            {
                logicSelectorComboBox.Items.AddRange(macrosInText.ToArray());
            }

            if (selectedItem != null && logicSelectorComboBox.Items.Contains(selectedItem))
            {
                logicSelectorComboBox.SelectedItem = selectedItem;
            }
            else if (logicSelectorComboBox.Items.Count > 0)
            {
                logicSelectorComboBox.SelectedIndex = 0;
            }

            _isProgrammaticallyChanging = false;
        }

        private void logicSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isProgrammaticallyChanging) return;
            LoadSelectedLogic();
        }

        private void LoadSelectedLogic()
        {
            _isProgrammaticallyChanging = true;
            RecycleAllMacroEditors();
            valueGroupTabControl.TabPages.Clear();
            _currentMacroValueGroups.Clear();

            var selectedItem = logicSelectorComboBox.SelectedItem?.ToString();

            if (selectedItem == "Condition" || _currentDisplayString?.IsDefault == true)
            {
                valueGroupToolStrip.Visible = false;
                _conditionEditor.Enabled = _currentDisplayString?.IsDefault == false;
                _conditionEditor.GroupBoxText = "Display Condition";
                _conditionEditor.GroupSeparator = 'S';
                _conditionEditor.TriggerText = _currentDisplayString?.Condition ?? "";

                var page = new TabPage("Core Logic");
                page.Controls.Add(_conditionEditor);
                valueGroupTabControl.TabPages.Add(page);
            }
            else if (selectedItem != null && _currentDisplayString != null)
            {
                valueGroupToolStrip.Visible = true;
                var macroPart = _currentDisplayString.Parts.FirstOrDefault(p => p.IsMacro && string.Equals(p.Text, selectedItem, StringComparison.OrdinalIgnoreCase));
                if (macroPart != null)
                {
                    _currentMacroValueGroups.AddRange(AchievementParser.ParseAchievementTrigger(macroPart.Parameter, '$'));
                    if (!_currentMacroValueGroups.Any())
                    {
                        _currentMacroValueGroups.Add(new AchievementConditionGroup { GroupName = "Value Group 1" });
                    }
                    PopulateValueGroupTabs();
                }
            }

            _isProgrammaticallyChanging = false;
        }

        private void PopulateValueGroupTabs()
        {
            valueGroupTabControl.TabPages.Clear();
            for (int i = 0; i < _currentMacroValueGroups.Count; i++)
            {
                var group = _currentMacroValueGroups[i];
                var page = new TabPage($"Value Group {i + 1}") { Tag = i };
                var editor = GetOrCreateTriggerEditor();

                editor.GroupSeparator = '$';
                var groupText = string.Join("_", group.Conditions.Select(TriggerEditorControl.ConditionToString));
                editor.TriggerText = groupText;
                editor.GroupBoxText = $"Logic for Value Group {i + 1}";

                var field = typeof(TriggerEditorControl).GetField("TriggerTextChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field != null) field.SetValue(editor, null);

                editor.TriggerTextChanged += (s, e) => RebuildMacroParameter();

                page.Controls.Add(editor);
                valueGroupTabControl.TabPages.Add(page);
            }
            removeGroupButton.Enabled = valueGroupTabControl.TabPages.Count > 1;
        }

        private void RebuildMacroParameter()
        {
            if (_isProgrammaticallyChanging) return;
            var macroName = logicSelectorComboBox.SelectedItem?.ToString();
            if (macroName == null || macroName == "Condition" || _currentDisplayString == null) return;

            var macroPart = _currentDisplayString.Parts.FirstOrDefault(p => p.IsMacro && string.Equals(p.Text, macroName, StringComparison.OrdinalIgnoreCase));
            if (macroPart == null) return;

            var groupTexts = new List<string>();
            foreach (TabPage page in valueGroupTabControl.TabPages)
            {
                if (page.Controls.Count > 0 && page.Controls[0] is TriggerEditorControl editor)
                {
                    groupTexts.Add(editor.TriggerText);
                }
            }

            macroPart.Parameter = string.Join("$", groupTexts);
            _dataChangedAction?.Invoke();
        }

        private void addGroupButton_Click(object sender, EventArgs e)
        {
            _currentMacroValueGroups.Add(new AchievementConditionGroup());
            PopulateValueGroupTabs();
            valueGroupTabControl.SelectedIndex = valueGroupTabControl.TabCount - 1;
            RebuildMacroParameter();
        }

        private void removeGroupButton_Click(object sender, EventArgs e)
        {
            if (valueGroupTabControl.SelectedIndex < 0) return;
            int index = valueGroupTabControl.SelectedIndex;

            _currentMacroValueGroups.RemoveAt(index);
            PopulateValueGroupTabs();
            valueGroupTabControl.SelectedIndex = Math.Min(index, valueGroupTabControl.TabCount - 1);
            RebuildMacroParameter();
        }

        private void macroContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            while (macroContextMenu.Items.Count > 2)
            {
                macroContextMenu.Items.RemoveAt(2);
            }

            if (_currentScript == null) return;

            var lookups = _currentScript.Lookups.Where(l => l.Entries.Any() || l.Default != null).ToList();
            var formatters = _currentScript.Lookups.Where(l => !l.Entries.Any() && l.Default == null).ToList();

            if (lookups.Any())
            {
                foreach (var lookup in lookups)
                {
                    var item = new ToolStripMenuItem($"Insert {{{lookup.Name}}}", null, InsertMacro_Click) { Tag = lookup.Name };
                    macroContextMenu.Items.Add(item);
                }
            }

            if (formatters.Any() && lookups.Any())
            {
                macroContextMenu.Items.Add(new ToolStripSeparator());
            }

            if (formatters.Any())
            {
                foreach (var formatter in formatters)
                {
                    var item = new ToolStripMenuItem($"Insert {{{formatter.Name}}}", null, InsertMacro_Click) { Tag = formatter.Name };
                    macroContextMenu.Items.Add(item);
                }
            }
        }

        private void InsertMacro_Click(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem item || item.Tag is not string macroName) return;
            masterDisplayTextBox.SelectedText = $"{{{macroName}}}";
        }

        private void insertNewMacroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentScript == null) return;

            using var form = new NewMacroForm(_currentScript);
            if (form.ShowDialog() == DialogResult.OK)
            {
                NewMacroRequested?.Invoke(this, new NewMacroEventArgs(form.MacroName, form.MacroType));
                masterDisplayTextBox.SelectedText = $"{{{form.MacroName}}}";
            }
        }
    }
}