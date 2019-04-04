using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace ImageHandler.Extensions
{
    public static class RectangleExtension
    {
        public static Point LeftTop(this Rectangle r)
        {
            return r.Location;
        }

        public static Point LeftBottom(this Rectangle r)
        {
            return new Point(r.Left, r.Bottom - 1);
        }

        public static Point RightTop(this Rectangle r)
        {
            return new Point(r.Right - 1, r.Top);
        }

        public static Point RightBottom(this Rectangle r)
        {
            return new Point(r.Right - 1, r.Bottom - 1);
        }

        /// <summary>
        /// Проверяет вхождение точки в пр\моугольник, учитывая границу
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool ContainsWithEdge(this Rectangle r, Point p)
        {
            bool result = false;

            bool xInside = p.X <= r.Right && p.X >= r.Left;
            bool yInside = p.Y <= r.Bottom && p.Y >= r.Top;

            if (xInside && yInside)
                result = true;

            return result;
        }
    }
}
