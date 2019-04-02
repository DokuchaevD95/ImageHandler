using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageHandler.Utils
{
    public class IntegralImage
    {
        GreyImage greyImage;
        int[,] integralImage;

        public int Width { get => greyImage.Width; }
        public int Height { get => greyImage.Height; }

        public IntegralImage(GreyImage image)
        {
            greyImage = image;
            integralImage = TransformToIntegral(image);
        }

        public int GetPixel(int x, int y)
        {
            return integralImage[x, y];
        }

        // Проверяет, что точка лежит внутри изображения
        private bool IsInside(Point p)
        {
            bool xInside = 0 < p.X && p.X < Width;
            bool yInside = 0 < p.Y && p.Y < Height;

            return xInside && yInside;
        }

        // Преобразует изображение оттенков серого в интегральное
        public static int[,] TransformToIntegral(GreyImage image)
        {
            int[,] result = new int[image.Width, image.Height];

            for (var i = 0; i < image.Width; i++)
                for(var j = 0; j < image.Height; j++)
                {
                    int digonalSum = i != 0 && j != 0 ? result[i, j] : 0;
                    int leftSum = i != 0 ? result[i - 1, j] : 0;
                    int topSum = j != 0 ? result[i, j - 1] : 0;

                    result[i, j] = (int)image.GetValue(i, j) + leftSum + topSum - digonalSum;
                }

            return result;
        }
    }
}
