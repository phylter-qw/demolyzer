using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QuantumBitDesigns.Charting
{
    public static class ChartUtil
    {
        internal static Point ComputePoint(int xIndex, int xCount, double yValue, double controlWidth, double controlHeight, bool isZeroBased, double maxY)
        {
            Point point = new Point();
            point.X = controlWidth * (double)xIndex / (double)xCount;
            if (isZeroBased == true)
            {
                point.Y = controlHeight * (1d - yValue / maxY);
            }
            else
            {
                point.Y = controlHeight * (0.5d - 0.5d * yValue / maxY);
            }
            return point;
        }

        public static double[] CalculateAxisYPercentages(int count)
        {
            double increment = 1.0d / count;

            List<double> percentages = new List<double>();

            double currentPercent = 0d;
            for (int i = 0; i < count; ++i)
            {
                percentages.Add(currentPercent);
                currentPercent+= increment;
            }
            return percentages.ToArray();
        }

        public static double RoundMaxY(double maxY)
        {
            double places = Math.Truncate(Math.Log10(maxY));
            double tmp = maxY / (Math.Pow(10d, places - 1d));
            tmp = Math.Ceiling(tmp);
            tmp = tmp * (Math.Pow(10d, places - 1d));
            if (Double.IsNaN(tmp) == true)
            {
                return 0d;
            }
            return tmp;
        }
    }
}
