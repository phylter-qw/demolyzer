using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Demolyzer.ViewModel;
using Visiblox.Charts;
using System.Windows.Controls.Primitives;
using Demolyzer.Model;

namespace Demolyzer.View
{
    /// <summary>
    /// Interaction logic for MapCanvasView.xaml
    /// </summary>
    public partial class MapCanvasView : UserControl
    {
        //private List<ToggleButton> _toggleButtons;
        private LinearGradientBrush _chartBrush;

        public MapCanvasView()
        {
            InitializeComponent();

            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(new GradientStop(Colors.DarkBlue, 0));
            gsc.Add(new GradientStop(Color.FromArgb(255,0,0,210), 0.5d));
            gsc.Add(new GradientStop(Colors.DarkBlue, 1));
            this._chartBrush = new LinearGradientBrush(gsc, 0);
            this._chartBrush.Freeze();

            this.SizeChanged += MapCanvasView_SizeChanged;
            this.CheckBoxShowCharts.Click += CheckBoxShowCharts_Click;

            //ColumnSeries series = new ColumnSeries();
            //series.SelectionMode = Visiblox.Charts.SelectionMode.None;
            //series.PointFill = Brushes.Blue;
            //series.BevelDepth = 10;
            //this.MainChart.Series.Add(series);

            this.ButtonDamageEfficiency.Click += (o, e) => ShowDamageEfficiencyChart();
            this.ButtonDamageGiven.Click += (o, e) => ShowDamageGivenChart();
            this.ButtonDamageTaken.Click += (o, e) => ShowDamageTakenChart();
            this.ButtonDamageTeam.Click += (o, e) => ShowDamageTeamChart();
            this.ButtonArmorTaken.Click += (o, e) => ShowArmorTakenChart();
            this.ButtonAvgTimeToGetRL.Click += (o, e) => ShowAvgTimeToGetRLChart();
            this.ButtonEfficiency.Click += (o, e) => ShowEfficiencyChart();
            this.ButtonHealthTaken.Click += (o, e) => ShowHealthTakenChart();
            this.ButtonKillsPerRL.Click += (o, e) => ShowKillsPerRLChart();
            this.ButtonKillsWithRL.Click += (o, e) => ShowKillsWithRLChart();
            this.ButtonQKills.Click += (o, e) => ShowQKillsChart();
            this.ButtonQuads.Click += (o, e) => ShowQuadsChart();
            this.ButtonRAs.Click += (o, e) => ShowRAsChart();
            this.ButtonRating.Click += (o, e) => ShowRatingChart();
            this.ButtonRLKills.Click += (o, e) => ShowRLKillsChart();
            this.ButtonRLs.Click += (o, e) => ShowRLsChart();
            this.ButtonRLsLost.Click += (o, e) => ShowRLsLostChart();
            this.ButtonScore.Click += (o, e) => ShowScoreChart();
            this.ButtonSpawnFrags.Click += (o, e) => ShowSpawnFragsChart();
            this.ButtonTimePerRL.Click += (o, e) => ShowTimePerRLChart();
            this.ButtonTimeWithRL.Click += (o, e) => ShowTimeWithRLChart();

            this.Loaded += new RoutedEventHandler(MapCanvasView_Loaded);
        }

        void MapCanvasView_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ContentLoaded += new EventHandler(ViewModel_ContentLoaded);
        }

        void ViewModel_ContentLoaded(object sender, EventArgs e)
        {
            this.ButtonScore.IsChecked = true;
            ShowScoreChart();
        }

        private IEnumerable<ParticipantInfo> Players
        {
            get
            {
                return this.ViewModel.FinalPlayerStats.Where(p => p.IsSpectator == false);
            }
        }

