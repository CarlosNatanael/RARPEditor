using RARPEditor.Models;
using System.Linq;

namespace RARPEditor.Forms
{
    public enum MacroType
    {
        Lookup,
        Formatter
    }

    public partial class NewMacroForm : Form
    {
        public string MacroName => nameTextBox.Text;
        public MacroType MacroType => newLookupRadioButton.Checked ? MacroType.Lookup : MacroType.Formatter;

        private readonly RichPresenceScript _script;

        public NewMacroForm(RichPresenceScript script)
        {
            InitializeComponent();
            _script = script;
            ValidateName();
        }

        private void ValidateName()
        {
            string name = nameTextBox.Text;
            bool isValid = !string.IsNullOrWhiteSpace(name) && !name.Contains(" ");

            if (isValid)
            {
                // Check for duplicates, case-insensitive
                if (_script.Lookups.Any(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    isValid = false;
                }
            }

            okButton.Enabled = isValid;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateName();
        }
    }
}