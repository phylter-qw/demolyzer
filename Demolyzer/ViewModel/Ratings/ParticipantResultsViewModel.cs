//#define CHART_BY_DATE
#define SMA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demolyzer.Model.Ratings;
using QuantumBitDesigns.Charting;
using QuantumBitDesigns.Mvvm;
using System.Windows.Media;

namespace Demolyzer.ViewModel.Ratings
{
    public class ParticipantResultsViewModel : ViewModelBase
    {
        public string Name { get { return this._player.Name; } }
        //public float ELO { get { return this._player.EloFrags.ELO; } }

        public bool Is1on1Matches { get { return this._player.Is1on1Matches; } }

        //Combined ELO
        public float ELOCombinedIndicatorWidth { get; set; }
        public bool IsELOCombinedAboveAverage { get { return this.ELOCombined >= 0; } }
        public float ELOCombined 
        { 
            get 
            {
                if (this._player.Is1on1Matches == true)
                {
                    return this.ELOFrags;
                }
                else
                {
                    return 0.25f * this.ELOFrags + 0.25f * this.ELODamageGiven + 0.25f * this.ELOEfficiency + 0.25f * this.ELOTeamplay; 
                }
            } 
        }

        //frags
        public float ELOFragsIndicatorWidth { get; set; }
        public bool IsELOFragsAboveAverage { get { return this.ELOFrags >= 0; } }
        public float ELOFrags { get { return this._player.EloFrags.ELOAverageNormalized; } }

        //damage given
        public float ELODamageGivenIndicatorWidth { get; set; }
        public bool IsELODamageGivenAboveAverage { get { return this.ELODamageGiven >= 0; } }
        public float ELODamageGiven { get { return this._player.EloDamageGiven.ELOAverageNormalized; } }

        //efficiency
        public float ELOEfficiencyIndicatorWidth { get; set; }
        public bool IsELOEfficiencyAboveAverage { get { return this.ELOEfficiency >= 0; } }
        public float ELOEfficiency { get { return this._player.EloEfficiency.ELOAverageNormalized; } }

        //efficiency
        public float ELOTeamplayIndicatorWidth { get; set; }
        public bool IsELOTeamplayAboveAverage { get { return this.ELOTeamplay >= 0; } }
        public float ELOTeamplay { get { return this._player.EloTeamplay.ELOAverageNormalized; } }

        //RLsKilled
        public float ELORLsKilledIndicatorWidth { get; set; }
        public bool IsELORLsKilledAboveAverage { get { return this.ELORLsKilled >= 0; } }
        public float ELORLsKilled { get { return this._player.EloStats[iELOUtil.RLsKilled].ELOAverageNormalized; } }

        //Armors
        public float ELOArmorsIndicatorWidth { get; set; }
        public bool IsELOArmorsAboveAverage { get { return this.ELOArmors >= 0; } }
        public float ELOArmors { get { return this._player.EloStats[iELOUtil.Armors].ELOAverageNormalized; } }

        //Powerups
        public float ELOPowerupsIndicatorWidth { get; set; }
        public bool IsELOPowerupsAboveAverage { get { return this.ELOPowerups >= 0; } }
        public float ELOPowerups { get { return this._player.EloStats[iELOUtil.Powerups].ELOAverageNormalized; } }

        //RLsDropped
        public float ELORLsDroppedIndicatorWidth { get; set; }
        public bool IsELORLsDroppedAboveAverage { get { return this.ELORLsDropped >= 0; } }
        public float ELORLsDropped { get { return -1 * this._player.EloStats[iELOUtil.RLsDropped].ELOAverageNormalized; } }

        //TimeWithRL
        public float ELOTimeWithRLIndicatorWidth { get; set; }
        public bool IsELOTimeWithRLAboveAverage { get { return this.ELOTimeWithRL >= 0; } }
        public float ELOTimeWithRL { get { return this._player.EloStats[iELOUtil.TimeWithRL].ELOAverageNormalized; } }

        //AveragePower
        public float ELOAveragePowerIndicatorWidth { get; set; }
        public bool IsELOAveragePowerAboveAverage { get { return this.ELOAveragePower >= 0; } }
        public float ELOAveragePower { get { return this._player.EloStats[iELOUtil.AveragePower].ELOAverageNormalized; } }

        //RLvsRL
        public float ELORLvsRLIndicatorWidth { get; set; }
        public bool IsELORLvsRLAboveAverage { get { return this.ELORLvsRL >= 0; } }
        public float ELORLvsRL { get { return this._player.EloStats[iELOUtil.RLvsRL].ELOAverageNormalized; } }

