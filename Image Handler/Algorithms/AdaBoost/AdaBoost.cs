using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageHandler.Algorithms.AdaBoost
{
    using ImageHandler.Utils;
    using ImageHandler.Extensions;

    class AdaBoostException : Exception
    {
        public AdaBoostException(string message) : base(message) { }
    }


    /// <summary>
    /// Обертка для обучающего примера из выборки
    /// </summary>
    internal class TrainingObject
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


    /// <summary>
    /// Порог признака Хаара
    /// </summary>
    internal class Treashold
    {
        public byte sign;
        public double tresholdValue;
        public HaarFeature feature;

        public Treashold(byte sign, double tresholdValue, HaarFeature feature)
        {
            this.sign = sign;
            this.tresholdValue = tresholdValue;
            this.feature = feature;
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

            List<(TrainingObject, int)> calculatedPairs = GetPairs(feature, trainingSet);
            (double fullPositiveSumm, double fullNegtiveSumm) = GetPosisitveAndNegativeSumms(calculatedPairs);

            byte sign = 0;
            double minError = 1, treshold = 0;
            double prevPositiveSumm = 0, prevNegativeSumm = 0, currError, currTreshold;

            for (int i = 1; i < calculatedPairs.Count; i++)
            {
                (TrainingObject, int) currPair = calculatedPairs[i], prevPair = calculatedPairs[i - 1];

                if (currPair.Item1.classNumber == 1)
                    prevPositiveSumm += currPair.Item1.weight;
                else
                    prevNegativeSumm += currPair.Item1.weight;

                currTreshold = (prevPair.Item2 + currPair.Item2) / 2.0;

                // С положительным знаков
                currError = prevNegativeSumm + fullPositiveSumm - prevPositiveSumm;
                if (currError < minError)
                {
                    sign = 1;
                    minError = currError;
                    treshold = currTreshold;
                }

                // С отрицательным знаком
                currError = prevPositiveSumm + fullNegtiveSumm - prevNegativeSumm;
                if (currError < minError)
                {
                    sign = 1;
                    minError = currError;
                    treshold = currTreshold;
                }
            }

            return new Treashold(sign, treshold, feature);
        }

        /// <summary>
        /// Вычисляет признаки Хаара для всей обучающей выборки и возвращает лист кортежей
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="trainingSet"></param>
        /// <returns>(объект тренировочной выборки, значение знака Хаара)</returns>
        private static List<(TrainingObject, int)> GetPairs(HaarFeature feature, List<TrainingObject> trainingSet)
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
        private static (double positive, double negative) GetPosisitveAndNegativeSumms(List<(TrainingObject, int)> pairs)
        {
            (double, double) result = (0, 0);

            foreach ((TrainingObject, int) pair in pairs)
            {
                if (pair.Item1.classNumber == 1)
                    result.Item1 += pair.Item1.weight;
                else
                    result.Item2 += pair.Item1.weight;
            }

            return result;
        }
    }

    /// <summary>
    /// Слабый классификатор на базе которых строиться сильный
    /// </summary>
    internal class WeakClassifier
    {
        public readonly HaarFeature feature;
        public readonly Treashold treshold;

        public WeakClassifier(HaarFeature feature, Treashold treshold)
        {
            this.feature = feature;
            this.treshold = treshold;
        }

        public int GetValue(IntegralImage img)
        {
            int featureValue = feature.GetValue(img);

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
                result += (double)(obj.weight * Math.Abs(GetValue(obj.img) - obj.classNumber));
            }

            return result;
        }
    }


    class AdaBoost
    {
        private static readonly int trainingImageSize = Convert.ToInt32(ConfigurationManager.AppSettings["AdaBoostTrainingImageSize"]);

        /// <summary>
        /// Обучает классиикатор (возвращает лист слабых классиикаторов)
        /// </summary>
        /// <param name="featuresAmount"></param>
        /// <returns></returns>
        public static List<WeakClassifier> Train(int featuresAmount)
        {
            List<TrainingObject> trainingSet = InitTrainingSet();
            List<WeakClassifier> result = new List<WeakClassifier>();
            List<HaarFeature> allFeatures = GetAllAvailableHaarFeatures();

            for (int i = 0; i < featuresAmount; i++)
            {
                trainingSet = NormlizeWeights(trainingSet);

                List<(WeakClassifier weakClassifier, double error)> weakClassifierErrors = new List<(WeakClassifier, double)>();
                foreach (HaarFeature feature in allFeatures)
                {
                    Treashold treshold = Treashold.CalculateTreshold(feature, trainingSet);
                    WeakClassifier wc = new WeakClassifier(feature, treshold);

                    weakClassifierErrors.Add((wc, wc.GetError(trainingSet)));
                }

                WeakClassifier bestClassifier = weakClassifierErrors.OrderBy(pair => pair.error).First().weakClassifier;
                result.Add(bestClassifier);
            }

            return result;
        }

        /// <summary>
        /// Собирает в единый лист все призаки Хаара для текущего
        /// размера изображения из обучающей выборки по всем маскам
        /// </summary>
        /// <returns></returns>
        private static List<HaarFeature> GetAllAvailableHaarFeatures()
        {
            List<HaarFeature> result = new List<HaarFeature>();
            Size imgSize = new Size(trainingImageSize, trainingImageSize);

            foreach (HaarMaskTemplate template in HaarMaskTemplatesContainer.Templates)
            {
                HaarMask mask = new HaarMask(template);
                List<HaarFeature> availableFeatures = HaarFeature.GetAvailableHaarFeatures(mask, imgSize);

                result.AddRange(availableFeatures);
            }

            return result;
        }

        /// <summary>
        /// Нормализует веса, разделяя их на общую сумму
        /// </summary>
        /// <param name="trainingSet"></param>
        /// <returns></returns>
        private static List<TrainingObject> NormlizeWeights(List<TrainingObject> trainingSet)
        {
            double summ = 0;

            foreach (TrainingObject item in trainingSet)
                summ += item.weight;

            trainingSet.ForEach(item => item.weight = item.weight / summ);

            return trainingSet;
        }

        /// <summary>
        /// Инициализирует обучающую выборку. Присваивает начальные веса.
        /// </summary>
        /// <returns></returns>
        private static List<TrainingObject> InitTrainingSet()
        {
            List<TrainingObject> result = new List<TrainingObject>();

            int trueAmount = TrainingImagesSet.CountTrue;
            foreach (Bitmap trainingImage in TrainingImagesSet.GetTrueSet(trainingImageSize))
                result.Add(new TrainingObject(trainingImage, 1, 1 / trueAmount));

            int falseAmount = TrainingImagesSet.CountFalse;
            foreach (Bitmap trainingImage in TrainingImagesSet.GetFalseSet(trainingImageSize))
                result.Add(new TrainingObject(trainingImage, 0, 1 / falseAmount));

            return result;
        }
    }
}
