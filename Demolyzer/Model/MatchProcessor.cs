using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Demolyzer.Model
{
    public class MatchProcessor
    {
        public event EventHandler<PackEventArgs> PackDropped;
        public event EventHandler<PackEventArgs> PackPickup;

        private Player[] _players;
        private MatchKill[] _kills;
        private Entity[] _entities;
        private List<Entity> _droppedPacks;
        private bool _isMatchStarted;
        private bool _isMatchComplete;
        public double DemoTime { get; set; }
        public double MatchTime { get; set; }
        private double _lastFrameTime;
        private ParticipantInfo[] _teamPower;
        private ServerInfo _serverInfo;

        public MatchProcessor(Entity[] entities)
        {
            this._entities = entities;
            this._players = new Player[32];
            for (int i = 0; i < this._players.Length; ++i)
            {
                this._players[i] = new Player();
            }
            this._kills = new MatchKill[4];
            for (int i = 0; i < this._kills.Length; ++i)
            {
                this._kills[i] = new MatchKill();
            }
            this._droppedPacks = new List<Entity>();
        }

        private void CalculateParticipantPower()
        {
            for (int i = 0; i < this._players.Length; ++i)
            {
                if (this._players[i].CurrentStats.IsSpectator == false)
                {
                    int index = (int)this.MatchTime;
                    ParticipantInfo currentStats = this._players[i].CurrentStats;
                    if (index < currentStats.MatchPower.Length)
                    {
                        uint multiplier = 1;
                        if ((currentStats.CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER
                            || (currentStats.CurrentWeapons & Weapon.IT_LIGHTNING) == Weapon.IT_LIGHTNING)
                        {
                            multiplier *= 2;
                        }
                        if ((currentStats.CurrentPowerups & Powerup.IT_QUAD) == Powerup.IT_QUAD
                            || (currentStats.CurrentPowerups & Powerup.IT_INVULNERABILITY) == Powerup.IT_INVULNERABILITY)
                        {
                            multiplier *= 4;
                        }
                        currentStats.MatchPower[index] = multiplier * (currentStats.CurrentValueHealth + currentStats.CurrentValueArmor);
                        currentStats.MatchScore[index] = currentStats.CurrentScore;
                    }
                }
            }
        }

        public void PostProcess()
        {
            CalculateTeamPowerAndScores();
        }

        private void CalculateTeamPowerAndScores()
        {
            this._teamPower = new ParticipantInfo[2];
            this._teamPower[0] = new ParticipantInfo();
            this._teamPower[1] = new ParticipantInfo();

            Player[] players = this._players.Where(p => p.CurrentStats.IsSpectator == false).ToArray();
            this._teamPower[0].MatchType = players[0].CurrentStats.MatchType;
            this._teamPower[1].MatchType = players[0].CurrentStats.MatchType;

            for (int x = 0; x < players.Length; ++x)
            {
                ParticipantInfo player = players[x].CurrentStats;
                int teamIndex = player.TeamIndex;
                //if 1on1, make team be just the player index
                if (players.Length == 2)
                {
                    teamIndex = x;
                }
                ParticipantInfo team = this._teamPower[teamIndex];
                if (team.Team == null)
                {
                    if (players.Length == 2)
                    {
                        team.Team = player.Name;
                    }
                    else
                    {
                        team.Team = player.Team;
                    }
                }
                for (int i = 0; i < player.MatchPower.Length; ++i)
                {
                    team.MatchPower[i] += player.MatchPower[i];
                    team.MatchScore[i] += player.MatchScore[i];
                }
            }
        }

        public ParticipantInfo[] TeamPower
        {
            get
            {
                return this._teamPower;
            }
        }

        public void Process(DemoDelta packet)
        {
            if (packet.DemoTime > 0)
            {
                this.DemoTime += packet.DemoTime;
                if (this._isMatchStarted == true)
                {
                    this.MatchTime += packet.DemoTime;
                    this._lastFrameTime = packet.DemoTime;
                    if (this._isMatchComplete == false)
                    {
                        CalculateParticipantPower();
                    }
                }
            }
            switch (packet.Type)
            {
                case DemoDeltaType.MatchStarted:
                    //set the team ID of each player so that we can display a color for each team, 
                    //as well as identify players on same or different team
                    List<string> teams = new List<string>();
                    for (int x = 0; x < this._players.Length; ++x)
                    {
                        if (String.IsNullOrEmpty(this._players[x].CurrentStats.Team) == false && this._players[x].CurrentStats.IsSpectator == false)
                        {
                            int teamIndex = teams.IndexOf(this._players[x].CurrentStats.Team);
                            if (teamIndex < 0)
                            {
                                teamIndex = teams.Count;
                                teams.Add(this._players[x].CurrentStats.Team);
                            }
                            this._players[x].CurrentStats.TeamIndex = teamIndex;
                        }
                    }
                    int playerCount = this._players.Count(p => p.CurrentStats.IsSpectator == false);
                    MatchType matchType = MatchType.Unknown;
                    switch (playerCount)
                    {
                        case 2:
                            matchType = MatchType.Match1on1;
                            break;
                        case 4:
                            matchType = MatchType.Match2on2;
                            break;
                        case 8:
                            matchType = MatchType.Match4on4;
                            break;
                        default:
                            break;
                    }
                    //set match type for each player to initialize power and score array
                    for (int playerIndex = 0; playerIndex < this._players.Length; ++playerIndex)
                    {
                        if (this._players[playerIndex].CurrentStats.IsSpectator == false)
                        {
                            this._players[playerIndex].CurrentStats.MatchType = matchType;
                        }
                    }

                    this._isMatchStarted = true;
                    break;
                case DemoDeltaType.MatchComplete:
                    for (int playerIndex = 0; playerIndex < this._players.Length; ++playerIndex)
                    {
                        if (this._players[playerIndex].CurrentStats.IsSpectator == false)
                        {
                            if ((this._players[playerIndex].CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER)
                            {
                                double timeWithRL = this.MatchTime - this._players[playerIndex].MatchTimeRLBegin;
                                this._players[playerIndex].CurrentStats.TotalTimeWithRL += timeWithRL;
                            }
                            else //otherwise player did not have RL, so increment duration of RL get attempt
                            {
                                double totalTimeToGetRl = this.MatchTime - this._players[playerIndex].MatchTimeRLEnd;
                                this._players[playerIndex].CurrentStats.TotalTimeToGetRL += totalTimeToGetRl;
                            }

                            //check for new fragstreak max
                            uint curMax = this._players[playerIndex].CurrentStats.MaxFragStreak;
                            this._players[playerIndex].CurrentStats.MaxFragStreak = Math.Max(curMax, this._players[playerIndex].CurrentStats.FragStreak);

                            //check for quadstreak max
                            curMax = this._players[playerIndex].CurrentStats.MaxQuadStreak;
                            this._players[playerIndex].CurrentStats.MaxQuadStreak = Math.Max(curMax, this._players[playerIndex].CurrentStats.QuadStreak);
                        }
                    }
                    this._isMatchComplete = true;
                    break;
                case DemoDeltaType.ServerInfo:
                    ServerInfoDemoDelta serverInfo = (ServerInfoDemoDelta)packet;
                    this._serverInfo = serverInfo.ServerInfo;
                    break;
                case DemoDeltaType.Damage:
                    Damage damage = (Damage)packet;
                    this._players[damage.Player].CurrentStats.DamageHealth += damage.Health;
                    this._players[damage.Player].CurrentStats.DamageArmor += damage.Armor;
                    break;
                case DemoDeltaType.PlayerPing:
                    PlayerPing playerPing = (PlayerPing)packet;
                    this._players[playerPing.Player].CurrentStats.Ping = playerPing.Ping;
                    this._players[playerPing.Player].CurrentStats.TotalPing += playerPing.Ping;
                    this._players[playerPing.Player].CurrentStats.TotalPingCount++;
                    break;
                case DemoDeltaType.PlayerPL:
                    PlayerPL playerPL = (PlayerPL)packet;
                    this._players[playerPL.Player].CurrentStats.PL = playerPL.PL;
                    this._players[playerPL.Player].CurrentStats.TotalPL += playerPL.PL;
                    this._players[playerPL.Player].CurrentStats.TotalPLCount++;
                    break;
                case DemoDeltaType.Health:
                    Health health = (Health)packet;
                    uint healthValue = this._players[health.Player].HealthValue;
                    healthValue = (uint)(healthValue + health.DeltaHealth);

                    //keep track of time when user spawned, so we can calculate spawn frags and spawn deaths
                    if (health.DeltaHealth == 100 && this._players[health.Player].HealthValue == 0)
                    {
                        this._players[health.Player].SpawnTime = this.MatchTime;
                    }

                    if (health.DeltaHealth > 0 && health.DeltaHealth <= 25)
                    {
                        Entity healthEntity = GetClosestEntityHealth(this._players[health.Player].Location);
                        healthEntity.BeginPickup(this.MatchTime);
                    }

                    //if the player picked up some kind of health, keep track of how much
                    if (health.DeltaHealth > 0 && this._players[health.Player].HealthValue != 0)
                    {
                        this._players[health.Player].CurrentStats.HealthAcquired += (uint)health.DeltaHealth;

                        //if the player is above 100 health (any health gained must be mega)
                        //OR if the health gained is 100
                        if (this._players[health.Player].HealthValue > 100 || health.DeltaHealth == 100)
                        {
                            this._players[health.Player].CurrentStats.ItemCountMega++;
                        }
                    }

                    this._players[health.Player].HealthValue = healthValue;
                    this._players[health.Player].CurrentStats.CurrentValueHealth = healthValue;

                    this._players[health.Player].IsAlive = healthValue > 0;
                    break;
                case DemoDeltaType.ArmorAcquired:
                    ArmorChanged armor = (ArmorChanged)packet;
                    uint armorValue = this._players[armor.Player].ArmorValue;
                    armorValue = (uint)(armorValue + armor.DeltaArmor);
                    this._players[armor.Player].ArmorValue = armorValue;

                    //only change armor if armor increased
                    if (armor.DeltaArmor > 0)
                    {
                        this._players[armor.Player].CurrentArmor = armor.NewArmor;
                        this._players[armor.Player].CurrentStats.CurrentArmor = armor.NewArmor;
                    }
                    else
                    {
                        if (armorValue == 0)
                        {
                            this._players[armor.Player].CurrentArmor = Armor.None;
                            this._players[armor.Player].CurrentStats.CurrentArmor = Armor.None;
                        }
                    }

                    this._players[armor.Player].CurrentStats.CurrentValueArmor = armorValue;

                    if (armor.IsPickup == true)
                    {
                        Entity armorEntity = GetClosestEntityArmor(this._players[armor.Player].Location);
                        armorEntity.BeginPickup(this.MatchTime);

                        this._players[armor.Player].CurrentStats.ArmorAcquired += (uint)armor.DeltaArmor;
                        switch (armor.NewArmor)
                        {
                            case Armor.IT_ARMOR1:
                                this._players[armor.Player].CurrentStats.ItemCountGA++;
                                break;
                            case Armor.IT_ARMOR2:
                                this._players[armor.Player].CurrentStats.ItemCountYA++;
                                break;
                            case Armor.IT_ARMOR3:
                                this._players[armor.Player].CurrentStats.ItemCountRA++;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("armor.NewArmor", armor.NewArmor, "Unsupported armor");
                        }
                    }
                    break;
                case DemoDeltaType.PlayerMovement:
                    PlayerMovement playerMovement = (PlayerMovement)packet;
                    Vector v = new Vector(playerMovement.DeltaX, -playerMovement.DeltaY);
                    double distance = v.Length;
                    if (this._lastFrameTime > 0)
                    {
                        double speed = distance / this._lastFrameTime;
                        if (speed >= 320 && speed < 2000) //do not keep track of time moping, or spawning, or teleporting
                        {
                            this._players[playerMovement.Player].CurrentStats.TotalDistanceRan += distance;
                            this._players[playerMovement.Player].CurrentStats.TotalRunningDuration += this._lastFrameTime;
                        }
                    }
                    this._players[playerMovement.Player].UpdatePosition(playerMovement.DeltaX, -playerMovement.DeltaY);
                    break;
                case DemoDeltaType.ActiveWeaponChanged:
                    ActiveWeaponChanged activeWeaponChanged = (ActiveWeaponChanged)packet;
                    this._players[activeWeaponChanged.Player].ActiveWeapon = activeWeaponChanged.NewWeapon;
                    this._players[activeWeaponChanged.Player].CurrentStats.ActiveWeapon = activeWeaponChanged.NewWeapon;
                    break;
                case DemoDeltaType.Score:
                    Score score = (Score)packet;
                    this._players[score.Player].CurrentStats.CurrentScore += score.ScoreChange;
                    break;
                case DemoDeltaType.UserInfo:
                    UserInfo userInfo = (UserInfo)packet;
                    Player player = this._players[userInfo.Player];
                    player.Client = userInfo.Client;
                    player.VIP = userInfo.VIP;
                    player.Chat = userInfo.Chat;
                    player.Skin = userInfo.Skin;
                    player.BottomColor = userInfo.BottomColor;
                    player.TopColor = userInfo.TopColor;
                    player.CurrentStats.Team = userInfo.Team;
                    player.CurrentStats.Name = userInfo.Name;
                    player.Spectator = userInfo.Spectator;
                    if (player.Spectator == null) //if the player is not a spectator
                    {
                        player.IsAlive = true;
                        player.CurrentStats.IsSpectator = false;
                    }
                    else
                    {
                        player.CurrentStats.IsSpectator = true;
                    }
                    break;
                case DemoDeltaType.SetInfo:
                    SetInfo setInfo = (SetInfo)packet;
                    string name = setInfo.Name;
                    //if a player changes name
                    if (name != null)
                    {
                        this._players[setInfo.Player].CurrentStats.Name = name;
                    }
                    break;
                case DemoDeltaType.WeaponPickup:
                    WeaponPickup weaponPickup = (WeaponPickup)packet;
                    if (weaponPickup.Weapon != Weapon.None)
                    {
                        Entity weaponEntity = GetClosestEntityWeapon(this._players[weaponPickup.Player].Location, weaponPickup.Weapon);
                        if (weaponEntity != null)
                        {
                            //if the weapon was gained via a pack
                            if (weaponEntity.IsPack == true)
                            {
                                //if the pack is from an Enemy
                                if (this._players[weaponPickup.Player].CurrentStats.TeamIndex != this._players[weaponEntity.PackSourcePlayer].CurrentStats.TeamIndex)
                                {
                                    this._players[weaponPickup.Player].HasEnemyPack = true;
                                    this._players[weaponPickup.Player].PackSourcePlayer = weaponEntity.PackSourcePlayer;

                                    //count number of RLs acquired from picking up an enemy pack
                                    if (weaponPickup.Weapon == Weapon.IT_ROCKET_LAUNCHER)
                                    {
                                        this._players[weaponPickup.Player].CurrentStats.ItemCountRLFromEnemyPack++;
                                    }
                                    if (weaponPickup.Weapon == Weapon.IT_LIGHTNING)
                                    {
                                        this._players[weaponPickup.Player].CurrentStats.ItemCountLGFromEnemyPack++;
                                    }
                                }
                                else //otherwise pack is from a teammate
                                {
                                    //count number of RLs acquired from picking up a teammate pack
                                    if (weaponPickup.Weapon == Weapon.IT_ROCKET_LAUNCHER)
                                    {
                                        this._players[weaponPickup.Player].CurrentStats.ItemCountRLFromTeamPack++;
                                    }
                                    if (weaponPickup.Weapon == Weapon.IT_LIGHTNING)
                                    {
                                        this._players[weaponPickup.Player].CurrentStats.ItemCountLGFromTeamPack++;
                                    }
                                }

                                //if the dropped pack was given to the other team, it is considered 'lost'
                                if (this._players[weaponEntity.PackSourcePlayer].CurrentStats.TeamIndex != this._players[weaponPickup.Player].CurrentStats.TeamIndex)
                                {
                                    if (weaponPickup.Weapon == Weapon.IT_ROCKET_LAUNCHER)
                                    {
                                        this._players[weaponEntity.PackSourcePlayer].CurrentStats.ItemCountRLLost++;
                                    }
                                    if (weaponPickup.Weapon == Weapon.IT_LIGHTNING)
                                    {
                                        this._players[weaponEntity.PackSourcePlayer].CurrentStats.ItemCountLGLost++;
                                    }
                                }
                                else //otherwise pack was given to teammate, so it is considered 'donated'
                                {
                                    if (weaponPickup.Weapon == Weapon.IT_ROCKET_LAUNCHER)
                                    {
                                        this._players[weaponEntity.PackSourcePlayer].CurrentStats.ItemCountRLDonated++;
                                    }
                                    if (weaponPickup.Weapon == Weapon.IT_LIGHTNING)
                                    {
                                        this._players[weaponEntity.PackSourcePlayer].CurrentStats.ItemCountLGDonated++;
                                    }
                                }

                                //remove pack
                                OnPackPickup(new PackEventArgs(weaponEntity));
                                this._droppedPacks.Remove(weaponEntity);
                            }
                            else
                            {
                                weaponEntity.BeginPickup(this.MatchTime);
                            }
                        }
                        switch (weaponPickup.Weapon)
                        {
                            case Weapon.IT_SUPER_SHOTGUN:
                                this._players[weaponPickup.Player].CurrentStats.ItemCountSSG++;
                                break;
                            case Weapon.IT_NAILGUN:
                                this._players[weaponPickup.Player].CurrentStats.ItemCountNG++;
                                break;
                            case Weapon.IT_SUPER_NAILGUN:
                                this._players[weaponPickup.Player].CurrentStats.ItemCountSNG++;
                                break;
                            case Weapon.IT_GRENADE_LAUNCHER:
                                this._players[weaponPickup.Player].CurrentStats.ItemCountGL++;
                                break;
                            case Weapon.IT_ROCKET_LAUNCHER:
                                this._players[weaponPickup.Player].CurrentStats.ItemCountRL++;
                                double totalTimeToGetRl = this.MatchTime - this._players[weaponPickup.Player].MatchTimeRLEnd;
                                this._players[weaponPickup.Player].CurrentStats.TotalTimeToGetRL += totalTimeToGetRl;
                                this._players[weaponPickup.Player].MatchTimeRLBegin = this.MatchTime;
                                break;
                            case Weapon.IT_LIGHTNING:
                                this._players[weaponPickup.Player].CurrentStats.ItemCountLG++;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("weaponPickup.Weapon", weaponPickup.Weapon, "Unsupported weapon");
                        }
                    }
                    this._players[weaponPickup.Player].CurrentWeapons |= weaponPickup.Weapon;
                    this._players[weaponPickup.Player].CurrentStats.CurrentWeapons |= weaponPickup.Weapon;
                    break;
                case DemoDeltaType.Death:
                    {
                        Death death = (Death)packet;
                        this._players[death.Player].CurrentStats.Deaths++;

                        //check for new fragstreak max
                        uint curMax = this._players[death.Player].CurrentStats.MaxFragStreak;
                        this._players[death.Player].CurrentStats.MaxFragStreak = Math.Max(curMax, this._players[death.Player].CurrentStats.FragStreak);
                        this._players[death.Player].CurrentStats.FragStreak = 0;

                        //if the player had an RL, mark the match time they no longer have it
                        //and keep track of how long they had the RL
                        if ((this._players[death.Player].CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER)
                        {
                            double timeWithRL = this.MatchTime - this._players[death.Player].MatchTimeRLBegin;
                            this._players[death.Player].CurrentStats.TotalTimeWithRL += timeWithRL;
                            this._players[death.Player].MatchTimeRLEnd = this.MatchTime;
                        }
                        else //otherwise player did not have RL, so increment counter that keeps track of number of attempts
                        {
                            this._players[death.Player].CurrentStats.RLTakeAttempts++;
                        }

                        if (this._serverInfo.Deathmatch == 1)
                        {
                            Weapon activeWeapon = this._players[death.Player].ActiveWeapon;
                            if (activeWeapon == Weapon.IT_ROCKET_LAUNCHER || activeWeapon == Weapon.IT_LIGHTNING)
                            {
                                EntityType entityType = EntityType.None;
                                if (activeWeapon == Weapon.IT_ROCKET_LAUNCHER && this._players[death.Player].CurrentStats.CurrentValueRockets > 0)
                                {
                                    entityType = EntityType.RocketLauncher;
                                    this._players[death.Player].CurrentStats.ItemCountRLDropped++;
                                }
                                if (activeWeapon == Weapon.IT_LIGHTNING && this._players[death.Player].CurrentStats.CurrentValueCells > 0)
                                {
                                    entityType = EntityType.Lightning;
                                    this._players[death.Player].CurrentStats.ItemCountLGDropped++;
                                }
                                if (entityType == EntityType.RocketLauncher || entityType == EntityType.Lightning)
                                {
                                    Point location = this._players[death.Player].Location;
                                    Entity droppedPackEntity = new Entity();
                                    droppedPackEntity.Type = entityType;
                                    droppedPackEntity.IsPack = true;
                                    droppedPackEntity.OffsetX = location.X;
                                    droppedPackEntity.OffsetY = location.Y;
                                    droppedPackEntity.PackSourcePlayer = death.Player; //tag the pack with the player that dropped it
                                    this._droppedPacks.Add(droppedPackEntity);
                                    OnPackDropped(new PackEventArgs(droppedPackEntity));
                                }
                            }
                            else //otherwise player died without active powerful weapon, but check to see if they did have one (for stats)
                            {
                                if ((this._players[death.Player].CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER)
                                {
                                    this._players[death.Player].CurrentStats.ItemCountRLNoDrop++;
                                }
                                if ((this._players[death.Player].CurrentWeapons & Weapon.IT_LIGHTNING) == Weapon.IT_LIGHTNING)
                                {
                                    this._players[death.Player].CurrentStats.ItemCountLGNoDrop++;
                                }
                            }
                        }

                        this._players[death.Player].CurrentWeapons = Weapon.IT_SHOTGUN;
                        this._players[death.Player].ActiveWeapon = Weapon.IT_SHOTGUN;
                        this._players[death.Player].CurrentStats.CurrentWeapons = Weapon.IT_SHOTGUN;
                        this._players[death.Player].CurrentStats.ActiveWeapon = Weapon.IT_SHOTGUN;
                        this._players[death.Player].HasEnemyPack = false; //player no longer has a pack that someone else dropped
                    }
                    break;
                case DemoDeltaType.PowerupPickup:
                    PowerupPickup powerupPickup = (PowerupPickup)packet;
                    if (powerupPickup.IsExpired == false)
                    {
                        this._players[powerupPickup.Player].Powerup |= powerupPickup.Powerup;
                        this._players[powerupPickup.Player].CurrentStats.CurrentPowerups |= powerupPickup.Powerup;

                        Entity entity = null;
                        if (powerupPickup.Powerup == Powerup.IT_QUAD)
                        {
                            entity = GetEntity(EntityType.Quad);
                            this._players[powerupPickup.Player].CurrentStats.ItemCountQuad++;

                            //make sure streak starts at 0
                            this._players[powerupPickup.Player].CurrentStats.QuadStreak = 0;
                        }
                        if (powerupPickup.Powerup == Powerup.IT_INVISIBILITY)
                        {
                            entity = GetEntity(EntityType.Eyes);
                            this._players[powerupPickup.Player].CurrentStats.ItemCountEyes++;
                        }
                        if (powerupPickup.Powerup == Powerup.IT_INVULNERABILITY)
                        {
                            entity = GetEntity(EntityType.Pent);
                            this._players[powerupPickup.Player].CurrentStats.ItemCountPent++;
                        }
                        entity.BeginPickup(this.MatchTime);
                    }
                    else //otherwise the powerup expired
                    {
                        //if player no longer has quad, check for max quadstreak
                        if (powerupPickup.Powerup == Powerup.IT_QUAD)
                        {
                            uint curMax = this._players[powerupPickup.Player].CurrentStats.MaxQuadStreak;
                            this._players[powerupPickup.Player].CurrentStats.MaxQuadStreak = Math.Max(curMax, this._players[powerupPickup.Player].CurrentStats.QuadStreak);
                        }
                        this._players[powerupPickup.Player].Powerup &= ~powerupPickup.Powerup;
                        this._players[powerupPickup.Player].CurrentStats.CurrentPowerups &= ~powerupPickup.Powerup;
                    }
                    break;
                case DemoDeltaType.StatChanged:
                    StatChanged statChanged = (StatChanged)packet;
                    switch (statChanged.Stat)
                    {
                        case Stat.STAT_ROCKETS:
                            if (statChanged.DeltaValue > 0)
                            {
                                Entity rocketsEntity = GetClosestEntityRocketsOrRL(this._players[statChanged.Player].Location);

                                if (rocketsEntity != null)
                                {
                                    if (rocketsEntity.IsPack == true)
                                    {
                                        //remove pack
                                        OnPackPickup(new PackEventArgs(rocketsEntity));
                                        this._droppedPacks.Remove(rocketsEntity);
                                    }
                                    else
                                    {
                                        rocketsEntity.BeginPickup(this.MatchTime);
                                    }
                                }
                            }
                            this._players[statChanged.Player].CurrentStats.CurrentValueRockets += statChanged.DeltaValue;
                            break;
                        case Stat.STAT_CELLS:
                            this._players[statChanged.Player].CurrentStats.CurrentValueCells += statChanged.DeltaValue;
                            break;
                        default:
                            break;
                    }
                    break;
                case DemoDeltaType.PlayerEndGameStats:
                    PlayerEndGameStats playerEndGameStats = (PlayerEndGameStats)packet;
                    this._players[playerEndGameStats.Player].CurrentStats.DamageGiven = playerEndGameStats.DamageGiven;
                    this._players[playerEndGameStats.Player].CurrentStats.DamageTeam = playerEndGameStats.DamageTeam;

                    this._players[playerEndGameStats.Player].CurrentStats.LGPercent = playerEndGameStats.LGPercent;
                    this._players[playerEndGameStats.Player].CurrentStats.RLPercent = playerEndGameStats.RLPercent;
                    this._players[playerEndGameStats.Player].CurrentStats.GLPercent = playerEndGameStats.GLPercent;
                    this._players[playerEndGameStats.Player].CurrentStats.SGPercent = playerEndGameStats.SGPercent;
                    this._players[playerEndGameStats.Player].CurrentStats.SSGPercent = playerEndGameStats.SSGPercent;
                    this._players[playerEndGameStats.Player].CurrentStats.RLAvgDamage = playerEndGameStats.RLAvgDamage;
                    this._players[playerEndGameStats.Player].CurrentStats.RLDirectHits = playerEndGameStats.RLDirectHits;

                    break;
                case DemoDeltaType.Suicide:
                    Suicide suicide = (Suicide)packet;
                    this._players[suicide.Player].CurrentStats.Suicides++;
                    break;
                case DemoDeltaType.Kill:
                    Kill kill = (Kill)packet;
                    Player fraggerPlayer = this._players[kill.Fragger];
                    Player deathPlayer = this._players[kill.Death];

                    if (fraggerPlayer.CurrentStats.TeamIndex == deathPlayer.CurrentStats.TeamIndex)
                    {
                        fraggerPlayer.CurrentStats.KillsTeam++;
                        deathPlayer.CurrentStats.DeathsTeam++;
                    }
                    else //otherwise this was an opponent kill
                    {
                        fraggerPlayer.CurrentStats.KillsEnemy++;

                        //keep track of frag streak
                        fraggerPlayer.CurrentStats.FragStreak++;

                        //if the player that died was alive for less than 4 seconds, consider it a spawn frag
                        double durationAlive = this.MatchTime - deathPlayer.SpawnTime;
                        if (durationAlive < 3d)
                        {
                            fraggerPlayer.CurrentStats.KillsSpawn++;
                            deathPlayer.CurrentStats.DeathsSpawns++;
                        }

                        //keep track of kills with quad
                        if ((fraggerPlayer.CurrentStats.CurrentPowerups & Powerup.IT_QUAD) == Powerup.IT_QUAD)
                        {
                            fraggerPlayer.CurrentStats.QuadStreak++;
                            fraggerPlayer.CurrentStats.KillsWithQuad++;
                        }

                        switch (kill.Weapon)
                        {
                            case Weapon.IT_GRENADE_LAUNCHER:
                                fraggerPlayer.CurrentStats.KillsGL++;
                                break;
                            case Weapon.IT_ROCKET_LAUNCHER:
                                fraggerPlayer.CurrentStats.KillsRL++;
                                break;
                            case Weapon.IT_LIGHTNING:
                                fraggerPlayer.CurrentStats.KillsLG++;
                                break;
                            case Weapon.IT_NAILGUN:
                                fraggerPlayer.CurrentStats.KillsNG++;
                                break;
                            case Weapon.IT_SHOTGUN:
                                fraggerPlayer.CurrentStats.KillsSG++;
                                break;
                            default:
                                break;
                        }

                        //number of enemy kills while fragger is holding RL (to eventually calculate avg # kills per rl taken)
                        if ((fraggerPlayer.CurrentStats.CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER)
                        {
                            fraggerPlayer.CurrentStats.KillsWithRL++;
                        }
                        if ((deathPlayer.CurrentStats.CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER)
                        {
                            fraggerPlayer.CurrentStats.KilledRL++;
                        }
                        if ((deathPlayer.CurrentStats.CurrentWeapons & Weapon.IT_LIGHTNING) == Weapon.IT_LIGHTNING)
                        {
                            fraggerPlayer.CurrentStats.KilledLG++;
                        }
                        if ((deathPlayer.CurrentStats.CurrentPowerups & Powerup.IT_QUAD) == Powerup.IT_QUAD)
                        {
                            fraggerPlayer.CurrentStats.KilledQuad++;
                        }
                        //if the fragger did the frag due to previously picking up a pack
                        if (fraggerPlayer.HasEnemyPack == true)
                        {
                            //increment the number of kills due to picking up an enemy pack
                            fraggerPlayer.CurrentStats.KillsFromEnemyPack++;

                            //increment the 'opponent frag count' for the player that dropped the pack
                            this._players[fraggerPlayer.PackSourcePlayer].CurrentStats.TeamDeathsFromDroppedPack++;
                        }

                        if (((fraggerPlayer.CurrentStats.CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER
                            || (fraggerPlayer.CurrentStats.CurrentWeapons & Weapon.IT_LIGHTNING) == Weapon.IT_LIGHTNING))
                        {
                            //if RL/LG vs RL/LG
                            if (((deathPlayer.CurrentStats.CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER
                                || (deathPlayer.CurrentStats.CurrentWeapons & Weapon.IT_LIGHTNING) == Weapon.IT_LIGHTNING))
                            {
                                fraggerPlayer.CurrentStats.RLvsRLWon++;
                                deathPlayer.CurrentStats.RLvsRLLost++;
                            }
                            else //otherwise deathplayer does not have RL/LG
                            {
                                fraggerPlayer.CurrentStats.RLvsXWon++;
                                deathPlayer.CurrentStats.XvsRLLost++;
                            }
                        }
                        else //otherwise fragger does not have RL/LG
                        {
                            //if SG vs RL/LG
                            if (((deathPlayer.CurrentStats.CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER
                                || (deathPlayer.CurrentStats.CurrentWeapons & Weapon.IT_LIGHTNING) == Weapon.IT_LIGHTNING))
                            {
                                fraggerPlayer.CurrentStats.XvsRLWon++;
                                deathPlayer.CurrentStats.RLvsXLost++;
                            }
                            else //otherwise deathplayer does not have RL/LG
                            {
                                fraggerPlayer.CurrentStats.XvsXWon++;
                                deathPlayer.CurrentStats.XvsXLost++;
                            }
                        }


                    }
                    //for (int i = 0; i < this._kills.Length; ++i)
                    //{
                    //    if (this._kills[i].IsActive == false)
                    //    {
                    //        this._kills[i].FraggerName = this._players[kill.Fragger].Name;
                    //        this._kills[i].FraggerOffsetX = this._players[kill.Fragger].OffsetX;
                    //        this._kills[i].FraggerOffsetY = this._players[kill.Fragger].OffsetY;
                    //        this._kills[i].DeathName = this._players[kill.Death].Name;
                    //        this._kills[i].DeathOffsetX = this._players[kill.Death].OffsetX;
                    //        this._kills[i].DeathOffsetY = this._players[kill.Death].OffsetY;
                    //        this._kills[i].MatchTime = this.MatchTime;
                    //        this._kills[i].IsActive = true;
                    //    }
                    //}
                    break;
                default:
                    break;
            }
            if (packet.DemoTime > 0)
            {
                for (int i = 0; i < this._entities.Length; ++i)
                {
                    this._entities[i].UpdateTime(this.MatchTime);
                }

                //for (int i = 0; i < this._kills.Length; ++i)
                //{
                //    if (this._kills[i].IsActive == true && this.MatchTime - this._kills[i].MatchTime > 1d)
                //    {
                //        this._kills[i].IsActive = false;
                //    }
                //}
            }
        }

        private Entity GetEntity(EntityType type)
        {
            foreach (Entity entity in this._entities)
            {
                if (entity.Type == type)
                {
                    return entity;
                }
            }
            return null;
        }

        private Entity GetClosestEntityArmor(Point playerLocation)
        {
            double distance = Double.MaxValue;
            Entity closestEntity = null;
            foreach (Entity entity in this._entities)
            {
                Vector vector = playerLocation - entity.Location;
                if (vector.Length < distance && entity.IsArmor == true)
                {
                    closestEntity = entity;
                    distance = vector.Length;
                }
            }
            return closestEntity;
        }

        private Entity GetClosestEntityHealth(Point playerLocation)
        {
            double distance = Double.MaxValue;
            Entity closestEntity = null;
            foreach (Entity entity in this._entities)
            {
                Vector vector = playerLocation - entity.Location;
                if (vector.Length < distance && entity.IsHealth == true)
                {
                    closestEntity = entity;
                    distance = vector.Length;
                }
            }
            return closestEntity;
        }

        private Entity GetClosestEntityWeapon(Point playerLocation, Weapon weapon)
        {
            double distance = Double.MaxValue;
            Entity closestEntity = null;
            if (this._serverInfo.Deathmatch == 1)
            {
                foreach (Entity entity in this._droppedPacks)
                {
                    Vector vector = playerLocation - entity.Location;
                    if (vector.Length < distance)
                    {
                        if (weapon == Weapon.IT_LIGHTNING && entity.Type == EntityType.Lightning
                            || weapon == Weapon.IT_ROCKET_LAUNCHER && entity.Type == EntityType.RocketLauncher)
                        {
                            closestEntity = entity;
                            distance = vector.Length;
                        }
                    }
                }
            }
            foreach (Entity entity in this._entities)
            {
                Vector vector = playerLocation - entity.Location;
                if (vector.Length < distance)
                {
                    if (weapon == Weapon.IT_GRENADE_LAUNCHER && entity.Type == EntityType.GrenadeLauncher
                        || weapon == Weapon.IT_LIGHTNING && entity.Type == EntityType.Lightning
                        || weapon == Weapon.IT_ROCKET_LAUNCHER && entity.Type == EntityType.RocketLauncher
                        || weapon == Weapon.IT_SUPER_NAILGUN && entity.Type == EntityType.SuperNailgun
                        || weapon == Weapon.IT_SUPER_SHOTGUN && entity.Type == EntityType.SuperShotgun)
                    {
                        closestEntity = entity;
                        distance = vector.Length;
                    }
                }
            }
            //if the player was far away from the entity, something was wrong, so do not do anything with the entity
            if (distance > 100)
            {
                return null;
            }
            return closestEntity;
        }

        private Entity GetClosestEntityRocketsOrRL(Point playerLocation)
        {
            double distance = Double.MaxValue;
            Entity closestEntity = null;
            if (this._serverInfo.Deathmatch == 1)
            {
                foreach (Entity entity in this._droppedPacks)
                {
                    Vector vector = playerLocation - entity.Location;
                    if (vector.Length < distance)
                    {
                        closestEntity = entity;
                        distance = vector.Length;
                    }
                }
            }
            foreach (Entity entity in this._entities)
            {
                Vector vector = playerLocation - entity.Location;
                if (vector.Length < distance && (entity.Type == EntityType.Rockets10 || entity.Type == EntityType.Rockets5 || entity.Type == EntityType.RocketLauncher))
                {
                    closestEntity = entity;
                    distance = vector.Length;
                }
            }
            //if the player was far away from the entity, something was wrong, so do not do anything with the entity
            if (distance > 150)
            {
                return null;
            }
            return closestEntity;
        }

        public Player[] Players
        {
            get
            {
                return this._players;
            }
        }

        public MatchKill[] Kills
        {
            get
            {
                return this._kills;
            }
        }

        protected virtual void OnPackDropped(PackEventArgs e)
        {
            if (this.PackDropped != null)
            {
                this.PackDropped(this, e);
            }
        }

        protected virtual void OnPackPickup(PackEventArgs e)
        {
            if (this.PackPickup != null)
            {
                this.PackPickup(this, e);
            }
        }
    }
}
