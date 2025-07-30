using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Demolyzer.Model
{
    public class Player
    {
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public uint HealthValue { get; set; }
        public uint ArmorValue { get; set; }
        public Armor CurrentArmor { get; set; }
        public Weapon ActiveWeapon { get; set; }
        public Weapon CurrentWeapons { get; set; }
        public Powerup Powerup { get; set; }
        public bool IsAlive { get; set; }
        //public bool IsSpectator { get; set; }
        public bool HasEnemyPack { get; set; }
        public uint PackSourcePlayer { get; set; }

        public double MatchTimeRLBegin { get; set; }
        public double MatchTimeRLEnd { get; set; }
        public double SpawnTime { get; set; }

        public string Client { get; set; }
        public string VIP { get; set; }
        public string Chat { get; set; }
        public string Skin { get; set; }
        public string BottomColor { get; set; }
        public string TopColor { get; set; }
        public string Spectator { get; set; }

        public Point Location { get { return new Point(this.OffsetX, this.OffsetY); } }

        //STATS
        public ParticipantInfo CurrentStats { get; private set; }
        //public ParticipantInfo FinalStats { get; set; }


        public void UpdatePosition(double deltaX, double deltaY)
        {
            this.OffsetX += deltaX;
            this.OffsetY += deltaY;
        }

        public Player()
        {
            this.CurrentStats = new ParticipantInfo();
        }

        public void Reset()
        {
            this.OffsetX = 0;
            this.OffsetY = 0;
            this.HealthValue = 0;
            this.ArmorValue = 0;
            this.CurrentArmor = Model.Armor.None;
        }
    }
}
