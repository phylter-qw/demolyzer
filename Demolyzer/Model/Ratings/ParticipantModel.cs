#define SMA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model.Ratings
{
    public class Elo
    {
        private SimpleMovingAverage _average;
        public float ELO { get; private set; }
        public List<float> History { get; private set; }
        private bool _containsValue;

        public Elo()
        {
            this._average = new SimpleMovingAverage(100);
            this.ELO = RatingsUtil.ELOStartPoints;
            this.History = new List<float>(10000);
        }

        public void UpdateELO(float elo)
        {
            this.ELO = elo;
            this._average.AddValue(elo);
            this.History.Add(elo);
            this._containsValue = true;
        }

        public float ELOAverage
        {
            get
            {
                return (float)this._average.Average;
            }
        }

        public float ELOAverageNormalized
        {
            get
            {
                if (this._containsValue == false)
                {
                    return 0;
                }
                return this.ELOAverage - RatingsUtil.ELOStartPoints;
            }
        }

        public float ELONormalized
        {
            get
            {
                return this.ELO - RatingsUtil.ELOStartPoints;
            }
        }
    }

    public class Averager
    {
        private float _value;
        private float _total;

        public Averager()
        {
        }

        public void Add(float value)
        {
            this._value += value;
            this._total += 1;
        }

        public void Add(float value, float total)
        {
            this._value += value;
            this._total += total;
        }

        public float Average
        {
            get
            {
                if (this._total == 0)
                {
                    return 0f;
                }
                return this._value / (float)this._total;
            }
        }
    }

    public class ParticipantModel
    {
        public bool Is1on1Matches { get; set; }

        public Elo EloFrags { get; private set; }
        public Elo EloDamageGiven { get; private set; }
        public Elo EloEfficiency { get; private set; }
        public Elo EloTeamplay { get; private set; }
        
        public float EloCombinedNormalized
        {
            get
            {
                return 0.25f * this.EloFrags.ELOAverageNormalized + 0.25f * this.EloEfficiency.ELOAverageNormalized + 0.25f * this.EloDamageGiven.ELOAverageNormalized + 0.25f * this.EloTeamplay.ELOAverageNormalized;
            }
        }

        public Dictionary<string, Elo> EloStats { get; private set; }
        public short LossCount { get; set; }
        public short WinCount { get; set; }
        public short Division { get; set; }
        public short Points { get; set; }
        public int ParticipationPoints { get; set; }
        public int WinPoints { get; set; }
        public int TotalPoints { get { return this.ParticipationPoints + this.WinPoints; } }
        public Dictionary<string, Averager> StatsAverages { get; set; }
        public Dictionary<string, Averager> StatsTotalPercents { get; set; }

        public List<ParticipantMatchModel> Matches { get; set; }
        private Dictionary<string, int> _teams;

        public string Name { get; set; }

        public int MatchCount
        {
            get
            {
                return this.LossCount + this.WinCount;
            }
        }

        public ParticipantModel(string name)
        {
            this.Name = name;
            this.Matches = new List<ParticipantMatchModel>();
            this.EloFrags = new Elo();
            this.EloDamageGiven = new Elo();
            this.EloEfficiency = new Elo();
            this.EloTeamplay = new Elo();
            this._teams = new Dictionary<string, int>();

            this.EloStats = new Dictionary<string, Elo>();
            this.EloStats[iELOUtil.RLsKilled] = new Elo();
            this.EloStats[iELOUtil.Armors] = new Elo();
            this.EloStats[iELOUtil.Powerups] = new Elo();
            this.EloStats[iELOUtil.RLsDropped] = new Elo();
            this.EloStats[iELOUtil.TimeWithRL] = new Elo();
            this.EloStats[iELOUtil.AveragePower] = new Elo();
            this.EloStats[iELOUtil.RLvsRL] = new Elo();
            this.EloStats[iELOUtil.RLvsX] = new Elo();
            this.EloStats[iELOUtil.XvsRL] = new Elo();
            this.EloStats[iELOUtil.XvsX] = new Elo();

            this.StatsAverages = new Dictionary<string, Averager>();
            foreach (string statName in StatNamesAverages.All)
            {
                this.StatsAverages[statName] = new Averager();
            }

            this.StatsTotalPercents = new Dictionary<string, Averager>();
            foreach (string statName in StatNamesTotalPercents.All)
            {
                this.StatsTotalPercents[statName] = new Averager();
            }
        }

        public void AddTeam(string team)
        {
            if (team == null)// || team == "mix" || team == "red" || team == "blue" || team == "1" || team == "2")
            {
                return;
            }
            if (this._teams.ContainsKey(team) == false)
            {
                this._teams.Add(team, 0);
            }
            this._teams[team]++;
        }

        private string _team;

        public string Team
        {
            get
            {
                if (this._team == null)
                {
                    int maxCount = 0;
                    foreach (var kvp in this._teams)
                    {
                        if (kvp.Value > maxCount)
                        {
                            this._team = kvp.Key;
                        }
                        maxCount = Math.Max(maxCount, kvp.Value);
                    }
                }
                return this._team;
            }
        }
    }
}
