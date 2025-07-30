using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumBitDesigns.Mvvm;
using Demolyzer.Model;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Demolyzer.ViewModel
{
    public class ParticipantInfoViewModel : ViewModelBase
    {
        private ParticipantInfo _participantInfo;
        private ParticipantInfo[] _participantInfos;
        private bool _isTeamStats;

        public ParticipantInfoViewModel(ParticipantInfo participantInfo)
        {
            this._participantInfo = participantInfo;
        }

        public ParticipantInfoViewModel(ParticipantInfo[] participantInfos, bool dynamicCombine)
        {
            //if this is static combine (final stats) go ahead and combine all stats
            if (dynamicCombine == false)
            {
                this._participantInfo = ParticipantInfo.Sum(participantInfos);

                //use team as 'name' since the UI is bound to Name
                this._participantInfo.Name = participantInfos[0].Team;
            }
            else //otherwise we will be doing combining on the fly
            {
                this._participantInfo = new ParticipantInfo();
                this._participantInfo.TeamIndex = participantInfos[0].TeamIndex;
                this._participantInfo.Team = participantInfos[0].Team;
                this._participantInfos = participantInfos;
            }
            this._isTeamStats = true;
        }

        public ParticipantInfo ParticipantInfo
        {
            get
            {
                return this._participantInfo;
            }
        }

        public void RefreshUIBindings()
        {
            if (this._isTeamStats == true)
            {
                this._participantInfo.Ping = (uint)this._participantInfos.Average(i => i.Ping);
                this._participantInfo.PL = (uint)this._participantInfos.Average(i => i.PL);
                this._participantInfo.CurrentScore = (int)this._participantInfos.Sum(i => i.CurrentScore);
                this._participantInfo.CurrentValueArmor = (uint)this._participantInfos.Sum(i => i.CurrentValueArmor);
                this._participantInfo.CurrentValueHealth = (uint)this._participantInfos.Sum(i => i.CurrentValueHealth);

                this._participantInfo.ItemCountRL = 0;
                this._participantInfo.ItemCountLG = 0;
                for (int i = 0; i < this._participantInfos.Length; ++i)
                {
                    if ((this._participantInfos[i].CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER)
                    {
                        this._participantInfo.ItemCountRL++;
                    }
                    if ((this._participantInfos[i].CurrentWeapons & Weapon.IT_LIGHTNING) == Weapon.IT_LIGHTNING)
                    {
                        this._participantInfo.ItemCountLG++;
                    }
                }
                OnPropertyChanged("HasRL1");
                OnPropertyChanged("HasRL2");
                OnPropertyChanged("HasRL3");
                OnPropertyChanged("HasRL4");
                OnPropertyChanged("HasLG1");
                OnPropertyChanged("HasLG2");
                OnPropertyChanged("HasLG3");
                OnPropertyChanged("HasLG4");
            }
            OnPropertyChanged("Ping");
            OnPropertyChanged("PL");
            OnPropertyChanged("Score");
            OnPropertyChanged("CurrentValueArmor");
            OnPropertyChanged("CurrentArmorColor");
            OnPropertyChanged("CurrentValueHealth");
            OnPropertyChanged("HasRL");
            OnPropertyChanged("HasLG");
        }

        public bool HasRL1 { get { return this._participantInfo.ItemCountRL >= 1; } }
        public bool HasRL2 { get { return this._participantInfo.ItemCountRL >= 2; } }
        public bool HasRL3 { get { return this._participantInfo.ItemCountRL >= 3; } }
        public bool HasRL4 { get { return this._participantInfo.ItemCountRL >= 4; } }

        public bool HasLG1 { get { return this._participantInfo.ItemCountLG >= 1; } }
        public bool HasLG2 { get { return this._participantInfo.ItemCountLG >= 2; } }
        public bool HasLG3 { get { return this._participantInfo.ItemCountLG >= 3; } }
        public bool HasLG4 { get { return this._participantInfo.ItemCountLG >= 4; } }

        public Brush TeamColor
        {
            get
            {
                if (this._participantInfo.TeamIndex == 0)
                {
                    return TeamColors.TeamRedAlpha150;
                }
                return TeamColors.TeamBlueAlpha150;
            }
        }

        public Brush RowBackground
        {
            get
            {
                if (this._isTeamStats == true)
                {
                    if (this._participantInfo.TeamIndex == 0)
                    {
                        return TeamColors.TeamRedAlpha80;
                    }
                    if (this._participantInfo.TeamIndex == 1)
                    {
                        return TeamColors.TeamBlueAlpha80;
                    }
                }
                else
                {
                    if (this._participantInfo.TeamIndex == 0)
                    {
                        return TeamColors.TeamRedAlpha30;
                    }
                    if (this._participantInfo.TeamIndex == 1)
                    {
                        return TeamColors.TeamBlueAlpha30;
                    }
                }
                return Brushes.Black;
            }
        }

        public Brush CurrentArmorColor
        {
            get
            {
                if ((this._participantInfo.CurrentArmor & Model.Armor.IT_ARMOR1) == Model.Armor.IT_ARMOR1)
                {
                    return Brushes.Chartreuse;
                }
                if ((this._participantInfo.CurrentArmor & Model.Armor.IT_ARMOR2) == Model.Armor.IT_ARMOR2)
                {
                    return Brushes.Yellow;
                }
                if ((this._participantInfo.CurrentArmor & Model.Armor.IT_ARMOR3) == Model.Armor.IT_ARMOR3)
                {
                    return Brushes.Red;
                }
                return Brushes.Gray;
            }
        }

        public bool HasRL
        {
            get
            {
                return (this._participantInfo.CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER;
            }
        }

        public bool HasLG
        {
            get
            {
                return (this._participantInfo.CurrentWeapons & Weapon.IT_LIGHTNING) == Weapon.IT_LIGHTNING;
            }
        }

        public BitmapImage ImageRL
        {
            get
            {
                return _imageRL;
            }
        }

        public BitmapImage ImageLG
        {
            get
            {
                return _imageLG;
            }
        }

        private static BitmapImage _imageRL;
        private static BitmapImage _imageLG;
 
        static ParticipantInfoViewModel()
        {
            _imageRL = new BitmapImage();
            _imageRL.BeginInit();
            _imageRL.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/RocketLauncher.png"));
            _imageRL.EndInit();
            _imageRL.Freeze();

            _imageLG = new BitmapImage();
            _imageLG.BeginInit();
            _imageLG.UriSource = new Uri(String.Format(@"pack://application:,,,/Demolyzer;component/Resources/Entities/Lightning.png"));
            _imageLG.EndInit();
            _imageLG.Freeze();
        }

        public string Name { get { return this._participantInfo.Name; } }
        public string Team { get { return this._participantInfo.Team; } }
        public int Score { get { return this._participantInfo.CurrentScore; } }
        public uint Ping { get { return (uint)this._participantInfo.Ping; } }
        public uint PL { get { return (uint)this._participantInfo.PL; } }
        public uint AveragePing { get { return (uint)this._participantInfo.AveragePing; } }
        public uint AveragePL { get { return (uint)this._participantInfo.AveragePL; } }
        public int Rating { get { return (int)this._participantInfo.Rating; } }

        public uint KillsEnemy { get { return this._participantInfo.KillsEnemy; } }
        public uint Deaths { get { return this._participantInfo.Deaths; } }
        public string Efficiency { get { return String.Format("{0:F0}%", this._participantInfo.EfficiencyPercent); } }
        public uint MaxFragStreak { get { return this._participantInfo.MaxFragStreak; } }
        public uint KillsWithQuad { get { return this._participantInfo.KillsWithQuad; } }
        public uint MaxQuadStreak { get { return this._participantInfo.MaxQuadStreak; } }
        //public string AverageFragStreak { get { return String.Format("{0:F1}", this._participantInfo.AverageFragStreak); } }
        public uint KillsFromEnemyPack { get { return this._participantInfo.KillsFromEnemyPack; } }
        public uint KillsSpawn { get { return this._participantInfo.KillsSpawn; } }
        public uint KilledRL { get { return this._participantInfo.KilledRL; } }
        public uint KilledLG { get { return this._participantInfo.KilledLG; } }
        public uint KilledQuad { get { return this._participantInfo.KilledQuad; } }
        public int KillsTeam { get { return Math.Max(0, (int)((int)KillsEnemy - (int)Score) - (int)Suicides); } }
        public uint DeathsTeam { get { return this._participantInfo.DeathsTeam; } }
        public uint DeathsSpawns { get { return this._participantInfo.DeathsSpawns; } }
        public uint Suicides { get { return this._participantInfo.Suicides; } }
        public uint TeamDeathsFromDroppedPack { get { return this._participantInfo.TeamDeathsFromDroppedPack; } }

        public uint KillsSG { get { return this._participantInfo.KillsSG; } }
        public uint KillsNG { get { return this._participantInfo.KillsNG; } }
        public uint KillsGL { get { return this._participantInfo.KillsGL; } }
        public uint KillsRL { get { return this._participantInfo.KillsRL; } }
        public uint KillsLG { get { return this._participantInfo.KillsLG; } }

        public string DamageEfficiency { get { return String.Format("{0:F0}%", this._participantInfo.DamageEfficiency); } }
        public uint DamageGiven { get { return this._participantInfo.DamageGiven; } }
        public uint DamageTeam { get { return this._participantInfo.DamageTeam; } }
        public uint Damage { get { return this._participantInfo.DamageTotal; } }
        public uint DamageArmor { get { return this._participantInfo.DamageArmor; } }
        public uint DamageHealth { get { return this._participantInfo.DamageHealth; } }
        public string PercentArmor { get { return String.Format("{0:F0}%", 100d * (double)this.DamageArmor / (double)this.Damage); } }
        public uint Health { get { return this._participantInfo.HealthAcquired; } }
        public uint Armor { get { return this._participantInfo.ArmorAcquired; } }

        public string LGPercent { get { return String.Format("{0:F1}%", this._participantInfo.LGPercent); } }
        public string RLPercent { get { return String.Format("{0:F1}%", this._participantInfo.RLPercent); } }
        public string GLPercent { get { return String.Format("{0:F1}%", this._participantInfo.GLPercent); } }
        public string SGPercent { get { return String.Format("{0:F1}%", this._participantInfo.SGPercent); } }
        public string SSGPercent { get { return String.Format("{0:F1}%", this._participantInfo.SSGPercent); } }
        public uint RLAvgDamage { get { return (uint)this._participantInfo.RLAvgDamage; } }
        public uint RLDirectHits { get { return this._participantInfo.RLDirectHits; } }

        public uint ItemCountRA { get { return this._participantInfo.ItemCountRA; } }
        public uint ItemCountYA { get { return this._participantInfo.ItemCountYA; } }
        public uint ItemCountGA { get { return this._participantInfo.ItemCountGA; } }
        public uint ItemCountMega { get { return this._participantInfo.ItemCountMega; } }
        public uint ItemCountQuad { get { return this._participantInfo.ItemCountQuad; } }
        public uint ItemCountPent { get { return this._participantInfo.ItemCountPent; } }
        public uint ItemCountEyes { get { return this._participantInfo.ItemCountEyes; } }
        //public uint ItemCountSSG { get { return this._participantInfo.ItemCountSSG; } }
        //public uint ItemCountNG { get { return this._participantInfo.ItemCountNG; } }
        //public uint ItemCountSNG { get { return this._participantInfo.ItemCountSNG; } }
        public uint ItemCountGL { get { return this._participantInfo.ItemCountGL; } }
        public uint ItemCountRL { get { return this._participantInfo.ItemCountRL; } }
        public uint ItemCountRLFromTeamPack { get { return this._participantInfo.ItemCountRLFromTeamPack; } }
        public uint ItemCountRLFromEnemyPack { get { return this._participantInfo.ItemCountRLFromEnemyPack; } }
        public uint ItemCountLG { get { return this._participantInfo.ItemCountLG; } }
        public uint ItemCountLGFromTeamPack { get { return this._participantInfo.ItemCountLGFromTeamPack; } }
        public uint ItemCountLGFromEnemyPack { get { return this._participantInfo.ItemCountLGFromEnemyPack; } }

        public uint ItemCountRLNoDrop { get { return this._participantInfo.ItemCountRLNoDrop; } }
        public uint ItemCountRLDropped { get { return this._participantInfo.ItemCountRLDropped; } }
        public uint TP_RLDropPercent { get { return this._participantInfo.TP_RLDropPercent; } }
        public uint ItemCountRLLost { get { return this._participantInfo.ItemCountRLLost; } }
        public uint ItemCountRLDonated { get { return this._participantInfo.ItemCountRLDonated; } }
        public uint ItemCountLGNoDrop { get { return this._participantInfo.ItemCountLGNoDrop; } }
        public uint ItemCountLGDropped { get { return this._participantInfo.ItemCountLGDropped; } }
        public uint ItemCountLGLost { get { return this._participantInfo.ItemCountLGLost; } }
        public uint ItemCountLGDonated { get { return this._participantInfo.ItemCountLGDonated; } }

        public uint AverageSpeed { get { return (uint)this._participantInfo.AverageSpeed; } }
        public uint AveragePower { get { return (uint)this._participantInfo.AveragePower; } }

        public uint KillsWithRL { get { return this._participantInfo.KillsWithRL; } }
        public string KillsPerRL { get { return String.Format("{0:F1}", this._participantInfo.AverageKillsPerRLTaken); } }
        public string LivesToGetRL { get { return String.Format("{0:F1}", this._participantInfo.AverageLivesToGetRL); } }
        public string TimeWithRL { get { return String.Format("{0:mm\\:ss} ({1:F0}%)", TimeSpan.FromSeconds(this._participantInfo.TotalTimeWithRL), this._participantInfo.PercentTimeWithRL * 100d); } }
        public string AverageTimeWithRL { get { return String.Format("{0:mm\\:ss}", TimeSpan.FromSeconds(this._participantInfo.AverageTimeWithRL)); } }
        public string AverageTimeToGetRL { get { return String.Format("{0:mm\\:ss}", TimeSpan.FromSeconds(this._participantInfo.AverageTimeToGetRL)); } }

        public uint CurrentValueArmor { get { return this._participantInfo.CurrentValueArmor; } }
        public uint CurrentValueHealth { get { return this._participantInfo.CurrentValueHealth; } }


        public int TP_Rating { get { return this._participantInfo.TP_Rating; } }
        public uint TP_AvgPower { get { return this._participantInfo.TP_AvgPower; } }
        public uint TP_KillRLLG { get { return (uint)this._participantInfo.TP_KillRL + (uint)this._participantInfo.TP_KillLG; } }
        public uint TP_KillQuad { get { return this._participantInfo.TP_KillQuad; } }
        public uint TP_RA { get { return this._participantInfo.TP_RA; } }
        public uint TP_YA { get { return this._participantInfo.TP_YA; } }
        public uint TP_GA { get { return this._participantInfo.TP_GA; } }
        public uint TP_Quad { get { return this._participantInfo.TP_Quad; } }
        public uint TP_Pent { get { return this._participantInfo.TP_Pent; } }
        public uint TP_Eyes { get { return this._participantInfo.TP_Eyes; } }
        public int TP_RLLGLost { get { return this._participantInfo.TP_RLLGLost; } }
        public uint TP_RLLGDonated { get { return this._participantInfo.TP_RLLGDonated; } }
        public uint TP_RLTime { get { return this._participantInfo.TP_RLTime; } }

        public uint RLvsRLWon { get { return (uint)this._participantInfo.RLvsRLWon; } }
        public uint RLvsRLLost { get { return (uint)this._participantInfo.RLvsRLLost; } }
        public uint RLvsRLPercent { get { return (uint)this._participantInfo.RLvsRLPercent; } }
        public uint RLvsXWon { get { return (uint)this._participantInfo.RLvsXWon; } }
        public uint RLvsXLost { get { return (uint)this._participantInfo.RLvsXLost; } }
        public uint RLvsXPercent { get { return (uint)this._participantInfo.RLvsXPercent; } }
        public uint XvsRLWon { get { return (uint)this._participantInfo.XvsRLWon; } }
        public uint XvsRLLost { get { return (uint)this._participantInfo.XvsRLLost; } }
        public uint XvsRLPercent { get { return (uint)this._participantInfo.XvsRLPercent; } }
        public uint XvsXWon { get { return (uint)this._participantInfo.XvsXWon; } }
        public uint XvsXLost { get { return (uint)this._participantInfo.XvsXLost; } }
        public uint XvsXPercent { get { return (uint)this._participantInfo.XvsXPercent; } }
    }
}
