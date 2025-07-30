using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model.Ratings
{
    public class MatchResult
    {
        public string MvdFullName { get; set; }

        public DateTime DateTime { get; set; }
        public string Date { get; set; }
        public string Map { get; set; }
        public string TeamA { get; set; }
        public string TeamB { get; set; }
        public int TeamAScore { get; set; }
        public int TeamBScore { get; set; }
        public float TeamAPercent { get; set; }
        public float TeamBPercent { get; set; }
        public string[] TeamAPlayerNames { get; set; }
        public string[] TeamBPlayerNames { get; set; }
        public int[] TeamAScores { get; set; }
        public int[] TeamBScores { get; set; }
        public float[] TeamAPercents { get; set; }
        public float[] TeamBPercents { get; set; }

        public int[] TeamADamagesGiven { get; set; }
        public int[] TeamBDamagesGiven { get; set; }
        public int[] TeamAEfficiencies { get; set; }
        public int[] TeamBEfficiencies { get; set; }
        public int[] TeamATeamplays { get; set; }
        public int[] TeamBTeamplays { get; set; }

        public Dictionary<string, int[]> TeamAStats { get; set; }
        public Dictionary<string, int[]> TeamBStats { get; set; }

        public Dictionary<string, float[]> TeamAStatsAverages { get; set; }
        public Dictionary<string, float[]> TeamBStatsAverages { get; set; }
        public Dictionary<string, Tuple<float, float>[]> TeamAStatsTotalPercents { get; set; }
        public Dictionary<string, Tuple<float, float>[]> TeamBStatsTotalPercents { get; set; }


        public MatchResult()
        {
            this.TeamAStats = new Dictionary<string, int[]>();
            this.TeamBStats = new Dictionary<string, int[]>();

            this.TeamAStatsAverages = new Dictionary<string, float[]>();
            this.TeamBStatsAverages = new Dictionary<string, float[]>();

            this.TeamAStatsTotalPercents = new Dictionary<string, Tuple<float, float>[]>();
            this.TeamBStatsTotalPercents = new Dictionary<string, Tuple<float, float>[]>();
        }

        //public static DateTime CreateDateTime(string date)
        //{
        //    //08/07/2010 19:53:00
        //    return new DateTime(
        //        Convert.ToInt32(date.Substring(6, 4)),
        //        Convert.ToInt32(date.Substring(3, 2)),
        //        Convert.ToInt32(date.Substring(0, 2)),
        //        Convert.ToInt32(date.Substring(11, 2)),
        //        Convert.ToInt32(date.Substring(14, 2)),
        //        0);
        //}

        public int PlayerCount
        {
            get
            {
                if (this.TeamAPlayerNames == null)
                {
                    return 1;
                }
                return this.TeamAPlayerNames.Length;
            }
        }

        public string MatchType
        {
            get
            {
                switch (this.PlayerCount)
                {
                    case 1:
                        return "1on1";
                    case 2:
                        return "2on2";
                    case 4:
                        return "4on4";
                    default:
                        throw new ArgumentOutOfRangeException("PlayerCount", this.PlayerCount, "Unsupported player count");
                }
            }
        }

        public MatchResult Clone()
        {
            MatchResult result = new MatchResult();
            result.MvdFullName = this.MvdFullName;
            result.DateTime = this.DateTime;
            result.Map = this.Map;
            result.TeamA = this.TeamA;
            result.TeamB = this.TeamB;
            result.TeamAScore = this.TeamAScore;
            result.TeamBScore = this.TeamBScore;
            result.TeamAPercent = this.TeamAPercent;
            result.TeamBPercent = this.TeamBPercent;
            result.TeamAPlayerNames = this.TeamAPlayerNames;
            result.TeamBPlayerNames = this.TeamBPlayerNames;
            result.TeamAScores = this.TeamAScores;
            result.TeamBScores = this.TeamBScores;
            result.TeamAPercents = this.TeamAPercents;
            result.TeamBPercents = this.TeamBPercents;

            result.TeamADamagesGiven = this.TeamADamagesGiven;
            result.TeamBDamagesGiven = this.TeamBDamagesGiven;

            result.TeamAEfficiencies = this.TeamAEfficiencies;
            result.TeamBEfficiencies = this.TeamBEfficiencies;

            result.TeamATeamplays = this.TeamATeamplays;
            result.TeamBTeamplays = this.TeamBTeamplays;

            result.TeamAStats = this.TeamAStats;
            result.TeamBStats = this.TeamBStats;

            result.TeamAStatsAverages = this.TeamAStatsAverages;
            result.TeamBStatsAverages = this.TeamBStatsAverages;

            result.TeamAStatsTotalPercents = this.TeamAStatsTotalPercents;
            result.TeamBStatsTotalPercents = this.TeamBStatsTotalPercents;

            return result;
        }
    }
}
