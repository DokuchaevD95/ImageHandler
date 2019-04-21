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
    public class HaarMaskTemplate
    {
        public readonly byte[,] Template;
        public readonly int Width;
        public readonly int Height;
        public readonly string name;

        public HaarMaskTemplate(string name, string content, string lineDelimeter = "\r\n", string columnDelimeter = " ")
        {
            this.name = name;

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

        // индексатор
        public byte this[int x, int y]
        {
            get
            {
                return Template[x, y];
            }
        }

        /// <summary>
        /// Считывает один из шаблонов и сохраняет в двумерном байтовом массиве
        /// </summary>
        public static HaarMaskTemplate ReadTemplate(string filepath)
        {
            HaarMaskTemplate template;
            string name = Path.GetFileName(filepath);

            using (StreamReader reader = new StreamReader(filepath))
            {
                string content = reader.ReadToEnd();
                template = new HaarMaskTemplate(name, content);
            }

            return template;
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
                HaarMaskTemplate template = HaarMaskTemplate.ReadTemplate(templateName);
                Templates.Add(template);
            }
        }
    }
}
