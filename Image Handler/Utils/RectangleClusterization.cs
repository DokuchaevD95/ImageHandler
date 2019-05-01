using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace ImageHandler.Utils
{
    public class Cluster
    {
        public readonly List<Rectangle> internalItems;

        public Rectangle Center
        {
            get
            {
                int xComponent = 0;
                int yComponent = 0;
                int widthComponent = 0;
                int heightComponent = 0;

                foreach (Rectangle rect in internalItems)
                {
                    xComponent += rect.X;
                    yComponent += rect.Y;
                    widthComponent += rect.Width;
                    heightComponent += rect.Height;
                }

                xComponent /= internalItems.Count;
                yComponent /= internalItems.Count;
                widthComponent /= internalItems.Count;
                heightComponent /= internalItems.Count;

                return new Rectangle(xComponent, yComponent, widthComponent, heightComponent);
            }
        }
        
        public int AverageWidth
        {
            get
            {
                int summ = 0;

                foreach (Rectangle rect in internalItems)
                {
                    summ += rect.Width;
                }

                return summ / internalItems.Count;
            }
        }

        public Cluster()
        {
            internalItems = new List<Rectangle>();
        }

        public Cluster(List<Rectangle> internalItems)
        {
            this.internalItems = internalItems;
        }
    }

    class RectangleClusterization
    {
        /// <summary>
        /// Вычисляет лист кластеров
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static List<Cluster> GetClusters(List<Rectangle> points)
        {
            List<Rectangle> centers = GetCentersOfClusters(points);
            return RelateCentersAndClusters(centers, points);
        }

        /// <summary>
        /// Связывает центры кластеров со всем остальными точками
        /// </summary>
        /// <param name="centers"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private static List<Cluster> RelateCentersAndClusters(List<Rectangle> centers, List<Rectangle> rectangles)
        {
            // Инициализация словаря с центром и кластером
            Dictionary<Rectangle, Cluster> centerRelatedWithCluster = new Dictionary<Rectangle, Cluster>();
            foreach (Rectangle center in centers)
            {
                centerRelatedWithCluster.Add(center, new Cluster());
                centerRelatedWithCluster[center].internalItems.Add(center);
            }

            foreach (Rectangle rectangle in rectangles)
            {
                double minimalDistance = double.MaxValue;
                Rectangle closeCenter = new Rectangle();

                foreach (Rectangle center in centerRelatedWithCluster.Keys)
                {
                    double currDistance = Distance(center, rectangle);
                    if (currDistance < minimalDistance)
                    {
                        minimalDistance = currDistance;
                        closeCenter = center;
                    }
                }

                if (minimalDistance != 0)
                {
                    centerRelatedWithCluster[closeCenter].internalItems.Add(rectangle);
                }
            }

            return centerRelatedWithCluster.Values.ToList();
        }

        /// <summary>
        /// Находит центры кластеров
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static List<Rectangle> GetCentersOfClusters(List<Rectangle> rectangles)
        {
            List<Rectangle> centers = new List<Rectangle>();
            centers.Add(rectangles.First()); // доабвление первой точки, в качестве центра кластера
            double distancesSumm = 0;

            do
            {
                // поиск второго (самого отдаленного) центра
                if (centers.Count == 1)
                {
                    Rectangle firstCenter = centers.First();
                    (Rectangle secondCenter, double maxDistance) = (
                        from currentPoint in rectangles
                        let currDistnce = Distance(firstCenter, currentPoint)
                        orderby currDistnce descending
                        select (currentPoint, currDistnce)
                    ).First();

                    centers.Add(secondCenter);
                    distancesSumm += maxDistance;
                }
                // поиск последующих центров
                else
                {
                    double maxMinDistance = 0;
                    Rectangle probableCenter = new Rectangle();
                    double thresholdValue = distancesSumm / centers.Count;

                    foreach (Rectangle currCenter in centers)
                    {
                        (Rectangle currentProbableCenter, double probalbeDistance) = (
                            from currentPoint in rectangles
                            let currDistance = Distance(currCenter, currentPoint)
                            orderby currDistance
                            select (currentPoint, currDistance)
                        ).First();

                        if (maxMinDistance < probalbeDistance)
                        {
                            maxMinDistance = probalbeDistance;
                            probableCenter = currentProbableCenter;
                        }
                    }

                    if (thresholdValue < maxMinDistance && maxMinDistance != 0)
                    {
                        centers.Add(probableCenter);
                        distancesSumm += maxMinDistance;
                    }
                    else break;
                }
            } while (true);

            return centers;
        }

        /// <summary>
        /// Вычисляет Евклидово расстоянеие
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static double Distance(Rectangle r1, Rectangle r2)
        {
            return Math.Sqrt(
                (r1.X - r2.X) * (r1.X - r2.X) +
                (r1.Y - r2.Y) * (r1.Y - r2.Y) +
                (r1.Width - r2.Width) * (r1.Width - r2.Width) +
                (r1.Height - r2.Height) * (r1.Height - r2.Height)
            );
        }
    }
}