        //RLvsX
        public float ELORLvsXIndicatorWidth { get; set; }
        public bool IsELORLvsXAboveAverage { get { return this.ELORLvsX >= 0; } }
        public float ELORLvsX { get { return this._player.EloStats[iELOUtil.RLvsX].ELOAverageNormalized; } }

        //XvsRL
        public float ELOXvsRLIndicatorWidth { get; set; }
        public bool IsELOXvsRLAboveAverage { get { return this.ELOXvsRL >= 0; } }
        public float ELOXvsRL { get { return this._player.EloStats[iELOUtil.XvsRL].ELOAverageNormalized; } }

        //XvsX
        public float ELOXvsXIndicatorWidth { get; set; }
        public bool IsELOXvsXAboveAverage { get { return this.ELOXvsX >= 0; } }
        public float ELOXvsX { get { return this._player.EloStats[iELOUtil.XvsX].ELOAverageNormalized; } }

        public float FPM { get { return this._player.StatsAverages[StatNamesAverages.FPM].Average; } }
        public float FPMIndicatorWidth { get { return this.FPM * 10f; } }

        public float FPMwithRL { get { return this._player.StatsAverages[StatNamesAverages.FPMwithRL].Average; } }
        public float FPMwithRLIndicatorWidth { get { return this.FPMwithRL * 10f; } }

        public float FPMwithNoRL { get { return this._player.StatsAverages[StatNamesAverages.FPMwithNoRL].Average; } }
        public float FPMwithNoRLIndicatorWidth { get { return this.FPMwithNoRL * 10f; } }

        public float DPM { get { return this._player.StatsAverages[StatNamesAverages.DPM].Average; } }
        public float DPMIndicatorWidth { get { return this.DPM * 10f; } }


        public float HighFragStreak { get { return this._player.StatsAverages[StatNamesAverages.HighFragStreak].Average; } }
        public float HighFragStreakIndicatorWidth { get { return this.HighFragStreak * 2f; } }

        public float TeamDamage { get { return this._player.StatsAverages[StatNamesAverages.TeamDamage].Average; } }
        public float TeamDamageIndicatorWidth { get { return this.TeamDamage / 10f; } }

        public float KillSpawns { get { return this._player.StatsAverages[StatNamesAverages.KillSpawns].Average; } }
        public float KillSpawnsIndicatorWidth { get { return this.KillSpawns * 3.5f; } }

        public float KillRL { get { return this._player.StatsAverages[StatNamesAverages.KillRL].Average; } }
        public float KillRLIndicatorWidth { get { return this.KillRL * 5f; } }

        public float KillQuad { get { return this._player.StatsAverages[StatNamesAverages.KillQuad].Average; } }
        public float KillQuadIndicatorWidth { get { return this.KillQuad * 30f; } }

        public float KillTeam { get { return this._player.StatsAverages[StatNamesAverages.KillTeam].Average; } }
        public float KillTeamIndicatorWidth { get { return this.KillTeam * 13f; } }

        public float Quads { get { return this._player.StatsAverages[StatNamesAverages.Quads].Average; } }
        public float QuadsIndicatorWidth { get { return this.Quads * 10f; } }

        public float RLDrop { get { return this._player.StatsTotalPercents[StatNamesTotalPercents.RLDrop].Average * 100f; } }
        public float RLDropIndicatorWidth { get { return this.RLDrop; } }
        public float KillsPerRL { get { return this._player.StatsTotalPercents[StatNamesTotalPercents.KillsPerRL].Average; } }
        public float KillsPerRLIndicatorWidth { get { return this.KillsPerRL * 6f; } }
        public float LivesToGetRL { get { return this._player.StatsTotalPercents[StatNamesTotalPercents.LivesToGetRL].Average; } }
        public float LivesToGetRLIndicatorWidth { get { return this.LivesToGetRL * 7f; } }
        public float TimePerRL { get { return this._player.StatsTotalPercents[StatNamesTotalPercents.TimePerRL].Average; } }
        public float TimePerRLIndicatorWidth { get { return this.TimePerRL * 0.7f; } }
        public float TimeToGetRL { get { return this._player.StatsTotalPercents[StatNamesTotalPercents.TimeToGetRL].Average; } }
        public float TimeToGetRLIndicatorWidth { get { return this.TimeToGetRL * 0.5f; } }
        public float TimeWithRL { get { return this._player.StatsTotalPercents[StatNamesTotalPercents.TimeWithRL].Average * 100f; } }
        public float TimeWithRLIndicatorWidth { get { return this.TimeWithRL; } }
        public float RLvsRL { get { return this._player.StatsTotalPercents[StatNamesTotalPercents.RLvsRL].Average * 100f; } }
        public float RLvsRLIndicatorWidth { get { return this.RLvsRL; } }
        public float RLvsX { get { return this._player.StatsTotalPercents[StatNamesTotalPercents.RLvsX].Average * 100f; } }
        public float RLvsXIndicatorWidth { get { return this.RLvsX * 0.9f; } }
        public float XvsRL { get { return this._player.StatsTotalPercents[StatNamesTotalPercents.XvsRL].Average * 100f; } }
        public float XvsRLIndicatorWidth { get { return this.XvsRL * 2f; } }
        public float XvsX { get { return this._player.StatsTotalPercents[StatNamesTotalPercents.XvsX].Average * 100f; } }
        public float XvsXIndicatorWidth { get { return this.XvsX; } }

