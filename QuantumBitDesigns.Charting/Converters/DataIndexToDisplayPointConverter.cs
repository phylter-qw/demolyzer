using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace QuantumBitDesigns.Charting
{
    public class DataIndexToDisplayPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[1] == DependencyProperty.UnsetValue)
            {
                return 0d;
            }

            int dataIndex = (int)(values[0]);
            List<DataPoint> data = (List<DataPoint>)(values[1]);
            double controlWidth = (double)(values[2]);
            double controlHeight = (double)(values[3]);

            if (dataIndex >= data.Count)
            {
                return 0d;
            }

            DataPoint dataItem = data[dataIndex];

            Point point = ChartUtil.ComputePoint(dataIndex, data.Count, 0, controlWidth, controlHeight, false, 0);
            return point.X;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Not supported");
        }
    }
}
