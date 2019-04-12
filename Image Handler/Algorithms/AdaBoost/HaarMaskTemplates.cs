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
    class HaarMaskTemplate
    {
        public readonly byte[,] Template;
        public readonly int Width;
        public readonly int Height;

        private HaarMaskTemplate(byte[,] template, int width, int height)
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

        public HaarMaskTemplate(string content, string lineDelimeter="\r\n", string columnDelimeter=" ")
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
    }

    /// <summary>
    /// Контейнер. Считывает и хранит шаблоны признаков Хаара при первом обращении
    /// </summary>
    class HaarMaskTemplatesContainer
    {
        private static string templatesPath = Path.Combine(Directory.GetCurrentDirectory(), @"Algorithms\AdaBoost\HaarFeatureTemplates");
        private readonly static string[] templatesFileNames = Directory.GetFiles(templatesPath);
        public static readonly List<HaarMaskTemplate> Templates;

        static HaarMaskTemplatesContainer()
        {
            Templates = new List<HaarMaskTemplate>();

            foreach (string templateName in templatesFileNames)
            {
                HaarMaskTemplate template = ReadTemplate(templateName);
                Templates.Add(template);
            }
        }

        /// <summary>
        /// Считывает один из шаблонов и сохраняет в двумерном байтовом массиве
        /// </summary>
        private static HaarMaskTemplate ReadTemplate(string filepath)
        {
            HaarMaskTemplate template;

            using (StreamReader reader = new StreamReader(filepath))
            {
                string content = reader.ReadToEnd();
                template = new HaarMaskTemplate(content);
            }

            return template;
        }
    }
}
