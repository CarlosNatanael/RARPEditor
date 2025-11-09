#nullable disable

namespace RARPEditor.Controls
{
    partial class DisplayLogicEditorControl
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
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.displayStringGroupBox = new System.Windows.Forms.GroupBox();
            this.masterDisplayTextBox = new System.Windows.Forms.TextBox();
            this.macroContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertNewMacroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.macrosSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.logicEditorTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.logicSelectorComboBox = new System.Windows.Forms.ComboBox();
            this.valueGroupTabControl = new System.Windows.Forms.TabControl();
            this.valueGroupToolStrip = new System.Windows.Forms.ToolStrip();
            this.addGroupButton = new System.Windows.Forms.ToolStripButton();
            this.removeGroupButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.displayStringGroupBox.SuspendLayout();
            this.macroContextMenu.SuspendLayout();
            this.logicEditorTableLayoutPanel.SuspendLayout();
            this.valueGroupToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.displayStringGroupBox);
            this.mainSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(5);
            this.mainSplitContainer.Panel1MinSize = 100;
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.logicEditorTableLayoutPanel);
            this.mainSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(5, 0, 5, 5);
            this.mainSplitContainer.Panel2MinSize = 200;
            this.mainSplitContainer.Size = new System.Drawing.Size(700, 550);
            this.mainSplitContainer.SplitterDistance = 150;
            this.mainSplitContainer.TabIndex = 0;
            // 
            // displayStringGroupBox
            // 
            this.displayStringGroupBox.Controls.Add(this.masterDisplayTextBox);
            this.displayStringGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayStringGroupBox.Location = new System.Drawing.Point(5, 5);
            this.displayStringGroupBox.Name = "displayStringGroupBox";
            this.displayStringGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.displayStringGroupBox.Size = new System.Drawing.Size(690, 140);
            this.displayStringGroupBox.TabIndex = 0;
            this.displayStringGroupBox.TabStop = false;
            this.displayStringGroupBox.Text = "Display String Template (Right-click to insert macro)";
            // 
            // masterDisplayTextBox
            // 
            this.masterDisplayTextBox.AcceptsReturn = true;
            this.masterDisplayTextBox.ContextMenuStrip = this.macroContextMenu;
            this.masterDisplayTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.masterDisplayTextBox.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.masterDisplayTextBox.Location = new System.Drawing.Point(8, 24);
            this.masterDisplayTextBox.Multiline = true;
            this.masterDisplayTextBox.Name = "masterDisplayTextBox";
            this.masterDisplayTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.masterDisplayTextBox.Size = new System.Drawing.Size(674, 108);
            this.masterDisplayTextBox.TabIndex = 0;
            this.masterDisplayTextBox.TextChanged += new System.EventHandler(this.masterDisplayTextBox_TextChanged);
            this.masterDisplayTextBox.Leave += new System.EventHandler(this.masterDisplayTextBox_Leave);
            // 
            // macroContextMenu
            // 
            this.macroContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertNewMacroToolStripMenuItem,
            this.macrosSeparator});
            this.macroContextMenu.Name = "macroContextMenu";
            this.macroContextMenu.Size = new System.Drawing.Size(171, 32);
            this.macroContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.macroContextMenu_Opening);
            // 
            // insertNewMacroToolStripMenuItem
            // 
            this.insertNewMacroToolStripMenuItem.Name = "insertNewMacroToolStripMenuItem";
            this.insertNewMacroToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.insertNewMacroToolStripMenuItem.Text = "Insert New Macro...";
            this.insertNewMacroToolStripMenuItem.Click += new System.EventHandler(this.insertNewMacroToolStripMenuItem_Click);
            // 
            // macrosSeparator
            // 
            this.macrosSeparator.Name = "macrosSeparator";
            this.macrosSeparator.Size = new System.Drawing.Size(167, 6);
            // 
            // logicEditorTableLayoutPanel
            // 
            this.logicEditorTableLayoutPanel.ColumnCount = 1;
            this.logicEditorTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.logicEditorTableLayoutPanel.Controls.Add(this.logicSelectorComboBox, 0, 0);
            this.logicEditorTableLayoutPanel.Controls.Add(this.valueGroupTabControl, 0, 2);
            this.logicEditorTableLayoutPanel.Controls.Add(this.valueGroupToolStrip, 0, 1);
            this.logicEditorTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logicEditorTableLayoutPanel.Location = new System.Drawing.Point(5, 0);
            this.logicEditorTableLayoutPanel.Name = "logicEditorTableLayoutPanel";
            this.logicEditorTableLayoutPanel.RowCount = 3;
            this.logicEditorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.logicEditorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.logicEditorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.logicEditorTableLayoutPanel.Size = new System.Drawing.Size(690, 391);
            this.logicEditorTableLayoutPanel.TabIndex = 1;
            // 
            // logicSelectorComboBox
            // 
            this.logicSelectorComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logicSelectorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logicSelectorComboBox.FormattingEnabled = true;
            this.logicSelectorComboBox.Location = new System.Drawing.Point(3, 3);
            this.logicSelectorComboBox.Name = "logicSelectorComboBox";
            this.logicSelectorComboBox.Size = new System.Drawing.Size(684, 23);
            this.logicSelectorComboBox.TabIndex = 0;
            this.logicSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.logicSelectorComboBox_SelectedIndexChanged);
            // 
            // valueGroupTabControl
            // 
            this.valueGroupTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueGroupTabControl.Location = new System.Drawing.Point(3, 63);
            this.valueGroupTabControl.Name = "valueGroupTabControl";
            this.valueGroupTabControl.SelectedIndex = 0;
            this.valueGroupTabControl.Size = new System.Drawing.Size(684, 325);
            this.valueGroupTabControl.TabIndex = 1;
            // 
            // valueGroupToolStrip
            // 
            this.valueGroupToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.valueGroupToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addGroupButton,
            this.removeGroupButton});
            this.valueGroupToolStrip.Location = new System.Drawing.Point(0, 29);
            this.valueGroupToolStrip.Name = "valueGroupToolStrip";
            this.valueGroupToolStrip.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.valueGroupToolStrip.Size = new System.Drawing.Size(690, 25);
            this.valueGroupToolStrip.TabIndex = 2;
            this.valueGroupToolStrip.Text = "toolStrip1";
            this.valueGroupToolStrip.Visible = false;
            // 
            // addGroupButton
            // 
            this.addGroupButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.addGroupButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addGroupButton.Name = "addGroupButton";
            this.addGroupButton.Size = new System.Drawing.Size(111, 22);
            this.addGroupButton.Text = "Add Value Group +";
            this.addGroupButton.Click += new System.EventHandler(this.addGroupButton_Click);
            // 
            // removeGroupButton
            // 
            this.removeGroupButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.removeGroupButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeGroupButton.Name = "removeGroupButton";
            this.removeGroupButton.Size = new System.Drawing.Size(127, 22);
            this.removeGroupButton.Text = "Remove Value Group -";
            this.removeGroupButton.Click += new System.EventHandler(this.removeGroupButton_Click);
            // 
            // DisplayLogicEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Name = "DisplayLogicEditorControl";
            this.Size = new System.Drawing.Size(700, 550);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.displayStringGroupBox.ResumeLayout(false);
            this.displayStringGroupBox.PerformLayout();
            this.macroContextMenu.ResumeLayout(false);
            this.logicEditorTableLayoutPanel.ResumeLayout(false);
            this.logicEditorTableLayoutPanel.PerformLayout();
            this.valueGroupToolStrip.ResumeLayout(false);
            this.valueGroupToolStrip.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion

        private SplitContainer mainSplitContainer;
        private GroupBox displayStringGroupBox;
        private TextBox masterDisplayTextBox;
        private ContextMenuStrip macroContextMenu;
        private ToolStripMenuItem insertNewMacroToolStripMenuItem;
        private ToolStripSeparator macrosSeparator;
        private TableLayoutPanel logicEditorTableLayoutPanel;
        private ComboBox logicSelectorComboBox;
        private TabControl valueGroupTabControl;
        private ToolStrip valueGroupToolStrip;
        private ToolStripButton addGroupButton;
        private ToolStripButton removeGroupButton;
    }
}
#nullable restore