using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageHandler.Algorithms.AdaBoost
{
    /// <summary>
    /// Класс для хранения коэффициентов скалирования признака Хаара
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class HaarScale
    {
        [JsonProperty] public readonly int widthMultiplier;
        [JsonProperty] public readonly int heightMultiplier;

        [JsonConstructor]
        public HaarScale(int widthMultiplier, int heightMultiplier)
        {
            if (widthMultiplier < 1 || heightMultiplier < 1)
                throw new Exception("Масштаб признака не может быть меньше 1");

            this.widthMultiplier = widthMultiplier;
            this.heightMultiplier = heightMultiplier;
        }
    }
}
