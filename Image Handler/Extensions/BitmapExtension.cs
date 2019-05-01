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

        // вырезает прямоугольник из изображения
        public static Bitmap CropImage(this Bitmap img, Rectangle section)
        {
            Bitmap result = new Bitmap(section.Width, section.Height);

            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(img, 0, 0, section, GraphicsUnit.Pixel);

            return result;
        }

        // Выделяет границу прямоугольной области на изображении красным цветом и толщиной 3 пикселя
        public static Bitmap DrawBorder(this Bitmap img, Rectangle section)
        {
            Pen pen = new Pen(Color.Red, 1);

            using (Graphics g = Graphics.FromImage(img))
                g.DrawRectangle(pen, section);

            return img;
        }

        public static void Show(this Bitmap img)
        { 
            ImageForm form = new ImageForm(img);
            form.Show();
        }
    }
}
