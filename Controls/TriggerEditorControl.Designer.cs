#nullable disable

namespace RARPEditor.Controls
{
    partial class TriggerEditorControl
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
            this.components = new System.ComponentModel.Container();
            this.mainGroupBox = new System.Windows.Forms.GroupBox();
            this.triggerGrid = new RARPEditor.Controls.DoubleBufferedDataGridView();
            this.ColID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColFlag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCmp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColRType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColRSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColRValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColHits = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addConditionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.copyLogicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbarPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.showDecimalCheckBox = new System.Windows.Forms.CheckBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.mainGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.triggerGrid)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.toolbarPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainGroupBox
            // 
            this.mainGroupBox.Controls.Add(this.triggerGrid);
            this.mainGroupBox.Controls.Add(this.toolbarPanel);
            this.mainGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainGroupBox.Location = new System.Drawing.Point(0, 0);
            this.mainGroupBox.Name = "mainGroupBox";
            this.mainGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.mainGroupBox.Size = new System.Drawing.Size(700, 300);
            this.mainGroupBox.TabIndex = 0;
            this.mainGroupBox.TabStop = false;
            this.mainGroupBox.Text = "Logic";
            // 
            // triggerGrid
            // 
            this.triggerGrid.AllowUserToAddRows = false;
            this.triggerGrid.AllowUserToDeleteRows = false;
            this.triggerGrid.AllowUserToResizeRows = false;
            this.triggerGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.triggerGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.triggerGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColID,
            this.ColFlag,
            this.ColLType,
            this.ColLSize,
            this.ColLValue,
            this.ColCmp,
            this.ColRType,
            this.ColRSize,
            this.ColRValue,
            this.ColHits});
            this.triggerGrid.ContextMenuStrip = this.contextMenuStrip;
            this.triggerGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.triggerGrid.Location = new System.Drawing.Point(8, 63);
            this.triggerGrid.Name = "triggerGrid";
            this.triggerGrid.RowHeadersVisible = false;
            this.triggerGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.triggerGrid.Size = new System.Drawing.Size(684, 229);
            this.triggerGrid.TabIndex = 1;
            // 
            // ColID
            // 
            this.ColID.HeaderText = "ID";
            this.ColID.Name = "ColID";
            this.ColID.ReadOnly = true;
            this.ColID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColID.Width = 35;
            // 
            // ColFlag
            // 
            this.ColFlag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColFlag.FillWeight = 120F;
            this.ColFlag.HeaderText = "Flag";
            this.ColFlag.MinimumWidth = 80;
            this.ColFlag.Name = "ColFlag";
            this.ColFlag.ReadOnly = true;
            this.ColFlag.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColFlag.ToolTipText = "Condition Type (e.g., Reset If, Pause If)";
            // 
            // ColLType
            // 
            this.ColLType.HeaderText = "Type";
            this.ColLType.Name = "ColLType";
            this.ColLType.ReadOnly = true;
            this.ColLType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColLType.ToolTipText = "Left Operand Type (Mem, Value, Delta, etc.)";
            this.ColLType.Width = 50;
            // 
            // ColLSize
            // 
            this.ColLSize.HeaderText = "Size";
            this.ColLSize.Name = "ColLSize";
            this.ColLSize.ReadOnly = true;
            this.ColLSize.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColLSize.ToolTipText = "Left Operand Memory Size (8-bit, 16-bit, etc.)";
            this.ColLSize.Width = 60;
            // 
            // ColLValue
            // 
            this.ColLValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColLValue.FillWeight = 130F;
            this.ColLValue.HeaderText = "Memory";
            this.ColLValue.MinimumWidth = 85;
            this.ColLValue.Name = "ColLValue";
            this.ColLValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColLValue.ToolTipText = "Left Operand Memory Address or Value";
            // 
            // ColCmp
            // 
            this.ColCmp.HeaderText = "Cmp";
            this.ColCmp.Name = "ColCmp";
            this.ColCmp.ReadOnly = true;
            this.ColCmp.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColCmp.ToolTipText = "Comparison Operator";
            this.ColCmp.Width = 40;
            // 
            // ColRType
            // 
            this.ColRType.HeaderText = "Type";
            this.ColRType.Name = "ColRType";
            this.ColRType.ReadOnly = true;
            this.ColRType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColRType.ToolTipText = "Right Operand Type";
            this.ColRType.Width = 50;
            // 
            // ColRSize
            // 
            this.ColRSize.HeaderText = "Size";
            this.ColRSize.Name = "ColRSize";
            this.ColRSize.ReadOnly = true;
            this.ColRSize.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColRSize.ToolTipText = "Right Operand Memory Size";
            this.ColRSize.Width = 60;
            // 
            // ColRValue
            // 
            this.ColRValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColRValue.FillWeight = 130F;
            this.ColRValue.HeaderText = "Mem/Val";
            this.ColRValue.MinimumWidth = 85;
            this.ColRValue.Name = "ColRValue";
            this.ColRValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColRValue.ToolTipText = "Right Operand Memory Address or Value";
            // 
            // ColHits
            // 
            this.ColHits.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColHits.FillWeight = 80F;
            this.ColHits.HeaderText = "Hits";
            this.ColHits.MinimumWidth = 50;
            this.ColHits.Name = "ColHits";
            this.ColHits.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColHits.ToolTipText = "Required number of consecutive frames the condition must be true";
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.toolStripSeparator1,
            this.addConditionToolStripMenuItem,
            this.duplicateToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator2,
            this.copyLogicToolStripMenuItem,
            this.copyRowToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(222, 198);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Up)));
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Down)));
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(218, 6);
            // 
            // addConditionToolStripMenuItem
            // 
            this.addConditionToolStripMenuItem.Name = "addConditionToolStripMenuItem";
            this.addConditionToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.addConditionToolStripMenuItem.Text = "Add Condition";
            this.addConditionToolStripMenuItem.Click += new System.EventHandler(this.addConditionToolStripMenuItem_Click);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.duplicateToolStripMenuItem.Text = "Duplicate";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(218, 6);
            // 
            // copyLogicToolStripMenuItem
            // 
            this.copyLogicToolStripMenuItem.Name = "copyLogicToolStripMenuItem";
            this.copyLogicToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyLogicToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.copyLogicToolStripMenuItem.Text = "Copy Logic";
            this.copyLogicToolStripMenuItem.Click += new System.EventHandler(this.copyLogicToolStripMenuItem_Click);

            // 
            // copyRowToolStripMenuItem
            // 
            this.copyRowToolStripMenuItem.Name = "copyRowToolStripMenuItem";
            this.copyRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
            | System.Windows.Forms.Keys.C)));
            this.copyRowToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.copyRowToolStripMenuItem.Text = "Copy Row";
            this.copyRowToolStripMenuItem.Click += new System.EventHandler(this.copyRowToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.pasteToolStripMenuItem.Text = "Paste Logic";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolbarPanel
            // 
            this.toolbarPanel.Controls.Add(this.showDecimalCheckBox);
            this.toolbarPanel.Controls.Add(this.clearButton);
            this.toolbarPanel.Controls.Add(this.copyButton);
            this.toolbarPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbarPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.toolbarPanel.Location = new System.Drawing.Point(8, 24);
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.Size = new System.Drawing.Size(684, 39);
            this.toolbarPanel.TabIndex = 2;
            // 
            // showDecimalCheckBox
            // 
            this.showDecimalCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.showDecimalCheckBox.Location = new System.Drawing.Point(551, 5);
            this.showDecimalCheckBox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.showDecimalCheckBox.Name = "showDecimalCheckBox";
            this.showDecimalCheckBox.Size = new System.Drawing.Size(130, 29);
            this.showDecimalCheckBox.TabIndex = 2;
            this.showDecimalCheckBox.Text = "Show Decimal";
            this.showDecimalCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.showDecimalCheckBox.UseVisualStyleBackColor = true;
            this.showDecimalCheckBox.CheckedChanged += new System.EventHandler(this.showDecimalCheckBox_CheckedChanged);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(470, 5);
            this.clearButton.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 29);
            this.clearButton.TabIndex = 1;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(389, 5);
            this.copyButton.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 29);
            this.copyButton.TabIndex = 0;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // TriggerEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainGroupBox);
            this.Name = "TriggerEditorControl";
            this.Size = new System.Drawing.Size(700, 300);
            this.mainGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.triggerGrid)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.toolbarPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private GroupBox mainGroupBox;
        private DoubleBufferedDataGridView triggerGrid;
        private DataGridViewTextBoxColumn ColID;
        private DataGridViewTextBoxColumn ColFlag;
        private DataGridViewTextBoxColumn ColLType;
        private DataGridViewTextBoxColumn ColLSize;
        private DataGridViewTextBoxColumn ColLValue;
        private DataGridViewTextBoxColumn ColCmp;
        private DataGridViewTextBoxColumn ColRType;
        private DataGridViewTextBoxColumn ColRSize;
        private DataGridViewTextBoxColumn ColRValue;
        private DataGridViewTextBoxColumn ColHits;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem moveUpToolStripMenuItem;
        private ToolStripMenuItem moveDownToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem addConditionToolStripMenuItem;
        private FlowLayoutPanel toolbarPanel;
        private Button copyButton;
        private Button clearButton;
        private CheckBox showDecimalCheckBox;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem copyRowToolStripMenuItem;
        private ToolStripMenuItem copyLogicToolStripMenuItem;
        private ToolStripMenuItem duplicateToolStripMenuItem;
    }
}