using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;
using System.Collections.Specialized;

namespace ImageHandler.Utils
{
    class TrainingSet
    {
        public static string trueSetPath = @"trainingSet\true";
        public static string falseSetPath = @"trainingSet\false";

        public static readonly int defaultWidth = Convert.ToInt32(ConfigurationManager.AppSettings["TriningWidth"]); 
        public static readonly int defaultHeight = Convert.ToInt32(ConfigurationManager.AppSettings["TriningHeight"]); 

        public static List<Bitmap> GetTrueSet()
        {
            if (!Directory.Exists(trueSetPath))
                throw new DirectoryNotFoundException("Папка с обучающей выборкой не найдена (true набор)");

            List<Bitmap> result = new List<Bitmap>();
            
            foreach(string fileName in Directory.GetFiles(trueSetPath))
            {
                Bitmap downloadedImage = new Bitmap(Image.FromFile(fileName), defaultWidth, defaultHeight);
                result.Add(downloadedImage);
            }

            return result;
        }

        public static List<Bitmap> GetFalseSet()
        {
            if (!Directory.Exists(trueSetPath))
                throw new DirectoryNotFoundException("Папка с обучающей выборкой не найдена (false набор)");

            List<Bitmap> result = new List<Bitmap>();

            foreach (string fileName in Directory.GetFiles(falseSetPath))
            {
                Bitmap downloadedImage = new Bitmap(Image.FromFile(fileName), defaultWidth, defaultHeight);
                result.Add(downloadedImage);
            }

            return result;
        }
    }
}
