using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageHandler.Forms
{
    struct FormControlDistances
    {
        public static int Width = 14;
        public static int Height = 38;
    }

    public partial class ImageForm : Form
    {
        private Bitmap image;
        private ToolTip toolTip;
        
        public ImageForm(Bitmap image)
        {
            InitializeComponent();
            toolTip = new ToolTip();

            this.image = image;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        public ImageForm(Bitmap image, string toolTipMessage)
        {
            InitializeComponent();
            toolTip = new ToolTip();

            this.image = image;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            toolTip.SetToolTip(pictureBox1, toolTipMessage);
        }

        private void ImageForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = (Image)this.image;
        }
        
        private void ImageForm_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Height = this.Height - FormControlDistances.Height;
            pictureBox1.Width = this.Width - FormControlDistances.Width;
        }
    }
}
