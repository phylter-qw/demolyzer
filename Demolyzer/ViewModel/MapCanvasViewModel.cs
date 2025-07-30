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

namespace Demolyzer.ViewModel
{
    public class MapCanvasViewModel : INotifyPropertyChanged
    {
        public event EventHandler ContentLoaded;

        private string _map;
        private SortableObservableCollection<PlayerViewModel> _players;
        private ObservableCollection<MatchKillViewModel> _matchKills;
        private ObservableCollection<EntityViewModel> _entities;
        private BitmapSource _mapImage;
        public ParticipantInfo[] FinalPlayerStats { get; private set; }

        public MapCanvasViewModel()
        {
            this._players = new SortableObservableCollection<PlayerViewModel>();
            this._matchKills = new ObservableCollection<MatchKillViewModel>();
            this._entities = new ObservableCollection<EntityViewModel>();
            this._isFitToDisplayEnabled = true;
            this._zoomMultiplier = 1d;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public bool IsStatsLoaded
        {
            get
            {
                return this.FinalPlayerStats != null;
            }
        }

        public void Load(string map, Player[] players, MatchKill[] matchKills, Entity[] entities, ParticipantInfo[] finalPlayerStats)
        {
            //create viewmodels for players
            this._players.Clear();
            for (int i = 0; i < players.Length; ++i)
            {
                if (players[i].CurrentStats.IsSpectator == false)
                {
                    this._players.Add(new PlayerViewModel(players[i]));
                }
            }

            this._matchKills.Clear();
            for (int i = 0; i < matchKills.Length; ++i)
            {
                this._matchKills.Add(new MatchKillViewModel(matchKills[i]));
            }

            this._entities.Clear();
            for (int i = 0; i < entities.Length; ++i)
            {
                entities[i].Reset();
                this._entities.Add(new EntityViewModel(entities[i]));
            }

            //now update the map which also updates player offsets
            this.Map = map;
            this.FinalPlayerStats = finalPlayerStats;
            OnPropertyChanged("IsStatsLoaded");
            OnContentLoaded(EventArgs.Empty);
        }

        protected virtual void OnContentLoaded(EventArgs e)
        {
            if (this.ContentLoaded != null)
            {
                this.ContentLoaded(this, e);
            }
        }


        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            for (int x = 0; x < this._players.Count; ++x)
            {
                this._players[x].UpdateUIBindings();
            }
            for (int x = 0; x < this._matchKills.Count; ++x)
            {
                this._matchKills[x].UpdateUIBindings();
            }
            for (int x = 0; x < this._entities.Count; ++x)
            {
                this._entities[x].UpdateUIBindings();
            }
        }

        private void SetPlayerPositionAdjustment(double adjustmentX, double adjustmentY)
        {
            this._entityAdjustment = new Vector(adjustmentX, adjustmentY);

            for (int x = 0; x < this._players.Count; ++x)
            {
                this._players[x].AdjustmentX = adjustmentX;
                this._players[x].AdjustmentY = adjustmentY;
            }
            for (int x = 0; x < this._matchKills.Count; ++x)
            {
                this._matchKills[x].AdjustmentX = adjustmentX;
                this._matchKills[x].AdjustmentY = adjustmentY;
            }
            for (int x = 0; x < this._entities.Count; ++x)
            {
                this._entities[x].AdjustmentX = adjustmentX;
                this._entities[x].AdjustmentY = adjustmentY;
            }
        }

        public void AddEntity(EntityViewModel entity)
        {
            entity.AdjustmentX = this._entityAdjustment.X;
            entity.AdjustmentY = this._entityAdjustment.Y;
            this._entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            for (int i = 0; i < this._entities.Count; ++i)
            {
                if (this._entities[i].Entity == entity)
                {
                    this._entities.RemoveAt(i);
                    return;
                }
            }
        }

        public ObservableCollection<PlayerViewModel> Players
        {
            get
            {
                return this._players;
            }
        }

        public ObservableCollection<MatchKillViewModel> MatchKills
        {
            get
            {
                return this._matchKills;
            }
        }

        public ObservableCollection<EntityViewModel> Entities
        {
            get
            {
                return this._entities;
            }
        }

        private Vector _entityAdjustment;

