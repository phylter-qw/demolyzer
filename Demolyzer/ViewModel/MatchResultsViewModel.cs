using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Demolyzer.Model;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using QuantumBitDesigns.Mvvm;
using System.Diagnostics;
using QuantumBitDesigns.Charting;
using System.Windows.Threading;

namespace Demolyzer.ViewModel
{
    public class MatchResultsViewModel : ViewModelBase
    {
        private DemoContent _demoContent;
        private MapCanvasViewModel _mapCanvas;
        private MatchRunner _demoRunner;
        private double _matchProgressIndicatorWidth;
        private DelegateMarshaler _marshaler;
        private bool _isError;
        private string _demoError;

        private DispatcherTimer _resetTimer;
        private int _resetToIndex;

        public DelegateCommand PlayCommand { get; set; }
        public DelegateCommand PauseCommand { get; set; }
        public DelegateCommand ResetCommand { get; set; }

        public MatchResultsViewModel()
        {
            this._mapCanvas = new MapCanvasViewModel();
            this._finalStatsPlayers = new SortableObservableCollection<ParticipantInfoViewModel>();
            this._finalStatsTeams = new SortableObservableCollection<ParticipantInfoViewModel>();
            this._currentStatsPlayers = new SortableObservableCollection<ParticipantInfoViewModel>();
            this._currentStatsTeams = new SortableObservableCollection<ParticipantInfoViewModel>();

            this.PlayCommand = new DelegateCommand(Play);
            this.PauseCommand = new DelegateCommand(Pause);
            this.ResetCommand = new DelegateCommand(Reset);

            this._marshaler = DelegateMarshaler.Create();

            this._resetTimer = new DispatcherTimer();
            this._resetTimer.Interval = TimeSpan.FromSeconds(1);
            this._resetTimer.Tick += new EventHandler(_resetTimer_Tick);

            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            OnPropertyChanged("CurrentPacketIndex");
            OnPropertyChanged("DemoTime");
            OnPropertyChanged("MatchProgressIndicatorOffsetX");
            foreach (var player in this._currentStatsPlayers)
            {
                player.RefreshUIBindings();
            }
            foreach (var team in this._currentStatsTeams)
            {
                team.RefreshUIBindings();
            }
            this._currentStatsPlayers.Sort(x => x.Score, ListSortDirection.Descending);
        }

        public bool IsStopEnabled
        {
            get
            {
                if (this._demoRunner == null)
                {
                    return false;
                }
                return this._demoRunner.IsExecuting == true;
            }
        }

        public bool IsPauseEnabled
        {
            get
            {
                if (this._demoRunner == null)
                {
                    return false;
                }
                return this._demoRunner.IsExecuting == true && this._demoRunner.IsPaused == false;
            }
        }

        public bool IsPlayEnabled
        {
            get
            {
                if (this._demoContent == null)
                {
                    return false;
                }
                return this._demoRunner == null || this._demoRunner.IsExecuting == false || this._demoRunner.IsPaused == true;
            }
        }

        public double PlaybackRate
        {
            get
            {
                if (this._demoRunner == null)
                {
                    return 1d;
                }
                return this._demoRunner.Rate;
            }
            set
            {
                if (this._demoRunner != null)
                {
                    this._demoRunner.Rate = value;
                }
                OnPropertyChanged("PlaybackRate");
            }
        }

        private void Play()
        {
            //if stopped, reload data
            if (this._demoRunner == null)
            {
                Reset();
            }
            this._demoRunner.Play();
            OnButtonPropertiesChanged();
        }

        private void Pause()
        {
            this._demoRunner.Pause();
            OnButtonPropertiesChanged();
        }

        private void Stop()
        {
            if (this._demoRunner != null)
            {
                this._demoRunner.Stop();
                this._demoRunner.PackDropped -= demoRunner_PackDropped;
                this._demoRunner.PackPickup -= demoRunner_PackPickup;
                this._demoRunner = null;
            }
            OnPropertyChanged("PlaybackRate");
            OnButtonPropertiesChanged();
        }

        private void OnButtonPropertiesChanged()
        {
            OnPropertyChanged("IsStopEnabled");
            OnPropertyChanged("IsPlayEnabled");
            OnPropertyChanged("IsPauseEnabled");
        }


        private void Reset()
        {
            Stop();
            this._demoRunner = new MatchRunner(this._demoContent);
            this._demoRunner.PackDropped += demoRunner_PackDropped;
            this._demoRunner.PackPickup += demoRunner_PackPickup;
            this._mapCanvas.Load(this._demoContent.ServerInfo.Map, 
                this._demoRunner.Processor.Players, 
                this._demoRunner.Processor.Kills, 
                this._demoContent.Entities.ToArray(),
                this._demoContent.FinalPlayerStats);

            BuildStatViewModels(this._demoContent);
            OnPropertyChanged("PlaybackRate");
            OnButtonPropertiesChanged();
        }

