﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageHandler.Utils
{
    /// <summary>
    /// Пиксель, с возможностью менять значение компонент
    /// </summary>
    public class Pixel
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte GetGreyState()
        {
            return (byte)(R * 0.3 + G * 0.59 + B * 0.11);
        }
    }

    /// <summary>
    /// Класс предоставляющий доступ к пикселам.
    /// </summary>
    public class ImagePixels: IDisposable
    {
        public static PixelFormat pixelformat = PixelFormat.Format24bppRgb;
        public static int componentsAmount = Bitmap.GetPixelFormatSize(pixelformat) / 8;

        private bool disposed;

        private Bitmap image;
        private Rectangle rectArea;
        private BitmapData bmpData;
        private Pixel[,] pixelMatrix;

        public ImagePixels(Bitmap img)
        {
            image = img;
            rectArea = new Rectangle(0, 0, image.Width, image.Height);
            bmpData = image.LockBits(rectArea, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            InitPixelMatrix();
        }

        public ImagePixels(Bitmap img, Rectangle rect)
        {
            image = img;
            rectArea = rect;
            bmpData = image.LockBits(rectArea, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            InitPixelMatrix();
        }

        // индексатор
        public Pixel this [int x, int y]
        {
            get
            {
                return pixelMatrix[x, y];
            }
        }

        /// <summary>
        /// Достает данные о пикселах в массиве байтов и преобразует их в двумерный масив
        /// </summary>
        /// <returns>двумерный массив пикселей</returns>
        private Pixel[,] InitPixelMatrix()
        {
            unsafe
            {
                byte* firstPixelPtr = (byte*)bmpData.Scan0;

                pixelMatrix = new Pixel[rectArea.Width, rectArea.Height];

                for (int y = 0; y < rectArea.Height; y++)
                {
                    byte* currentLine = firstPixelPtr + ((rectArea.Y + y) * bmpData.Stride);

                    for (int x = 0; x < rectArea.Width; x++)
                    {
                        byte* currentPixelPtr = currentLine + ((rectArea.X + x) * componentsAmount);

                        pixelMatrix[x, y] = new Pixel(
                            r: currentPixelPtr[2], // B
                            g: currentPixelPtr[1], // G
                            b: currentPixelPtr[0] // R
                        );
                    }
                }
            }

            return pixelMatrix;
        }

        /// <summary>
        /// Сохраняет изменения и разблокирует пиксели
        /// </summary>
        /// <returns></returns>
        private Pixel[,] SaveAndUnlockPixelsParallel()
        {
            unsafe
            {
                byte* firstPixelPtr = (byte*)bmpData.Scan0;

                for (int y = 0; y < rectArea.Height; y++)
                { 
                    byte* currentLine = firstPixelPtr + ((rectArea.Y + y) * bmpData.Stride);

                    for (int x = 0; x < rectArea.Width; x++)
                    {
                        byte* currentPixelPtr = currentLine + ((rectArea.X + x) * componentsAmount);

                        pixelMatrix[x, y] = new Pixel(
                            r: currentPixelPtr[2], // B
                            g: currentPixelPtr[1], // G
                            b: currentPixelPtr[0] // R
                        );
                    }
                }

                image.UnlockBits(bmpData);
            }

            return pixelMatrix;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // освобождение управляемых ресурсов
                    SaveAndUnlockPixelsParallel();
                }
                // освобождение неуправляемых ресурсов
                disposed = true;
            }
        }

        ~ImagePixels()
        {
            Dispose(false);
        }
    }
}