using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ImageHandler.Algorithms.AdaBoost
{
    using ImageHandler.Forms;
    using ImageHandler.Extensions;

    enum HaarSizeDirection
    {
        Width,
        Height
    }

    /// <summary>
    /// Признак Хаара
    /// </summary>
    class HaarFeature
    {
        private readonly HaarFeatureTemplate template;

        public int Width { get => whiteArea.Width; }
        public int Height { get => whiteArea.Height; }

        public Rectangle whiteArea;
        public List<Rectangle> blackAreas;

        private int currentScale;

        public HaarFeature(HaarFeatureTemplate template)
        {
            this.template = template;

            whiteArea = new Rectangle(0, 0, template.Width, template.Height);
            blackAreas = InitilizeBlackAreas();

            currentScale = 1;
        }

        /// <summary>
        /// Смещает признак Хаара на координаты X и Y точки
        /// </summary>
        /// <param name="p"></param>
        void Offset(Point p)
        {
            whiteArea.Offset(p);
            foreach (Rectangle area in blackAreas)
                area.Offset(p);
        }

        /// <summary>
        /// Увеличивает размер признака
        /// </summary>
        /// <param name="attr">Ширина либо высота</param>
        public void Increase(HaarSizeDirection direction)
        {
            currentScale *= 2;

            if (direction == HaarSizeDirection.Width)
            {
                whiteArea.Width *= 2;
                
                // Приходиться менять полностью структуру, т.к. struct не ссылочный тип
                for (int i = 0; i < blackAreas.Count; i++)
                {
                    Rectangle tmp = blackAreas[i];
                    tmp.Width *= 2;
                    tmp.Offset(new Point(tmp.X, 0));
                    blackAreas[i] = tmp;
                }
            }
            else
            {
                whiteArea.Height *= 2;

                // Приходиться менять полностью структуру, т.к. struct не ссылочный тип
                for (int i = 0; i < blackAreas.Count; i++)
                {
                    Rectangle tmp = blackAreas[i];
                    tmp.Height *= 2;
                    tmp.Offset(new Point(0, tmp.Y));
                    blackAreas[i] = tmp;
                }
            }
        }

        /// <summary>
        /// Увеличивает размер признака
        /// </summary>
        /// <param name="attr">Ширина либо высота</param>
        public void Decrease(HaarSizeDirection direction)
        {
            currentScale /= 2;
            if (currentScale < 1)
                throw new InvalidOperationException("Признак Хаара не может быть меньше своего шаблона.");

            if (direction == HaarSizeDirection.Width)
            {
                whiteArea.Width /= 2;

                // Приходиться менять полностью структуру, т.к. struct не ссылочный тип
                for (int i = 0; i < blackAreas.Count; i++)
                {
                    Rectangle tmp = blackAreas[i];
                    tmp.Width /= 2;
                    tmp.Offset(new Point(-tmp.X / 2, 0));
                    blackAreas[i] = tmp;
                }
            }
            else
            {
                whiteArea.Height /= 2;

                // Приходиться менять полностью структуру, т.к. struct не ссылочный тип
                for (int i = 0; i < blackAreas.Count; i++)
                {
                    Rectangle tmp = blackAreas[i];
                    tmp.Height /= 2;
                    tmp.Offset(new Point(0, -tmp.Y / 2));
                    blackAreas[i] = tmp;
                }
            }
        }

        /// <summary>
        /// Отображает признак в форме
        /// </summary>
        public void Show()
        {
            Bitmap featureImage = new Bitmap(whiteArea.Width, whiteArea.Height);

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    Color color = InAnyBlackArea(blackAreas, new Point(x, y)) ? Color.Black : Color.White;
                    featureImage.SetPixel(x, y, color);
                }

            ImageForm form = new ImageForm(featureImage);
            form.Show();
        }

        // Ищет черные прямоугольники в шаблоне
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

        // Высчитывает размеры черной области, начиная со стартовой точки
        private Rectangle CalculateBlackArea(Point startPoint)
        {
            Rectangle result;
            int blackAreaWidth = 1, blackAreaHeight = 1;

            for (int y = startPoint.Y; y < template.Height; y++)
            {
                // Вычисление ширины черной области
                if (y == startPoint.Y)
                {
                    for (int x = startPoint.X; x < template.Width; x++)
                    {
                        if (template[x, y] == 1 && x == startPoint.X)
                            continue;
                        else if (template[x, y] == 1 && x != startPoint.X)
                            blackAreaWidth++;
                        else
                            break;
                    }
                }
                // Вычисление ее высоты
                else
                {
                    bool allPointsIsBlack = true;

                    // Проверка, чтобы пиксеkи по всей шиирbне области были черными
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

            result = new Rectangle(startPoint.X, startPoint.Y, blackAreaWidth, blackAreaHeight);
            return result;
        }

        // Ищет вхождение точки в уже опредленные черные области
        private static bool InAnyBlackArea(List<Rectangle> blackAreas, Point p)
        {
            bool result = false;

            foreach (Rectangle area in blackAreas)
            {
                if (area.Contains(p))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
