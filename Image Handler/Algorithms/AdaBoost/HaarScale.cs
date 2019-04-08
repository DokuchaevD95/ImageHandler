using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageHandler.Algorithms.AdaBoost
{
    /// <summary>
    /// Класс для хранения коэффициентов скалирования признака Хаара
    /// </summary>
    class HaarScale
    {
        public readonly int widthMultiplier;
        public readonly int heightMultiplier;

        public HaarScale(int widthMultiplier, int heightMultiplier)
        {
            if (widthMultiplier < 1 || heightMultiplier < 1)
                throw new Exception("Масштаб признака не может быть меньше 1");

            this.widthMultiplier = widthMultiplier;
            this.heightMultiplier = heightMultiplier;
        }
    }
}
