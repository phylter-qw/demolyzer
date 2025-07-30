using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace QuantumBitDesigns.Charting
{
    public class AxisXDateLabel
    {
        public static SolidColorBrush ThickBrush;
        public static SolidColorBrush ThinBrush;
        static AxisXDateLabel()
        {
            ThickBrush = new SolidColorBrush(Color.FromArgb(88, 33, 0xcc, 0xff));
            ThickBrush.Freeze();
            ThinBrush = new SolidColorBrush(Color.FromArgb(22, 33, 33, 33));
            ThinBrush.Freeze();
        }
        private enum SpanType
        {
            Hourly, //(< 2 days)
            Daily, //(< 90 days)
            Monthly, //(90 days to 2 years)
            Yearly, //(2 years+)
        };

        private int _startIndex;
        private int _endIndex;

        private string _text;
        private SolidColorBrush _brush;

        public AxisXDateLabel(int startIndex, int endIndex, string text, SolidColorBrush brush)
        {
            this._startIndex = startIndex;
            this._endIndex = endIndex;
            this._text = text;
            this._brush = brush;
        }

        public static AxisXDateLabel[] Create(List<DataPoint> dataPoints)
        {
            if (dataPoints.Count == 0)
            {
                return null;
            }

            DateTime start = dataPoints[0].X;
            DateTime end = dataPoints[dataPoints.Count - 1].X;
            TimeSpan timeSpan = end - start;

            SpanType spanType = SpanType.Hourly;

            if (timeSpan.TotalDays <= 2)
            {
                spanType = SpanType.Hourly;
            }
            else
            {
                if (timeSpan.TotalDays < 90)
                {
                    spanType = SpanType.Daily;
                }
                else
                {
                    if (timeSpan.TotalDays < 365 * 2)
                    {
                        spanType = SpanType.Monthly;
                    }
                    else
                    {
                        spanType = SpanType.Yearly;
                    }
                }
            }

            List<AxisXDateLabel> labels = new List<AxisXDateLabel>();
            switch (spanType)
            {
                case SpanType.Hourly:
                    {
                        int hour = dataPoints[0].X.Hour;
                        int hourShort = hour % 12;
                        int startIndex = 0;

                        for (int i = 1; i < dataPoints.Count; i++)
                        {
                            if (dataPoints[i].X.Hour != hour)
                            {
                                hourShort = hour % 12;
                                if (hourShort == 0)
                                {
                                    hourShort = 12;
                                }
                                labels.Add(new AxisXDateLabel(startIndex, i, hourShort.ToString(), ThickBrush));
                                hour = dataPoints[i].X.Hour;
                                startIndex = i;
                            }
                        }
                        hourShort = dataPoints[dataPoints.Count - 1].X.Hour % 12;
                        if (hourShort == 0)
                        {
                            hourShort = 12;
                        }
                        labels.Add(new AxisXDateLabel(startIndex, dataPoints.Count - 1, hourShort.ToString(), ThickBrush));
                        return labels.ToArray();
                    }
                case SpanType.Daily:
                    {
                        int day = dataPoints[0].X.Day;
                        int startIndex = 0;

                        for (int i = 1; i < dataPoints.Count; i++)
                        {
                            if (dataPoints[i].X.Day != day)
                            {
                                labels.Add(new AxisXDateLabel(startIndex, i, day.ToString(), ThickBrush));
                                day = dataPoints[i].X.Day;
                                startIndex = i;
                            }
                        }
                        labels.Add(new AxisXDateLabel(startIndex, dataPoints.Count - 1, dataPoints[dataPoints.Count - 1].X.Day.ToString(), ThickBrush));
                        return labels.ToArray();
                        //for (int i = 1; i < dataPoints.Count; i++)
                        //{
                        //    labels.Add(new AxisXDateLabel(i - 1, i, dataPoints[i].X.Day.ToString(), ThickBrush));
                        //}
                        //return labels.ToArray();
                    }
                case SpanType.Monthly:
                    {
                        int month = dataPoints[0].X.Month;
                        int startIndex = 0;

                        for (int i = 1; i < dataPoints.Count; i++)
                        {
                            if (dataPoints[i].X.Month > month || (month == 12 && dataPoints[i].X.Month == 1))
                            {
                                labels.Add(new AxisXDateLabel(startIndex, i, month.ToString(), ThickBrush));
                                month = dataPoints[i].X.Month;
                                startIndex = i;
                            }
                        }
                        labels.Add(new AxisXDateLabel(startIndex, dataPoints.Count - 1, dataPoints[dataPoints.Count - 1].X.Month.ToString(), ThickBrush));
                        return labels.ToArray();
                    }
                case SpanType.Yearly:
                    {
                        int year = dataPoints[0].X.Year;
                        int startIndex = 0;

                        for (int i = 1; i < dataPoints.Count; i++)
                        {
                            if (dataPoints[i].X.Year > year)
                            {
                                labels.Add(new AxisXDateLabel(startIndex, i, year.ToString(), ThickBrush));
                                year = dataPoints[i].X.Year;
                                startIndex = i;
                            }
                        }
                        labels.Add(new AxisXDateLabel(startIndex, dataPoints.Count - 1, dataPoints[dataPoints.Count - 1].X.Year.ToString(), ThickBrush));
                        return labels.ToArray();
                    }
                default:
                    throw new ArgumentOutOfRangeException("spanType", spanType, "Unsupported span type");
            }
        }

        public int StartIndex
        {
            get
            {
                return this._startIndex;
            }
        }
        public int EndIndex
        {
            get
            {
                return this._endIndex;
            }
        }

        public string Text
        {
            get
            {
                return this._text;
            }
        }

        public SolidColorBrush Brush
        {
            get
            {
                return this._brush;
            }
        }
    }
}
