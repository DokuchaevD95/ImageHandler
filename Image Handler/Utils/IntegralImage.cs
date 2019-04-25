using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageHandler.Utils
{
    using ImageHandler.Extensions;

    public class IntegralImage
    {
        private readonly long[,] integralImage;

        public readonly int Width;
        public readonly int Height;

        public IntegralImage(GreyImage image)
        {
            Width = image.Width;
            Height = image.Height;
            integralImage = TransformToIntegral(image);
        }

        public long GetPixel(int x, int y)
        {
            return integralImage[x, y];
        }

        /// <summary>
        /// Вычисляет сумму пикселей серого изображения в прямоугольнике
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public long GetRectangleSum(Rectangle r)
        {
            // TODO: оптимизировать вычисление прямоугольной области интегрального изображения
            Point rightBottom = r.RightBottom();
            long result = integralImage[rightBottom.X, rightBottom.Y];

            if (r.X != 0 && r.Y != 0)
            {
                Point leftTop = r.LeftTop(), leftBottom = r.LeftBottom(), rightTop = r.RightTop();

                result -= integralImage[leftBottom.X - 1, leftBottom.Y];
                result -= integralImage[rightTop.X, rightTop.Y - 1];
                result += integralImage[leftTop.X - 1, leftTop.Y - 1];
            }
            else if (r.X != 0)
            {
                Point leftBottom = r.LeftBottom();
                result -= integralImage[leftBottom.X - 1, leftBottom.Y];
            }
            else if (r.Y != 0)
            {
                Point rightTop = r.RightTop();
                result -= integralImage[rightTop.X, rightTop.Y - 1];
            }

            return result;
        }

        /// <summary>
        /// Проверяет, что точка лежит внутри интегрального изображения
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool PointIsInside(Point p)
        {
            bool xInside = p.X >= 0 && p.X < Width;
            bool yInside = p.Y >= 0 && p.Y < Height;

            return xInside && yInside;
        }

        /// <summary>
        /// Проверяет, что прямоугольник лежит внутри интегрального
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool RectangleIsInside(Rectangle r)
        {
            Point leftTop = r.LeftTop(), rightBottom = r.RightBottom();
            return PointIsInside(leftTop) && PointIsInside(rightBottom);
        }

        /// <summary>
        /// Преобразует изображение оттенков серого в интегральное
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static long[,] TransformToIntegral(GreyImage image)
        {
            long[,] result = new long[image.Width, image.Height];

            for (var x = 0; x < image.Width; x++)
                for(var y = 0; y < image.Height; y++)
                {
                    long digonalSum = x != 0 && y != 0 ? result[x - 1, y - 1] : 0;
                    long leftSum = x != 0 ? result[x - 1, y] : 0;
                    long topSum = y != 0 ? result[x, y - 1] : 0;

                    result[x, y] = image.GetValue(x, y) + leftSum + topSum - digonalSum;
                }

            return result;
        }
    }
}
