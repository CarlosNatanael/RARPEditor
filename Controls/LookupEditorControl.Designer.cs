#nullable disable

namespace RARPEditor.Controls
{
    partial class LookupEditorControl
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
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.nameLabel = new System.Windows.Forms.Label();
            this.defaultLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.defaultTextBox = new System.Windows.Forms.TextBox();
            this.entriesGroupBox = new System.Windows.Forms.GroupBox();
            this.entriesDataGridView = new RARPEditor.Controls.DoubleBufferedDataGridView();
            this.KeyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.moveToCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entryToolStrip = new System.Windows.Forms.ToolStrip();
            this.addEntryButton = new System.Windows.Forms.ToolStripButton();
            this.removeEntryButton = new System.Windows.Forms.ToolStripButton();
            this.categoryPanel = new System.Windows.Forms.Panel();
            this.reorderButton = new System.Windows.Forms.Button();
            this.deleteCategoryButton = new System.Windows.Forms.Button();
            this.renameCategoryButton = new System.Windows.Forms.Button();
            this.addCategoryButton = new System.Windows.Forms.Button();
            this.categoryComboBox = new System.Windows.Forms.ComboBox();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.mainTableLayoutPanel.SuspendLayout();
            this.entriesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entriesDataGridView)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.entryToolStrip.SuspendLayout();
            this.categoryPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.ColumnCount = 2;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Controls.Add(this.nameLabel, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.defaultLabel, 0, 1);
            this.mainTableLayoutPanel.Controls.Add(this.nameTextBox, 1, 0);
            this.mainTableLayoutPanel.Controls.Add(this.defaultTextBox, 1, 1);
            this.mainTableLayoutPanel.Controls.Add(this.entriesGroupBox, 0, 3);
            this.mainTableLayoutPanel.Controls.Add(this.categoryPanel, 0, 2);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.Padding = new System.Windows.Forms.Padding(5);
            this.mainTableLayoutPanel.RowCount = 4;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(550, 450);
            this.mainTableLayoutPanel.TabIndex = 0;
            // 
            // nameLabel
            // 
            this.nameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(8, 12);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(39, 15);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Name";
            // 
            // defaultLabel
            // 
            this.defaultLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.defaultLabel.AutoSize = true;
            this.defaultLabel.Location = new System.Drawing.Point(8, 41);
            this.defaultLabel.Name = "defaultLabel";
            this.defaultLabel.Size = new System.Drawing.Size(81, 15);
            this.defaultLabel.TabIndex = 1;
            this.defaultLabel.Text = "Default Value (*)";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameTextBox.Location = new System.Drawing.Point(95, 8);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(447, 23);
            this.nameTextBox.TabIndex = 0;
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            this.nameTextBox.Leave += new System.EventHandler(this.nameTextBox_Leave);
            // 
            // defaultTextBox
            // 
            this.defaultTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultTextBox.Location = new System.Drawing.Point(95, 37);
            this.defaultTextBox.Name = "defaultTextBox";
            this.defaultTextBox.Size = new System.Drawing.Size(447, 23);
            this.defaultTextBox.TabIndex = 1;
            this.defaultTextBox.TextChanged += new System.EventHandler(this.defaultTextBox_TextChanged);
            this.defaultTextBox.Leave += new System.EventHandler(this.defaultTextBox_Leave);
            // 
            // entriesGroupBox
            // 
            this.mainTableLayoutPanel.SetColumnSpan(this.entriesGroupBox, 2);
            this.entriesGroupBox.Controls.Add(this.entriesDataGridView);
            this.entriesGroupBox.Controls.Add(this.entryToolStrip);
            this.entriesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entriesGroupBox.Location = new System.Drawing.Point(8, 111);
            this.entriesGroupBox.Name = "entriesGroupBox";
            this.entriesGroupBox.Size = new System.Drawing.Size(534, 331);
            this.entriesGroupBox.TabIndex = 4;
            this.entriesGroupBox.TabStop = false;
            this.entriesGroupBox.Text = "Entries";
            // 
            // entriesDataGridView
            // 
            this.entriesDataGridView.AllowUserToAddRows = false;
            this.entriesDataGridView.AllowUserToDeleteRows = false;
            this.entriesDataGridView.AllowUserToResizeRows = false;
            this.entriesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.entriesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.KeyColumn,
            this.ValueColumn});
            this.entriesDataGridView.ContextMenuStrip = this.contextMenuStrip;
            this.entriesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entriesDataGridView.Location = new System.Drawing.Point(3, 44);
            this.entriesDataGridView.Name = "entriesDataGridView";
            this.entriesDataGridView.RowHeadersVisible = false;
            this.entriesDataGridView.RowTemplate.Height = 25;
            this.entriesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.entriesDataGridView.Size = new System.Drawing.Size(528, 284);
            this.entriesDataGridView.TabIndex = 4;
            this.entriesDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.entriesDataGridView_CellValueChanged);
            this.entriesDataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entriesDataGridView_KeyDown);
            // 
            // KeyColumn
            // 
            this.KeyColumn.HeaderText = "Key (Decimal or Hex)";
            this.KeyColumn.Name = "KeyColumn";
            this.KeyColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.KeyColumn.Width = 150;
            // 
            // ValueColumn
            // 
            this.ValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ValueColumn.HeaderText = "Value";
            this.ValueColumn.Name = "ValueColumn";
            this.ValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.duplicateToolStripMenuItem,
            this.toolStripSeparator1,
            this.moveToCategoryToolStripMenuItem,
            this.toolStripSeparator2,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(178, 170);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.duplicateToolStripMenuItem.Text = "Duplicate";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(174, 6);
            // 
            // moveToCategoryToolStripMenuItem
            // 
            this.moveToCategoryToolStripMenuItem.Name = "moveToCategoryToolStripMenuItem";
            this.moveToCategoryToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.moveToCategoryToolStripMenuItem.Text = "Move to Category";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(174, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Up)));
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Down)));
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // entryToolStrip
            // 
            this.entryToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addEntryButton,
            this.removeEntryButton});
            this.entryToolStrip.Location = new System.Drawing.Point(3, 19);
            this.entryToolStrip.Name = "entryToolStrip";
            this.entryToolStrip.Size = new System.Drawing.Size(528, 25);
            this.entryToolStrip.TabIndex = 3;
            this.entryToolStrip.Text = "toolStrip1";
            // 
            // addEntryButton
            // 
            this.addEntryButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.addEntryButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.addEntryButton.Name = "addEntryButton";
            this.addEntryButton.Size = new System.Drawing.Size(23, 22);
            this.addEntryButton.Text = "+";
            this.addEntryButton.ToolTipText = "Add Entry";
            this.addEntryButton.Click += new System.EventHandler(this.addEntryButton_Click);
            // 
            // removeEntryButton
            // 
            this.removeEntryButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.removeEntryButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.removeEntryButton.Name = "removeEntryButton";
            this.removeEntryButton.Size = new System.Drawing.Size(23, 22);
            this.removeEntryButton.Text = "-";
            this.removeEntryButton.ToolTipText = "Remove Selected Entries";
            this.removeEntryButton.Click += new System.EventHandler(this.removeEntryButton_Click);
            // 
            // categoryPanel
            // 
            this.mainTableLayoutPanel.SetColumnSpan(this.categoryPanel, 2);
            this.categoryPanel.Controls.Add(this.reorderButton);
            this.categoryPanel.Controls.Add(this.deleteCategoryButton);
            this.categoryPanel.Controls.Add(this.renameCategoryButton);
            this.categoryPanel.Controls.Add(this.addCategoryButton);
            this.categoryPanel.Controls.Add(this.categoryComboBox);
            this.categoryPanel.Controls.Add(this.categoryLabel);
            this.categoryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.categoryPanel.Location = new System.Drawing.Point(8, 66);
            this.categoryPanel.Name = "categoryPanel";
            this.categoryPanel.Size = new System.Drawing.Size(534, 39);
            this.categoryPanel.TabIndex = 2;
            // 
            // reorderButton
            // 
            this.reorderButton.Location = new System.Drawing.Point(452, 6);
            this.reorderButton.Name = "reorderButton";
            this.reorderButton.Size = new System.Drawing.Size(75, 23);
            this.reorderButton.TabIndex = 7;
            this.reorderButton.Text = "Reorder...";
            this.reorderButton.UseVisualStyleBackColor = true;
            this.reorderButton.Click += new System.EventHandler(this.reorderButton_Click);
            // 
            // deleteCategoryButton
            // 
            this.deleteCategoryButton.Location = new System.Drawing.Point(377, 6);
            this.deleteCategoryButton.Name = "deleteCategoryButton";
            this.deleteCategoryButton.Size = new System.Drawing.Size(69, 23);
            this.deleteCategoryButton.TabIndex = 6;
            this.deleteCategoryButton.Text = "Delete";
            this.deleteCategoryButton.UseVisualStyleBackColor = true;
            this.deleteCategoryButton.Click += new System.EventHandler(this.deleteCategoryButton_Click);
            // 
            // renameCategoryButton
            // 
            this.renameCategoryButton.Location = new System.Drawing.Point(302, 6);
            this.renameCategoryButton.Name = "renameCategoryButton";
            this.renameCategoryButton.Size = new System.Drawing.Size(69, 23);
            this.renameCategoryButton.TabIndex = 5;
            this.renameCategoryButton.Text = "Rename";
            this.renameCategoryButton.UseVisualStyleBackColor = true;
            this.renameCategoryButton.Click += new System.EventHandler(this.renameCategoryButton_Click);
            // 
            // addCategoryButton
            // 
            this.addCategoryButton.Location = new System.Drawing.Point(227, 6);
            this.addCategoryButton.Name = "addCategoryButton";
            this.addCategoryButton.Size = new System.Drawing.Size(69, 23);
            this.addCategoryButton.TabIndex = 4;
            this.addCategoryButton.Text = "Add";
            this.addCategoryButton.UseVisualStyleBackColor = true;
            this.addCategoryButton.Click += new System.EventHandler(this.addCategoryButton_Click);
            // 
            // categoryComboBox
            // 
            this.categoryComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.categoryComboBox.FormattingEnabled = true;
            this.categoryComboBox.Location = new System.Drawing.Point(63, 6);
            this.categoryComboBox.Name = "categoryComboBox";
            this.categoryComboBox.Size = new System.Drawing.Size(158, 23);
            this.categoryComboBox.TabIndex = 3;
            this.categoryComboBox.SelectedIndexChanged += new System.EventHandler(this.categoryComboBox_SelectedIndexChanged);
            // 
            // categoryLabel
            // 
            this.categoryLabel.AutoSize = true;
            this.categoryLabel.Location = new System.Drawing.Point(3, 10);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(58, 15);
            this.categoryLabel.TabIndex = 0;
            this.categoryLabel.Text = "Category:";
            // 
            // LookupEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Name = "LookupEditorControl";
            this.Size = new System.Drawing.Size(550, 450);
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            this.entriesGroupBox.ResumeLayout(false);
            this.entriesGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entriesDataGridView)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.entryToolStrip.ResumeLayout(false);
            this.entryToolStrip.PerformLayout();
            this.categoryPanel.ResumeLayout(false);
            this.categoryPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel mainTableLayoutPanel;
        private Label nameLabel;
        private Label defaultLabel;
        private TextBox nameTextBox;
        private TextBox defaultTextBox;
        private GroupBox entriesGroupBox;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem moveUpToolStripMenuItem;
        private ToolStripMenuItem moveDownToolStripMenuItem;
        private ToolStripMenuItem moveToCategoryToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private Panel categoryPanel;
        private Button reorderButton;
        private Button deleteCategoryButton;
        private Button renameCategoryButton;
        private Button addCategoryButton;
        private ComboBox categoryComboBox;
        private Label categoryLabel;
        private DoubleBufferedDataGridView entriesDataGridView;
        private ToolStrip entryToolStrip;
        private ToolStripButton addEntryButton;
        private ToolStripButton removeEntryButton;
        private DataGridViewTextBoxColumn KeyColumn;
        private DataGridViewTextBoxColumn ValueColumn;
        private ToolStripMenuItem duplicateToolStripMenuItem;
    }
}
#nullable restore