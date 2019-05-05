using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;

namespace ImageHandler.Algorithms.AdaBoost
{
    using ImageHandler.Utils;

    class CascadeException : Exception
    {
        public CascadeException(string message) : base(message) { }
    }

    /// <summary>
    /// Стадия Каскада
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CascadeStage
    {
        [JsonProperty] public readonly AdaBoost strongClassifier;
        [JsonProperty] public double strongTreashold;

        public CascadeStage(List<WeakClassifier> weakClassifiers, double strongTreashold)
        {
            strongClassifier = new AdaBoost(weakClassifiers);
            this.strongTreashold = strongTreashold;
        }

        public CascadeStage(List<WeakClassifier> weakClassifiers)
        {
            strongClassifier = new AdaBoost(weakClassifiers);
            strongTreashold = strongClassifier.GetStrongTreashold();
        }
    
        public bool GetStageValue(IntegralImage img)
        {
            double leftValue = 0, rightValue = 0;
            foreach (WeakClassifier weakClassifier in strongClassifier.weakClassifiers)
                leftValue += weakClassifier.GetAlpha() * weakClassifier.GetValue(img);

            bool result = leftValue >= strongTreashold ? true : false;

            return result;
        }
    }

    /// <summary>
    /// Каскад из AdaBoost и метода Виолы-Джонса
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    class Cascade
    {
        private static readonly int trainingImageSize = Convert.ToInt32(ConfigurationManager.AppSettings["AdaBoostTrainingImageSize"]);
        private static readonly string dumpSubDirectory = ConfigurationManager.AppSettings["DumpSubDirectory"];

        [JsonProperty]
        public readonly List<CascadeStage> stages;

        [JsonConstructor]
        public Cascade(List<CascadeStage> stages)
        {
            this.stages = stages;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acceptableFeaturesAmount">Количество доступных признаков Хаара</param>
        /// <param name="acceptableFP">допустимая доля ложных позитивных срабатываний</param>
        public static Cascade Train(int acceptableFeaturesAmount, double acceptableFPportion)
        {
            double minSubtrahend = double.Epsilon;
            List<TrainingObject> trainingSet = InitTrainingSet();
            List<TrainingObject> negativeSet = trainingSet.Where(item => item.classNumber == 0).ToList();
            List<TrainingObject> positiveSet = trainingSet.Where(item => item.classNumber == 1).ToList();
            double[] recognizedCoefficients = new double[positiveSet.Count];

            List<HaarFeature> haarFeatures = GetAllAvailableHaarFeatures();

            List<CascadeStage> resultStages = new List<CascadeStage>();

            double strongTreashold;
            List<WeakClassifier> weakClassifiers = new List<WeakClassifier>();

            do
            {
                // Получение признака Хаара и перевзвешивание обучающей выборки
                trainingSet = NormlizeWeights(trainingSet);
                WeakClassifier bestWeakClassifier = WeakClassifier.GetBestWeakClassifier(haarFeatures, trainingSet);
                trainingSet = UpdateWeights(trainingSet, bestWeakClassifier);

                // Обновление доп массива с коэффециентами распознаных изображений
                // и вычисление порога сильного лкассификатора
                weakClassifiers.Add(bestWeakClassifier);
                for (int i = 0; i < positiveSet.Count; i++)
                {
                    TrainingObject positiveImg = positiveSet[i];

                    if (bestWeakClassifier.GetValue(positiveImg.img) == 1)
                        recognizedCoefficients[i] += bestWeakClassifier.GetAlpha();
                }
                strongTreashold = recognizedCoefficients.Min() - minSubtrahend;

                CascadeStage stage = new CascadeStage(weakClassifiers, strongTreashold);
                int FPamount = negativeSet.Where(item => stage.GetStageValue(item.img)).ToList().Count;

                if (FPamount < acceptableFPportion * negativeSet.Count)
                    resultStages.Add(stage);
                else if (weakClassifiers.Count == acceptableFeaturesAmount)
                    resultStages.Add(new CascadeStage(weakClassifiers));
            } while (weakClassifiers.Count <= acceptableFeaturesAmount);

            return new Cascade(resultStages);
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
        /// Выгрузка в JSON
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            string fileName = $"{dumpSubDirectory}\\Cascade {DateTime.Now.ToString()}.json";
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
        public static Cascade Load(string filePath)
        {
            Cascade result;

            using (StreamReader file = new StreamReader(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                result = (Cascade)serializer.Deserialize(file, typeof(Cascade));
            }

            return result;
        }
    }
}
