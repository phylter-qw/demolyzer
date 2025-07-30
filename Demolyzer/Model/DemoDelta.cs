using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model
{
    public class ServerInfo
    {
        public string Map { get; set; }
        public string Fraglimit { get; set; }
        public string Watervis { get; set; }
        public string Antilag { get; set; }
        public string Version { get; set; }
        public string Timelimit { get; set; }
        public string Teamplay { get; set; }
        public int Deathmatch { get; set; }
        public string MaxClients { get; set; }
        public string MaxSpectators { get; set; }
        public string GameDir { get; set; }
        public string MaxFPS { get; set; }
        public string Hostname { get; set; }
        public string KTXVersion { get; set; }
        public string KTXBuild { get; set; }
        public string ServerDemo { get; set; }
        public bool IsInvalidMap { get; set; }
    }

    public class FragInfo
    {
        public int Fragger { get; set; }
        public int Death { get; set; }
        public Weapon Weapon { get; set; }
        public double DemoTime { get; set; } //USED FOR HACK

        public FragInfo()
        {
            this.Fragger = -1;
            this.Death = -1;
            this.Weapon = Weapon.None;
        }
    }

    public class PlayerEndGameStats : DemoDelta
    {
        public uint Player { get; private set; }
        public uint DamageGiven { get; private set; }
        public uint DamageTaken { get; private set; }
        public uint DamageTeam { get; private set; }
        public double LGPercent { get; private set; }
        public double RLPercent { get; private set; }
        public double GLPercent { get; private set; }
        public double SGPercent { get; private set; }
        public double SSGPercent { get; private set; }
        public double RLAvgDamage { get; private set; }
        public uint RLDirectHits { get; private set; }

        public PlayerEndGameStats(double demoTime, uint player, uint damageGiven, uint damageTaken, uint damageTeam, double lgPercent, double rlPercent, double glPercent, double sgPercent, double ssgPercent, double rlAvgDmg, uint rlDirectHits)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.PlayerEndGameStats;
            this.Player = player;
            this.DamageGiven = damageGiven;
            this.DamageTaken = damageTaken;
            this.DamageTeam = damageTeam;
            this.LGPercent = lgPercent;
            this.RLPercent = rlPercent;
            this.GLPercent = glPercent;
            this.SGPercent = sgPercent;
            this.SSGPercent = ssgPercent;
            this.RLAvgDamage = rlAvgDmg;
            this.RLDirectHits = rlDirectHits;
        }
    }

    public class Kill : DemoDelta
    {
        public uint Fragger { get; private set; }
        public uint Death { get; private set; }
        public Weapon Weapon { get; private set; }

        public Kill(double demoTime, uint fragger, uint death, Weapon weapon)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.Kill;
            this.Fragger = fragger;
            this.Death = death;
            this.Weapon = weapon;
        }
    }

    public class Suicide : DemoDelta
    {
        public uint Player { get; private set; }

        public Suicide(double demoTime, uint player)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.Suicide;
            this.Player = player;
        }
    }
    public class WeaponPickup : DemoDelta
    {
        public uint Player { get; private set; }
        public Weapon Weapon { get; private set; }

        public WeaponPickup(double demoTime, uint player, Weapon weapon)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.WeaponPickup;
            this.Player = player;
            this.Weapon = weapon;
        }
    }

    public class PowerupPickup : DemoDelta
    {
        public uint Player { get; private set; }
        public Powerup Powerup { get; private set; }
        public bool IsExpired { get; private set; }

        public PowerupPickup(double demoTime, uint player, Powerup powerup, bool isExpired)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.PowerupPickup;
            this.Player = player;
            this.Powerup = powerup;
            this.IsExpired = isExpired;
        }
    }


    public class ActiveWeaponChanged : DemoDelta
    {
        public uint Player { get; private set; }
        public Weapon OldWeapon { get; private set; }
        public Weapon NewWeapon { get; private set; }

        public ActiveWeaponChanged(double demoTime, uint player, Weapon oldWeapon, Weapon newWeapon)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.ActiveWeaponChanged;
            this.Player = player;
            this.OldWeapon = oldWeapon;
            this.NewWeapon = newWeapon;
        }
    }

    public class StatChanged : DemoDelta
    {
        public uint Player { get; private set; }
        public Stat Stat { get; private set; }
        public int DeltaValue { get; private set; }

        public StatChanged(double demoTime, uint player, Stat stat, int deltaValue)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.StatChanged;
            this.Player = player;
            this.Stat = stat;
            this.DeltaValue = deltaValue;
        }
    }

    public class ArmorChanged : DemoDelta
    {
        public uint Player { get; private set; }
        public int DeltaArmor { get; private set; }
        public Armor NewArmor { get; private set; }
        public bool IsPickup { get; private set; }

        public ArmorChanged(double demoTime, uint player, Armor newArmor, int deltaArmor, bool isPickup)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.ArmorAcquired;
            this.Player = player;
            this.DeltaArmor = deltaArmor;
            this.NewArmor = newArmor;
            this.IsPickup = isPickup;
        }
    }
    public class Death : DemoDelta
    {
        public uint Player { get; private set; }
        public Death(double demoTime, uint player)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.Death;
            this.Player = player;
        }
    }

    public class Score : DemoDelta
    {
        public uint Player { get; private set; }
        public int ScoreChange { get; private set; }

        public Score(double demoTime, uint player, int scoreChange)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.Score;
            this.Player = player;
            this.ScoreChange = scoreChange;
        }
    }

    public class PlayerPing : DemoDelta
    {
        public uint Player { get; private set; }
        public uint Ping { get; private set; }

        public PlayerPing(double demoTime, uint player, uint ping)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.PlayerPing;
            this.Player = player;
            this.Ping = ping;
        }
    }

    public class PlayerPL : DemoDelta
    {
        public uint Player { get; private set; }
        public uint PL { get; private set; }

        public PlayerPL(double demoTime, uint player, uint pl)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.PlayerPL;
            this.Player = player;
            this.PL = pl;
        }
    }

    public class Damage : DemoDelta
    {
        public uint Player { get; private set; }
        public byte Health { get; private set; }
        public byte Armor { get; private set; }

        public Damage(double demoTime, uint player, byte health, byte armor)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.Damage;
            this.Player = player;
            this.Health = health;
            this.Armor = armor;
        }
    }
    public class Health : DemoDelta
    {
        public uint Player { get; private set; }
        public int DeltaHealth { get; private set; }

        public Health(double demoTime, uint player, int deltaHealth)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.Health;
            this.Player = player;
            this.DeltaHealth = deltaHealth;
        }
    }

    public class MatchStarted : DemoDelta
    {
        public MatchStarted(double demoTime)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.MatchStarted;
        }
    }

    public class MatchComplete : DemoDelta
    {
        public MatchComplete(double demoTime)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.MatchComplete;
        }
    }

    public class PlayerMovement : DemoDelta
    {
        public uint Player { get; private set; }
        public double DeltaX { get; private set; }
        public double DeltaY { get; private set; }
        public PlayerMovement(double demoTime, uint player, double deltaX, double deltaY)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.PlayerMovement;
            this.Player = player;
            this.DeltaX = deltaX;
            this.DeltaY = deltaY;
        }
    }

    public class ServerInfoDemoDelta : DemoDelta
    {
        public ServerInfo ServerInfo { get; private set; }
        public ServerInfoDemoDelta(double demoTime, ServerInfo serverinfo)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.ServerInfo;
            this.ServerInfo = serverinfo;
        }
    }

    public class SetInfo : UserInfo
    {
        public SetInfo(double demoTime, uint player, Dictionary<string, string> info)
            : base(demoTime, player, info)
        {
            this.Type = DemoDeltaType.SetInfo;
        }
    }

    public class UserInfo : DemoDelta
    {
        public uint Player { get; private set; }
        public Dictionary<string, string> Info { get; private set; }

        public UserInfo(double demoTime, uint player, Dictionary<string, string> info)
            : base(demoTime)
        {
            this.Type = DemoDeltaType.UserInfo;
            this.Player = player;
            this.Info = info;
        }

        public string Client
        {
            get
            {
                string value = null;
                this.Info.TryGetValue("*client", out value);
                return value;
            }
        }

        public string VIP
        {
            get
            {
                string value = null;
                this.Info.TryGetValue("*VIP", out value);
                return value;
            }
        }

        public string Chat
        {
            get
            {
                string value = null;
                this.Info.TryGetValue("chat", out value);
                return value;
            }
        }

        public string Skin
        {
            get
            {
                string value = null;
                this.Info.TryGetValue("skin", out value);
                return value;
            }
        }

        public string BottomColor
        {
            get
            {
                string value = null;
                this.Info.TryGetValue("bottomcolor", out value);
                return value;
            }
        }

        public string TopColor
        {
            get
            {
                string value = null;
                this.Info.TryGetValue("topcolor", out value);
                return value;
            }
        }

        public string Team
        {
            get
            {
                string value = null;
                this.Info.TryGetValue("team", out value);
                return value;
            }
        }

        public string Name
        {
            get
            {
                string value = null;
                this.Info.TryGetValue("name", out value);
                return value;
            }
        }

        public string Spectator
        {
            get
            {
                string value = null;
                this.Info.TryGetValue("*spectator", out value);
                return value;
            }
        }
    }

    public abstract class DemoDelta
    {
        public DemoDelta(double demoTime)
        {
            this.DemoTime = demoTime;
        }
        public double DemoTime { get; private set; }
        public DemoDeltaType Type { get; protected set; }
    }
}
