using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumBitDesigns.Mvvm;
using Demolyzer.Model;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Threading.Tasks;
using Demolyzer.Model.Ratings;

namespace Demolyzer.ViewModel
{
    public class ApplicationShellViewModel : ViewModelBase
    {
        private MvdDirectoryTreeNodeViewModel _currentTreeRoot;
        private MvdReader _currentReader;
        private MatchResultsViewModel _matchResults;
        //private PlayerResultsViewModel _playerResults;
        private ProgressPanelViewModel _progressPanel;
        //private RatingsAndStatsViewModel _ratingsAndStats;
        private RatingsHostViewModel _ratingsHost;
        private string _currentQuakePath;
        private bool _sortByDate;

        public DelegateCommand<MvdDirectoryTreeNodeViewModel> ProcessDirectoryCommand { get; set; }
        public DelegateCommand<MvdDirectoryTreeNodeViewModel> ProcessDirectoriesCommand { get; set; }
        public DelegateCommand<MvdDirectoryTreeNodeViewModel> ProcessDirectoryAsTeamsCommand { get; set; }
        public DelegateCommand RefreshTreeCommand { get; set; }

        public ApplicationShellViewModel()
        {
            this._matchResults = new MatchResultsViewModel();
            this._progressPanel = new ProgressPanelViewModel();
            this._sortByDate = true;

            this.ProcessDirectoryCommand = new DelegateCommand<MvdDirectoryTreeNodeViewModel>(ProcessDirectory);
            this.ProcessDirectoriesCommand = new DelegateCommand<MvdDirectoryTreeNodeViewModel>(ProcessDirectories);
            this.ProcessDirectoryAsTeamsCommand = new DelegateCommand<MvdDirectoryTreeNodeViewModel>(ProcessDirectoryAsTeams);
            this.RefreshTreeCommand = new DelegateCommand(RefreshTree);
        }

        private bool _isProcessingDirectory;

        private void ProcessDirectoryAsTeams(MvdDirectoryTreeNodeViewModel directory)
        {
            Process(directory, true, false);
        }
        private void ProcessDirectory(MvdDirectoryTreeNodeViewModel directory)
        {
            Process(directory, false, false);
        }
        private void ProcessDirectories(MvdDirectoryTreeNodeViewModel directory)
        {
            Process(directory, false, true);
        }

        private void Process(MvdDirectoryTreeNodeViewModel directory, bool isAsTeams, bool includeSubdirectories)
        {
            iELOUtil.IsProcessAsTeamsEnabled = isAsTeams;

            List<MvdFileTreeNodeViewModel> files = directory.GetFiles(includeSubdirectories);

            this._progressPanel.ProgressMax = files.Count;
            this._progressPanel.Progress = 0;
            this._progressPanel.Description = String.Empty;
            this._isProcessingDirectory = true;
            OnPropertyChanged(() => MainContent);

            DelegateMarshaler marshaler = DelegateMarshaler.Create();

            Task.Factory.StartNew(() =>
            {
                //List<MatchResult> results = new List<MatchResult>(files.Count);
                Dictionary<string, List<MatchResult>> matchesByType = new Dictionary<string, List<MatchResult>>();

                List<string> failedMvdReads = new List<string>();
                for (int i = 0; i < files.Count; ++i)
                {
                    MvdReader analyzer = new MvdReader();
                    this._progressPanel.Progress = i;
                    this._progressPanel.Description = files[i].Name;
                    DemoContent demoContent = analyzer.Read(files[i].FullName, true);
                    if (demoContent.IsError == false)
                    {
                        MatchResult matchResult = demoContent.GetMatchResult();

                        if (matchesByType.ContainsKey(matchResult.MatchType) == false)
                        {
                            matchesByType[matchResult.MatchType] = new List<MatchResult>();
                        }
                        matchesByType[matchResult.MatchType].Add(matchResult);
                    }
                    else
                    {
                        failedMvdReads.Add(demoContent.MvdName);
                    }
                }
                if (failedMvdReads.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Unable to parse the following MVDs:");
                    sb.AppendLine("");
                    foreach (string str in failedMvdReads)
                    {
                        sb.AppendLine(str);
                    }
                    MessageBox.Show(sb.ToString());
                }

                List<RatingsAndStatsViewModel> ratings = new List<RatingsAndStatsViewModel>();
                foreach (var kvp in matchesByType)
                {
                    var ratingsAndStats = new RatingsAndStatsViewModel(kvp.Key, kvp.Value, includeSubdirectories);
                    ratingsAndStats.Initialize();
                    ratingsAndStats.RequestClose += (o, e) =>
                    {
                        if (ratingsAndStats.IsLoadSelectedMatchPending == true)
                        {
                            AnalyzeMvd(ratingsAndStats.SelectedParticipant.SelectedMatch.MatchResult.MvdFullName);
                        }
                        this._ratingsHost = null;
                        OnPropertyChanged(() => MainContent);
                    };

                    ratings.Add(ratingsAndStats);
                }

                this._ratingsHost = new RatingsHostViewModel(ratings);

                //post results back to UI thread
                marshaler.BeginInvoke(() =>
                {
                    this._isProcessingDirectory = false;
                    OnPropertyChanged(() => MainContent);
                });
            });

        }

        public void SortByDate()
        {
            this._sortByDate = true;
            ChangeDirectory(this._currentQuakePath);
        }

        public void SortByName()
        {
            this._sortByDate = false;
            ChangeDirectory(this._currentQuakePath);
        }

