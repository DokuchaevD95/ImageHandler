﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace ImageHandler.Forms
{
    using ImageHandler.Utils;
    using ImageHandler.Extensions;
    using ImageHandler.Algorithms.AdaBoost;

    public partial class AdaBoostForm : Form
    {
        private AdaBoost classifier = null;

        public AdaBoostForm()
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

        // Обучить
        private async void trainButton_Click(object sender, EventArgs e)
        {
            string returnedValue = InputBoxForm.Show("Введите количество слабых классиикаторов");
            int weakClassifiersAmount = Convert.ToInt32(returnedValue);

            DisableTrainingButtons();

            RunProgressBar();
            classifier = await Task.Run(()=>AdaBoost.Train(weakClassifiersAmount));
            StopProgressBar();

            loadImageButton.Enabled = true;
            classifier.Save();
        }

        // Загрсузить дамп
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
                        classifier = AdaBoost.Load(filePath);
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

        // Загрузить изображение
        private void loadImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.GIF";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = dialog.FileName;
                    string extension = Path.GetExtension(filePath);

                    if (new string[] { ".jpg", ".bmp", ".png" }.Contains(extension))
                    {
                        Bitmap loadedImage = new Bitmap(filePath);
                        pictureBox1.Image = loadedImage;

                        classifier.FindObject(loadedImage);
                    }
                    else
                    {
                        MessageBox.Show("Неверное расширение файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

    }
}