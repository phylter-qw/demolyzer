using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace QuantumBitDesigns.Charting
{
    public class ChartSeries : INotifyPropertyChanged
    {
        private double _maxY;
        private double _minY;
        private List<DataPoint> _data;
        private double _selectedPosition;

        private string _name;
        private Brush _seriesBrush;

        private ObservableCollection<int> _dateLabels;

        public ChartSeries(string name, Brush brush)
        {
            this._name = name;
            this._seriesBrush = brush;
            this._data = new List<DataPoint>();

            this._dateLabels = new ObservableCollection<int>();
            this._dateLabels.Add(333);
        }

        public ObservableCollection<int> DateLabels
        {
            get
            {
                return this._dateLabels;
            }
        }

        public void Add(double value)
        {
            Add(new DataPoint(DateTime.MinValue, value));
        }

        public void Add(DataPoint dataPoint)
        {
            this._maxY = Math.Max(this._maxY, dataPoint.Value);
            this._minY = Math.Min(this._minY, dataPoint.Value);
            this._data.Add(dataPoint);
            OnDataAdded(EventArgs.Empty);
        }

        public double SelectedPosition
        {
            get
            {
                return this._selectedPosition;
            }
            set
            {
                this._selectedPosition = value;
                OnPropertyChanged("SelectedPosition");
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public Brush Brush
        {
            get
            {
                return this._seriesBrush;
            }
        }

        public List<DataPoint> Data
        {
            get
            {
                return this._data;
            }
        }

        public double MaxY
        {
            get
            {
                return this._maxY;
            }
        }

        public double MinY
        {
            get
            {
                return this._minY;
            }
        }
        protected virtual void OnDataAdded(EventArgs e)
        {
            if (this.DataAdded != null)
            {
                this.DataAdded(this, e);
            }
        }
        public event EventHandler DataAdded;


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