        public MapCanvasViewModel MapCanvas
        {
            get
            {
                return this._mapCanvas;
            }
        }

        private SortableObservableCollection<ParticipantInfoViewModel> _finalStatsPlayers;
        private SortableObservableCollection<ParticipantInfoViewModel> _finalStatsTeams;
        private SortableObservableCollection<ParticipantInfoViewModel> _currentStatsPlayers;
        private SortableObservableCollection<ParticipantInfoViewModel> _currentStatsTeams;

        public SortableObservableCollection<ParticipantInfoViewModel> FinalStatsPlayers
        {
            get
            {
                return this._finalStatsPlayers;
            }
        }

        public SortableObservableCollection<ParticipantInfoViewModel> FinalStatsTeams
        {
            get
            {
                return this._finalStatsTeams;
            }
        }

        public SortableObservableCollection<ParticipantInfoViewModel> CurrentStatsPlayers
        {
            get
            {
                return this._currentStatsPlayers;
            }
        }

        public SortableObservableCollection<ParticipantInfoViewModel> CurrentStatsTeams
        {
            get
            {
                return this._currentStatsTeams;
            }
        }

        public bool IsError
        {
            get
            {
                return this._isError;
            }
            set
            {
                this._isError = value;
                OnPropertyChanged("IsError");
            }
        }

        public string DemoError
        {
            get
            {
                return this._demoError;
            }
            set
            {
                this._demoError = value;
                OnPropertyChanged("DemoError");
            }
        }

        public void LoadDemoContent(DemoContent content)
        {
            //make sure a pending reset does not occur
            this._resetTimer.Stop();

            this.IsError = content.IsError;

            if (content.IsError == true)
            {
                if (content.ServerInfo != null && content.ServerInfo.IsInvalidMap == true)
                {
                    this.DemoError = String.Format("Unsupported map: {0}", content.ServerInfo.Map);
                }
                else
                {
                    if (content.IsPlayerDropped == true)
                    {
                        this.DemoError = String.Format("Unable to parse demo: \r\n\r\n{0}\r\n\r\nA player dropped mid-game. \r\nA future version of Demolyzer will support this feature.", content.MvdName);
                    }
                    else
                    {
                        this.DemoError = String.Format("Unable to parse demo: \r\n\r\n{0}\r\n\r\nPlease send mvd to demos@demolyzer.com", content.MvdName);
                    }
                }
                return;
            }

            Stop();

            this._demoContent = content;

            Reset();
            //this._demoRunner = new MatchRunner(content);
            //this._demoRunner.PackDropped += demoRunner_PackDropped;
            //this._demoRunner.PackPickup += demoRunner_PackPickup;
            //this._mapCanvas.Load(content.ServerInfo.Map, this._demoRunner.Processor.Players, this._demoRunner.Processor.Kills, content.Entities.ToArray());

            //BuildStatViewModels(content);

            OnPropertyChanged("ServerInfo");
            OnPropertyChanged("PacketCount");
            OnPropertyChanged("PlaybackRate");

            BuildTeamPowerChart();
            BuildTeamLeadChart();
            BuildTeamFragsPerMinuteChart();
        }

        void _resetTimer_Tick(object sender, EventArgs e)
        {
            double currentRate = this.PlaybackRate;
            this._resetTimer.Stop();
            Reset();
            this._demoRunner.SeekToIndex(this._resetToIndex);
            this.PlaybackRate = currentRate;
            this._demoRunner.Play();
            OnButtonPropertiesChanged();
        }

