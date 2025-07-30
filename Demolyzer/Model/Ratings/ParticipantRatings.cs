using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model.Ratings
{
    public class ParticipantRatings
    {
        private Dictionary<string, double> _ratings;

        public ParticipantRatings()
        {
            this._ratings = new Dictionary<string, double>();
        }
    }
}
