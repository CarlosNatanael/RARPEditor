#nullable disable

namespace RARPEditor.Forms
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Lookups");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Formatters");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Display Logic");
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainContainer = new System.Windows.Forms.SplitContainer();
            this.projectExplorerTreeView = new RARPEditor.Controls.DoubleBufferedTreeView();
            this.projectExplorerContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addLookupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFormatterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.rightContainer = new System.Windows.Forms.SplitContainer();
            this.editorPanel = new System.Windows.Forms.Panel();
            this.welcomeLabel = new System.Windows.Forms.Label();
            this.previewContainer = new System.Windows.Forms.SplitContainer();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.mainMenu.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).BeginInit();
            this.mainContainer.Panel1.SuspendLayout();
            this.mainContainer.Panel2.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.projectExplorerContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightContainer)).BeginInit();
            this.rightContainer.Panel1.SuspendLayout();
            this.rightContainer.Panel2.SuspendLayout();
            this.rightContainer.SuspendLayout();
            this.editorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewContainer)).BeginInit();
            this.previewContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(1264, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.recentFilesToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // recentFilesToolStripMenuItem
            // 
            this.recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
            this.recentFilesToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.recentFilesToolStripMenuItem.Text = "Recent Files";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 659);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1264, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(39, 17);
            this.statusLabel.Text = "Ready";
            // 
            // mainContainer
            // 
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 24);
            this.mainContainer.Name = "mainContainer";
            // 
            // mainContainer.Panel1
            // 
            this.mainContainer.Panel1.Controls.Add(this.projectExplorerTreeView);
            this.mainContainer.Panel1.Controls.Add(this.searchTextBox);
            this.mainContainer.Panel1MinSize = 200;
            // 
            // mainContainer.Panel2
            // 
            this.mainContainer.Panel2.Controls.Add(this.rightContainer);
            this.mainContainer.Panel2MinSize = 600;
            this.mainContainer.Size = new System.Drawing.Size(1264, 635);
            this.mainContainer.SplitterDistance = 250;
            this.mainContainer.TabIndex = 2;
            // 
            // projectExplorerTreeView
            // 
            this.projectExplorerTreeView.AllowDrop = true;
            this.projectExplorerTreeView.ContextMenuStrip = this.projectExplorerContextMenuStrip;
            this.projectExplorerTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectExplorerTreeView.HideSelection = false;
            this.projectExplorerTreeView.Location = new System.Drawing.Point(0, 23);
            this.projectExplorerTreeView.Name = "projectExplorerTreeView";
            treeNode1.Name = "lookupsNode";
            treeNode1.Text = "Lookups";
            treeNode2.Name = "formattersNode";
            treeNode2.Text = "Formatters";
            treeNode3.Name = "displayLogicNode";
            treeNode3.Text = "Display Logic";
            this.projectExplorerTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.projectExplorerTreeView.Size = new System.Drawing.Size(250, 612);
            this.projectExplorerTreeView.TabIndex = 1;
            this.projectExplorerTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.projectExplorerTreeView_AfterSelect);
            this.projectExplorerTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.projectExplorerTreeView_ItemDrag);
            this.projectExplorerTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.projectExplorerTreeView_NodeMouseDoubleClick);
            this.projectExplorerTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.projectExplorerTreeView_DragDrop);
            this.projectExplorerTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.projectExplorerTreeView_DragEnter);
            this.projectExplorerTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.projectExplorerTreeView_KeyDown);
            this.projectExplorerTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.projectExplorerTreeView_MouseDown);
            // 
            // projectExplorerContextMenuStrip
            // 
            this.projectExplorerContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLookupToolStripMenuItem,
            this.addFormatterToolStripMenuItem,
            this.addDisplayToolStripMenuItem,
            this.toolStripSeparator2,
            this.duplicateToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator3,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.projectExplorerContextMenuStrip.Name = "projectExplorerContextMenuStrip";
            this.projectExplorerContextMenuStrip.Size = new System.Drawing.Size(181, 170);
            this.projectExplorerContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.projectExplorerContextMenuStrip_Opening);
            // 
            // addLookupToolStripMenuItem
            // 
            this.addLookupToolStripMenuItem.Name = "addLookupToolStripMenuItem";
            this.addLookupToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addLookupToolStripMenuItem.Text = "Add Lookup";
            this.addLookupToolStripMenuItem.Click += new System.EventHandler(this.addLookupToolStripMenuItem_Click);
            // 
            // addFormatterToolStripMenuItem
            // 
            this.addFormatterToolStripMenuItem.Name = "addFormatterToolStripMenuItem";
            this.addFormatterToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addFormatterToolStripMenuItem.Text = "Add Formatter";
            this.addFormatterToolStripMenuItem.Click += new System.EventHandler(this.addFormatterToolStripMenuItem_Click);
            // 
            // addDisplayToolStripMenuItem
            // 
            this.addDisplayToolStripMenuItem.Name = "addDisplayToolStripMenuItem";
            this.addDisplayToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addDisplayToolStripMenuItem.Text = "Add Display String";
            this.addDisplayToolStripMenuItem.Click += new System.EventHandler(this.addDisplayToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.duplicateToolStripMenuItem.Text = "Duplicate";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Up)));
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Down)));
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchTextBox.Location = new System.Drawing.Point(0, 0);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.PlaceholderText = "Search...";
            this.searchTextBox.Size = new System.Drawing.Size(250, 23);
            this.searchTextBox.TabIndex = 0;
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // rightContainer
            // 
            this.rightContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightContainer.Location = new System.Drawing.Point(0, 0);
            this.rightContainer.Name = "rightContainer";
            // 
            // rightContainer.Panel1
            // 
            this.rightContainer.Panel1.Controls.Add(this.editorPanel);
            this.rightContainer.Panel1MinSize = 400;
            // 
            // rightContainer.Panel2
            // 
            this.rightContainer.Panel2.Controls.Add(this.previewContainer);
            this.rightContainer.Panel2MinSize = 250;
            this.rightContainer.Size = new System.Drawing.Size(1010, 635);
            this.rightContainer.SplitterDistance = 650;
            this.rightContainer.TabIndex = 0;
            // 
            // editorPanel
            // 
            this.editorPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.editorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editorPanel.Controls.Add(this.welcomeLabel);
            this.editorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorPanel.Location = new System.Drawing.Point(0, 0);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Padding = new System.Windows.Forms.Padding(5);
            this.editorPanel.Size = new System.Drawing.Size(650, 635);
            this.editorPanel.TabIndex = 0;
            // 
            // welcomeLabel
            // 
            this.welcomeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.welcomeLabel.Location = new System.Drawing.Point(5, 5);
            this.welcomeLabel.Name = "welcomeLabel";
            this.welcomeLabel.Size = new System.Drawing.Size(638, 623);
            this.welcomeLabel.TabIndex = 0;
            this.welcomeLabel.Text = "Open a -Rich.txt file to begin editing.\r\n(File > Open)\r\n\r\nDouble-click or press E" +
    "nter on an item to edit it.";
            this.welcomeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // previewContainer
            // 
            this.previewContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewContainer.Location = new System.Drawing.Point(0, 0);
            this.previewContainer.Name = "previewContainer";
            this.previewContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.previewContainer.Panel1MinSize = 200;
            this.previewContainer.Panel2MinSize = 200;
            this.previewContainer.Size = new System.Drawing.Size(356, 635);
            this.previewContainer.SplitterDistance = 315;
            this.previewContainer.TabIndex = 0;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Rich Presence Files (*-Rich.txt)|*-Rich.txt|All files (*.*)|*.*";
            this.openFileDialog.Title = "Open Rich Presence Script";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Rich Presence Files (*-Rich.txt)|*-Rich.txt|All files (*.*)|*.*";
            this.saveFileDialog.Title = "Save Rich Presence Script";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.MinimumSize = new System.Drawing.Size(960, 600);
            this.Name = "MainForm";
            this.Text = "RARP Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mainContainer.Panel1.ResumeLayout(false);
            this.mainContainer.Panel1.PerformLayout();
            this.mainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).EndInit();
            this.mainContainer.ResumeLayout(false);
            this.projectExplorerContextMenuStrip.ResumeLayout(false);
            this.rightContainer.Panel1.ResumeLayout(false);
            this.rightContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rightContainer)).EndInit();
            this.rightContainer.ResumeLayout(false);
            this.editorPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.previewContainer)).EndInit();
            this.previewContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private MenuStrip mainMenu;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private SplitContainer mainContainer;
        private Controls.DoubleBufferedTreeView projectExplorerTreeView;
        private SplitContainer rightContainer;
        private Panel editorPanel;
        private SplitContainer previewContainer;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private Label welcomeLabel;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ContextMenuStrip projectExplorerContextMenuStrip;
        private ToolStripMenuItem moveUpToolStripMenuItem;
        private ToolStripMenuItem moveDownToolStripMenuItem;
        private ToolStripMenuItem addDisplayToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem addLookupToolStripMenuItem;
        private ToolStripMenuItem addFormatterToolStripMenuItem;
        private ToolStripMenuItem recentFilesToolStripMenuItem;
        private ToolStripMenuItem duplicateToolStripMenuItem;
        private TextBox searchTextBox;
    }
}
#nullable restore