using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model.Ratings
{
    public class SimpleMovingAverage
    {
        private int _history;
        private Queue<double> _values;
        private double _total;

        public SimpleMovingAverage(int history)
        {
            this._history = history;
            this._values = new Queue<double>();
        }

        public void AddValue(double value)
        {
            this._values.Enqueue(value);
            this._total += value;
            if (this._values.Count > this._history)
            {
                this._total -= this._values.Dequeue();
            }
        }

        public double Average
        {
            get
            {
                if (this._values.Count == 0)
                {
                    return 0d;
                }
                return this._total / (double)this._values.Count;
            }
        }
    }
}