        private void BuildStatViewModels(DemoContent content)
        {
            this._currentStatsPlayers.Clear();
            this._currentStatsTeams.Clear();
            this._finalStatsPlayers.Clear();
            this._finalStatsTeams.Clear();
            for (int i = 0; i < content.FinalPlayerStats.Length; ++i)
            {
                if (content.FinalPlayerStats[i].IsSpectator == false)
                {
                    this._finalStatsPlayers.Add(new ParticipantInfoViewModel(content.FinalPlayerStats[i]));

                    //use the processor's players since it creates new player objects
                    this._currentStatsPlayers.Add(new ParticipantInfoViewModel(this._demoRunner.Processor.Players[i].CurrentStats));
                }
            }

            this._finalStatsPlayers.Sort(x => x.Score, ListSortDirection.Descending);

            ParticipantInfo[] playerStats = content.FinalPlayerStats.Where(p => p.IsSpectator == false).ToArray();
            
            //if 2on2 or 4on4, add team stats
            if (playerStats.Length >= 4)
            {
                this._finalStatsPlayers.Add(new ParticipantInfoViewModel(playerStats.Where(info => info.TeamIndex == 0).ToArray(), false));
                this._finalStatsPlayers.Add(new ParticipantInfoViewModel(playerStats.Where(info => info.TeamIndex == 1).ToArray(), false));
                this._currentStatsTeams.Add(new ParticipantInfoViewModel(this._currentStatsPlayers.Where(x => x.ParticipantInfo.TeamIndex == 0).Select(vm => vm.ParticipantInfo).ToArray(), true));
                this._currentStatsTeams.Add(new ParticipantInfoViewModel(this._currentStatsPlayers.Where(x => x.ParticipantInfo.TeamIndex == 1).Select(vm => vm.ParticipantInfo).ToArray(), true));
            }
        }

        void demoRunner_PackPickup(object sender, PackEventArgs e)
        {
            this._marshaler.BeginInvoke(() => this._mapCanvas.RemoveEntity(e.Pack));
        }

        void demoRunner_PackDropped(object sender, PackEventArgs e)
        {
            this._marshaler.BeginInvoke(() => this._mapCanvas.AddEntity(new EntityViewModel(e.Pack)));
        }

        private void BuildTeamPowerChart()
        {
            ChartViewModel chart = new ChartViewModel("Team power", true);
            ChartSeries teamA = new ChartSeries(this._demoContent.TeamPower[0].Team, Brushes.Red);
            
            int len = this._demoContent.TeamPower[0].MatchPower.Length;
            //if this is not a 4on4, the match length is only 10 minutes instead of 20
            if (this.MatchPlayerCount != 8)
            {
                len = len / 2;
            }

            for (int i = 0; i < len; ++i)
            {
                teamA.Add(this._demoContent.TeamPower[0].MatchPower[i]);
            }
            ChartSeries teamB = new ChartSeries(this._demoContent.TeamPower[1].Team, Brushes.Blue);
            for (int i = 0; i < len; ++i)
            {
                teamB.Add(this._demoContent.TeamPower[1].MatchPower[i]);
            }
            chart.Add(teamA);
            chart.Add(teamB);
            chart.AxisYLineCount = 5;
            chart.FontSize = 18d;
            this.TeamPower = chart;
        }

        private int MatchPlayerCount
        {
            get
            {
                if (this._demoContent == null)
                {
                    return 0;
                }
                return this._demoContent.FinalPlayerStats.Count(p => p.IsSpectator == false);
            }
        }
        private void BuildTeamLeadChart()
        {
            ChartViewModel chart = new ChartViewModel("Team lead", true);
            int[] teamAScores = this._demoContent.TeamPower[0].MatchScore;
            int[] teamBScores = this._demoContent.TeamPower[1].MatchScore;
            int len = teamAScores.Length;
            //if this is not a 4on4, the match length is only 10 minutes instead of 20
            if (this.MatchPlayerCount != 8)
            {
                len = len / 2;
            }
            /*
            int[] teamALeads = new int[len];
            int[] teamBLeads = new int[len];

            for (int i = 0; i < len; ++i)
            {
                int teamALead = teamAScores[i] - teamBScores[i];
                if (teamALead > 0)
                {
                    teamALeads[i] = teamALead;
                }
                int teamBLead = -teamALead;
                if (teamBLead > 0)
                {
                    teamBLeads[i] = teamBLead;
                }
            }
            */
            int[] teamALeads = teamAScores;
            int[] teamBLeads = teamBScores;

            ChartSeries teamA = new ChartSeries(this._demoContent.TeamPower[0].Team, Brushes.Red);
            for (int i = 0; i < teamALeads.Length; ++i)
            {
                teamA.Add(teamALeads[i]);
            }
            ChartSeries teamB = new ChartSeries(this._demoContent.TeamPower[1].Team, Brushes.Blue);
            for (int i = 0; i < teamBLeads.Length; ++i)
            {
                teamB.Add(teamBLeads[i]);
            }
            chart.Add(teamA);
            chart.Add(teamB);
            chart.AxisYLineCount = 5;
            chart.FontSize = 18d;
            this.TeamLead = chart;
        }

