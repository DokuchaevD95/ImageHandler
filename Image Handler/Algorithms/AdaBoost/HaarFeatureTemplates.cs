using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ImageHandler.Algorithms.AdaBoost
{
    /// <summary>
    /// Шаблон признака Хаара
    /// </summary>
    class HaarFeatureTemplate: ICloneable
    {
        public readonly byte[,] Template;
        public readonly int Width;
        public readonly int Height;

        private HaarFeatureTemplate(byte[,] template, int width, int height)
        {
            Width = width;
            Height = height;
            Template = new byte[Width, Height];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Template[x, y] = template[x, y];
        }

        // индексатор
        public byte this[int x, int y]
        {
            get
            {
                return Template[x, y];
            }
        }

        public HaarFeatureTemplate(string content, string lineDelimeter="\r\n", string columnDelimeter=" ")
        {
            string[] lines = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Height = lines.Length;

            if (Height != 0)
                Width = lines[0].Split(' ').Length;

            Template = new byte[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                byte[] lineItems = (from item in lines[y].Split(' ') select Convert.ToByte(item)).ToArray();

                for (int x = 0; x < Width; x++)
                    Template[x, y] = lineItems[x];
            }
        }

        public object Clone()
        {
            return new HaarFeatureTemplate(Template, Width, Height);
        }
    }

    /// <summary>
    /// Контейнер. Считывает и хранит шаблоны признаков Хаара при первом обращении
    /// </summary>
    class HaarFeatureContainer
    {
        private static string templatesPath = Path.Combine(Directory.GetCurrentDirectory(), @"Algorithms\AdaBoost\HaarFeatureTemplates");
        private readonly static string[] templatesFileNames = Directory.GetFiles(templatesPath);
        public static readonly List<HaarFeatureTemplate> Templates;

        static HaarFeatureContainer()
        {

            Templates = new List<HaarFeatureTemplate>();

            foreach (string templateName in templatesFileNames)
            {
                HaarFeatureTemplate template = ReadTemplate(templateName);
                Templates.Add(template);
            }
        }

        /// <summary>
        /// Считывает один из шаблонов и сохраняет в двумерном байтовом массиве
        /// </summary>
        private static HaarFeatureTemplate ReadTemplate(string filepath)
        {
            HaarFeatureTemplate template;

            using (StreamReader reader = new StreamReader(filepath))
            {
                string content = reader.ReadToEnd();
                template = new HaarFeatureTemplate(content);
            }

            return template;
        }
    }
}
