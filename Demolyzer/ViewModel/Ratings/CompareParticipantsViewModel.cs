using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumBitDesigns.Mvvm;
using Demolyzer.Model.Ratings;

namespace Demolyzer.ViewModel.Ratings
{
    public class CompareParticipantsViewModel : ViewModelBase
    {
        private int _selectedTabIndex;
        private List<ParticipantResultsViewModel> _participantsA;
        private List<ParticipantResultsViewModel> _participantsB;
        private ParticipantInfo[] _teamA;
        private ParticipantInfo[] _teamB;
        private int _totalFragCount;
        private int _totalA;
        private int _totalB;

        public DelegateCommand EqualizeCommand { get; set; }

        public CompareParticipantsViewModel()
        {
            this._participantsA = new List<ParticipantResultsViewModel>();
            this._participantsB = new List<ParticipantResultsViewModel>();
            this.TotalFragCount = 50;

            this.EqualizeCommand = new DelegateCommand(Equalize);
        }

        void Singleton_ComparisonChanged(object sender, EventArgs e)
        {
            this._participantsA = ParticipantCompareService.Singleton.GetTeamA();
            this._participantsB = ParticipantCompareService.Singleton.GetTeamB();

            SetFragCount();
            Compute();
        }

        public void Load()
        {
            this._participantsA.Clear();
            this._participantsB.Clear();

            ParticipantCompareService.Singleton.Clear();
            ParticipantCompareService.Singleton.ComparisonChanged += new EventHandler(Singleton_ComparisonChanged);
        }

        public void Close()
        {
            this._participantsA.Clear();
            this._participantsB.Clear();

            ParticipantCompareService.Singleton.Clear();
            ParticipantCompareService.Singleton.ComparisonChanged -= new EventHandler(Singleton_ComparisonChanged);
        }

