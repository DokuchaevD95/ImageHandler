using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageHandler.Utils
{
    using ImageHandler.Extensions;

    /// <summary>
    /// Класс для получения чб изображения из Bitmap
    /// </summary>
    public class GreyImage
    {
        public byte[,] greyValues;

        public readonly int Width;
        public readonly int Height;

        public GreyImage(Bitmap image)
        {
            Width = image.Width;
            Height = image.Height;
            greyValues = TransformToGreyViaLockedPixels(image);
        }

        public byte this[int x, int y]
        {
            get
            {
                return greyValues[x, y];
            }

            set
            {
                greyValues[x, y] = value;
            }
        }

        /// <summary>
        /// Преобразование RGB в серый через медленный GetPixel
        /// </summary>
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

        /// <summary>
        /// Преобразование RGB в серый через блокирование в памяти и произвольный доступ
        /// При работе с изображениями большого разрешения показывает значительный прирост/
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
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


        /// <summary>
        /// преобразует матрицу значений в Bitmap
        /// </summary>
        public Bitmap ToBitmap()
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

        /*
        public GreyImage Convolute()
        {
            const int maskSize = 3;

            // Маска оператора Собеля
            short[,] gxMask = new short[maskSize, maskSize] { 
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };

            short[,] gyMask = new short[maskSize, maskSize] {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 }
            };

            byte[,] gradientMatrix = new byte[Width, Height];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    byte[,] cropingResult = CropMatrix(new Point(x, y), maskSize);
                    
                    int gxValue = ImposeMask(cropingResult, gxMask, maskSize);
                    int gyValue = ImposeMask(cropingResult, gyMask, maskSize);

                    int gradientValue = (int)Math.Sqrt(gxValue * gxValue + gyValue * gyValue);
                    gradientMatrix[x, y] = (byte)(0.5 * gradientValue);
                }
            greyValues = gradientMatrix;

            return this;
        }

        private static int ImposeMask(byte[,] imgComponent, short[,] mask, int size)
        {
            int result = 0;

            for (int x = 0; x < size; x++)
                for(int y = 0; y < size; y++)
                    result += mask[x, y] * imgComponent[x, y];

            return result;
        }

        private byte[,] CropMatrix(Point center, int size)
        {
            byte[,] result = new byte[size, size];
            int halfSize = size / 2;

            for (int x = center.X - halfSize, cropX = 0; x <= center.X + halfSize; x++, cropX++)
                for (int y = center.Y - halfSize, cropY = 0; y <= center.Y + halfSize; y++, cropY++)
                    if (x >= 0 && x < Width && y >= 0 && y < Height)
                        result[cropX, cropY] = this[x, y];

            return result;
        }
        */
    }
}
