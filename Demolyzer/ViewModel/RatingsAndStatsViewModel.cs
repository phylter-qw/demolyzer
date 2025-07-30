using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Demolyzer.Model;
using QuantumBitDesigns.Charting;
using QuantumBitDesigns.Mvvm;
using Demolyzer.Model.Ratings;
using Demolyzer.ViewModel.Ratings;

namespace Demolyzer.ViewModel
{
    public class RatingsHostViewModel : ViewModelBase
    {
        public List<RatingsAndStatsViewModel> Ratings { get; set; }
        public RatingsHostViewModel(List<RatingsAndStatsViewModel> ratings)
        {
            this.Ratings = ratings;
        }
    }

    public class RatingsAndStatsViewModel : ViewModelBase
    {
        public event EventHandler RequestClose;

        private List<MatchResult> _matchResults;
        private List<ParticipantResultsViewModel> _participants;
        private List<ParticipantResultsViewModel> _participants2;

        public bool IsLoadSelectedMatchPending { get; private set; }

        public DelegateCommand DemolyzeSelectedMatchCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }

        public CompareParticipantsViewModel CompareParticipantsViewModel { get; private set; }

        public string MatchType { get; private set; }
        public bool IsTotals { get; private set; }

        public RatingsAndStatsViewModel(string matchType, List<MatchResult> results, bool isTotals)//Dictionary<MatchType, Dictionary<string, List<ParticipantInfo>>> playerMatchResults
        {
            this.MatchType = matchType;
            this._matchResults = results;
            this.IsTotals = isTotals;

            this.CompareParticipantsViewModel = new CompareParticipantsViewModel();

            this.DemolyzeSelectedMatchCommand = new DelegateCommand(DemolyzeSelectedMatch);
            this.CloseCommand = new DelegateCommand(Close);
        }

        private void Close()
        {
            this.CompareParticipantsViewModel.Close();
            OnRequestClose(EventArgs.Empty);
        }

        private void DemolyzeSelectedMatch()
        {
            this.IsLoadSelectedMatchPending = true;
            this.CompareParticipantsViewModel.Close();
            OnRequestClose(EventArgs.Empty);
        }

