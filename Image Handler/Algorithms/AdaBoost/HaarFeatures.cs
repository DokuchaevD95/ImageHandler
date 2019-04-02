using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace ImageHandler.Algorithms.AdaBoost
{
    /// <summary>
    /// Считывает и хранит шаблоны признаков Хаара при первом обращении
    /// </summary>
    class FeaturesTemplates
    {
        private static string templatesPath = Path.Combine(Directory.GetCurrentDirectory(), @"Algorithms\AdaBoost\HaarFeatureTemplates");
        private readonly static string[] templatesFileNames = Directory.GetFiles(templatesPath);

        private static List<byte[,]> templates = new List<byte[,]>();

        public static List<byte[,]> Templates { get => templates; }

        static FeaturesTemplates()
        {
            foreach(string templateName in templatesFileNames)
            {
                byte[,] template = ReadTemplate(templateName);
                templates.Add(template);
            }
        }

        /// <summary>
        /// Считывает один из шаблонов и сохраняет в двумерном байтовом массиве
        /// </summary>
        private static byte[,] ReadTemplate(string filepath)
        {
            byte[,] template;
            int width = 0, height = 0;

            using (StreamReader reader = new StreamReader(filepath))
            {
                string content = reader.ReadToEnd();
                string[] lines = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                height = lines.Length;

                if (height != 0)
                    width = lines[0].Split(' ').Length;

                template = new byte[width, height];

                for (int y = 0; y < height; y++)
                {
                    byte[] lineItems = (from item in lines[y].Split(' ') select Convert.ToByte(item)).ToArray();

                    for (int x = 0; x < width; x++)
                        template[x, y] = lineItems[x];
                }
            }

            return template;
        }
    }

    /// <summary>
    /// Признак Хаара
    /// </summary>
    class HaarFeature
    {
        private byte[,] template;
        private readonly int templateWidth;
        private readonly int templateHeight;
        public Rectangle whiteArea;
        public readonly List<Rectangle> blackAreas;

        public HaarFeature(byte[,] template)
        {
            this.template = template;

            if (this.template.Length < 1)
                throw new ArgumentException("Шаблон признака не может быть пустым");

            templateWidth = this.template.GetUpperBound(0) + 1;
            templateHeight = this.template.Length / templateWidth;

            whiteArea = new Rectangle(0, 0, templateWidth - 1, templateHeight - 1);
            blackAreas = InitilizeBlackAreas();
        }

        /// <summary>
        /// Ищет черные прямоугольники в шаблоне
        /// </summary>
        /// <returns>Лист черных областей шаблона</returns>
        private List<Rectangle> InitilizeBlackAreas()
        {
            List<Rectangle> areas = new List<Rectangle>();

            for (int y = 0; y < templateHeight; y++)
            {
                for (int x = 0; x < templateWidth; x++)
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

            for (int y = startPoint.Y; y < templateHeight; y++)
            {
                // Вычисление ширины черной области
                if (y == startPoint.Y)
                {
                    blackAreaHeight++;

                    for (int x = startPoint.X; x < templateWidth; x++)
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
                        if (startPoint.X + blackAreaWidth <= templateWidth)
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
                // Contains не подходит, странная работа с гранцей прямоугольника
                bool xInside = p.X <= area.Right && p.X >= area.Left;
                bool yInside = p.Y <= area.Bottom && p.Y >= area.Top;

                if (xInside && yInside)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
