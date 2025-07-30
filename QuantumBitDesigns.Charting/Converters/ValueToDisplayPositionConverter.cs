using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace QuantumBitDesigns.Charting
{
    public class ValueToDisplayPositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
            {
                return 0d;
            }
            List<DataPoint> data = (List<DataPoint>)(values[0]);
            bool isZeroBased = (bool)(values[1]);
            double maxY = (double)(values[2]);
            double controlWidth = (double)(values[3]);
            double controlHeight = (double)(values[4]);
            double position = (double)(values[5]);

            string displayCalculationParameter = (string)parameter;

            for (int i = 0; i < data.Count; ++i)
            {
                DataPoint dataItem = data[i];

                Point point = ChartUtil.ComputePoint(i, data.Count, dataItem.Value, controlWidth, controlHeight, isZeroBased, maxY);
                if (point.X > position)
                {
                    if (displayCalculationParameter.ToLower() == "horizontal")
                    {
                        return point.X;
                    }
                    if (displayCalculationParameter.ToLower() == "vertical")
                    {
                        return point.Y;
                    }
                }
            }
            return 0d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Not supported");
        }
    }
}