        public void Initialize()
        {
            this._matchResults.Sort((a, b) => { return a.DateTime.CompareTo(b.DateTime); });

            //Stopwatch sw = Stopwatch.StartNew();
            ParticipantLookupTable participants = ParticipantLookupTable.Create(this._matchResults, false);
            List<MatchEloResult> matchEloResults = iELOUtil.CalculateiELO(this._matchResults, participants, 500, false);
            //MessageBox.Show(sw.ElapsedDuration().ToString());

            for (int i = 0; i < this._matchResults.Count; ++i)
            {
                MatchResult match = this._matchResults[i];
                MatchEloResult matchEloResult = matchEloResults[i];

                for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                {
                    string playerA = match.TeamAPlayerNames[x];
                    string playerB = match.TeamBPlayerNames[x];

                    ParticipantModel participantA = participants.GetParticipant(match.TeamAPlayerNames[x]);
                    ParticipantModel participantB = participants.GetParticipant(match.TeamBPlayerNames[x]);

                    participantA.Matches.Add(new ParticipantMatchModel
                    {
                        MatchResult = match,
                        MatchEloResult = matchEloResult,
                        IsPlayerA = true,
                        PlayerIndex = x,
                        Player = match.TeamA,
                        Opponent = match.TeamB,
                        Map = match.Map,
                        Date = match.DateTime,
                        IsWin = match.TeamAScore >= match.TeamBScore,
                    });

                    participantB.Matches.Add(new ParticipantMatchModel
                    {
                        MatchResult = match,
                        MatchEloResult = matchEloResult,
                        IsPlayerA = false,
                        PlayerIndex = x,
                        Player = match.TeamB,
                        Opponent = match.TeamA,
                        Map = match.Map,
                        Date = match.DateTime,
                        IsWin = match.TeamBScore >= match.TeamAScore,
                    });
                }
            }
            this._participants = participants.GetParticipants().Select(participant => { return new ParticipantResultsViewModel(participant); }).ToList();
            this._participants2 = this._participants.ToList();
            this._participants2.Sort((a, b) => { return b.TotalPoints.CompareTo(a.TotalPoints); });

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELOFrags));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELOFragsIndicatorWidth = 100f * Math.Abs(player.ELOFrags) / maxELO;
                }

                //this._participants.Sort((a, b) => { return b.ELOFrags.CompareTo(a.ELOFrags); });
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELODamageGiven));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELODamageGivenIndicatorWidth = 100f * Math.Abs(player.ELODamageGiven) / maxELO;
                }
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELOEfficiency));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELOEfficiencyIndicatorWidth = 100f * Math.Abs(player.ELOEfficiency) / maxELO;
                }
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELOTeamplay));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELOTeamplayIndicatorWidth = 100f * Math.Abs(player.ELOTeamplay) / maxELO;
                }
            }

            //combined ELO
            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELOCombined));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELOCombinedIndicatorWidth = 100f * Math.Abs(player.ELOCombined) / maxELO;
                }

                this._participants.Sort((a, b) => { return b.ELOCombined.CompareTo(a.ELOCombined); });
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELORLsKilled));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELORLsKilledIndicatorWidth = 100f * Math.Abs(player.ELORLsKilled) / maxELO;
                }
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELOArmors));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELOArmorsIndicatorWidth = 100f * Math.Abs(player.ELOArmors) / maxELO;
                }
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELOPowerups));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELOPowerupsIndicatorWidth = 100f * Math.Abs(player.ELOPowerups) / maxELO;
                }
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELORLsDropped));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELORLsDroppedIndicatorWidth = 100f * Math.Abs(player.ELORLsDropped) / maxELO;
                }
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELOTimeWithRL));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELOTimeWithRLIndicatorWidth = 100f * Math.Abs(player.ELOTimeWithRL) / maxELO;
                }
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELOAveragePower));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELOAveragePowerIndicatorWidth = 100f * Math.Abs(player.ELOAveragePower) / maxELO;
                }
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELORLvsRL));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELORLvsRLIndicatorWidth = 100f * Math.Abs(player.ELORLvsRL) / maxELO;
                }
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELORLvsX));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELORLvsXIndicatorWidth = 100f * Math.Abs(player.ELORLvsX) / maxELO;
                }
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELOXvsRL));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELOXvsRLIndicatorWidth = 100f * Math.Abs(player.ELOXvsRL) / maxELO;
                }
            }

            {
                float maxELO = this._participants.Max(participant => Math.Abs(participant.ELOXvsX));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ELOXvsXIndicatorWidth = 100f * Math.Abs(player.ELOXvsX) / maxELO;
                }
            }

            {
                float max = this._participants.Max(participant => Math.Abs(participant.ParticipationPoints));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.ParticipationPointsIndicatorWidth = 100f * Math.Abs(player.ParticipationPoints) / max;
                }
            }

            {
                float max = this._participants.Max(participant => Math.Abs(participant.WinPoints));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.WinPointsIndicatorWidth = 100f * Math.Abs(player.WinPoints) / max;
                }
            }

            {
                float max = this._participants.Max(participant => Math.Abs(participant.TotalPoints));

                foreach (ParticipantResultsViewModel player in this._participants)
                {
                    player.TotalPointsIndicatorWidth = 100f * Math.Abs(player.TotalPoints) / max;
                }
            }

            StringBuilder sb = new StringBuilder(1000000);
            foreach (var participant in this._participants)
            {
                sb.AppendLine(String.Format("==========================================================================================================================="));
                sb.AppendLine(String.Format("Player: {0}", participant.Name));
                sb.AppendLine(String.Format("iELO: {0:F0}", participant.ELOCombined));
                foreach (ParticipantMatchModel match in participant.Matches)
                {
                    sb.AppendLine(String.Format("------------------------ {0} vs {1} [{2}] Map={3} -----------------------", match.Player, match.Opponent, match.Date.ToString(), match.Map));
                    sb.AppendLine(String.Format("Frags Expected:    {0,8}: {1,4:F0} {2,8}: {3,4:F0}", match.Player, match.TeamAExpectedFragsTotal, match.Opponent, match.TeamBExpectedFragsTotal));
                    sb.AppendLine(String.Format("Frags Actual:      {0,8}: {1,4:F0} {2,8}: {3,4:F0}", match.Player, match.TeamAFragsTotal, match.Opponent, match.TeamBFragsTotal));
                    sb.AppendLine(String.Format("Frags Performance: {0,8}: {1,4:F0} {2,8}: {3,4:F0}", match.Player, match.TeamAFragPerformanceTotal, match.Opponent, match.TeamBFragPerformanceTotal));
                    sb.AppendLine(String.Format("-------------"));
                    sb.AppendLine(String.Format("Frags Expected:"));
                    for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                    {
                        sb.AppendLine(String.Format("{0,12}: {1,4:F0}  {2,12}: {3,4:F0}", match.TeamAPlayerNames[x], match.TeamAExpectedFrags[x], match.TeamBPlayerNames[x], match.TeamBExpectedFrags[x]));
                    }
                    sb.AppendLine(String.Format("Frags Actual:"));
                    for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                    {
                        sb.AppendLine(String.Format("{0,12}: {1,4:F0}  {2,12}: {3,4:F0}", match.TeamAPlayerNames[x], match.TeamAFrags[x], match.TeamBPlayerNames[x], match.TeamBFrags[x]));
                    }
                    sb.AppendLine(String.Format("Frags Performance:"));
                    for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                    {
                        sb.AppendLine(String.Format("{0,12}: {1,4:F0}  {2,12}: {3,4:F0}", match.TeamAPlayerNames[x], match.TeamAFragPerformance[x], match.TeamBPlayerNames[x], match.TeamBFragPerformance[x]));
                    }
                    sb.AppendLine(String.Format("-------------"));
                    if (participant.Is1on1Matches == false)
                    {
                        sb.AppendLine(String.Format("Damage Expected:"));
                        for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                        {
                            sb.AppendLine(String.Format("{0,12}: {1,7:F0}  {2,12}: {3,7:F0}", match.TeamAPlayerNames[x], match.TeamAExpectedDamageGiven[x], match.TeamBPlayerNames[x], match.TeamBExpectedDamageGiven[x]));
                        }
                        sb.AppendLine(String.Format("Damage Actual:"));
                        for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                        {
                            sb.AppendLine(String.Format("{0,12}: {1,7:F0}  {2,12}: {3,7:F0}", match.TeamAPlayerNames[x], match.TeamADamageGiven[x], match.TeamBPlayerNames[x], match.TeamBDamageGiven[x]));
                        }
                        sb.AppendLine(String.Format("Damage Performance:"));
                        for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                        {
                            sb.AppendLine(String.Format("{0,12}: {1,7:F0}  {2,12}: {3,7:F0}", match.TeamAPlayerNames[x], match.TeamADamageGivenPerformance[x], match.TeamBPlayerNames[x], match.TeamBDamageGivenPerformance[x]));
                        }
                        sb.AppendLine(String.Format("-------------"));
                        sb.AppendLine(String.Format("Efficiency Expected:"));
                        for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                        {
                            sb.AppendLine(String.Format("{0,12}: {1,4:F0}  {2,12}: {3,4:F0}", match.TeamAPlayerNames[x], match.TeamAExpectedEfficiency[x], match.TeamBPlayerNames[x], match.TeamBExpectedEfficiency[x]));
                        }
                        sb.AppendLine(String.Format("Efficiency Actual:"));
                        for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                        {
                            sb.AppendLine(String.Format("{0,12}: {1,4:F0}  {2,12}: {3,4:F0}", match.TeamAPlayerNames[x], match.TeamAEfficiency[x], match.TeamBPlayerNames[x], match.TeamBEfficiency[x]));
                        }
                        sb.AppendLine(String.Format("Efficiency Performance:"));
                        for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                        {
                            sb.AppendLine(String.Format("{0,12}: {1,4:F0}  {2,12}: {3,4:F0}", match.TeamAPlayerNames[x], match.TeamAEfficiencyPerformance[x], match.TeamBPlayerNames[x], match.TeamBEfficiencyPerformance[x]));
                        }
                    }

                    //for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                    //{
                    //    sb.AppendLine(String.Format("{0}: {1:F0}", match.TeamBPlayerNames[x], match.TeamBExpectedFrags[x]));
                    //}
                    //sb.AppendLine(String.Format("Frags Actual:"));
                    //for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                    //{
                    //    sb.AppendLine(String.Format("{0}: {1:F0}", match.TeamAPlayerNames[x], match.TeamAFrags[x]));
                    //}
                    //for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                    //{
                    //    sb.AppendLine(String.Format("{0}: {1:F0}", match.TeamBPlayerNames[x], match.TeamBFrags[x]));
                    //}
                }
            }
            File.WriteAllText("out.log", sb.ToString());
        }

        public List<ParticipantResultsViewModel> Participants
        {
            get
            {
                return this._participants;
            }
        }

        public List<ParticipantResultsViewModel> Participants2
        {
            get
            {
                return this._participants2;
            }
        }

        private bool _isComparisonVisible;

        public bool IsComparisonVisible
        {
            get
            {
                return this._isComparisonVisible;
            }
            set
            {
                this._isComparisonVisible = value;

                foreach (var participant in this._participants)
                {
                    participant.IsComparisonCheckboxesVisible = this.IsComparisonVisible;
                }

                if (value == true)
                {
                    this.CompareParticipantsViewModel.Load();
                }
                else
                {
                    this.CompareParticipantsViewModel.Close();
                }
                OnPropertyChanged("IsComparisonVisible");
            }
        }

        private ParticipantResultsViewModel _selectedParticipant;

        public ParticipantResultsViewModel SelectedParticipant
        {
            get
            {
                return this._selectedParticipant;
            }
            set
            {
                //clear previous selected participant's memory
                if (this._selectedParticipant != null)
                {
                    this._selectedParticipant.TotalPointsChart = null;
                    this._selectedParticipant.OpponentResults = null;
                }

                this._selectedParticipant = value;
                if (this._selectedParticipant != null)
                {
                    //this._selectedParticipant.UpdateELOHistoryChart();
                    this._selectedParticipant.UpdateOpponentResults();
                    if (this._selectedParticipant.Matches.Count > 0)
                    {
                        this._selectedParticipant.SelectedMatch = this._selectedParticipant.Matches[0];
                    }
                    //double avg = this._selectedParticipant.Matches.Average(x => { return Math.Abs(x.ELOPointsGained); });
                    //Console.WriteLine(String.Format("{0}: {1:F1}", this._selectedParticipant.Name, avg));
                }
                OnPropertyChanged(() => SelectedParticipant);
            }
        }

        protected virtual void OnRequestClose(EventArgs e)
        {
            if (this.RequestClose != null)
            {
                this.RequestClose(this, e);
            }
        }

        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1MapAll;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm2;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm4;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm6;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapztndm3;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapaerowalk;

        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults2on2MapAll;
        ////private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm2;
        ////private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm4;
        ////private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm6;
        ////private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapztndm3;
        ////private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapaerowalk;

        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults4on4MapAll;

        //public SortableObservableCollection<ParticipantInfoViewModel> PlayerResults4on4MapAll
        //{
        //    get
        //    {
        //        return this._playerResults4on4MapAll;
        //    }
        //}
    }
}
