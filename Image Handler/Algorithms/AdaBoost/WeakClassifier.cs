using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ImageHandler.Algorithms.AdaBoost
{
    using ImageHandler.Utils;

    /// <summary>
    /// Слабый классификатор на базе которых строиться сильный
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class WeakClassifier
    {
        [JsonProperty] public readonly HaarFeature feature;
        [JsonProperty] public readonly Treashold treshold;
        [JsonProperty] public double beta;
        public double alpha { get => Math.Log10(1.0 / beta); }

        [JsonConstructor]
        public WeakClassifier(HaarFeature feature, Treashold treshold, double beta)
        {
            this.feature = feature;
            this.treshold = treshold;
            this.beta = beta;
        }

        public WeakClassifier(HaarFeature feature, Treashold treshold)
        {
            this.feature = feature;
            this.treshold = treshold;
            this.beta = 0;
        }

        public int GetValue(IntegralImage img)
        {
            long featureValue = feature.GetValue(img);
            int result;

            if (treshold.sign * featureValue < treshold.sign * treshold.tresholdValue)
                result = 1;
            else
                result = 0;

            return result;
        }

        public double GetError(List<TrainingObject> trainingSet)
        {
            double result = 0;

            foreach (TrainingObject obj in trainingSet)
            {
                result += obj.weight * Math.Abs(GetValue(obj.img) - obj.classNumber);
            }

            return result;
        }
    }

}
