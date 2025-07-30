using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Demolyzer.Model
{
    public class Entity
    {
        private int _respawnDuration;
        private EntityType _type;

        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public bool IsActive { get; set; }
        public bool IsPack { get; set; }
        public uint PackSourcePlayer { get; set; } //player that dropped the pack
        public int DeathMatch { get; set; }

        public Point Location
        {
            get
            {
                return new Point(this.OffsetX, this.OffsetY);
            }
        }

        public bool IsArmor
        {
            get
            {
                return this._type == EntityType.ArmorRA || this._type == EntityType.ArmorYA || this._type == EntityType.ArmorGA;
            }
        }

        public bool IsHealth
        {
            get
            {
                return this._type == EntityType.Health10 || this._type == EntityType.Health25;
            }
        }

        public Entity()
        {
            Reset();
        }

        private double _startTime;
        private double _currentTime;

        public void BeginPickup(double startTime)
        {
            this._startTime = startTime;
        }

        public void UpdateTime(double matchTime)
        {
            this._currentTime = matchTime;
            this.IsActive = this.RemainingRespawnTime == 0;
        }

        public void Reset()
        {
            this._startTime = -100000; //make the entity start out 'active'
            this.IsActive = true;
        }

        public double RemainingRespawnTime
        {
            get
            {
                double val = this._respawnDuration - (this._currentTime - this._startTime);
                if (val > 0d)
                {
                    return val;
                }
                return 0d;
            }
        }

        public EntityType Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
                switch (this._type)
                {
                    case EntityType.ArmorGA:
                    case EntityType.ArmorRA:
                    case EntityType.ArmorYA:
                    case EntityType.Health10:
                    case EntityType.Health25:
                        this._respawnDuration = 20;
                        break;
                    case EntityType.SuperShotgun:
                    case EntityType.SuperNailgun:
                    case EntityType.GrenadeLauncher:
                    case EntityType.RocketLauncher:
                    case EntityType.Lightning:
                        if (this.DeathMatch == 1)
                        {
                            this._respawnDuration = 30;
                        }
                        else
                        {
                            this._respawnDuration = 0;
                        }
                        break;
                    case EntityType.Cells:
                    case EntityType.Rockets10:
                    case EntityType.Rockets5:
                        if (this.DeathMatch == 1)
                        {
                            this._respawnDuration = 30;
                        }
                        else
                        {
                            this._respawnDuration = 15;
                        }
                        break;
                    case EntityType.Quad:
                        this._respawnDuration = 60;
                        break;
                    case EntityType.Eyes:
                    case EntityType.Pent:
                        this._respawnDuration = 5 * 60;
                        break;
                    case EntityType.Mega:
#warning TODO
                        this._respawnDuration = 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("_type", this._type, "Unsupported entity");
                }
            }
        }
    }
}
