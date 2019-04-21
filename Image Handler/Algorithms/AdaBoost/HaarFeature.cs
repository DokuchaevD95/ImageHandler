using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ImageHandler.Algorithms.AdaBoost
{
    using ImageHandler.Utils;

    [JsonObject(MemberSerialization.OptIn)]
    public class HaarFeature
    {
        [JsonProperty] public readonly HaarMask mask;
        [JsonProperty] public readonly Point startPoint;
        [JsonProperty] public readonly HaarScale scale;

        [JsonConstructor]
        public HaarFeature(HaarMask mask, Point startPoint, HaarScale scale)
        {
            this.mask = mask;
            this.startPoint = startPoint;
            this.scale = scale;
        }

        public long GetValue(IntegralImage img)
        {
            Rectangle whiteArea = mask.whiteArea;

            whiteArea.Offset(startPoint);
            whiteArea.Width *= scale.widthMultiplier;
            whiteArea.Height *= scale.heightMultiplier;

            long result = img.GetRectangleSum(whiteArea);

            for(int i = 0; i < mask.blackAreas.Count; i++)
            {
                Rectangle blackArea = mask.blackAreas[i];

                blackArea.X = startPoint.X + blackArea.X * scale.widthMultiplier;
                blackArea.Y = startPoint.Y + blackArea.Y * scale.heightMultiplier;
                blackArea.Width *= scale.widthMultiplier;
                blackArea.Height *= scale.heightMultiplier;

                result -= img.GetRectangleSum(blackArea);
            }

            return result;
        }

        public static List<HaarFeature> GetAvailableHaarFeatures(HaarMask mask, Size imgSize)
        {
            List<HaarFeature> result = new List<HaarFeature>();

            for (int x = 0; x + mask.Width <= imgSize.Width; x++)
                for (int y = 0; y + mask.Height <= imgSize.Height; y++)
                {
                    Point currentPoint = new Point(x, y);

                    foreach (HaarScale scale in GetAvailableScales(mask, currentPoint, imgSize))
                        result.Add(new HaarFeature(mask, currentPoint, scale));
                }


            return result;
        }

        public static List<HaarScale> GetAvailableScales(HaarMask mask, Point startPoint, Size imgSize)
        {
            List<HaarScale> result = new List<HaarScale>();

            int heightMultiplier = 1;
            while (startPoint.Y + mask.Height * heightMultiplier <= imgSize.Height)
            {
                int widthMultiplier = 1;

                while (startPoint.X + mask.Width * widthMultiplier <= imgSize.Width)
                {
                    result.Add(new HaarScale(widthMultiplier, heightMultiplier));
                    widthMultiplier++;
                }

                heightMultiplier++;
            }

            return result;
        }

    }
}