        public string Map
        {
            get
            {
                return this._map;
            }
            private set
            {
                this._map = value;
                this.MapImage = GetMapImage(value);

                switch (this.Map)
                {
                    case "a2":
                        SetPlayerPositionAdjustment(125d, 420d);
                        break;
                    case "aerowalk":
                        SetPlayerPositionAdjustment(475d, 340d);
                        break;
                    case "dm2":
                        SetPlayerPositionAdjustment(-550d, 180d);
                        break;
                    case "dm3":
                        SetPlayerPositionAdjustment(500d, 570d);
                        break;
                    case "dm4":
                        SetPlayerPositionAdjustment(140d, 320d);
                        break;
                    case "dm6":
                        SetPlayerPositionAdjustment(15d, -50d);
                        break;
                    case "ztndm3":
                        SetPlayerPositionAdjustment(550d, 470d);
                        break;
                    case "e1m2":
                        SetPlayerPositionAdjustment(395d, 868d);
                        break;
                    case "cmt3":
                        SetPlayerPositionAdjustment(555d, 1040d);
                        break;
                    case "cmt4":
                        SetPlayerPositionAdjustment(596d, 172d);
                        break;
                    case "schloss":
                        SetPlayerPositionAdjustment(244d, -813d);
                        break;
                    default:
                        break;
                }
                OnPropertyChanged("Map");
            }
        }

        public double MapWidth
        {
            get
            {
                if (this._mapImage == null)
                {
                    return 0;
                }
                return this._mapImage.PixelWidth;
            }
        }

        public double MapHeight
        {
            get
            {
                if (this._mapImage == null)
                {
                    return 0;
                }
                return this._mapImage.PixelHeight;
            }
        }

        public BitmapSource MapImage
        {
            get
            {
                return this._mapImage;
            }
            private set
            {
                this._mapImage = value;
                OnPropertyChanged("MapImage");
                OnPropertyChanged("MapWidth");
                OnPropertyChanged("MapHeight");
                PerformZoom();
            }
        }

        private BitmapImage GetMapImage(string map)
        {
            try
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                switch (map)
                {
                    case "a2":
                        img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Maps/a2.png"));
                        break;
                    case "aerowalk":
                        img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Maps/aerowalk.png"));
                        break;
                    case "dm2":
                        img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Maps/dm2.png"));
                        break;
                    case "dm3":
                        img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Maps/dm3.png"));
                        break;
                    case "dm4":
                        img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Maps/dm4.png"));
                        break;
                    case "dm6":
                        img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Maps/dm6.png"));
                        break;
                    case "e1m2":
                        img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Maps/e1m2.png"));
                        break;
                    case "ztndm3":
                        img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Maps/ztndm3.png"));
                        break;
                    case "cmt3":
                        img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Maps/cmt3.png"));
                        break;
                    case "cmt4":
                        img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Maps/cmt4.png"));
                        break;
                    case "schloss":
                        img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Maps/schloss.png"));
                        break;
                    default:
                        break;
                }

                img.EndInit();
                img.Freeze();
                return img;
            }
            catch
            {
                return null;
            }
        }

        private Size _displaySize;

        public void UpdateCanvasSize(Size size)
        {
            this._displaySize = size;
            PerformZoom();
        }

        private double _zoomScale;
        private double _zoomMultiplier;

        private bool _isFitToDisplayEnabled;
        public bool IsFitToDisplayEnabled
        {
            get
            {
                return this._isFitToDisplayEnabled;
            }
            set
            {
                this._isFitToDisplayEnabled = value;
                if (this._isFitToDisplayEnabled == true)
                {
                    this._zoomMultiplier = 1d;
                }
                PerformZoom();
            }
        }
        private void PerformZoom()
        {
            //do not allow any scaling if there is no display size information or source image size information
            if (this._displaySize.IsEmpty == true || this.MapWidth == 0 || this.MapHeight == 0)
            {
                this.ZoomScale = 1d;
                return;
            }

            double scaleX = this._displaySize.Width / this.MapWidth;
            double scaleY = this._displaySize.Height / this.MapHeight;

            this.ZoomScale = this._zoomMultiplier * Math.Min(scaleX, scaleY);
        }

        public double ZoomScale
        {
            get
            {
                return this._zoomScale;
            }
            private set
            {
                this._zoomScale = value;
                OnPropertyChanged("ZoomScale");
            }
        }

        public double ZoomMultiplier
        {
            get
            {
                return this._zoomMultiplier;
            }
            set
            {
                this._zoomMultiplier = value;
                OnPropertyChanged("ZoomMultiplier");
                PerformZoom();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
