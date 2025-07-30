using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model
{
    public class PacketComparer : IComparer<DemoDelta>
    {
        public int Compare(DemoDelta x, DemoDelta y)
        {
#warning TODO optimize by using Field instead of property
            int xVal = (int)x.Type;
            int yVal = (int)y.Type;
            if (xVal > (int)DemoDeltaType.StatChanged && yVal > (int)DemoDeltaType.StatChanged)
            {
                return 0; //items are equal, no need to sort
            }

            //now sort according to enum values
            return xVal.CompareTo(yVal);
        }
    }
}
