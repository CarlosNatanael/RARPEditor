#nullable disable

namespace RARPEditor.Controls
{
    partial class FormatterEditorControl
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
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.nameLabel = new System.Windows.Forms.Label();
            this.formatTypeLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.formatTypeComboBox = new System.Windows.Forms.ComboBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.mainTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.ColumnCount = 2;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Controls.Add(this.nameLabel, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.formatTypeLabel, 0, 1);
            this.mainTableLayoutPanel.Controls.Add(this.nameTextBox, 1, 0);
            this.mainTableLayoutPanel.Controls.Add(this.formatTypeComboBox, 1, 1);
            this.mainTableLayoutPanel.Controls.Add(this.descriptionLabel, 0, 2);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.Padding = new System.Windows.Forms.Padding(10);
            this.mainTableLayoutPanel.RowCount = 3;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(484, 261);
            this.mainTableLayoutPanel.TabIndex = 0;
            // 
            // nameLabel
            // 
            this.nameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(13, 17);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(39, 15);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Name";
            // 
            // formatTypeLabel
            // 
            this.formatTypeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.formatTypeLabel.AutoSize = true;
            this.formatTypeLabel.Location = new System.Drawing.Point(13, 47);
            this.formatTypeLabel.Name = "formatTypeLabel";
            this.formatTypeLabel.Size = new System.Drawing.Size(73, 15);
            this.formatTypeLabel.TabIndex = 1;
            this.formatTypeLabel.Text = "Format Type";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameTextBox.Location = new System.Drawing.Point(92, 13);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(379, 23);
            this.nameTextBox.TabIndex = 2;
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            this.nameTextBox.Leave += new System.EventHandler(this.nameTextBox_Leave);
            // 
            // formatTypeComboBox
            // 
            this.formatTypeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.formatTypeComboBox.FormattingEnabled = true;
            this.formatTypeComboBox.Location = new System.Drawing.Point(92, 42);
            this.formatTypeComboBox.Name = "formatTypeComboBox";
            this.formatTypeComboBox.Size = new System.Drawing.Size(379, 23);
            this.formatTypeComboBox.TabIndex = 3;
            this.formatTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.formatTypeComboBox_SelectedIndexChanged);
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.mainTableLayoutPanel.SetColumnSpan(this.descriptionLabel, 2);
            this.descriptionLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.descriptionLabel.Location = new System.Drawing.Point(13, 68);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.descriptionLabel.Size = new System.Drawing.Size(437, 40);
            this.descriptionLabel.TabIndex = 4;
            this.descriptionLabel.Text = "Formatters define how raw memory values are displayed.\r\nExample: A value of 3600" +
    " with a Format Type of SECS will be displayed as \"1:00:00\".";
            // 
            // FormatterEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Name = "FormatterEditorControl";
            this.Size = new System.Drawing.Size(484, 261);
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel mainTableLayoutPanel;
        private Label nameLabel;
        private Label formatTypeLabel;
        private TextBox nameTextBox;
        private ComboBox formatTypeComboBox;
        private Label descriptionLabel;
    }
}
#nullable restore