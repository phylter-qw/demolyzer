using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Demolyzer.Model;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Demolyzer.ViewModel
{
    public class PlayerViewModel : DependencyObject
    {
        private const double DiameterScale = 0.15d;
        private const double MapImageScale = 0.5d;
        public const double MaxPlayerDiameter = DiameterScale * 1200;
        public const double PlayerDiameter = DiameterScale * 650;
        public const double LightninggunDiameter = DiameterScale * 550; //200 armor and 250 health
        public const double RocketLauncherDiameter = DiameterScale * 490; 
        public const double WeaponBorderThickness = DiameterScale * 30;

        private Player _player;

        //public bool IsVisible { get { return (bool)GetValue(IsVisibleProperty); } set { SetValue(IsVisibleProperty, value); } }
        public bool HasLightningGun { get { return (bool)GetValue(HasLightningGunProperty); } set { SetValue(HasLightningGunProperty, value); } }
        public bool HasRocketLauncher { get { return (bool)GetValue(HasRocketLauncherProperty); } set { SetValue(HasRocketLauncherProperty, value); } }
        public bool IsPowerupQuad { get { return (bool)GetValue(IsPowerupQuadProperty); } set { SetValue(IsPowerupQuadProperty, value); } }
        public bool IsPowerupPent { get { return (bool)GetValue(IsPowerupPentProperty); } set { SetValue(IsPowerupPentProperty, value); } }
        public bool IsPowerupEyes { get { return (bool)GetValue(IsPowerupEyesProperty); } set { SetValue(IsPowerupEyesProperty, value); } }
        public double OffsetX { get { return (double)GetValue(OffsetXProperty); } set { SetValue(OffsetXProperty, value); } }
        public double OffsetY { get { return (double)GetValue(OffsetYProperty); } set { SetValue(OffsetYProperty, value); } }
        public double HealthDiameter { get { return (double)GetValue(HealthDiameterProperty); } set { SetValue(HealthDiameterProperty, value); } }
        public double ArmorDiameter { get { return (double)GetValue(ArmorDiameterProperty); } set { SetValue(ArmorDiameterProperty, value); } }
        public Brush ArmorBrush { get { return (Brush)GetValue(ArmorBrushProperty); } set { SetValue(ArmorBrushProperty, value); } }


        //public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register("IsVisible", typeof(bool), typeof(PlayerViewModel), new UIPropertyMetadata(false));
        public static readonly DependencyProperty HasLightningGunProperty = DependencyProperty.Register("HasLightningGun", typeof(bool), typeof(PlayerViewModel), new UIPropertyMetadata(false));
        public static readonly DependencyProperty HasRocketLauncherProperty = DependencyProperty.Register("HasRocketLauncher", typeof(bool), typeof(PlayerViewModel), new UIPropertyMetadata(false));
        public static readonly DependencyProperty IsPowerupQuadProperty = DependencyProperty.Register("IsPowerupQuad", typeof(bool), typeof(PlayerViewModel), new UIPropertyMetadata(false));
        public static readonly DependencyProperty IsPowerupPentProperty = DependencyProperty.Register("IsPowerupPent", typeof(bool), typeof(PlayerViewModel), new UIPropertyMetadata(false));
        public static readonly DependencyProperty IsPowerupEyesProperty = DependencyProperty.Register("IsPowerupEyes", typeof(bool), typeof(PlayerViewModel), new UIPropertyMetadata(false));
        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register("OffsetX", typeof(double), typeof(PlayerViewModel), new UIPropertyMetadata(0d));
        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register("OffsetY", typeof(double), typeof(PlayerViewModel), new UIPropertyMetadata(0d));
        public static readonly DependencyProperty HealthDiameterProperty = DependencyProperty.Register("HealthDiameter", typeof(double), typeof(PlayerViewModel), new UIPropertyMetadata(0d));
        public static readonly DependencyProperty ArmorDiameterProperty = DependencyProperty.Register("ArmorDiameter", typeof(double), typeof(PlayerViewModel), new UIPropertyMetadata(0d));
        public static readonly DependencyProperty ArmorBrushProperty = DependencyProperty.Register("ArmorBrush", typeof(Brush), typeof(PlayerViewModel), new UIPropertyMetadata(Brushes.Red));


        public PlayerViewModel(Player player)
        {
            this._player = player;
        }

        internal void UpdateUIBindings()
        {
            if (this._player.IsAlive == false)
            {
                return;
            }

            bool hasLightningGun = (this._player.CurrentWeapons & Weapon.IT_LIGHTNING) == Weapon.IT_LIGHTNING;
            if (this.HasLightningGun != hasLightningGun)
            {
                this.HasLightningGun = hasLightningGun;
            }
            bool hasRocketLauncher = (this._player.CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER;
            if (this.HasRocketLauncher != hasRocketLauncher)
            {
                this.HasRocketLauncher = hasRocketLauncher;
            }
            bool isPowerupQuad = (this._player.Powerup & Powerup.IT_QUAD) == Powerup.IT_QUAD;
            if (this.IsPowerupQuad != isPowerupQuad)
            {
                this.IsPowerupQuad = isPowerupQuad;
            }
            bool isPowerupPent = (this._player.Powerup & Powerup.IT_INVULNERABILITY) == Powerup.IT_INVULNERABILITY;
            if (this.IsPowerupPent != isPowerupPent)
            {
                this.IsPowerupPent = isPowerupPent;
            }
            bool isPowerupEyes = (this._player.Powerup & Powerup.IT_INVISIBILITY) == Powerup.IT_INVISIBILITY;
            if (this.IsPowerupEyes != isPowerupEyes)
            {
                this.IsPowerupEyes = isPowerupEyes;
            }
            double offsetX = MapImageScale * this._player.OffsetX + AdjustmentX - 0.5d * MaxPlayerDiameter;
            if (this.OffsetX != offsetX)
            {
                this.OffsetX = offsetX;
            }
            double offsetY = MapImageScale * this._player.OffsetY + AdjustmentY - 0.5d * MaxPlayerDiameter;
            if (this.OffsetY != offsetY)
            {
                this.OffsetY = offsetY;
            }
            double healthDiameter = this._player.HealthValue * DiameterScale;
            if (this.HealthDiameter != healthDiameter)
            {
                this.HealthDiameter = healthDiameter;
            }
            double armorDiameter = (this._player.HealthValue + this._player.ArmorValue) * DiameterScale;
            if (this.ArmorDiameter != armorDiameter)
            {
                this.ArmorDiameter = armorDiameter;
            }
            Brush armorBrush = Brushes.Transparent;
            switch (this._player.CurrentArmor)
            {
                case Armor.IT_ARMOR1:
                    armorBrush = Brushes.Green;
                    break;
                case Armor.IT_ARMOR2:
                    armorBrush = Brushes.Yellow;
                    break;
                case Armor.IT_ARMOR3:
                    armorBrush = Brushes.Red;
                    break;
                default:
                    armorBrush = Brushes.Transparent;
                    break;
            }
            if (this.ArmorBrush != armorBrush)
            {
                this.ArmorBrush = armorBrush;
            }

            //OnPropertyChanged("OffsetX");
            //OnPropertyChanged("OffsetY");
            //OnPropertyChanged("HealthDiameter");
            //OnPropertyChanged("ArmorDiameter");
            //OnPropertyChanged("ArmorBrush");
            //OnPropertyChanged("ActiveWeaponBrush");
            //OnPropertyChanged("IsVisible");
            //OnPropertyChanged("TeamColor");
            //OnPropertyChanged("Name");
            //OnPropertyChanged("IsPowerupQuad");
            //OnPropertyChanged("IsPowerupPent");
            //OnPropertyChanged("IsPowerupEyes");
            //OnPropertyChanged("HasLightningGun");
            //OnPropertyChanged("HasRocketLauncher");
        }
        ////DP
        //public bool IsVisible
        //{
        //    get
        //    {
        //        return this._player.IsAlive;
        //    }
        //}

        ////DP
        //public bool HasLightningGun
        //{
        //    get
        //    {
        //        return (this._player.CurrentWeapons & Weapon.IT_LIGHTNING) == Weapon.IT_LIGHTNING;
        //    }
        //}

        ////DP
        //public bool HasRocketLauncher
        //{
        //    get
        //    {
        //        return (this._player.CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER;
        //    }
        //}

        ////DP
        //public bool IsPowerupQuad
        //{
        //    get
        //    {
        //        return (this._player.Powerup & Powerup.IT_QUAD) == Powerup.IT_QUAD;
        //    }
        //}

        ////DP
        //public bool IsPowerupPent
        //{
        //    get
        //    {
        //        return (this._player.Powerup & Powerup.IT_INVULNERABILITY) == Powerup.IT_INVULNERABILITY;
        //    }
        //}

        ////DP
        //public bool IsPowerupEyes
        //{
        //    get
        //    {
        //        return (this._player.Powerup & Powerup.IT_INVISIBILITY) == Powerup.IT_INVISIBILITY;
        //    }
        //}

        ////DP
        //public double OffsetX
        //{
        //    get
        //    {
        //        return MapImageScale * this._player.OffsetX + AdjustmentX - 0.5d * MaxPlayerDiameter;
        //    }
        //}

        ////DP
        //public double OffsetY
        //{
        //    get
        //    {
        //        return MapImageScale * this._player.OffsetY + AdjustmentY - 0.5d * MaxPlayerDiameter;
        //    }
        //}

        public double AdjustmentX { get; set; }
        public double AdjustmentY { get; set; }

        ////DP
        //public double HealthDiameter
        //{
        //    get
        //    {
        //        return this._player.HealthValue * DiameterScale;
        //    }
        //}

        ////DP
        //public double ArmorDiameter
        //{
        //    get
        //    {
        //        return (this._player.HealthValue + this._player.ArmorValue) * DiameterScale;
        //    }
        //}

        ////DP
        //public Brush ArmorBrush
        //{
        //    get
        //    {
        //        switch (this._player.CurrentArmor)
        //        {
        //            case Armor.IT_ARMOR1:
        //                return Brushes.Green;
        //            case Armor.IT_ARMOR2:
        //                return Brushes.Yellow;
        //            case Armor.IT_ARMOR3:
        //                return Brushes.Red;
        //            default:
        //                return Brushes.Transparent;
        //        }
        //    }
        //}

        //public Brush ActiveWeaponBrush
        //{
        //    get
        //    {
        //        switch (this._player.ActiveWeapon)
        //        {
        //            case Weapon.IT_SHOTGUN:
        //                return Brushes.Purple;
        //            case Weapon.IT_NAILGUN:
        //                return Brushes.LightSkyBlue;
        //            case Weapon.IT_SUPER_NAILGUN:
        //                return Brushes.Blue;
        //            case Weapon.IT_GRENADE_LAUNCHER:
        //                return Brushes.Gold;
        //            case Weapon.IT_ROCKET_LAUNCHER:
        //                return Brushes.Sienna;
        //            case Weapon.IT_LIGHTNING:
        //                return Brushes.White;
        //            default:
        //                return Brushes.Transparent;
        //        }
        //    }
        //}

        public Brush TeamColor
        {
            get
            {
                switch(this._player.CurrentStats.TeamIndex)
                {
                    case 0:
                        return TeamColors.TeamRedAlpha80;
                    case 1:
                        return TeamColors.TeamBlueAlpha80;
                    default:
                        return Brushes.Transparent;
                }
            }
        }

        public string Name
        {
            get
            {
                return this._player.CurrentStats.Name;
            }
        }

        public double MaxPlayerDiameterValue
        {
            get
            {
                return MaxPlayerDiameter;
            }
        }

        public double PlayerDiameterValue
        {
            get
            {
                return PlayerDiameter;
            }
        }

        public double LightninggunDiameterValue
        {
            get
            {
                return LightninggunDiameter;
            }
        }

        public double RocketLauncherDiameterValue
        {
            get
            {
                return RocketLauncherDiameter;
            }
        }

        public double WeaponBorderThicknessValue
        {
            get
            {
                return WeaponBorderThickness;
            }
        }

        //public Player Player
        //{
        //    get
        //    {
        //        return this._player;
        //    }
        //}

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    if (this.PropertyChanged != null)
        //    {
        //        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        //public ParticipantInfoViewModel FinalStats
        //{
        //    get
        //    {
        //        return this._finalStats;
        //    }
        //}

        //public ParticipantInfoViewModel CurrentStats
        //{
        //    get
        //    {
        //        return this._currentStats;
        //    }
        //}
    }

    public class TeamColors
    {
        public static Brush TeamBlueAlpha30;
        public static Brush TeamRedAlpha30;
        public static Brush TeamBlueAlpha80;
        public static Brush TeamRedAlpha80;
        public static Brush TeamBlueAlpha150;
        public static Brush TeamRedAlpha150;

        static TeamColors()
        {
            TeamBlueAlpha30 = new SolidColorBrush(Color.FromArgb(30, 0, 0, 255));
            TeamBlueAlpha30.Freeze();
            TeamRedAlpha30 = new SolidColorBrush(Color.FromArgb(30, 255, 0, 0));
            TeamRedAlpha30.Freeze();
            TeamBlueAlpha80 = new SolidColorBrush(Color.FromArgb(80, 0, 0, 255));
            TeamBlueAlpha80.Freeze();
            TeamRedAlpha80 = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0));
            TeamRedAlpha80.Freeze();
            TeamBlueAlpha150 = new SolidColorBrush(Color.FromArgb(150, 0, 0, 255));
            TeamBlueAlpha150.Freeze();
            TeamRedAlpha150 = new SolidColorBrush(Color.FromArgb(150, 255, 0, 0));
            TeamRedAlpha150.Freeze();
        }
    }

    public class MatchKillViewModel : INotifyPropertyChanged
    {
        private const double MapImageScale = 0.5d;

        private MatchKill _matchKill;

        public MatchKillViewModel(MatchKill matchKill)
        {
            this._matchKill = matchKill;
        }

        public bool IsVisible
        {
            get
            {
                return this._matchKill.IsActive;
            }
        }

        public double FraggerOffsetX
        {
            get
            {
                return MapImageScale * this._matchKill.FraggerOffsetX + AdjustmentX;
            }
        }

        public double FraggerOffsetY
        {
            get
            {
                return MapImageScale * this._matchKill.FraggerOffsetY + AdjustmentY;
            }
        }

        public double DeathOffsetX
        {
            get
            {
                return MapImageScale * this._matchKill.DeathOffsetX + AdjustmentX;
            }
        }

        public double DeathOffsetY
        {
            get
            {
                return MapImageScale * this._matchKill.DeathOffsetY + AdjustmentY;
            }
        }

        public double AdjustmentX { get; set; }
        public double AdjustmentY { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void UpdateUIBindings()
        {
            if (this.IsVisible == false)
            {
                return;
            }
            OnPropertyChanged("FraggerOffsetX");
            OnPropertyChanged("FraggerOffsetY");
            OnPropertyChanged("DeathOffsetX");
            OnPropertyChanged("DeathOffsetY");
            OnPropertyChanged("IsVisible");
        }
    }

    public class EntityViewModel : INotifyPropertyChanged
    {
        private const double MapImageScale = 0.5d;
        private const double DiameterScale = 0.15d;
        public const double EntityDiameter = DiameterScale * 600;
        public const double PackDiameter = DiameterScale * 250;
        public const double ImageDiameter = DiameterScale * 150;
        public bool IsVisible { get; private set; }

        private Entity _entity;

        private bool _isSpawned;
        private bool _isRespawnTimeVisible;
        private int _remainingRespawnTime;

        //public bool IsSpawned { get { return (bool)GetValue(IsSpawnedProperty); } set { SetValue(IsSpawnedProperty, value); } }
        //public bool IsRespawnTimeVisible { get { return (bool)GetValue(IsRespawnTimeVisibleProperty); } set { SetValue(IsRespawnTimeVisibleProperty, value); } }
        //public int RemainingRespawnTime { get { return (int)GetValue(RemainingRespawnTimeProperty); } set { SetValue(RemainingRespawnTimeProperty, value); } }

        //public static readonly DependencyProperty IsSpawnedProperty = DependencyProperty.Register("IsSpawned ", typeof(bool), typeof(EntityViewModel), new UIPropertyMetadata(true));
        //public static readonly DependencyProperty IsRespawnTimeVisibleProperty = DependencyProperty.Register("IsRespawnTimeVisible ", typeof(bool), typeof(EntityViewModel), new UIPropertyMetadata(true));
        //public static readonly DependencyProperty RemainingRespawnTimeProperty = DependencyProperty.Register("RemainingRespawnTime ", typeof(int), typeof(EntityViewModel), new UIPropertyMetadata(30));

        public EntityViewModel(Entity entity)
        {
            this._entity = entity;
            this.IsVisible = entity.Type != EntityType.Mega;
            //UpdateUIBindings();
        }

        internal void UpdateUIBindings()
        {
            //if (this.IsVisible == false)
            //{
            //    return;
            //}
            //OnPropertyChanged("OffsetX");
            //OnPropertyChanged("OffsetY");

            //OnPropertyChanged("IsSpawned");
            //OnPropertyChanged("IsRespawnTimeVisible");
            //OnPropertyChanged("RemainingRespawnTime");
            bool isSpawned = this._entity.IsActive || this._entity.IsPack;
            if (this._isSpawned != isSpawned)
            {
                this._isSpawned = isSpawned;
                OnPropertyChanged("IsSpawned");
            }
            bool isRespawnTimeVisible = isSpawned == false;
            if (this._isRespawnTimeVisible != isRespawnTimeVisible)
            {
                this._isRespawnTimeVisible = isRespawnTimeVisible;
                OnPropertyChanged("IsRespawnTimeVisible");
            }
            int remainingRespawnTime = (int)this._entity.RemainingRespawnTime;
            if (this._remainingRespawnTime != remainingRespawnTime)
            {
                this._remainingRespawnTime = remainingRespawnTime;
                OnPropertyChanged("RemainingRespawnTime");
            }
        }

        public bool IsSpawned
        {
            get
            {
                return this._isSpawned;
            }
        }

        public bool IsRespawnTimeVisible
        {
            get
            {
                return this._isRespawnTimeVisible;
            }
        }

        public int RemainingRespawnTime
        {
            get
            {
                return this._remainingRespawnTime;
            }
        }

        public Entity Entity
        {
            get
            {
                return this._entity;
            }
        }

        public double EntityDiameterValue
        {
            get
            {
                return EntityDiameter;
            }
        }

        public double PackDiameterValue
        {
            get
            {
                return PackDiameter;
            }
        }

        public bool IsPack
        {
            get
            {
                return this._entity.IsPack;
            }
        }

        //public bool IsSpawned
        //{
        //    get
        //    {
        //        return this._entity.IsActive || this._entity.IsPack;
        //    }
        //}

        //public bool IsRespawnTimeVisible
        //{
        //    get
        //    {
        //        return this.IsSpawned == false;
        //    }
        //}

        public double OffsetX
        {
            get
            {
                return MapImageScale * this._entity.OffsetX + AdjustmentX - 0.5d * EntityDiameter;
            }
        }

        public double OffsetY
        {
            get
            {
                return MapImageScale * this._entity.OffsetY + AdjustmentY - 0.5d * EntityDiameter;
            }
        }

        public double ImageDiameterValue
        {
            get
            {
                return ImageDiameter;
            }
        }

        //public int RemainingRespawnTime
        //{
        //    get
        //    {
        //        return (int)this._entity.RemainingRespawnTime;
        //    }
        //}

        public bool IsPowerupQuad
        {
            get
            {
                return this._entity.Type == EntityType.Quad;
            }
        }
        public bool IsPowerupEyes
        {
            get
            {
                return this._entity.Type == EntityType.Eyes;
            }
        }
        public bool IsPowerupPent
        {
            get
            {
                return this._entity.Type == EntityType.Pent;
            }
        }

        public bool IsPowerup
        {
            get
            {
                return this._entity.Type == EntityType.Eyes || this._entity.Type == EntityType.Quad || this._entity.Type == EntityType.Pent;
            }
        }

        public bool IsPowerupVisible
        {
            get
            {
                return this.IsPowerup && true;
            }
        }

        public bool IsImage
        {
            get
            {
                return this.IsPowerup == false;
            }
        }

        public bool IsImageVisible
        {
            get
            {
                //do not show image if item is a powerup
                return this.IsImage && true;
            }
        }

        public BitmapImage Image
        {
            get
            {
                try
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    switch (this._entity.Type)
                    {
                        case EntityType.ArmorGA:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/ArmorGA.png"));
                            break;
                        case EntityType.ArmorYA:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/ArmorYA.png"));
                            break;
                        case EntityType.ArmorRA:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/ArmorRA.png"));
                            break;
                        case EntityType.SuperShotgun:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/SuperShotgun.png"));
                            break;
                        case EntityType.SuperNailgun:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/SuperNailgun.png"));
                            break;
                        case EntityType.GrenadeLauncher:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/GrenadeLauncher.png"));
                            break;
                        case EntityType.RocketLauncher:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/RocketLauncher.png"));
                            break;
                        case EntityType.Lightning:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/Lightning.png"));
                            break;
                        case EntityType.Rockets5:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/Rockets5.png"));
                            break;
                        case EntityType.Rockets10:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/Rockets10.png"));
                            break;
                        case EntityType.Health10:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/Health10.png"));
                            break;
                        case EntityType.Health25:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/Health25.png"));
                            break;
                        case EntityType.Mega:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/Mega.png"));
                            break;
                        case EntityType.Cells:
                            img.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/Cells.png"));
                            break;
                        case EntityType.Quad:
                            return null;
                        case EntityType.Eyes:
                            return null;
                        case EntityType.Pent:
                            return null;
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
        }

        public double AdjustmentX { get; set; }
        public double AdjustmentY { get; set; }

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
