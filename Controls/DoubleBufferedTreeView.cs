namespace RARPEditor.Controls
{
    // A custom TreeView with DoubleBuffered enabled to prevent flickering during updates and selections.
    public class DoubleBufferedTreeView : System.Windows.Forms.TreeView
    {
        public DoubleBufferedTreeView()
        {
            this.DoubleBuffered = true;
        }
    }
}