// Add using directive for the new InputForm.
using RARPEditor.Forms;

namespace RARPEditor.Forms
{
    public partial class CategoryReorderForm : Form
    {
        public List<string> CategoryOrder { get; private set; }

        public CategoryReorderForm(List<string> categories)
        {
            InitializeComponent();
            CategoryOrder = categories;
            categoryListBox.DataSource = CategoryOrder;
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = categoryListBox.SelectedIndex;
            if (selectedIndex > 0)
            {
                var item = CategoryOrder[selectedIndex];
                CategoryOrder.RemoveAt(selectedIndex);
                CategoryOrder.Insert(selectedIndex - 1, item);
                RefreshListBox(selectedIndex - 1);
            }
        }

        private void moveDownButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = categoryListBox.SelectedIndex;
            if (selectedIndex < CategoryOrder.Count - 1 && selectedIndex != -1)
            {
                var item = CategoryOrder[selectedIndex];
                CategoryOrder.RemoveAt(selectedIndex);
                CategoryOrder.Insert(selectedIndex + 1, item);
                RefreshListBox(selectedIndex + 1);
            }
        }

        private void renameButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = categoryListBox.SelectedIndex;
            if (selectedIndex < 0) return;

            string oldName = CategoryOrder[selectedIndex];

            // Use the new, consistent InputForm instead of the VB InputBox.
            using var form = new InputForm("Rename Category", "Enter new category name:", oldName);
            if (form.ShowDialog() == DialogResult.OK)
            {
                string newName = form.InputValue;
                if (newName != oldName)
                {
                    if (CategoryOrder.Any(c => string.Equals(c, newName, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show("A category with this name already exists.", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    CategoryOrder[selectedIndex] = newName;
                    RefreshListBox(selectedIndex);
                }
            }
        }

        private void RefreshListBox(int newSelectedIndex)
        {
            categoryListBox.DataSource = null;
            categoryListBox.DataSource = CategoryOrder;
            categoryListBox.SelectedIndex = newSelectedIndex;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void categoryListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            moveUpButton.Enabled = categoryListBox.SelectedIndex > 0;
            moveDownButton.Enabled = categoryListBox.SelectedIndex < categoryListBox.Items.Count - 1 && categoryListBox.SelectedIndex != -1;
            renameButton.Enabled = categoryListBox.SelectedIndex != -1;
        }
    }
}