        public ViewModelBase MainContent
        {
            get
            {
                if (this._isProcessingDirectory == true)
                {
                    return this._progressPanel;
                }
                if (this._ratingsHost != null)
                {
                    return this._ratingsHost;
                }
                return this._matchResults;
            }
        }

        public void Initialize()
        {
            string quakepath = null;
            if (File.Exists("quakepath.ini") == true)
            {
                string[] lines = File.ReadAllLines("quakepath.ini");
                if (lines != null && lines.Length > 0)
                {
                    quakepath = lines[0];
                    if (Directory.Exists(quakepath) == false)
                    {
                        quakepath = null;
                    }
                }
            }
            if (quakepath == null)
            {
                quakepath = BrowseForQuakePath();
            }
            ChangeDirectory(quakepath);
        }

        public void ChangeDirectory()
        {
            ChangeDirectory(BrowseForQuakePath());
        }

        private void ChangeDirectory(string quakepath)
        {
            if (Directory.Exists(quakepath) == false)
            {
                return;
            }
            this._currentQuakePath = quakepath;
            DirectoryInfo qwDirectoryInfo = new DirectoryInfo(quakepath);
            FileInfo[] mvds = qwDirectoryInfo.GetFiles("*.mvd", SearchOption.AllDirectories);
            //mvds = mvds.Where(mvd => mvd.Name.Contains("pov") == false && mvd.Name.Contains("end") == false).ToArray();
            LoadTodaysDemos(mvds);
            List<MvdListViewNodeViewModel> listViewItems = mvds.Select(fi => new MvdListViewNodeViewModel(fi.Name, fi.FullName, fi.CreationTime)).ToList();
            listViewItems = listViewItems.OrderByDescending(x => x.Date).ToList();
            this._demoList = listViewItems;
            this._currentTreeRoot = new MvdDirectoryTreeNodeViewModel(qwDirectoryInfo, this._sortByDate);
            this._demoTree = this._currentTreeRoot.Children;

            OnPropertyChanged(() => DemoTree);
            OnPropertyChanged(() => DemoList);
        }

        private void RefreshTree()
        {
            this.DemoTree.Clear();
            ChangeDirectory(this._currentQuakePath);
        }

        private string BrowseForQuakePath()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = false;
            folderBrowser.Description = "Browse for Quake (or demos) folder";
            if (folderBrowser.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return null;
            }
            string quakepath = folderBrowser.SelectedPath;
            File.WriteAllLines("quakepath.ini", new string[] { quakepath });
            return quakepath;
        }

        //public void RefreshToday()
        //{
        //    if (Directory.Exists(this._currentQuakePath) == false)
        //    {
        //        return;
        //    }
        //    DirectoryInfo qwDirectoryInfo = new DirectoryInfo(this._currentQuakePath);
        //    FileInfo[] mvds = qwDirectoryInfo.GetFiles("*.mvd", SearchOption.AllDirectories);
        //    LoadTodaysDemos(mvds);
        //}

        private void LoadTodaysDemos(FileInfo[] mvds)
        {
            //for (int i = 111; i < mvds.Length; ++i)
            //{
            //    MvdAnalyzer analyzer = new MvdAnalyzer();
            //    var content = analyzer.Read(mvds[i].FullName);
            //    if (content.IsError == true)
            //    {
            //        int asd = 0;
            //    }
            //}

            DateTime now = DateTime.Now;
            var mvdlist = mvds.Where(fi => fi.CreationTime.Year == now.Year && fi.CreationTime.DayOfYear == now.DayOfYear);
            List<MvdListViewNodeViewModel> listViewItems = mvdlist.Select(fi => new MvdListViewNodeViewModel(fi.Name, fi.FullName, fi.CreationTime)).ToList();
            this._demoListToday = listViewItems.OrderByDescending(x => x.Date).ToList();
            OnPropertyChanged(() => DemoListToday);
        }

        private List<MvdListViewNodeViewModel> _demoListToday;
        private List<MvdListViewNodeViewModel> _demoList;
        private ObservableCollection<TreeNodeViewModel> _demoTree;

        public List<MvdListViewNodeViewModel> DemoListToday
        {
            get
            {
                return this._demoListToday;
            }
        }

        public ObservableCollection<TreeNodeViewModel> DemoTree
        {
            get
            {
                return this._demoTree;
            }
        }

        public List<MvdListViewNodeViewModel> DemoList
        {
            get
            {
                return this._demoList;
            }
        }

        public void AnalyzeMvd(string demoFileName)
        {
            DelegateMarshaler marshaler = DelegateMarshaler.Create();

            if (this._currentReader != null)
            {
                this._currentReader.Cancel();
            }
            MvdReader analyzer = new MvdReader();
            this._currentReader = analyzer;
            Task.Factory.StartNew(() =>
            {
                //Stopwatch sw = Stopwatch.StartNew();
                DemoContent demoContent = analyzer.Read(demoFileName, true);
                //System.Windows.MessageBox.Show(sw.ElapsedMilliseconds.ToString());

                if (analyzer.IsCanceled == false)
                {
                    //post results back to UI thread
                    marshaler.BeginInvoke(() =>
                    {
                        this._matchResults.LoadDemoContent(demoContent);

                        //clear reference to analyzer
                        if (analyzer == this._currentReader)
                        {
                            this._currentReader = null;
                        }
                    });
                }
            });
        }
    }
}