        private void ShowDamageEfficiencyChart()
        {
            LoadChartData("Damage Efficiency", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.DamageEfficiency)).ToArray());
        }

        private void ShowDamageGivenChart()
        {
            LoadChartData("Damage Given", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.DamageGiven)).ToArray());
        }

        private void ShowDamageTakenChart()
        {
            LoadChartData("Damage Taken", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.DamageTotal)).ToArray());
        }

        private void ShowDamageTeamChart()
        {
            LoadChartData("Damage Team", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.DamageTeam)).ToArray());
        }

        private void ShowAvgTimeToGetRLChart()
        {
            LoadChartData("Average time to get RL (minutes)", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.AverageTimeToGetRL / 60d)).ToArray());
        }

        private void ShowEfficiencyChart()
        {
            LoadChartData("Efficiency (%)", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.EfficiencyPercent)).ToArray());
        }

        private void ShowHealthTakenChart()
        {
            LoadChartData("Health Taken", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.HealthAcquired)).ToArray());
        }

        private void ShowKillsPerRLChart()
        {
            LoadChartData("Kills per RL", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.AverageKillsPerRLTaken)).ToArray());
        }

        private void ShowKillsWithRLChart()
        {
            LoadChartData("Kills w/ RL", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.KillsWithRL)).ToArray());
        }

        private void ShowQKillsChart()
        {
            LoadChartData("Quad Kills", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.KilledQuad)).ToArray());
        }

        private void ShowQuadsChart()
        {
            LoadChartData("Quads Taken", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.ItemCountQuad)).ToArray());
        }

        private void ShowRAsChart()
        {
            LoadChartData("RAs Taken", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.ItemCountRA)).ToArray());
        }

        private void ShowRatingChart()
        {
            LoadChartData("Rating", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.Rating)).ToArray());
        }

        private void ShowRLKillsChart()
        {
            LoadChartData("RL Kills", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.KilledRL)).ToArray());
        }

        private void ShowRLsChart()
        {
            LoadChartData("RLs Taken", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.ItemCountRL)).ToArray());
        }

        private void ShowRLsLostChart()
        {
            LoadChartData("RLs given to enemy (from dropped pack)", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.ItemCountRLLost)).ToArray());
        }

        private void ShowScoreChart()
        {
            LoadChartData("Score", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.CurrentScore)).ToArray());
        }

        private void ShowSpawnFragsChart()
        {
            LoadChartData("Spawn Frags", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.KillsSpawn)).ToArray());
        }

        private void ShowTimePerRLChart()
        {
            LoadChartData("Average time per RL (minutes)", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.AverageTimeWithRL / 60d)).ToArray());
        }

        private void ShowTimeWithRLChart()
        {
            LoadChartData("Total time with RL (minutes)", this.Players.Select(x => new PlayerStats(x.Name, x.TeamIndex, x.TotalTimeWithRL / 60d)).ToArray());
        }

        private void ShowArmorTakenChart()
        {
            LoadChartData("Armor Taken", this.ViewModel.FinalPlayerStats.Where(p => p.IsSpectator == false).Select(x => new PlayerStats(x.Name, x.TeamIndex, x.ArmorAcquired)).ToArray());
        }

        private void LoadChartData(string title, PlayerStats[] playerStats)
        {
            //var teamA = playerStats.Where(x => x.TeamIndex == 0).ToArray();
            //var teamB = playerStats.Where(x => x.TeamIndex == 1).ToArray();
            Array.Sort(playerStats, (a, b) => a.TeamIndex.CompareTo(b.TeamIndex));

            this.MainChart.Series.Clear();
            this.MainChart.Title = title;

            //TEAM A
            ColumnSeries series = new ColumnSeries();
            series.SelectionMode = Visiblox.Charts.SelectionMode.None;
            series.PointFill = this._chartBrush;
            series.BevelDepth = 10;
            series.BarWidthFraction = 0.5;
            DataSeries<string, double> dataSeries = new DataSeries<string, double>(false);
            foreach (var playerStat in playerStats)
            {
                dataSeries.Add(playerStat.Name, playerStat.Value);
            }
            series.DataSeries = dataSeries;
            this.MainChart.Series.Add(series);
        }

        void ButtonArmorTaken_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void CheckBoxShowCharts_Click(object sender, RoutedEventArgs e)
        {
            if (this.CheckBoxShowCharts.IsChecked.Value == true)
            {
                this.ButtonScore.IsChecked = true;
                ShowScoreChart();
            }
        }

        void MapCanvasView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ViewModel.UpdateCanvasSize(e.NewSize);
        }

        private MapCanvasViewModel ViewModel
        {
            get
            {
                return (MapCanvasViewModel)this.DataContext;
            }
        }

        public class PlayerStats
        {
            public string Name { get; set; }
            public int TeamIndex { get; set; }
            public double Value { get; set; }
            public PlayerStats(string name, int teamIndex, double value)
            {
                this.Name = name;
                this.TeamIndex = teamIndex;
                this.Value = value;
            }
        }
    }
}