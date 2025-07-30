using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace QuantumBitDesigns.Charting
{
    public class DataIndexesToDisplayWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[2] == DependencyProperty.UnsetValue)
            {
                return 0d;
            }

            int startIndex = (int)(values[0]);
            int endIndex = (int)(values[1]);
            List<DataPoint> data = (List<DataPoint>)(values[2]);
            double controlWidth = (double)(values[3]);
            double controlHeight = (double)(values[4]);

            Point startPoint = ChartUtil.ComputePoint(startIndex, data.Count, 0, controlWidth, controlHeight, false, 0);
            Point endPoint = ChartUtil.ComputePoint(endIndex, data.Count, 0, controlWidth, controlHeight, false, 0);
            return endPoint.X - startPoint.X;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Not supported");
        }
    }
}
