using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Threading.Tasks;

namespace ImageHandler.Utils
{
    class TestImageSet
    {
        public static string testTrueSetPath = ConfigurationManager.AppSettings["TestTrueSetPath"];
        public static string testFalseSetPath = ConfigurationManager.AppSettings["TestFalseSetPath"];

        public static int CountTrue
        {
            get
            {
                return Directory.GetFiles(testTrueSetPath).Length;
            }
        }

        public static int CountFalse
        {
            get
            {
                return Directory.GetFiles(testFalseSetPath).Length;
            }
        }

        public static int Count
        {
            get
            {
                return Directory.GetFiles(testTrueSetPath).Length + Directory.GetFiles(testFalseSetPath).Length;
            }
        }

        public static List<Bitmap> GetTrueSet(int size)
        {
            if (!Directory.Exists(testTrueSetPath))
                throw new DirectoryNotFoundException("Папка с обучающей выборкой не найдена (true набор)");

            List<Bitmap> result = new List<Bitmap>();

            foreach (string fileName in Directory.GetFiles(testTrueSetPath))
            {
                Bitmap downloadedImage = new Bitmap(Image.FromFile(fileName), size, size);
                result.Add(downloadedImage);
            }

            return result;
        }

        public static List<Bitmap> GetFalseSet(int size)
        {
            if (!Directory.Exists(testFalseSetPath))
                throw new DirectoryNotFoundException("Папка с обучающей выборкой не найдена (false набор)");

            List<Bitmap> result = new List<Bitmap>();

            foreach (string fileName in Directory.GetFiles(testFalseSetPath))
            {
                Bitmap downloadedImage = new Bitmap(Image.FromFile(fileName), size, size);
                result.Add(downloadedImage);
            }

            return result;
        }
    }
}
