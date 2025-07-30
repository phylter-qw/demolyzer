using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace QuantumBitDesigns.Charting
{
    public class ChartViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ChartSeries> _dataSeries;

        private double _maxY;

        private string _name;
        private bool _isZeroBased;
        private int _axisYLineCount;
        private AxisXDateLabel[] _axisXLabels;

        public string YAxisStringFormat { get; set; }
        public string ValueStringFormat { get; set; }

        public double FontSize { get; set; }

        public ChartViewModel(string name, bool isZeroBased)
            : this(name, isZeroBased,  null, 0)
        {
        }

        public ChartViewModel(string name, bool isZeroBased, ChartSeries series)
            : this(name, isZeroBased, series, 0)
        {

        }

        public ChartViewModel(string name, bool isZeroBased, ChartSeries series, double maxY)
        {
            this._name = name;
            this._isZeroBased = isZeroBased;
            this._dataSeries = new ObservableCollection<ChartSeries>();
            this._maxY = maxY;

            this._axisYLineCount = 5;

            if (series != null)
            {
                Add(series);
            }

            this.FontSize = 10d;
            this.YAxisStringFormat = "{0:F2}";
        }

        public int AxisYLineCount
        {
            get
            {
                return this._axisYLineCount;
            }
            set
            {
                this._axisYLineCount = value;
                OnPropertyChanged("AxisYPercentages");
            }
        }

        public void CreateXAxisLabels()
        {
            if (this._dataSeries.Count == 0)
            {
                return;
            }

            this._axisXLabels = AxisXDateLabel.Create(this._dataSeries[0].Data);
        }

        public AxisXDateLabel[] AxisXLabels
        {
            get
            {
                return this._axisXLabels;
            }
            private set
            {
                this._axisXLabels = value;
                OnPropertyChanged("AxisXLabels");
            }
        }

        public void Clear()
        {
            this._dataSeries.Clear();
        }

        public void Add(ChartSeries series)
        {
            series.DataAdded += Series_DataAdded;
            this._dataSeries.Add(series);

            this._maxY = Math.Max(this._maxY, series.MaxY);
            this._maxY = Math.Max(this._maxY, Math.Abs(series.MinY));
        }

        void Series_DataAdded(object sender, EventArgs e)
        {
            ChartSeries series = sender as ChartSeries;

            this._maxY = Math.Max(this._maxY, series.MaxY);
            this._maxY = Math.Max(this._maxY, Math.Abs(series.MinY));

            OnPropertyChanged("AxisXLabels");
            OnPropertyChanged("MaxY");
            OnPropertyChanged("Series");
        }

        internal void UpdateSelectedPosition(double position)
        {
            foreach (ChartSeries series in this._dataSeries)
            {
                series.SelectedPosition = position;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public ObservableCollection<ChartSeries> Series
        {
            get
            {
                return this._dataSeries;
            }
        }

        public bool IsZeroBased
        {
            get
            {
                return this._isZeroBased;
            }
        }

        public bool IsZeroCenter
        {
            get
            {
                return this._isZeroBased == false;
            }
        }

        public double MaxY
        {
            get
            {
                return ChartUtil.RoundMaxY(this._maxY);
            }
        }

        public double[] AxisYPercentages
        {
            get
            {
                if (this.MaxY == 0)
                {
                    return new double[] { };
                }
                return ChartUtil.CalculateAxisYPercentages(this._axisYLineCount);
            }
        }

        #region INotifyPropertyChanged Members

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
