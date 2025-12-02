using RARPEditor.Models;
using RARPEditor.Logic;

namespace RARPEditor.Controls
{
    public partial class FormatterEditorControl : UserControl
    {
        private RichPresenceLookup? _currentFormatter;
        private RichPresenceScript? _currentScript;
        private Action? _dataChangedAction;
        private bool _isProgrammaticallyChanging;
        // Store the original name to prevent creating undo states when no changes are made.
        private string _originalName = "";

        public FormatterEditorControl()
        {
            InitializeComponent();
            formatTypeComboBox.Items.AddRange(RichPresenceLookup.BuiltInFormatters);
        }

        public void LoadFormatter(RichPresenceLookup formatter, RichPresenceScript script, Action dataChangedAction)
        {
            _currentFormatter = formatter;
            _currentScript = script;
            _dataChangedAction = dataChangedAction;
            _isProgrammaticallyChanging = true;

            nameTextBox.Text = _currentFormatter.Name;
            // Store the name when the control is loaded.
            _originalName = _currentFormatter.Name;

            int formatIndex = Array.IndexOf(RichPresenceLookup.BuiltInFormatters, _currentFormatter.Format.ToUpper());
            formatTypeComboBox.SelectedIndex = formatIndex >= 0 ? formatIndex : 0;

            ValidateName();
            _isProgrammaticallyChanging = false;
        }

        private void ValidateName()
        {
            if (_currentScript == null || _currentFormatter == null) return;

            var errors = ScriptValidator.Validate(_currentFormatter, _currentScript);
            nameTextBox.BackColor = errors.Any() ? Color.LightCoral : SystemColors.Window;
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_isProgrammaticallyChanging || _currentFormatter == null) return;
            _currentFormatter.Name = nameTextBox.Text;
            ValidateName();
            // The expensive data changed action is no longer called on every keystroke.
        }

        // Create a single, atomic undo action when editing is complete.
        private void nameTextBox_Leave(object sender, EventArgs e)
        {
            // Only trigger the data changed action if the text has actually changed.
            if (_currentFormatter != null && _originalName != _currentFormatter.Name)
            {
                _dataChangedAction?.Invoke();
                _originalName = _currentFormatter.Name; // Update the original value for the next comparison
            }
        }

        private void formatTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isProgrammaticallyChanging || _currentFormatter == null) return;
            _currentFormatter.Format = formatTypeComboBox.SelectedItem?.ToString() ?? "VALUE";
            _dataChangedAction?.Invoke();
        }
    }
}