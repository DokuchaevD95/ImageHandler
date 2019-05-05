using System;

using System.Windows.Forms;

namespace ImageHandler.Forms
{
    public partial class InputBoxForm : Form
    {
        // the InputBox
        private static InputBoxForm newInputBoxForm;
        // строка, которая будет возвращена форме запроса
        private static string returnValue;

        public InputBoxForm()
        {
            InitializeComponent();
        }
        public static T Show<T>(string inputBoxText, Func<string, T> converter)
        {
            newInputBoxForm = new InputBoxForm();
            newInputBoxForm.titleLabel.Text = inputBoxText;
            newInputBoxForm.ShowDialog();

            return converter(returnValue);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            returnValue = inputTextBox.Text;
            newInputBoxForm.Dispose();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            returnValue = string.Empty;
            newInputBoxForm.Dispose();
        }
    }
}
