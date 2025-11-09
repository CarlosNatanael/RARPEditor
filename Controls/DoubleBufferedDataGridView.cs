namespace RARPEditor.Controls
{
    // A custom DataGridView with the DoubleBuffered property set to true.
    // This prevents flickering and improves rendering performance during scrolling and updates.
    public class DoubleBufferedDataGridView : DataGridView
    {
        public DoubleBufferedDataGridView()
        {
            this.DoubleBuffered = true;
        }

        // Override ProcessCmdKey to handle standard editing shortcuts (Copy, Paste, Cut)
        // when a cell is in edit mode. This ensures that the shortcuts are passed to the
        // underlying TextBox editor and prevents them from being intercepted by the grid's
        // own logic (e.g., for pasting entire rows).
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (IsCurrentCellInEditMode && EditingControl is TextBox editingControl)
            {
                switch (keyData)
                {
                    case Keys.Control | Keys.C:
                        editingControl.Copy();
                        return true; // Command handled
                    case Keys.Control | Keys.V:
                        editingControl.Paste();
                        return true; // Command handled
                    case Keys.Control | Keys.X:
                        editingControl.Cut();
                        return true; // Command handled
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}