        private void BuildTeamFragsPerMinuteChart()
        {
            ChartViewModel chart = new ChartViewModel("Team Frags per Minute", true);
            int[] teamAScores = this._demoContent.TeamPower[0].MatchScore;
            int[] teamBScores = this._demoContent.TeamPower[1].MatchScore;

            int len = teamAScores.Length;
            //if this is not a 4on4, the match length is only 10 minutes instead of 20
            if (this.MatchPlayerCount != 8)
            {
                len = len / 2;
            }
            double[] teamAfpm = new double[len];
            double[] teamBfpm = new double[len];

            Queue<int> scores = new Queue<int>();
            int score = 0;
            for (int i = 1; i < len; ++i)
            {
                int scoreChange = teamAScores[i] - teamAScores[i - 1];
                score += scoreChange;
                scores.Enqueue(scoreChange);
                if (scores.Count > 60)
                {
                    score -= scores.Dequeue();
                }
                double fpm = (double)score;
                if (i < 60 && i > 10)
                {
                    fpm = 60d * fpm / (double)scores.Count;
                }
                teamAfpm[i] = fpm;
            }
            score = 0;
            scores.Clear();
            for (int i = 1; i < len; ++i)
            {
                int scoreChange = teamBScores[i] - teamBScores[i - 1];
                score += scoreChange;
                scores.Enqueue(scoreChange);
                if (scores.Count > 60)
                {
                    score -= scores.Dequeue();
                }
                double fpm = (double)score;
                if (i < 60 && i > 10)
                {
                    fpm = 60d * fpm / (double)scores.Count;
                }
                teamBfpm[i] = fpm;
            }

            ChartSeries teamA = new ChartSeries(this._demoContent.TeamPower[0].Team, Brushes.Red);
            for (int i = 0; i < teamAfpm.Length; ++i)
            {
                teamA.Add(teamAfpm[i]);
            }
            ChartSeries teamB = new ChartSeries(this._demoContent.TeamPower[1].Team, Brushes.Blue);
            for (int i = 0; i < teamBfpm.Length; ++i)
            {
                teamB.Add(teamBfpm[i]);
            }
            chart.Add(teamA);
            chart.Add(teamB);
            chart.AxisYLineCount = 5;
            chart.FontSize = 18d;
            this.TeamFragsPerMinute = chart;
        }

        private ChartViewModel _teamPower;
        private ChartViewModel _teamLead;
        private ChartViewModel _teamFragsPerMinute;

        public ChartViewModel TeamPower
        {
            get
            {
                return this._teamPower;
            }
            private set
            {
                this._teamPower = value;
                OnPropertyChanged("TeamPower");
            }
        }

        public ChartViewModel TeamLead
        {
            get
            {
                return this._teamLead;
            }
            private set
            {
                this._teamLead = value;
                OnPropertyChanged("TeamLead");
            }
        }

        public ChartViewModel TeamFragsPerMinute
        {
            get
            {
                return this._teamFragsPerMinute;
            }
            private set
            {
                this._teamFragsPerMinute = value;
                OnPropertyChanged("TeamFragsPerMinute");
            }
        }

        public int CurrentPacketIndex
        {
            get
            {
                if (this._demoRunner == null)
                {
                    return 0;
                }
                if (this._resetTimer.IsEnabled == true)
                {
                    return this._resetToIndex;
                }
                return this._demoRunner.CurrentPacketIndex;
            }
            set
            {
                if (value < (this._demoRunner.CurrentPacketIndex - 1000))
                {
                    this._resetToIndex = value;
                    this._resetTimer.Stop();
                    this._resetTimer.Start();
                }
                else
                {
                    this._demoRunner.SeekToIndex(value);
                }
            }
        }

        public int PacketCount
        {
            get
            {
                if (this._demoRunner == null)
                {
                    return 0;
                }
                return this._demoRunner.PacketCount;
            }
        }

        public string DemoTime
        {
            get
            {
                if (this._demoRunner == null)
                {
                    return null;
                }
                TimeSpan span = TimeSpan.FromSeconds(this._demoRunner.Processor.MatchTime);
                return String.Format("{0:mm\\:ss\\.fff}", span);
            }
        }

        public double MatchProgressIndicatorOffsetX
        {
            get
            {
                if (this.PacketCount == 0)
                {
                    return 0;
                }
                return this.MatchProgressIndicatorWidth * (double)this.CurrentPacketIndex / (double)this.PacketCount;
            }
        }

        public double MatchProgressIndicatorWidth
        {
            get
            {
                return this._matchProgressIndicatorWidth;
            }
            set
            {
                this._matchProgressIndicatorWidth = value;
                OnPropertyChanged("MatchProgressIndicatorOffsetX");
            }
        }

        public ServerInfo ServerInfo
        {
            get
            {
                if (this._demoContent == null)
                {
                    return null;
                }
                return this._demoContent.ServerInfo;
            }
        }
    }
}
