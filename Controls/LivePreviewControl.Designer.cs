#nullable disable

namespace RARPEditor.Controls
{
    partial class LivePreviewControl
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
            this.livePreviewPanel = new System.Windows.Forms.Panel();
            this.previewLabel = new System.Windows.Forms.Label();
            this.lookupPreviewGrid = new RARPEditor.Controls.DoubleBufferedDataGridView();
            this.KeyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lookupSelectorComboBox = new System.Windows.Forms.ComboBox();
            this.livePreviewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lookupPreviewGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // livePreviewPanel
            // 
            this.livePreviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.livePreviewPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(49)))), ((int)(((byte)(54)))));
            this.livePreviewPanel.Controls.Add(this.previewLabel);
            this.livePreviewPanel.Location = new System.Drawing.Point(0, 0);
            this.livePreviewPanel.Name = "livePreviewPanel";
            this.livePreviewPanel.Size = new System.Drawing.Size(356, 158);
            this.livePreviewPanel.TabIndex = 0;
            // 
            // previewLabel
            // 
            this.previewLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.previewLabel.ForeColor = System.Drawing.Color.White;
            this.previewLabel.Location = new System.Drawing.Point(0, 0);
            this.previewLabel.Name = "previewLabel";
            this.previewLabel.Padding = new System.Windows.Forms.Padding(10);
            this.previewLabel.Size = new System.Drawing.Size(356, 158);
            this.previewLabel.TabIndex = 0;
            this.previewLabel.Text = "Live Preview Panel";
            this.previewLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // lookupPreviewGrid
            // 
            this.lookupPreviewGrid.AllowUserToAddRows = false;
            this.lookupPreviewGrid.AllowUserToDeleteRows = false;
            this.lookupPreviewGrid.AllowUserToResizeRows = false;
            this.lookupPreviewGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lookupPreviewGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lookupPreviewGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.KeyColumn,
            this.ValueColumn});
            this.lookupPreviewGrid.Location = new System.Drawing.Point(0, 192);
            this.lookupPreviewGrid.MultiSelect = false;
            this.lookupPreviewGrid.Name = "lookupPreviewGrid";
            this.lookupPreviewGrid.ReadOnly = true;
            this.lookupPreviewGrid.RowHeadersVisible = false;
            this.lookupPreviewGrid.RowTemplate.Height = 25;
            this.lookupPreviewGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.lookupPreviewGrid.Size = new System.Drawing.Size(356, 124);
            this.lookupPreviewGrid.TabIndex = 0;
            this.lookupPreviewGrid.Visible = false;
            this.lookupPreviewGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.lookupPreviewGrid_CellClick);
            // 
            // KeyColumn
            // 
            this.KeyColumn.HeaderText = "Key";
            this.KeyColumn.Name = "KeyColumn";
            this.KeyColumn.ReadOnly = true;
            // 
            // ValueColumn
            // 
            this.ValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ValueColumn.HeaderText = "Value";
            this.ValueColumn.Name = "ValueColumn";
            this.ValueColumn.ReadOnly = true;
            // 
            // lookupSelectorComboBox
            // 
            this.lookupSelectorComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lookupSelectorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lookupSelectorComboBox.FormattingEnabled = true;
            this.lookupSelectorComboBox.Location = new System.Drawing.Point(3, 163);
            this.lookupSelectorComboBox.Name = "lookupSelectorComboBox";
            this.lookupSelectorComboBox.Size = new System.Drawing.Size(350, 23);
            this.lookupSelectorComboBox.TabIndex = 1;
            this.lookupSelectorComboBox.Visible = false;
            this.lookupSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.lookupSelectorComboBox_SelectedIndexChanged);
            // 
            // LivePreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lookupSelectorComboBox);
            this.Controls.Add(this.lookupPreviewGrid);
            this.Controls.Add(this.livePreviewPanel);
            this.Name = "LivePreviewControl";
            this.Size = new System.Drawing.Size(356, 316);
            this.livePreviewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lookupPreviewGrid)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private Panel livePreviewPanel;
        private Label previewLabel;
        private DoubleBufferedDataGridView lookupPreviewGrid;
        private DataGridViewTextBoxColumn KeyColumn;
        private DataGridViewTextBoxColumn ValueColumn;
        private ComboBox lookupSelectorComboBox;
    }
}
#nullable restore