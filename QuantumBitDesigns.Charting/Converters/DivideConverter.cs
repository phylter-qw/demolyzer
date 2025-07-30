using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace QuantumBitDesigns.Charting
{
    public class DivideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double val = (double)(value);
            //double divisor = System.Convert.ToDouble(parameter);
            double divisor = 2.0d;

            if (divisor == 0)
            {
                return 0d;
            }

            return val / divisor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Not supported");
        }
    }
}