        public float ParticipationPointsIndicatorWidth { get; set; }
        public int ParticipationPoints { get { return this._player.ParticipationPoints; } }

        public float WinPointsIndicatorWidth { get; set; }
        public int WinPoints { get { return this._player.WinPoints; } }

        public float TotalPointsIndicatorWidth { get; set; }
        public int TotalPoints { get { return this._player.TotalPoints; } }


#if SMA
        //public bool IsELOAboveAverage { get { return this.ELOAverageNormalized >= 0; } }
        //public float ELOAverage { get { return this._player.EloFrags.ELOAverage; } }
        //public float ELOAverageNormalized { get { return this._player.EloFrags.ELOAverageNormalized; } }
#else
        public float ELONormalized
        {
            get
            {
                return this._player.ELONormalized;
            }
        }
        public bool IsELOAboveAverage { get { return this.ELONormalized >= 0; } }
#endif
        public string Record { get { return String.Format("{0}-{1}", this._player.WinCount, this._player.LossCount); } }
        public bool IsWinningRecord { get { return this._player.WinCount >= this._player.LossCount; } }
        public int Division { get { return this._player.Division; } }
        public int Points { get { return this._player.Points; } }
        public float WinPercent { get { return (float)this._player.WinCount / (float)(this._player.WinCount + this._player.LossCount); } }
        public string Team { get { return this._player.Team; } }

        public ChartViewModel TotalPointsChart { get; set; }
        public List<OpponentResultViewModel> OpponentResults { get; set; }

        private ParticipantModel _player;
        private ParticipantMatchModel _selectedMatch;

        public DelegateCommand DemolyzeCommand { get; set; }

        public ParticipantResultsViewModel(ParticipantModel player)
        {
            this._player = player;

            float maxPerformanceFrags = 0;
            float maxPerformanceDamageGiven = 0;
            float maxPerformanceEfficiency = 0;
            float maxPerformanceTeamplay = 0;
            foreach (ParticipantMatchModel match in this._player.Matches)
            {
                maxPerformanceFrags = Math.Max(maxPerformanceFrags, Math.Abs(match.PerformanceFrags));
                if (player.Is1on1Matches == false)
                {
                    maxPerformanceDamageGiven = Math.Max(maxPerformanceDamageGiven, Math.Abs(match.PerformanceDamageGiven));
                    maxPerformanceEfficiency = Math.Max(maxPerformanceEfficiency, Math.Abs(match.PerformanceEfficiency));
                    maxPerformanceTeamplay = Math.Max(maxPerformanceTeamplay, Math.Abs(match.PerformanceTeamplay));
                }
            }
            foreach (ParticipantMatchModel match in this._player.Matches)
            {
                match.PerformanceFragsIndicatorWidth = 100f * Math.Abs(match.PerformanceFrags) / maxPerformanceFrags;
                if (player.Is1on1Matches == false)
                {
                    match.PerformanceDamageGivenIndicatorWidth = 100f * Math.Abs(match.PerformanceDamageGiven) / maxPerformanceDamageGiven;
                    match.PerformanceEfficiencyIndicatorWidth = 100f * Math.Abs(match.PerformanceEfficiency) / maxPerformanceEfficiency;
                    match.PerformanceTeamplayIndicatorWidth = 100f * Math.Abs(match.PerformanceTeamplay) / maxPerformanceTeamplay;
                }
            }
        }

        public List<ParticipantMatchModel> Matches
        {
            get
            {
                return this._player.Matches;
            }
        }


