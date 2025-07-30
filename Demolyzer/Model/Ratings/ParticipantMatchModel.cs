using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model.Ratings
{
    public class ParticipantMatchModel
    {
        public string Player { get; set; }
        public string Opponent { get; set; }
        public string Map { get; set; }
        public DateTime Date { get; set; }
        //public float Percent { get; set; }
        //public float Weight { get; set; }
        public bool IsWin { get; set; }

        public MatchResult MatchResult { get; set; }
        public MatchEloResult MatchEloResult { get; set; }
        public bool IsPlayerA { get; set; }
        public int PlayerIndex { get; set; }
        //public byte IsShowBothExpected { get; set; }
        //public byte IsShowBothScore { get; set; }

        public string[] TeamAPlayerNames
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.TeamAPlayerNames;
                }
                else
                {
                    return this.MatchEloResult.TeamBPlayerNames;
                }
            }
        }

        public string[] TeamBPlayerNames
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.TeamBPlayerNames;
                }
                else
                {
                    return this.MatchEloResult.TeamAPlayerNames;
                }
            }
        }

        public int[] TeamAFrags
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamA;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamB;
                }
            }
        }

        public int[] TeamBFrags
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamB;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamA;
                }
            }
        }

        public float[] TeamAExpectedFrags
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamAExpected;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamBExpected;
                }
            }
        }

        public float[] TeamBExpectedFrags
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamBExpected;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamAExpected;
                }
            }
        }

        public int[] TeamAFragPerformance
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamAPerformance;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamBPerformance;
                }
            }
        }

        public int[] TeamBFragPerformance
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamBPerformance;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamAPerformance;
                }
            }
        }

        public int TeamAFragsTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamATotal;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamBTotal;
                }
            }
        }

        public int TeamBFragsTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamBTotal;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamATotal;
                }
            }
        }

        public float TeamAExpectedFragsTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamAExpectedTotal;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamBExpectedTotal;
                }
            }
        }

        public float TeamBExpectedFragsTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamBExpectedTotal;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamAExpectedTotal;
                }
            }
        }

        public int TeamAFragPerformanceTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamAPerformanceTotal;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamBPerformanceTotal;
                }
            }
        }

        public int TeamBFragPerformanceTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamBPerformanceTotal;
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamAPerformanceTotal;
                }
            }
        }


        public int[] TeamADamageGiven
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamA;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamB;
                }
            }
        }

        public int[] TeamBDamageGiven
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamB;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamA;
                }
            }
        }

        public float[] TeamAExpectedDamageGiven
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamAExpected;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamBExpected;
                }
            }
        }

        public float[] TeamBExpectedDamageGiven
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamBExpected;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamAExpected;
                }
            }
        }

        public int[] TeamADamageGivenPerformance
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamAPerformance;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamBPerformance;
                }
            }
        }

        public int[] TeamBDamageGivenPerformance
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamBPerformance;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamAPerformance;
                }
            }
        }

        public int TeamADamageGivenTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamATotal;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamBTotal;
                }
            }
        }

        public int TeamBDamageGivenTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamBTotal;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamATotal;
                }
            }
        }

        public float TeamAExpectedDamageGivenTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamAExpectedTotal;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamBExpectedTotal;
                }
            }
        }

        public float TeamBExpectedDamageGivenTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamBExpectedTotal;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamAExpectedTotal;
                }
            }
        }

        public int TeamADamageGivenPerformanceTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamAPerformanceTotal;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamBPerformanceTotal;
                }
            }
        }

        public int TeamBDamageGivenPerformanceTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamBPerformanceTotal;
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamAPerformanceTotal;
                }
            }
        }

        public int[] TeamAEfficiency
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamA;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamB;
                }
            }
        }

        public int[] TeamBEfficiency
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamB;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamA;
                }
            }
        }

        public float[] TeamAExpectedEfficiency
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamAExpected;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamBExpected;
                }
            }
        }

        public float[] TeamBExpectedEfficiency
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamBExpected;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamAExpected;
                }
            }
        }

        public int[] TeamAEfficiencyPerformance
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamAPerformance;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamBPerformance;
                }
            }
        }

        public int[] TeamBEfficiencyPerformance
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamBPerformance;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamAPerformance;
                }
            }
        }

        public int TeamAEfficiencyTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamATotal;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamBTotal;
                }
            }
        }

        public int TeamBEfficiencyTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamBTotal;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamATotal;
                }
            }
        }

        public float TeamAExpectedEfficiencyTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamAExpectedTotal;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamBExpectedTotal;
                }
            }
        }

        public float TeamBExpectedEfficiencyTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamBExpectedTotal;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamAExpectedTotal;
                }
            }
        }

        public int TeamAEfficiencyPerformanceTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamAPerformanceTotal;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamBPerformanceTotal;
                }
            }
        }

        public int TeamBEfficiencyPerformanceTotal
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamBPerformanceTotal;
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamAPerformanceTotal;
                }
            }
        }

        //public string ExpectedFragsText
        //{
        //    get
        //    {
        //        if (this.IsShowBothExpected == 0)
        //        {
        //            return String.Format("{0:F0}", this.ExpectedFragsPlayerA);
        //        }
        //        return String.Format("{0:F0}:{1:F0}", this.ExpectedFragsPlayerA, this.ExpectedFragsPlayerB);
        //    }
        //}

        //public string FragsText
        //{
        //    get
        //    {
        //        if (this.IsShowBothScore == 0)
        //        {
        //            return String.Format("{0}", this.FragsPlayerA);
        //        }
        //        return String.Format("{0}:{1}", this.FragsPlayerA, this.FragsPlayerB);
        //    }
        //}

        public float ExpectedFragsPlayerA
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamAExpected[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamBExpected[this.PlayerIndex];
                }
            }
        }

        public float ExpectedFragsPlayerB
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamBExpected[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamAExpected[this.PlayerIndex];
                }
            }
        }

        public int FragsPlayerA
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamA[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamB[this.PlayerIndex];
                }
            }
        }

        public int FragsPlayerB
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Frags.TeamB[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Frags.TeamA[this.PlayerIndex];
                }
            }
        }

        public float ExpectedDamageGivenPlayerA
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamAExpected[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamBExpected[this.PlayerIndex];
                }
            }
        }

        public float ExpectedDamageGivenPlayerB
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamBExpected[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamAExpected[this.PlayerIndex];
                }
            }
        }

        public int DamageGivenPlayerA
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamA[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamB[this.PlayerIndex];
                }
            }
        }

        public int DamageGivenPlayerB
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.DamageGiven.TeamB[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.DamageGiven.TeamA[this.PlayerIndex];
                }
            }
        }


        public float ExpectedEfficiencyPlayerA
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamAExpected[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamBExpected[this.PlayerIndex];
                }
            }
        }

        public float ExpectedEfficiencyPlayerB
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamBExpected[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamAExpected[this.PlayerIndex];
                }
            }
        }

        public int EfficiencyPlayerA
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamA[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamB[this.PlayerIndex];
                }
            }
        }

        public int EfficiencyPlayerB
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Efficiency.TeamB[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Efficiency.TeamA[this.PlayerIndex];
                }
            }
        }


        public float ExpectedTeamplayPlayerA
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Teamplay.TeamAExpected[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Teamplay.TeamBExpected[this.PlayerIndex];
                }
            }
        }

        public float ExpectedTeamplayPlayerB
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Teamplay.TeamBExpected[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Teamplay.TeamAExpected[this.PlayerIndex];
                }
            }
        }

        public int TeamplayPlayerA
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Teamplay.TeamA[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Teamplay.TeamB[this.PlayerIndex];
                }
            }
        }

        public int TeamplayPlayerB
        {
            get
            {
                if (this.IsPlayerA == true)
                {
                    return this.MatchEloResult.Teamplay.TeamB[this.PlayerIndex];
                }
                else
                {
                    return this.MatchEloResult.Teamplay.TeamA[this.PlayerIndex];
                }
            }
        }

        public string Result
        {
            get
            {
                if (this.IsWin == true)
                {
                    return "Win";
                }
                return "Loss";
            }
        }

        public string TotalScoreText
        {
            get
            {
                return String.Format("{0}:{1}", this.TeamAFragsTotal, this.TeamBFragsTotal);
            }
        }

        public int PerformanceFrags
        {
            get
            {
                return this.FragsPlayerA - (int)this.ExpectedFragsPlayerA;
            }
        }

        public float PerformanceFragsIndicatorWidth { get; set; }

        public bool IsPerformanceFragsPositive
        {
            get
            {
                return this.PerformanceFrags > 0;
            }
        }


        public int PerformanceDamageGiven
        {
            get
            {
                return this.DamageGivenPlayerA - (int)this.ExpectedDamageGivenPlayerA;
            }
        }

        public float PerformanceDamageGivenIndicatorWidth { get; set; }

        public bool IsPerformanceDamageGivenPositive
        {
            get
            {
                return this.PerformanceDamageGiven > 0;
            }
        }


        public int PerformanceEfficiency
        {
            get
            {
                return this.EfficiencyPlayerA - (int)this.ExpectedEfficiencyPlayerA;
            }
        }

        public float PerformanceEfficiencyIndicatorWidth { get; set; }

        public bool IsPerformanceEfficiencyPositive
        {
            get
            {
                return this.PerformanceEfficiency > 0;
            }
        }

        public int PerformanceTeamplay
        {
            get
            {
                return this.TeamplayPlayerA - (int)this.ExpectedTeamplayPlayerA;
            }
        }

        public float PerformanceTeamplayIndicatorWidth { get; set; }

        public bool IsPerformanceTeamplayPositive
        {
            get
            {
                return this.PerformanceTeamplay > 0;
            }
        }
    }
}
