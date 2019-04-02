using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageHandler.Utils
{
    using ImageHandler.Extensions;

    public class GreyImage
    {
        private Bitmap originalImage;
        private byte[,] greyValues;

        public int Width { get => originalImage.Width; }
        public int Height { get => originalImage.Height; }

        // преобразует матрицу значений в Bitmap
        public Bitmap AsBitmap
        {
            get
            {
                Bitmap bmp = new Bitmap(Width, Height);

                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        byte currentValue = greyValues[x, y];
                        bmp.SetPixel(x, y, Color.FromArgb(255, currentValue, currentValue, currentValue));
                    }

                return bmp;
            }
        }

        public GreyImage(Bitmap image)
        {
            originalImage = image;
            greyValues = TransformToGreyViaLockedPixels(image);
        }

        public byte GetValue(int x, int y)
        {
            return greyValues[x, y];
        }

        // Преобразование RGB в серый через медленный GetPixel
        private static byte[,] TransformToGray(Bitmap image)
        {
            byte[,] gray = new byte[image.Width, image.Height];

            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    byte R = image.GetPixel(x, y).R;
                    byte G = image.GetPixel(x, y).G;
                    byte B = image.GetPixel(x, y).B;

                    gray[x, y] = (byte)(R * 0.3 + G * 0.59 + B * 0.11);
                }

            return gray;
        }

        // Преобразование RGB в серый через блокирование в памяти и произвольный доступ
        // При работе с изображениями большого разрешения показывает значительный прирост
        private static byte[,] TransformToGreyViaLockedPixels(Bitmap image)
        {
            byte[,] gray = new byte[image.Width, image.Height];

            using (ImagePixels pixels = image.GetLockedPixels())
            {
               for (int x = 0; x < image.Width; x++)
                    for (int y = 0; y < image.Height; y++)
                        gray[x, y] = pixels[x, y].GetGreyState();
            }

            return gray;
        }
    }
}
