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

    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }

        private void StartForm_Load(object sender, EventArgs e)
        {

        }

        private void AdaBoost_Click(object sender, EventArgs e)
        {
            AdaBoostForm form = new AdaBoostForm();
            form.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var set = TrainingImagesSet.GetTrueSet(30);

            int count = 3;
            foreach (Bitmap img in set)
            {
                if (count-- > 0)
                    img.GetGreyImage().ToBitmap().Show();

                else break;
            }
        }

        private void Cascade_Click(object sender, EventArgs e)
        {
            CascadeForm form = new CascadeForm();
            form.Show();
        }
    }
}
