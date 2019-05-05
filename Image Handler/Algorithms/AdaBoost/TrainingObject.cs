using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageHandler.Algorithms.AdaBoost
{
    using ImageHandler.Utils;
    using ImageHandler.Extensions;
   
    /// <summary>
    /// Обертка для обучающего примера из выборки
    /// </summary>
    public class TrainingObject
    {
        public readonly IntegralImage img;
        public readonly byte classNumber;
        public double weight;

        public TrainingObject(Bitmap img, byte classNumber)
        {
            this.img = img.GetIntegralImage();
            this.classNumber = classNumber;

            weight = 0;
        }

        public TrainingObject(Bitmap img, byte classNumber, double weight)
        {
            this.img = img.GetIntegralImage();
            this.classNumber = classNumber;
            this.weight = weight;
        }
    }
}
