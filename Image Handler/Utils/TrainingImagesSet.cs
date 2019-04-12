using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;

namespace ImageHandler.Utils
{
    class TrainingImagesSet
    {
        public static string trueSetPath = ConfigurationManager.AppSettings["TrueSetPath"];
        public static string falseSetPath = ConfigurationManager.AppSettings["FlseSetPath"];

        public static int CountTrue
        {
            get
            {
                return Directory.GetFiles(trueSetPath).Length;
            }
        }

        public static int CountFalse
        {
            get
            {
                return Directory.GetFiles(falseSetPath).Length;
            }
        }

        public static int Count
        {
            get
            {
                return Directory.GetFiles(trueSetPath).Length + Directory.GetFiles(falseSetPath).Length;
            }
        }

        public static List<Bitmap> GetTrueSet(int size)
        {
            if (!Directory.Exists(trueSetPath))
                throw new DirectoryNotFoundException("Папка с обучающей выборкой не найдена (true набор)");

            List<Bitmap> result = new List<Bitmap>();
            
            foreach(string fileName in Directory.GetFiles(trueSetPath))
            {
                Bitmap downloadedImage = new Bitmap(Image.FromFile(fileName), size, size);
                result.Add(downloadedImage);
            }

            return result;
        }

        public static List<Bitmap> GetFalseSet(int size)
        {
            if (!Directory.Exists(trueSetPath))
                throw new DirectoryNotFoundException("Папка с обучающей выборкой не найдена (false набор)");

            List<Bitmap> result = new List<Bitmap>();

            foreach (string fileName in Directory.GetFiles(falseSetPath))
            {
                Bitmap downloadedImage = new Bitmap(Image.FromFile(fileName), size, size);
                result.Add(downloadedImage);
            }

            return result;
        }
    }
}
