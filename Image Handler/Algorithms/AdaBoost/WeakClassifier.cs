using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Concurrent;
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

        public double GetAlpha()
        {
            return Math.Log10(1.0 / beta);
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

        public static WeakClassifier GetBestWeakClassifier(List<HaarFeature> allFeatures, List<TrainingObject> trainingSet)
        {
            double minimalError = 0;
            WeakClassifier bestWeakClassifier = null;

            ConcurrentBag<(WeakClassifier weakClassifier, double relatedError)> bag = new ConcurrentBag<(WeakClassifier, double)>();
            OrderablePartitioner<Tuple<int, int>> partitioner = Partitioner.Create(0, allFeatures.Count, 1000);

            // Вычисление лучшего классиикатора
            Parallel.ForEach(partitioner, (range, loop) => {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Treashold treshold = Treashold.CalculateTreshold(allFeatures[i], trainingSet);
                    WeakClassifier currentWeakClassifier = new WeakClassifier(allFeatures[i], treshold);
                    double currentRelatedError = currentWeakClassifier.GetError(trainingSet);
                    bag.Add((currentWeakClassifier, currentRelatedError));
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            });
            //

            (bestWeakClassifier, minimalError) = bag.OrderBy((pair) => pair.relatedError).First();
            bestWeakClassifier.beta = minimalError / (1.0 - minimalError);


            return bestWeakClassifier;
        }
    }

}
