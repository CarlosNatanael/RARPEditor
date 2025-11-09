#nullable disable

namespace RARPEditor.Forms
{
    partial class NewMacroForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.nameLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.typeGroupBox = new System.Windows.Forms.GroupBox();
            this.newFormatterRadioButton = new System.Windows.Forms.RadioButton();
            this.newLookupRadioButton = new System.Windows.Forms.RadioButton();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.typeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(12, 15);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(79, 15);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Macro Name:";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(97, 12);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(225, 23);
            this.nameTextBox.TabIndex = 0;
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // typeGroupBox
            // 
            this.typeGroupBox.Controls.Add(this.newFormatterRadioButton);
            this.typeGroupBox.Controls.Add(this.newLookupRadioButton);
            this.typeGroupBox.Location = new System.Drawing.Point(12, 41);
            this.typeGroupBox.Name = "typeGroupBox";
            this.typeGroupBox.Size = new System.Drawing.Size(310, 80);
            this.typeGroupBox.TabIndex = 1;
            this.typeGroupBox.TabStop = false;
            this.typeGroupBox.Text = "Macro Type";
            // 
            // newFormatterRadioButton
            // 
            this.newFormatterRadioButton.AutoSize = true;
            this.newFormatterRadioButton.Location = new System.Drawing.Point(15, 47);
            this.newFormatterRadioButton.Name = "newFormatterRadioButton";
            this.newFormatterRadioButton.Size = new System.Drawing.Size(227, 19);
            this.newFormatterRadioButton.TabIndex = 1;
            this.newFormatterRadioButton.Text = "New Formatter (Formats a raw number)";
            this.newFormatterRadioButton.UseVisualStyleBackColor = true;
            // 
            // newLookupRadioButton
            // 
            this.newLookupRadioButton.AutoSize = true;
            this.newLookupRadioButton.Checked = true;
            this.newLookupRadioButton.Location = new System.Drawing.Point(15, 22);
            this.newLookupRadioButton.Name = "newLookupRadioButton";
            this.newLookupRadioButton.Size = new System.Drawing.Size(212, 19);
            this.newLookupRadioButton.TabIndex = 0;
            this.newLookupRadioButton.TabStop = true;
            this.newLookupRadioButton.Text = "New Lookup (Maps a value to text)";
            this.newLookupRadioButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(166, 127);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(247, 127);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // NewMacroForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(334, 161);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.typeGroupBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.nameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewMacroForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create New Macro";
            this.typeGroupBox.ResumeLayout(false);
            this.typeGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private Label nameLabel;
        private TextBox nameTextBox;
        private GroupBox typeGroupBox;
        private RadioButton newFormatterRadioButton;
        private RadioButton newLookupRadioButton;
        private Button okButton;
        private Button cancelButton;
    }
}
#nullable restore