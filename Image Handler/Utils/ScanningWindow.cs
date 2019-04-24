using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Configuration;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace ImageHandler.Utils
{
    using ImageHandler.Extensions;

    class ScanningWindow
    {
        private static readonly int step = Convert.ToInt32(ConfigurationManager.AppSettings["ScanningWindowStep"]);
        private static readonly int defaultSize = Convert.ToInt32(ConfigurationManager.AppSettings["ScanningWindowDefaultSize"]);

        private readonly Bitmap img;

        public ScanningWindow(Bitmap img)
        {
            this.img = img;
        }

        public IEnumerator<Rectangle> GetEnumerator()
        {
            int minimalImageSide = img.Height < img.Width ? img.Height : img.Width;

            for (int currentSize = defaultSize; currentSize < minimalImageSide; currentSize++)
                for (int y = 0; y + currentSize < img.Height; y += step)
                    for (int x = 0; x + currentSize < img.Width; x += step)
                    {
                        Rectangle area = new Rectangle(x, y, currentSize, currentSize);
                        yield return area;
                    }
        }

        public List<Rectangle> GetAllWindowRectangulars()
        {
            List<Rectangle> result = new List<Rectangle>();
            int minimalImageSide = img.Height < img.Width ? img.Height : img.Width;

            for (int currentSize = defaultSize; currentSize < minimalImageSide; currentSize++)
                for (int y = 0; y + currentSize < img.Height; y += step)
                    for (int x = 0; x + currentSize < img.Width; x += step)
                    {
                        Rectangle area = new Rectangle(x, y, currentSize, currentSize);
                        result.Add(area);
                    }

            return result;
        }
    }
}
