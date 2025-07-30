using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.ViewModel.Ratings
{
    public class OpponentResultViewModel
    {
        public string Opponent { get; set; }
        //public int ScorePlayer { get; set; }
        //public float ExpectedScore { get; set; }

        public int Wins { get; set; }
        public int Losses { get; set; }

        public string WinLoss
        {
            get
            {
                return String.Format("{0} : {1}", this.Wins, this.Losses);
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

        public bool IsWin
        {
            get
            {
                return this.Wins >= this.Losses;
            }
        }

        //private int TotalGames
        //{
        //    get
        //    {
        //        return this.Losses + this.Wins;
        //    }
        //}

        //public float AverageScorePerformance
        //{
        //    get
        //    {
        //        if (this.TotalGames == 0)
        //        {
        //            return 0d;
        //        }
        //        return (this.ScorePlayer - this.ExpectedScore) / (float)this.TotalGames;
        //    }
        //}

        //public bool IsPerformancePositive
        //{
        //    get
        //    {
        //        return this.AverageScorePerformance > 0;
        //    }
        //}
    }
}
