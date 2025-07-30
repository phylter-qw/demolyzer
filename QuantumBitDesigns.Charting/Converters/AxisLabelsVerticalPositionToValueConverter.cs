using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace QuantumBitDesigns.Charting
{
    public class AxisLabelsVerticalPositionToValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values[01] == DependencyProperty.UnsetValue 
                    || values[1] == DependencyProperty.UnsetValue
                    || values[2] == DependencyProperty.UnsetValue
                    || values[3] == DependencyProperty.UnsetValue
                    || values[4] == DependencyProperty.UnsetValue)
                {
                    return String.Empty;
                }
                double percent = System.Convert.ToDouble(values[0]);
                double controlHeight = (double)(values[1]);
                double maxY = (double)(values[2]);
                bool isZeroBased = (bool)(values[3]);
                string stringFormat = (string)(values[4]);

                double val = 0;

                if (isZeroBased == true)
                {
                    val = maxY * (1 - percent);
                }
                else
                {
                    val = 2 * maxY * (1 - percent) - maxY;
                }

                //return String.Format("{0:F0}", val);
                return String.Format(stringFormat, val);
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Not supported");
        }
    }
}
