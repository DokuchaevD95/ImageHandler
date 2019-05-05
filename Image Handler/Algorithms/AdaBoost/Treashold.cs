using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ImageHandler.Algorithms.AdaBoost
{
    /// <summary>
    /// Порог признака Хаара
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Treashold
    {
        [JsonProperty] public short sign;
        [JsonProperty] public double tresholdValue;

        [JsonConstructor]
        public Treashold(short sign, double tresholdValue)
        {
            this.sign = sign;
            this.tresholdValue = tresholdValue;
        }

        /// <summary>
        /// Вычисление порога для признака Хаара и обучающей выборки
        /// (Используется для построения слабого классиикатора)
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="trainingSet"></param>
        /// <returns></returns>
        public static Treashold CalculateTreshold(HaarFeature feature, List<TrainingObject> trainingSet)
        {
            if (trainingSet.Count <= 2)
                throw new AdaBoostException("Обучающая выборка из менее чем 2 элементов ???");

            List<(TrainingObject trainingObj, long featureValue)> calculatedPairs = GetPairs(feature, trainingSet);
            (double fullPositiveSumm, double fullNegtiveSumm) = GetPosisitveAndNegativeSumms(calculatedPairs);

            short sign = 0;
            double minError = 1, treshold = 0;
            double prevPositiveSumm = 0, prevNegativeSumm = 0, currError;

            for (int i = 1; i < calculatedPairs.Count; i++)
            {
                (TrainingObject trainingObj, long featureValue) currPair = calculatedPairs[i];
                (TrainingObject trainingObj, long featureValue) prevPair = calculatedPairs[i - 1];

                if (prevPair.trainingObj.classNumber == 1)
                    prevPositiveSumm += prevPair.trainingObj.weight;
                else
                    prevNegativeSumm += prevPair.trainingObj.weight;

                // С положительным знаков
                currError = prevNegativeSumm + fullPositiveSumm - prevPositiveSumm;
                if (currError < minError)
                {
                    sign = 1;
                    minError = currError;
                    treshold = (prevPair.featureValue + currPair.featureValue) / 2.0; ;
                }

                // С отрицательным знаком
                currError = prevPositiveSumm + fullNegtiveSumm - prevNegativeSumm;
                if (currError < minError)
                {
                    sign = -1;
                    minError = currError;
                    treshold = (prevPair.featureValue + currPair.featureValue) / 2.0; ;
                }
            }

            return new Treashold(sign, treshold);
        }

        /// <summary>
        /// Вычисляет признаки Хаара для всей обучающей выборки и возвращает лист кортежей
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="trainingSet"></param>
        /// <returns>(объект тренировочной выборки, значение знака Хаара)</returns>
        private static List<(TrainingObject obj, long featureValue)> GetPairs(HaarFeature feature, List<TrainingObject> trainingSet)
        {
            return (
                from obj in trainingSet
                let featureValue = feature.GetValue(obj.img)
                orderby featureValue
                select (obj, featureValue)
            ).ToList();
        }

        /// <summary>
        /// Вычисляет сумму весов положительных и отрицательных элементов в выборке 
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        private static (double positive, double negative) GetPosisitveAndNegativeSumms(List<(TrainingObject, long)> pairs)
        {
            (double, double) result = (0, 0);

            foreach ((TrainingObject, long) pair in pairs)
            {
                if (pair.Item1.classNumber == 1)
                    result.Item1 += pair.Item1.weight;
                else
                    result.Item2 += pair.Item1.weight;
            }

            return result;
        }
    }
}
