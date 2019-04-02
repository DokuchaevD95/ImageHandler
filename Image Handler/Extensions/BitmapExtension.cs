using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageHandler.Extensions
{
    using ImageHandler.Utils;
    using ImageHandler.Forms;

    public static class BitmapExtension
    {
        public static GreyImage GetGreyImage(this Bitmap img)
        {
            return new GreyImage(img);
        }

        public static IntegralImage GetIntegralImage(this Bitmap img)
        {
            return new IntegralImage(img.GetGreyImage());
        }

        public static ImagePixels GetLockedPixels(this Bitmap img)
        {
            return new ImagePixels(img);
        }

        public static void Show(this Bitmap img)
        { 
            ImageForm form = new ImageForm(img);
            form.Show();
        }
    }
}
