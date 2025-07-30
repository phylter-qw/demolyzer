using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model
{
    public class MatchKill
    {
        public string FraggerName { get; set; }
        public double FraggerOffsetX { get; set; }
        public double FraggerOffsetY { get; set; }

        public string DeathName { get; set; }
        public double DeathOffsetX { get; set; }
        public double DeathOffsetY { get; set; }

        public Weapon Weapon { get; set; }
        public double MatchTime { get; set; }

        public bool IsActive { get; set; }
    }
}
