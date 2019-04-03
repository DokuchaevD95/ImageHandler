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
        /// <summary>
        /// Возвращает левую нижнюю точку прямоугольника
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Point GetOppositeLocation(this Rectangle r)
        {
            return new Point(r.Right, r.Bottom);
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