        public ParticipantMatchModel SelectedMatch
        {
            get
            {
                return this._selectedMatch;
            }
            set
            {
                this._selectedMatch = value;
                OnPropertyChanged(() => SelectedMatch);
            }
        }

        private bool _isComparisonCheckboxesVisible;

        public bool IsComparisonCheckboxesVisible
        {
            get
            {
                return this._isComparisonCheckboxesVisible;
            }
            set
            {
                this._isComparisonCheckboxesVisible = value;
                OnPropertyChanged(() => IsComparisonCheckboxesVisible);
            }
        }

        private bool _isOnComparisonTeamA;

        public bool IsOnComparisionTeamA
        {
            get
            {
                return this._isOnComparisonTeamA;
            }
            set
            {
                this._isOnComparisonTeamA = value;
                if (this.IsOnComparisionTeamA == true)
                {
                    ParticipantCompareService.Singleton.AddToTeamA(this);
                }
                else
                {
                    ParticipantCompareService.Singleton.RemoveFromTeamA(this);
                }
                OnPropertyChanged(() => IsOnComparisionTeamA);
            }
        }

        private bool _isOnComparisonTeamB;

        public bool IsOnComparisionTeamB
        {
            get
            {
                return this._isOnComparisonTeamB;
            }
            set
            {
                this._isOnComparisonTeamB = value;
                if (this.IsOnComparisionTeamB == true)
                {
                    ParticipantCompareService.Singleton.AddToTeamB(this);
                }
                else
                {
                    ParticipantCompareService.Singleton.RemoveFromTeamB(this);
                }
                OnPropertyChanged(() => IsOnComparisionTeamB);
            }
        }

        public void UpdateOpponentResults()
        {
            Dictionary<string, OpponentResultViewModel> opponentResults = new Dictionary<string, OpponentResultViewModel>();

            for (int i = 0; i < this._player.Matches.Count; ++i)
            {
                ParticipantMatchModel match = this._player.Matches[i];

                string opponent = match.Opponent;

                if (opponentResults.ContainsKey(opponent) == false)
                {
                    opponentResults[opponent] = new OpponentResultViewModel();
                }

                OpponentResultViewModel opponentResult = opponentResults[opponent];

                if (match.IsWin == true)
                {
                    opponentResult.Wins++;
                }
                else
                {
                    opponentResult.Losses++;
                }

                opponentResult.Opponent = opponent;
            }

            this.OpponentResults = opponentResults.Values.ToList();
        }
#if FALSE
        public void UpdateELOHistoryChart()
        {
            ChartSeries totalPointsSeries = new ChartSeries("Total Points", Brushes.Chartreuse);
#if SMA
            ChartSeries eloSMASeries = new ChartSeries("Average[100]", Brushes.Yellow);
            SimpleMovingAverage sma = new SimpleMovingAverage(100);
#endif

#if CHART_BY_DATE
            int index = 0;
            float lastElo = 0;
            DateTime date = DateTime.MinValue;
            do
            {
                if (index < this._player.Matches.Count)
                {
                    ParticipantMatchModel match = this._player.Matches[index];

                    if (date == DateTime.MinValue)
                    {
                        date = match.Date;
                    }

                    //move to last match of the day
                    while (index < this._player.Matches.Count && this._player.Matches[index].Date.DayOfYear == date.DayOfYear)
                    {
#if SMA
                        sma.AddValue(this._player.Matches[index].ELOPointsCurrent - RatingsUtil.ELOStartPoints);
#endif
                        index++;
                    }

                    //back up 1 to get to last match of the day
                    index--;

                    match = this._player.Matches[index];

                    lastElo = match.ELOPointsCurrent - RatingsUtil.ELOStartPoints;

                    index++;

                    totalPointsSeries.Add(new DataPoint(date, lastElo));

#if SMA
                    eloSMASeries.Add(new DataPoint(date, sma.Average));
#endif

                    date = date.AddDays(1);
                }
                else
                {
                    break;
                }
            }
            while (true);
#else
            List<float> history = this._player.EloFrags.History;
            for (int i = 0; i < history.Count; ++i)
            {
                float eloPoints = history[i] - RatingsUtil.ELOStartPoints;
                totalPointsSeries.Add(eloPoints);
#if SMA
                sma.AddValue(eloPoints);
                eloSMASeries.Add(sma.Average);
#endif
            }
#endif

            ChartViewModel chart = new ChartViewModel(String.Empty, false);
            chart.Add(totalPointsSeries);
#if SMA
            chart.Add(eloSMASeries);
#endif
            chart.CreateXAxisLabels();
            this.TotalPointsChart = chart;
        }
#endif
    }
}
