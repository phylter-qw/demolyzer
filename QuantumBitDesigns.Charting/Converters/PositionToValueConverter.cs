using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace QuantumBitDesigns.Charting
{
    public class PositionToValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
            {
                return "0";
            }
            ChartSeries series = (ChartSeries)(values[0]);
            bool isZeroBased = (bool)(values[1]);
            double maxY = (double)(values[2]);
            double controlWidth = (double)(values[3]);
            double controlHeight = (double)(values[4]);
            double position = (double)(values[5]);
            string stringFormat = values[6] as string;

            List<DataPoint> data = series.Data;

            for (int i = 0; i < data.Count; ++i)
            {
                DataPoint dataItem = data[i];

                Point point = ChartUtil.ComputePoint(i, data.Count, dataItem.Value, controlWidth, controlHeight, isZeroBased, maxY);
                if (point.X > position)
                {
                    double val = 0d;

                    if (isZeroBased == true)
                    {
                        val = maxY * (controlHeight - point.Y) / controlHeight;
                    }
                    else
                    {
                        val = maxY * (controlHeight - 2 * point.Y) / controlHeight;
                    }

                    if (dataItem.X == DateTime.MinValue)
                    {
                        if (stringFormat != null)
                        {
                            return String.Format(stringFormat, val);
                        }
                        else
                        {
                            return String.Format("[{0}] {1:F0}", series.Name, val);
                        }
                    }
                    else
                    {
                        return String.Format("[{0}] {1:F0} ({2:g})", series.Name, val, dataItem.X);
                    }
                }
            }
            return "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Not supported");
        }
    }
}
