using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace ImageHandler.Forms
{
    using ImageHandler.Utils;
    using ImageHandler.Extensions;
    using ImageHandler.Algorithms.AdaBoost;

    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }

        private void downloadFileButtton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();

            string fileName = dialog.FileName;
            string[] splitedFileName = dialog.FileName.Split('.');
            string extension = splitedFileName[splitedFileName.Length - 1];

            if (new string[] { "png", "jpg", "bmp" }.Contains(extension))
            {
                Bitmap img = new Bitmap(Image.FromFile(fileName));
                img.Show();
            }
            else
            {
                MessageBox.Show("Неверное расширение файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartForm_Load(object sender, EventArgs e)
        {

        }
    }
}