        private void Equalize()
        {
            if (this.IsEqualizeEnabled == false)
            {
                return;
            }

            try
            {
                List<ParticipantResultsViewModel> players = new List<ParticipantResultsViewModel>();
                players.AddRange(this._participantsA);
                players.AddRange(this._participantsB);

                int minDifference = Int32.MaxValue;

                var xxx = CombinationUtil.Combinations(players, players.Count / 2).ToList();

                foreach (var team in CombinationUtil.Combinations(players, players.Count / 2))
                {
                    List<ParticipantResultsViewModel> teamAList = team.ToList();
                    List<ParticipantResultsViewModel> teamBList = new List<ParticipantResultsViewModel>(players.Except(teamAList));

                    ParticipantInfo[] teamA = teamAList.Select(p => { return new ParticipantInfo { Name = p.Name }; }).ToArray();
                    ParticipantInfo[] teamB = teamBList.Select(p => { return new ParticipantInfo { Name = p.Name }; }).ToArray();

                    int totalFragCount = 1000;
                    int playerCount = teamA.Length;
                    for (int indexA = 0; indexA < playerCount; ++indexA)
                    {
                        double expectedScore = 0;
                        for (int indexB = 0; indexB < playerCount; ++indexB)
                        {
                            expectedScore += iELOUtil.GetExpectedScore(teamAList[indexA].ELOCombined - teamBList[indexB].ELOCombined);
                        }
                        teamA[indexA].ExpectedScore = (int)(totalFragCount * expectedScore / (4d * playerCount));
                    }
                    for (int indexB = 0; indexB < playerCount; ++indexB)
                    {
                        double expectedScore = 0;
                        for (int indexA = 0; indexA < playerCount; ++indexA)
                        {
                            expectedScore += iELOUtil.GetExpectedScore(teamBList[indexB].ELOCombined - teamAList[indexA].ELOCombined);
                        }
                        teamB[indexB].ExpectedScore = (int)(totalFragCount * expectedScore / (4d * playerCount));
                    }
                    int totalA = teamA.Sum(p => p.ExpectedScore);
                    int totalB = teamB.Sum(p => p.ExpectedScore);
                    int difference = Math.Abs(totalA - totalB);
                    if (difference < minDifference)
                    {
                        this._participantsA = teamAList;
                        this._participantsB = teamBList;
                        minDifference = difference;
                    }
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Unable to equalize.  Make sure a single player is not chosen twice");
            }

            //SetFragCount
            Compute();
        }

        public bool IsEqualizeEnabled
        {
            get
            {
                return ((this._participantsA.Count == 2 && this._participantsB.Count == 2) ||
                    (this._participantsA.Count == 4 && this._participantsB.Count == 4));
            }
        }

        //public bool IsVisible
        //{
        //    get
        //    {
        //        return (this._participantsA.Count + this._participantsB.Count) > 0;
        //    }
        //}

        //private bool _isVisible;

        //public bool IsVisible
        //{
        //    get
        //    {
        //        return this._isVisible;
        //    }
        //    set
        //    {
        //        this._isVisible = value;
        //        OnPropertyChanged("IsVisible");
        //    }
        //}

        public int TotalFragCount
        {
            get
            {
                return this._totalFragCount;
            }
            set
            {
                this._totalFragCount = value;
                Compute();
                OnPropertyChanged(() => TotalFragCount);
            }
        }

        public int TotalA
        {
            get
            {
                return this._totalA;
            }
            set
            {
                this._totalA = value;
                OnPropertyChanged(() => TotalA);
            }
        }

        public int TotalB
        {
            get
            {
                return this._totalB;
            }
            set
            {
                this._totalB = value;
                OnPropertyChanged(() => TotalB);
            }
        }

        private void SetFragCount()
        {
            if (this._participantsA.Count == 1 && this._participantsB.Count == 1)
            {
                this._totalFragCount = 50;
            }
            if (this._participantsA.Count == 2 && this._participantsB.Count == 2)
            {
                this._totalFragCount = 150;
            }
            if (this._participantsA.Count == 4 && this._participantsB.Count == 4)
            {
                this._totalFragCount = 350;
            }
        }

        //public void AddParticipantA(ParticipantResultsViewModel participant)
        //{
        //    if (this._participantsA.Count >= 4)
        //    {
        //        return;
        //    }
        //    this._participantsA.Add(participant);
        //    SetFragCount();
        //    Compute();
        //    OnPropertyChanged(() => TotalFragCount);
        //    //OnPropertyChanged(() => IsVisible);
        //    OnPropertyChanged(() => IsEqualizeEnabled);
        //}

        //public void AddParticipantB(ParticipantResultsViewModel participant)
        //{
        //    if (this._participantsB.Count >= 4)
        //    {
        //        return;
        //    }
        //    this._participantsB.Add(participant);
        //    SetFragCount();
        //    Compute();
        //    OnPropertyChanged(() => TotalFragCount);
        //    //OnPropertyChanged(() => IsVisible);
        //    OnPropertyChanged(() => IsEqualizeEnabled);
        //}

        //public void SetParticipants(List<ParticipantResultsViewModel> participantsTeamA, List<ParticipantResultsViewModel> participantsTeamB)
        //{
        //    this._participantsA = participantsTeamA;
        //    this._participantsB = participantsTeamB;
        //}

        private void Compute()
        {
            //SetFragCount();

            ParticipantInfo[] teamA = this._participantsA.Select(p => { return new ParticipantInfo { Name = p.Name }; }).ToArray();
            ParticipantInfo[] teamB = this._participantsB.Select(p => { return new ParticipantInfo { Name = p.Name }; }).ToArray();

            if (this.TotalFragCount > 0)
            {
                if (teamA.Length == 1 && teamB.Length == 1)
                {
                    teamA[0].ExpectedScore = (int)(this.TotalFragCount * iELOUtil.GetExpectedScore(this._participantsA[0].ELOCombined - this._participantsB[0].ELOCombined));
                    teamB[0].ExpectedScore = (int)(this.TotalFragCount * iELOUtil.GetExpectedScore(this._participantsB[0].ELOCombined - this._participantsA[0].ELOCombined));
                }
                else
                {
                    int playerCount = teamA.Length;
                    if (teamA.Length == playerCount && teamB.Length == playerCount && (playerCount == 2 || playerCount == 4))
                    {
                        for (int indexA = 0; indexA < playerCount; ++indexA)
                        {
                            double expectedScore = 0;
                            for (int indexB = 0; indexB < playerCount; ++indexB)
                            {
                                expectedScore += iELOUtil.GetExpectedScore(this._participantsA[indexA].ELOCombined - this._participantsB[indexB].ELOCombined);
                            }
                            teamA[indexA].ExpectedScore = (int)(this.TotalFragCount * expectedScore / (4d * playerCount));
                        }
                        for (int indexB = 0; indexB < playerCount; ++indexB)
                        {
                            double expectedScore = 0;
                            for (int indexA = 0; indexA < playerCount; ++indexA)
                            {
                                expectedScore += iELOUtil.GetExpectedScore(this._participantsB[indexB].ELOCombined - this._participantsA[indexA].ELOCombined);
                            }
                            teamB[indexB].ExpectedScore = (int)(this.TotalFragCount * expectedScore / (4d * playerCount));
                        }
                    }
                }
            }

            this.TotalA = teamA.Sum(p => p.ExpectedScore);
            this.TotalB = teamB.Sum(p => p.ExpectedScore);
            this.TeamA = teamA;
            this.TeamB = teamB;

            //SetFragCount();
            OnPropertyChanged(() => TotalFragCount);
            OnPropertyChanged(() => IsEqualizeEnabled);
        }

        public ParticipantInfo[] TeamA
        {
            get
            {
                return this._teamA;
            }
            set
            {
                this._teamA = value;
                OnPropertyChanged(() => TeamA);
            }
        }

        public ParticipantInfo[] TeamB
        {
            get
            {
                return this._teamB;
            }
            set
            {
                this._teamB = value;
                OnPropertyChanged(() => TeamB);
            }
        }

        public int SelectedTabIndex
        {
            get
            {
                return this._selectedTabIndex;
            }
            set
            {
                this._selectedTabIndex = value;
                OnPropertyChanged(() => SelectedTabIndex);
            }
        }
    }

