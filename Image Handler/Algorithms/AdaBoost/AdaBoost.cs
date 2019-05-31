using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Configuration;
using System.Drawing;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageHandler.Algorithms.AdaBoost
{
    using ImageHandler.Utils;
    using ImageHandler.Extensions;

    class AdaBoostException : Exception
    {
        public AdaBoostException(string message) : base(message) { }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class AdaBoost
    {
        private static readonly int trainingImageSize = Convert.ToInt32(ConfigurationManager.AppSettings["AdaBoostTrainingImageSize"]);
        private static readonly string dumpSubDirectory = ConfigurationManager.AppSettings["DumpSubDirectory"];

        [JsonProperty] public readonly List<WeakClassifier> weakClassifiers;

        [JsonConstructor]
        public AdaBoost(List<WeakClassifier> weakClassifiers)
        {
            this.weakClassifiers = weakClassifiers;
        }

        /// <summary>
        /// Производит распознавание объекта в прямоугольной области
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public bool Recognize(Bitmap img)
        {
            Bitmap tmpImg = new Bitmap(img, new Size(trainingImageSize, trainingImageSize));

            IntegralImage integralImg = tmpImg.GetIntegralImage();

            double leftValue = 0, rightValue = 0;
            foreach (WeakClassifier weakClassifier in weakClassifiers)
            {
                leftValue += weakClassifier.GetAlpha() * weakClassifier.GetValue(integralImg);
                rightValue += weakClassifier.GetAlpha();
            }

            bool result = leftValue >= 0.5 * rightValue ? true : false;
            return result;
        }

        /// <summary>
        /// Производит поиск объекта согласно обученному классификатору
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Bitmap FindObject(Bitmap src)
        {
            ScanningWindow scanningWindow = new ScanningWindow(src);
            List<Rectangle> activatedAreas = new List<Rectangle>();

            foreach (Rectangle area in scanningWindow)
            {
                using (Bitmap cropedImage = src.CropImage(area))
                {
                    bool result = Recognize(cropedImage);

                    if (result)
                        activatedAreas.Add(area);
                }
            }

            /*
            List<Cluster> activatedAreaClusters = RectangleClusterization.GetClusters(activatedAreas);

            if (activatedAreaClusters.Count > 0)
                foreach(Cluster cluster in activatedAreaClusters)
                {
                    Rectangle center = cluster.Center;
                    src.DrawBorder(center);
                }*/
            
            
            foreach (Rectangle area in activatedAreas)
                src.DrawBorder(area);
                
            return src;
        }

        /// <summary>
        /// Обучает классиикатор (возвращает лист слабых классиикаторов)
        /// </summary>
        /// <param name="featuresAmount"></param>
        /// <returns></returns>
        public static AdaBoost Train(int featuresAmount)
        {
            List<WeakClassifier> result = new List<WeakClassifier>();
            List<TrainingObject> trainingSet = InitTrainingSet();
            List<HaarFeature> allFeatures = GetAllAvailableHaarFeatures();

            for (int i = 0; i < featuresAmount; i++)
            {
                trainingSet = NormlizeWeights(trainingSet);
                WeakClassifier bestWeakClassifier = WeakClassifier.GetBestWeakClassifier(allFeatures, trainingSet);
                trainingSet = UpdateWeights(trainingSet, bestWeakClassifier);

                result.Add(bestWeakClassifier);
            }

            return new AdaBoost(result);
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

            foreach (TrainingObject item in trainingSet)
                item.weight /= summ;

            return trainingSet;
        }

        /// <summary>
        /// Обновляет веса, относительно полученных слабого классификатора и ошибки
        /// </summary>
        /// <param name="trainingSet"></param>
        /// <param name="weakClassifier"></param>
        /// <param name="betta"></param>
        /// <returns></returns>
        private static List<TrainingObject> UpdateWeights(List<TrainingObject> trainingSet, WeakClassifier weakClassifier)
        {
            foreach (TrainingObject obj in trainingSet)
            {
                int degree = Math.Abs(weakClassifier.GetValue(obj.img) - obj.classNumber);
                double multiplier = Math.Pow(weakClassifier.beta, 1 - degree);

                obj.weight *= multiplier;
            }

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
                result.Add(new TrainingObject(trainingImage, 1, 1.0 / (2.0 * trueAmount)));

            int falseAmount = TrainingImagesSet.CountFalse;
            foreach (Bitmap trainingImage in TrainingImagesSet.GetFalseSet(trainingImageSize))
                result.Add(new TrainingObject(trainingImage, 0, 1.0 / (2.0 * falseAmount)));

            return result;
        }

        /// <summary>
        /// Выгрузка в JSON
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            string fileName = $"{dumpSubDirectory}\\AdaBoost {DateTime.Now.ToString()}.json";
            fileName = fileName.Replace(':', '.');

            using (StreamWriter file = new StreamWriter(fileName))
            using (JsonWriter writer = new JsonTextWriter(file))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, this);
            }

            return fileName;
        }

        /// <summary>
        /// Загрузка из JSON
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static AdaBoost Load(string filePath)
        {
            AdaBoost result;

            using (StreamReader file = new StreamReader(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                result = (AdaBoost)serializer.Deserialize(file, typeof(AdaBoost));
            }

            return result;
        }
    }
}
