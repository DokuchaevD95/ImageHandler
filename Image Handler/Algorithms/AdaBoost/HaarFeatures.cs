using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace ImageHandler.Algorithms.AdaBoost
{
    using ImageHandler.Extensions;

    /// <summary>
    /// Признак Хаара
    /// </summary>
    class HaarFeature
    {
        private readonly HaarFeatureTemplate template;

        public Rectangle whiteArea;
        public readonly List<Rectangle> blackAreas;

        public HaarFeature(HaarFeatureTemplate template)
        {
            this.template = template;

            whiteArea = new Rectangle(0, 0, template.Width - 1, template.Height - 1);
            blackAreas = InitilizeBlackAreas();
        }

        /// <summary>
        /// Ищет черные прямоугольники в шаблоне
        /// </summary>
        /// <returns>Лист черных областей шаблона</returns>
        private List<Rectangle> InitilizeBlackAreas()
        {
            List<Rectangle> areas = new List<Rectangle>();

            for (int y = 0; y < template.Height; y++)
            {
                for (int x = 0; x < template.Width; x++)
                {
                    Point currentPosition = new Point(x, y);

                    if (template[x, y] == 1 && !InAnyBlackArea(areas, currentPosition))
                    {
                        Rectangle newBlackArea = CalculateBlackArea(currentPosition);
                        areas.Add(newBlackArea);
                    }
                }
            }

            return areas;
        }

        /// <summary>
        /// Высчитывает размеры черной области, начиная со стартовой точки
        /// </summary>
        /// <param name="startPoint">Стартовая точка</param>
        /// <returns>Прямоугольную область</returns>
        private Rectangle CalculateBlackArea(Point startPoint)
        {
            Rectangle result;
            int blackAreaWidth = 0, blackAreaHeight = 0;

            for (int y = startPoint.Y; y < template.Height; y++)
            {
                // Вычисление ширины черной области
                if (y == startPoint.Y)
                {
                    blackAreaHeight++;

                    for (int x = startPoint.X; x < template.Width; x++)
                        if (template[x, y] == 1)
                        {
                            blackAreaWidth++;
                        }
                        else
                            break;
                }
                // Вычисление ее высоты
                else
                {
                    bool allPointsIsBlack = true;

                    // Проверка, чтобы пикслеи по всей шиирне области были черными
                    for (int x = startPoint.X; x < startPoint.X + blackAreaWidth; x++)
                        if (template[x, y] != 1)
                            allPointsIsBlack = false;

                    if (allPointsIsBlack)
                    {
                        bool leftEdgeIsBlack = false, rightEdgeIsBlack = false;

                        // Проверка края слева (возможно относиться к другому признаку)
                        if (startPoint.X - 1 >= 0)
                            leftEdgeIsBlack = template[startPoint.X - 1, y] == 1;

                        // Проверка края справа (возможно относиться к другому признаку)
                        if (startPoint.X + blackAreaWidth <= template.Width)
                            rightEdgeIsBlack = template[startPoint.X + blackAreaWidth, y] == 1;

                        if (leftEdgeIsBlack || rightEdgeIsBlack)
                            break;
                        else
                            blackAreaHeight++;
                    }
                    else
                        break;
                }

            }

            result = new Rectangle(startPoint.X, startPoint.Y, blackAreaWidth - 1, blackAreaHeight - 1);
            return result;
        }

        /// <summary>
        /// Ищет вхождение точки в уже опредленные черные области
        /// </summary>
        /// <param name="blackAreas">Набор областей</param>
        /// <param name="p">Точка</param>
        /// <returns></returns>
        private static bool InAnyBlackArea(List<Rectangle> blackAreas, Point p)
        {
            bool result = false;

            foreach (Rectangle area in blackAreas)
            {
                if (area.ContainsWithEdge(p))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
