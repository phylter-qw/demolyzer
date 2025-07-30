using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumBitDesigns.Charting
{
    public struct DataPoint
    {
        private DateTime _x;
        private double _value;

        public DataPoint(DateTime x, double value)
        {
            this._x = x;
            this._value = value;
        }

        public DateTime X
        {
            get
            {
                return this._x;
            }
        }

        public double Value
        {
            get
            {
                return this._value;
            }
        }    
    }
}
