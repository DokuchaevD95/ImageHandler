﻿using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageHandler.Algorithms.AdaBoost
{
    using ImageHandler.Forms;

    /// <summary>
    /// Маска Хаара
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class HaarMask
    {
        private readonly HaarMaskTemplate template;
        [JsonProperty] public string TemplateName { get => template.name; }

        public int Width { get => whiteArea.Width; }
        public int Height { get => whiteArea.Height; }

        public readonly Rectangle whiteArea;
        public readonly List<Rectangle> blackAreas;

        [JsonConstructor]
        public HaarMask(string templateName)
        {
            foreach (HaarMaskTemplate template in HaarMaskTemplatesContainer.Templates)
                if (template.name == templateName)
                {
                    this.template = template;
                    break;
                }

            whiteArea = new Rectangle(0, 0, template.Width, template.Height);
            blackAreas = InitilizeBlackAreas();
        }

        public HaarMask(HaarMaskTemplate template)
        {
            this.template = template;
            
            whiteArea = new Rectangle(0, 0, template.Width, template.Height);
            blackAreas = InitilizeBlackAreas();
        }

        /// <summary>
        /// Отображает маску в форме
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
                        if (startPoint.X + blackAreaWidth < template.Width)
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
