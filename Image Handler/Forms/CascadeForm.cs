using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.Windows.Forms;

namespace ImageHandler.Forms
{
    using ImageHandler.Utils;
    using ImageHandler.Algorithms.AdaBoost;

    public partial class CascadeForm : Form
    {
        private Cascade classifier = null;

        public CascadeForm()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        public void RunProgressBar()
        {
            progressBar1.Visible = true;
            progressBar1.MarqueeAnimationSpeed = 20;
            progressBar1.Style = ProgressBarStyle.Marquee;
        }

        public void StopProgressBar()
        {
            progressBar1.Visible = false;
            progressBar1.MarqueeAnimationSpeed = 0;
        }

        // блокирует кнопки обучения/загрузки классификатора
        public void DisableTrainingButtons()
        {
            this.trainButton.Enabled = this.loadButton.Enabled = false;
        }

        // Загрузить изображение на распознавание
        private async void loadImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = dialog.FileName;
                    string extension = Path.GetExtension(filePath);

                    if (new string[] { ".jpg", ".bmp", ".png" }.Contains(extension))
                    {
                        Bitmap loadedImage = new Bitmap(filePath);

                        Stopwatch watch = new Stopwatch();

                        RunProgressBar();
                        watch.Start();
                        bool recognitionResult = await Task.Run(() => classifier.Recognize(loadedImage));
                        watch.Stop();
                        StopProgressBar();

                        TimeSpan span = watch.Elapsed;
                        label1.Text = recognitionResult ? "Лицо найдено\n" : "Лицо не обнаружено\n";
                        label1.Text += $"Обработка заняла: {span.TotalMilliseconds}";

                        pictureBox1.Image = loadedImage;
                    }
                    else
                    {
                        MessageBox.Show("Неверное расширение файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Обучить каскад
        private async void trainButton_Click(object sender, EventArgs e)
        {
            int weakClassifiersAmount = InputBoxForm.Show("Введите количество слабых классиикаторов", Convert.ToInt32);
            double acceptableFPportion = InputBoxForm.Show("Допустимая доля ложных положительных срабатываний на слой", Convert.ToDouble);

            DisableTrainingButtons();

            Stopwatch watch = new Stopwatch();

            RunProgressBar();
            watch.Start();
            classifier = await Task.Run(() => Cascade.Train(weakClassifiersAmount, acceptableFPportion));
            watch.Stop();
            StopProgressBar();

            TimeSpan span = watch.Elapsed;
            label1.Text = $"Процесс поиска занял: {span.Hours}:{span.Minutes}:{span.Seconds}";

            loadImageButton.Enabled = true;
            classifier.Save();
        }

        // Загрузить дамп каскада
        private void loadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + ConfigurationManager.AppSettings["DumpSubDirectory"];
                dialog.CheckFileExists = true;
                dialog.Filter = "json files (*.json)|*.json";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = dialog.FileName;
                    string extension = Path.GetExtension(filePath);

                    if (new string[] { ".json" }.Contains(extension))
                    {
                        classifier = Cascade.Load(filePath);
                        DisableTrainingButtons();
                        loadImageButton.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Неверное расширение файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            List<Bitmap> trueSet = TestImageSet.GetTrueSet(AdaBoost.trainingImageSize);
            List<Bitmap> falseSet = TestImageSet.GetFalseSet(AdaBoost.trainingImageSize);

            int falseNegative = 0, falsePositive = 0;
            double avrTreatTime = 0;

            // true объекты
            foreach (Bitmap img in trueSet)
            {
                Stopwatch watch = new Stopwatch();

                watch.Start();
                bool recognitionResult = await Task.Run(() => classifier.Recognize(img));
                watch.Stop();

                TimeSpan span = watch.Elapsed;
                avrTreatTime += span.TotalMilliseconds;

                if (!recognitionResult)
                    falseNegative++;

            }

            // false объекты
            foreach (Bitmap img in falseSet)
            {
                Stopwatch watch = new Stopwatch();

                watch.Start();
                bool recognitionResult = await Task.Run(() => classifier.Recognize(img));
                watch.Stop();

                TimeSpan span = watch.Elapsed;
                avrTreatTime += span.TotalMilliseconds;

                if (recognitionResult)
                    falsePositive++;
            }

            avrTreatTime /= trueSet.Count + falseSet.Count;
            int trueRecognizedAmount = TestImageSet.Count - falseNegative - falsePositive;
            double trueRecognizedPercents = (100.0 / TestImageSet.Count) * trueRecognizedAmount;

            label1.Text = $"Сред. аремя обр. изобр. {avrTreatTime:f4}\n" +
                $"Число ЛН срабатываний {falseNegative}\n" +
                $"Число ЛП срабатываний {falsePositive}\n" +
                $"Число верно распозн. {trueRecognizedPercents:f4}";
        }
    }
}
