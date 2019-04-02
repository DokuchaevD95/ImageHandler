using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageHandler.Algorithms.AdaBoost
{
    using ImageHandler.Utils;

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

        // считывает шаблон признака Хаара в двумерный массив
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

                template = new byte[height, width];

                for (int x = 0; x < height; x++)
                {
             
                    byte[] lineItems = (from item in lines[x].Split(' ') select Convert.ToByte(item)).ToArray();

                    for (int y = 0; y < width; y++)
                        template[x, y] = lineItems[y];
                }
            }

            return template;
        }
    }

    class HaarFeature
    {
        private byte[,] template;
        private readonly int templateWidth;
        private readonly int templateHeight;
        private Rectangle whiteArea;
        private readonly List<Rectangle> blackAreas;

        public HaarFeature(byte[,] template)
        {
            this.template = template;

            if (this.template.Length < 1)
                throw new ArgumentException("Шаблон признака не может быть пустым");

            templateHeight = this.template.GetUpperBound(0) + 1;
            templateWidth = this.template.Length / templateHeight;

            whiteArea = new Rectangle(0, 0, templateWidth, templateHeight);
            blackAreas = GetBlackAreas(this.template);
        }

        private static List<Rectangle> GetBlackAreas(byte[,] template)
        {
            List<Rectangle> result = new List<Rectangle>();
            int height = template.GetUpperBound(0) + 1;
            int width = template.Length / height;

            return result;
        }
    }
}
