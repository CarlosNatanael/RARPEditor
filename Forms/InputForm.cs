using System;
using System.Windows.Forms;

namespace RARPEditor.Forms
{
    public partial class InputForm : Form
    {
        public string InputValue => inputTextBox.Text;

        public InputForm(string title, string prompt, string defaultValue = "")
        {
            InitializeComponent();
            this.Text = title;
            promptLabel.Text = prompt;
            inputTextBox.Text = defaultValue;
            inputTextBox.SelectAll();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                MessageBox.Show("The value cannot be empty.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}