    public class ParticipantInfo
    {
        public string Name { get; set; }
        public int ExpectedScore { get; set; }

        public ParticipantInfo()
        {
        }
    }

    public static class CombinationUtil
    {
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
    }

    public class ParticipantCompareService
    {
        public event EventHandler ComparisonChanged;

        public static ParticipantCompareService Singleton = new ParticipantCompareService();

        private HashSet<ParticipantResultsViewModel> _teamA;
        private HashSet<ParticipantResultsViewModel> _teamB;

        public ParticipantCompareService()
        {
            this._teamA = new HashSet<ParticipantResultsViewModel>();
            this._teamB = new HashSet<ParticipantResultsViewModel>();
        }

        public void Clear()
        {
            this._teamA = new HashSet<ParticipantResultsViewModel>();
            this._teamB = new HashSet<ParticipantResultsViewModel>();
        }

        public List<ParticipantResultsViewModel> GetTeamA()
        {
            return this._teamA.ToList();
        }

        public List<ParticipantResultsViewModel> GetTeamB()
        {
            return this._teamB.ToList();
        }

        public void AddToTeamA(ParticipantResultsViewModel participant)
        {
            if (this._teamA.Count < 4)
            {
                this._teamA.Add(participant);
            }
            OnComparisonChanged(EventArgs.Empty);
        }

        public void AddToTeamB(ParticipantResultsViewModel participant)
        {
            if (this._teamB.Count < 4)
            {
                this._teamB.Add(participant);
            }
            OnComparisonChanged(EventArgs.Empty);
        }

        public void RemoveFromTeamA(ParticipantResultsViewModel participant)
        {
            this._teamA.Remove(participant);
            OnComparisonChanged(EventArgs.Empty);
        }

        public void RemoveFromTeamB(ParticipantResultsViewModel participant)
        {
            this._teamB.Remove(participant);
            OnComparisonChanged(EventArgs.Empty);
        }

        //public bool IsWatched(string participant)
        //{
        //    return this._teamA.Contains(participant);
        //}

        protected virtual void OnComparisonChanged(EventArgs e)
        {
            if (this.ComparisonChanged != null)
            {
                this.ComparisonChanged(this, e);
            }
        }
    }
}
