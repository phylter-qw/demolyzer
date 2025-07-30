using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace QuantumBitDesigns.Charting
{
    public class DataToPathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
            {
                return null;
            }

            List<DataPoint> data = (List<DataPoint>)(values[0]);
            bool isZeroBased = (bool)(values[1]);
            double maxY = (double)(values[2]);
            double controlWidth = (double)(values[3]);
            double controlHeight = (double)(values[4]);

            StreamGeometry sg = new StreamGeometry();
            using (StreamGeometryContext sgc = sg.Open())
            {
                bool isFigureStarted = false;

                for (int i = 0; i < data.Count; ++i)
                {
                    DataPoint dataItem = data[i];

                    Point point = ChartUtil.ComputePoint(i, data.Count, dataItem.Value, controlWidth, controlHeight, isZeroBased, maxY);
                    if (isFigureStarted == false)
                    {
                        sgc.BeginFigure(point, false, false);
                        isFigureStarted = true;
                    }
                    else
                    {
                        sgc.LineTo(point, true, false);
                    }
                }
            }
            sg.Freeze();
            return sg;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Not supported");
        }
    }
}
