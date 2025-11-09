#nullable disable

namespace RARPEditor.Controls
{
    partial class HelpAndValidationControl
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

        #region Component Designer generated code
        private void InitializeComponent()
        {
            this.helpRichTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // helpRichTextBox
            // 
            this.helpRichTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.helpRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.helpRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpRichTextBox.Location = new System.Drawing.Point(5, 5);
            this.helpRichTextBox.Name = "helpRichTextBox";
            this.helpRichTextBox.ReadOnly = true;
            this.helpRichTextBox.Size = new System.Drawing.Size(335, 290);
            this.helpRichTextBox.TabIndex = 0;
            this.helpRichTextBox.Text = "Select an item in the Project Explorer to see help and validation information.";
            // 
            // HelpAndValidationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.helpRichTextBox);
            this.Name = "HelpAndValidationControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(345, 300);
            this.ResumeLayout(false);
        }
        #endregion

        private RichTextBox helpRichTextBox;
    }
}