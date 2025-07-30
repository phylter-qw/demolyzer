using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumBitDesigns.Mvvm;

namespace Demolyzer.Model
{
    public class ParticipantInfo
    {
        public ParticipantInfo()
        {
            //this.MatchPower = new uint[20 * 60];
            //this.MatchScore = new int[20 * 60];

            //default to true so that we must manually reset once player info is known
            this.IsSpectator = true; 
        }

        private MatchType _matchType;

        public MatchType MatchType
        {
            get
            {
                return this._matchType;
            }
            set
            {
                this._matchType = value;
                switch (value)
                {
                    case MatchType.Match1on1:
                        this.MatchPower = new uint[10 * 60];
                        this.MatchScore = new int[10 * 60];
                        break;
                    case MatchType.Match2on2:
                        this.MatchPower = new uint[10 * 60];
                        this.MatchScore = new int[10 * 60];
                        break;
                    case MatchType.Match4on4:
                        this.MatchPower = new uint[20 * 60];
                        this.MatchScore = new int[20 * 60];
                        break;
                    default:
                        break;
                }
            }
        }

        public static ParticipantInfo Sum(ParticipantInfo[] infos)
        {
            ParticipantInfo info = new ParticipantInfo();
            info.MatchType = infos[0].MatchType;
            info.ItemCountGA = (uint)infos.Sum(i => i.ItemCountGA);
            info.ItemCountYA = (uint)infos.Sum(i => i.ItemCountYA);
            info.ItemCountRA = (uint)infos.Sum(i => i.ItemCountRA);
            info.ItemCountMega = (uint)infos.Sum(i => i.ItemCountMega);
            info.ItemCountQuad = (uint)infos.Sum(i => i.ItemCountQuad);
            info.ItemCountPent = (uint)infos.Sum(i => i.ItemCountPent);
            info.ItemCountEyes = (uint)infos.Sum(i => i.ItemCountEyes);
            info.ItemCountSSG = (uint)infos.Sum(i => i.ItemCountSSG);
            info.ItemCountNG = (uint)infos.Sum(i => i.ItemCountNG);
            info.ItemCountSNG = (uint)infos.Sum(i => i.ItemCountSNG);
            info.ItemCountGL = (uint)infos.Sum(i => i.ItemCountGL);
            info.ItemCountRL = (uint)infos.Sum(i => i.ItemCountRL);
            info.ItemCountRLFromEnemyPack = (uint)infos.Sum(i => i.ItemCountRLFromEnemyPack);
            info.ItemCountRLFromTeamPack = (uint)infos.Sum(i => i.ItemCountRLFromTeamPack);
            info.ItemCountLG = (uint)infos.Sum(i => i.ItemCountLG);
            info.ItemCountLGFromEnemyPack = (uint)infos.Sum(i => i.ItemCountLGFromEnemyPack);
            info.ItemCountLGFromTeamPack = (uint)infos.Sum(i => i.ItemCountLGFromTeamPack);
            info.KillsEnemy = (uint)infos.Sum(i => i.KillsEnemy);
            info.KillsTeam = (uint)infos.Sum(i => i.KillsTeam);
            info.KilledLG = (uint)infos.Sum(i => i.KilledLG);
            info.KilledRL = (uint)infos.Sum(i => i.KilledRL);
            info.KilledQuad = (uint)infos.Sum(i => i.KilledQuad);
            info.Deaths = (uint)infos.Sum(i => i.Deaths);
            info.DeathsTeam = (uint)infos.Sum(i => i.DeathsTeam);
            info.Suicides = (uint)infos.Sum(i => i.Suicides);

            info.KillsFromEnemyPack = (uint)infos.Sum(i => i.KillsFromEnemyPack);
            info.TeamDeathsFromDroppedPack = (uint)infos.Sum(i => i.TeamDeathsFromDroppedPack);

            info.ItemCountLGNoDrop = (uint)infos.Sum(i => i.ItemCountLGNoDrop);
            info.ItemCountLGDropped = (uint)infos.Sum(i => i.ItemCountLGDropped);
            info.ItemCountLGLost = (uint)infos.Sum(i => i.ItemCountLGLost);
            info.ItemCountLGDonated = (uint)infos.Sum(i => i.ItemCountLGDonated);
            info.ItemCountRLNoDrop = (uint)infos.Sum(i => i.ItemCountRLNoDrop);
            info.ItemCountRLDropped = (uint)infos.Sum(i => i.ItemCountRLDropped);
            info.ItemCountRLLost = (uint)infos.Sum(i => i.ItemCountRLLost);
            info.ItemCountRLDonated = (uint)infos.Sum(i => i.ItemCountRLDonated);

            //these cannot be 'summed' as it doesnt make sense. they need to be averaged when combining for teamstats
            info.LGPercent = infos.Sum(i => i.LGPercent) / infos.Length;
            info.RLPercent = infos.Sum(i => i.RLPercent) / infos.Length;
            info.GLPercent = infos.Sum(i => i.GLPercent) / infos.Length;
            info.SGPercent = infos.Sum(i => i.SGPercent) / infos.Length;
            info.SSGPercent = infos.Sum(i => i.RLAvgDamage) / infos.Length;
            info.RLAvgDamage = infos.Sum(i => i.RLAvgDamage) / infos.Length;
            info.RLDirectHits = (uint)infos.Sum(i => i.RLDirectHits);

            info.DamageGiven = (uint)infos.Sum(i => i.DamageGiven);
            info.DamageTeam = (uint)infos.Sum(i => i.DamageTeam);
            info.DamageArmor = (uint)infos.Sum(i => i.DamageArmor);
            info.DamageHealth = (uint)infos.Sum(i => i.DamageHealth);
            info.HealthAcquired = (uint)infos.Sum(i => i.HealthAcquired);
            info.ArmorAcquired = (uint)infos.Sum(i => i.ArmorAcquired);

            info.TotalDistanceRan = (double)infos.Sum(i => i.TotalDistanceRan);
            info.TotalRunningDuration = (double)infos.Sum(i => i.TotalRunningDuration);
            info.TotalTimeToGetRL = (double)infos.Sum(i => i.TotalTimeToGetRL);
            info.RLTakeAttempts = (uint)infos.Sum(i => i.RLTakeAttempts);
            info.TotalTimeWithRL = (double)infos.Sum(i => i.TotalTimeWithRL);
            info.KillsWithRL = (uint)infos.Sum(i => i.KillsWithRL);
            info.KillsRL = (uint)infos.Sum(i => i.KillsRL);
            info.KillsLG = (uint)infos.Sum(i => i.KillsLG);
            info.KillsGL = (uint)infos.Sum(i => i.KillsGL);
            info.KillsNG = (uint)infos.Sum(i => i.KillsNG);
            info.KillsSG = (uint)infos.Sum(i => i.KillsSG);

            info.KillsSpawn = (uint)infos.Sum(i => i.KillsSpawn);
            info.DeathsSpawns = (uint)infos.Sum(i => i.DeathsSpawns);
            info.FragStreak = (uint)infos.Max(i => i.FragStreak);
            info.MaxFragStreak = (uint)infos.Max(i => i.MaxFragStreak);
            info.QuadStreak = (uint)infos.Max(i => i.QuadStreak);
            info.MaxQuadStreak = (uint)infos.Max(i => i.MaxQuadStreak);
            info.KillsWithQuad = (uint)infos.Sum(i => i.KillsWithQuad);

            info.TotalPing = (uint)infos.Sum(i => i.TotalPing);
            info.TotalPingCount = (uint)infos.Sum(i => i.TotalPingCount);
            info.TotalPL = (uint)infos.Sum(i => i.TotalPL);
            info.TotalPLCount = (uint)infos.Sum(i => i.TotalPLCount);

            info.CurrentScore = (int)infos.Sum(i => i.CurrentScore);

            info.RLvsRLWon = (uint)infos.Sum(i => i.RLvsRLWon);
            info.RLvsRLLost = (uint)infos.Sum(i => i.RLvsRLLost);
            info.RLvsXWon = (uint)infos.Sum(i => i.RLvsXWon);
            info.RLvsXLost = (uint)infos.Sum(i => i.RLvsXLost);
            info.XvsRLWon = (uint)infos.Sum(i => i.XvsRLWon);
            info.XvsRLLost = (uint)infos.Sum(i => i.XvsRLLost);
            info.XvsXWon = (uint)infos.Sum(i => i.XvsXWon);
            info.XvsXLost = (uint)infos.Sum(i => i.XvsXLost);

            info.Name = infos[0].Name;
            info.Team = infos[0].Team;
            info.TeamIndex = infos[0].TeamIndex;

            for (int x = 0; x < infos.Length; ++x )
            {
                int len = infos[x].MatchPower.Length;
                for (int i = 0; i < len; ++i)
                {
                    info.MatchPower[i] += infos[x].MatchPower[i];
                    //info.MatchScore[i] += infos[x].MatchScore[i];
                }
            }

            return info;
        }

        public static ParticipantInfo Average(ParticipantInfo[] infos)
        {
            ParticipantInfo info = Sum(infos);

            uint len = (uint)infos.Length;
            info.ItemCountGA /= len;
            info.ItemCountYA /= len;
            info.ItemCountRA /= len;
            info.ItemCountMega /= len;
            info.ItemCountQuad /= len;
            info.ItemCountPent /= len;
            info.ItemCountEyes /= len;
            info.ItemCountSSG /= len;
            info.ItemCountNG /= len;
            info.ItemCountSNG /= len;
            info.ItemCountGL /= len;
            info.ItemCountRL /= len;
            info.ItemCountRLFromEnemyPack /= len;
            info.ItemCountRLFromTeamPack /= len;
            info.ItemCountLG /= len;
            info.ItemCountLGFromEnemyPack /= len;
            info.ItemCountLGFromTeamPack /= len;
            info.KillsEnemy /= len;
            info.KillsTeam /= len;
            info.KilledLG /= len;
            info.KilledRL /= len;
            info.KilledQuad /= len;

            info.KillsSG /= len;
            info.KillsNG /= len;
            info.KillsGL /= len;
            info.KillsRL /= len;
            info.KillsLG /= len;

            //these are already averaged during the Sum() method
            //info.LGPercent /= len;
            //info.RLPercent /= len;
            //info.GLPercent /= len;
            //info.SGPercent /= len;
            //info.SSGPercent /= len;
            //info.RLAvgDamage /= len;

            info.RLDirectHits /= len;

            info.Deaths /= len;
            info.DeathsTeam /= len;
            info.Suicides /= len;
            info.KillsFromEnemyPack /= len;
            info.TeamDeathsFromDroppedPack /= len;
            info.ItemCountLGNoDrop /= len;
            info.ItemCountLGDropped /= len;
            info.ItemCountLGLost /= len;
            info.ItemCountLGDonated /= len;
            info.ItemCountRLNoDrop /= len;
            info.ItemCountRLDropped /= len;
            info.ItemCountRLLost /= len;
            info.ItemCountRLDonated /= len;
            info.DamageGiven /= len;
            info.DamageTeam /= len;
            info.DamageArmor /= len;
            info.DamageHealth /= len;
            info.HealthAcquired /= len;
            info.ArmorAcquired /= len;

            info.TotalDistanceRan /= len;
            info.TotalRunningDuration /= len;
            info.TotalTimeToGetRL /= len;
            info.RLTakeAttempts /= len;
            info.TotalTimeWithRL /= len;
            info.KillsWithRL /= len;
            info.KillsSpawn /= len;
            info.DeathsSpawns /= len;
            //info.FragStreak /= len;
            //info.MaxFragStreak = (uint)infos.Max(i => i.MaxFragStreak);
            //info.QuadStreak /= len;
            //info.MaxQuadStreak = (uint)infos.Max(i => i.MaxQuadStreak);
            info.KillsWithQuad /= len;
            info.TotalPing /= len;
            info.TotalPingCount /= len;
            info.TotalPL /= len;
            info.TotalPLCount /= len;

            info.CurrentScore /= (int)len;

            //already done during Sum()
            //info.Name = infos[0].Team;
            //info.TeamIndex = infos[0].TeamIndex;

            for (int x = 0; x < infos.Length; ++x)
            {
                int arrlen = infos[x].MatchPower.Length;
                for (int i = 0; i < arrlen; ++i)
                {
                    info.MatchPower[i] /= len;
                    //info.MatchScore[i] += infos[x].MatchScore[i];
                }
            }

            return info;
        }

        public double OffsetX;
        public double OffsetY;

        //public uint ShotsFiredRL;
        //public uint ShotsFiredGL;
        //public uint ShotsFiredLG;
        //public uint ShotsFiredNails;
        //public uint ShotsFiredShells;

        public uint ItemCountGA; //COMPLETE
        public uint ItemCountYA; //COMPLETE
        public uint ItemCountRA; //COMPLETE
        public uint ItemCountMega; //COMPLETE
        public uint ItemCountQuad; //COMPLETE
        public uint ItemCountPent; //COMPLETE
        public uint ItemCountEyes; //COMPLETE
        public uint ItemCountSSG; //COMPLETE
        public uint ItemCountNG; //COMPLETE
        public uint ItemCountSNG; //COMPLETE
        public uint ItemCountGL; //COMPLETE
        public uint ItemCountRL; //COMPLETE
        public uint ItemCountRLFromEnemyPack; //COMPLETE
        public uint ItemCountRLFromTeamPack; //COMPLETE
        public uint ItemCountLG; //COMPLETE
        public uint ItemCountLGFromEnemyPack; //COMPLETE
        public uint ItemCountLGFromTeamPack; //COMPLETE

        public uint KillsEnemy;  //COMPLETE
        public uint KillsTeam;  //COMPLETE
        public uint KilledLG; //COMPLETE
        public uint KilledRL; //COMPLETE
        public uint KilledQuad;  //COMPLETE
        public uint Deaths; //COMPLETE
        public uint DeathsTeam; //COMPLETE
        public uint Suicides; //COMPLETE

        public uint KillsFromEnemyPack;  //COMPLETE
        public uint TeamDeathsFromDroppedPack;  //COMPLETE

        //NOTE: Totals may not add up, since a dropped item may go to someone else who already has the item
        //In that case, the item is considered dropped, but it is neither lost to enemy nor donated to teammate
        public uint ItemCountLGNoDrop; //COMPLETE
        public uint ItemCountLGDropped; //COMPLETE
        public uint ItemCountLGLost; //COMPLETE
        public uint ItemCountLGDonated; //COMPLETE
        public uint ItemCountRLNoDrop; //COMPLETE
        public uint ItemCountRLDropped; //COMPLETE
        public uint ItemCountRLLost; //COMPLETE
        public uint ItemCountRLDonated; //COMPLETE

        public double DamageEfficiency
        {
            get
            {
                double denominator = this.DamageGiven + this.DamageTotal;
                if (denominator == 0)
                {
                    return 0d;
                }
                return 100d * this.DamageGiven / denominator;
            }
        }

        public double LGPercent;
        public double RLPercent;
        public double GLPercent;
        public double SGPercent;
        public double SSGPercent;
        public double RLAvgDamage;
        public uint RLDirectHits;

        public uint DamageGiven; //COMPLETE
        public uint DamageTeam; //COMPLETE
        public uint DamageArmor; //COMPLETE
        public uint DamageHealth; //COMPLETE
        public uint DamageTotal { get { return DamageArmor + DamageHealth; } } //COMPLETE
        public uint HealthAcquired; //COMPLETE
        public uint ArmorAcquired; //COMPLETE

        public Weapon ActiveWeapon;   //COMPLETE
        public Weapon CurrentWeapons;  //COMPLETE
        public Armor CurrentArmor;  //COMPLETE
        public Powerup CurrentPowerups; //COMPLETE
        public int CurrentValueShells;
        public int CurrentValueNails;
        public int CurrentValueRockets; //COMPLETE
        public int CurrentValueCells; //COMPLETE

        public uint CurrentValueArmor; //COMPLETE
        public uint CurrentValueHealth;  //COMPLETE
        public uint PreviousValueHealth; //used for finding maximum allowed health damage (ie. if player has 66 health, they can only take o maximum damage of 66 health)

        public double TotalDistanceRan;
        public double TotalRunningDuration;

        public double AverageSpeed
        {
            get
            {
                if (this.TotalRunningDuration == 0)
                {
                    return 0;
                }
                return TotalDistanceRan / TotalRunningDuration;
            }
        }

        public uint KillsWithoutRL
        {
            get
            {
                return this.KillsEnemy - this.KillsWithRL;
            }
        }

        public double TotalTimeToGetRL;
        public uint RLTakeAttempts;
        public double TotalTimeWithRL;
        public uint KillsWithRL;
        public uint KillsRL;
        public uint KillsLG;
        public uint KillsGL;
        public uint KillsNG;
        //public uint KillsSNG;
        //public uint KillsSSG;
        public uint KillsSG;

        public double AverageTimeToGetRL 
        {
            get 
            { 
                if (this.ItemCountRL == 0)
                {
                    return 0;
                }
                return TotalTimeToGetRL / (double)this.ItemCountRL;
            }
        }

        public double AverageLivesToGetRL
        {
            get
            {
                if (this.ItemCountRL == 0)
                {
                    return 0;
                }
                return (double)this.RLTakeAttempts / (double)this.ItemCountRL;
            }
        }

        public double AverageTimeWithRL
        {
            get
            {
                if (this.ItemCountRL == 0)
                {
                    return 0;
                }
                return this.TotalTimeWithRL / (double)this.ItemCountRL;
            }
        }

        public double PercentTimeWithRL
        {
            get
            {
                return TotalTimeWithRL / (this.TotalTimeWithRL + this.TotalTimeToGetRL);
            }
        }

        public double AverageKillsPerRLTaken
        {
            get
            {
                if (this.ItemCountRL == 0)
                {
                    return 0d;
                }
                return (double)this.KillsWithRL / (double)this.ItemCountRL;
            }
        }

        public double AverageFragStreak
        {
            get
            {
                return (double)KillsEnemy / ((double)Deaths + 1d);
            }
        }

        public uint KillsSpawn;
        public uint DeathsSpawns;

        public uint FragStreak;
        public uint MaxFragStreak;
        public uint QuadStreak;
        public uint MaxQuadStreak;

        public uint KillsWithQuad;

        public double AverageQuadStreak
        {
            get
            {
                if (this.ItemCountQuad == 0)
                {
                    return 0d;
                }
                return (double)KillsWithQuad / (double)this.ItemCountQuad;
            }
        }

        public uint[] MatchPower;
        public int[] MatchScore;

        public double AveragePower
        {
            get
            {
                uint sum = 0;
                for (int x = 0; x < this.MatchPower.Length; ++x)
                {
                    sum += this.MatchPower[x];
                }
                return sum / this.MatchPower.Length;
            }
        }

        public uint Ping;
        public uint TotalPing;
        public uint TotalPingCount;
        public uint PL;
        public uint TotalPL;
        public uint TotalPLCount;

        public double AveragePing
        {
            get
            {
                if (this.TotalPingCount == 0)
                {
                    return 0;
                }
                return (double)this.TotalPing / (double)this.TotalPingCount;
            }
        }

        public double AveragePL
        {
            get
            {
                if (this.TotalPLCount == 0)
                {
                    return 0;
                }
                return (double)this.TotalPL / (double)this.TotalPLCount;
            }
        }

        public bool IsSpectator;
        public string Name;
        public string Team;
        public int TeamIndex;

        public int CurrentScore;

        public double EfficiencyPercent
        {
            get
            {
                double denominator = this.CurrentScore + this.Deaths;
                if (denominator == 0)
                {
                    return 0d;
                }
                return 100d * this.CurrentScore / denominator;
            }
        }

        public double Rating
        {
            get
            {
                double rating = this.EfficiencyPercent 
                    + this.KilledRL 
                    + 4 * this.ItemCountQuad
                    + 4 * this.KilledQuad
                    - 2 * this.ItemCountRLLost
                    - 2 * this.KillsTeam
                    + 100d * this.PercentTimeWithRL;
                return rating;
            }
        }

                  //<DataGridTextColumn Header="TP_Rating" Binding="{Binding Path=TP_Rating}"/>
                  //<DataGridTextColumn Header="TP_KillRLLG" Binding="{Binding Path=TP_KillRLLG}"/>
                  //<DataGridTextColumn Header="TP_RA" Binding="{Binding Path=TP_RA}"/>
                  //<DataGridTextColumn Header="TP_YA" Binding="{Binding Path=TP_YA}"/>
                  //<DataGridTextColumn Header="TP_GA" Binding="{Binding Path=TP_GA}"/>
                  //<DataGridTextColumn Header="TP_Quad" Binding="{Binding Path=TP_Quad}"/>
                  //<DataGridTextColumn Header="TP_Pent" Binding="{Binding Path=TP_Pent}"/>
                  //<DataGridTextColumn Header="TP_Eyes" Binding="{Binding Path=TP_Eyes}"/>
                  //<DataGridTextColumn Header="TP_RLLGLost" Binding="{Binding Path=TP_RLLGLost}"/>
                  //<DataGridTextColumn Header="TP_RLLGDonated" Binding="{Binding Path=TP_RLLGDonated}"/>
                  //<DataGridTextColumn Header="TP_RLTime" Binding="{Binding Path=TP_RLTime}"/>


                    //teamplay += (uint)playerStats[x].AveragePower;
                    //teamplay += 20 * (playerStats[x].KilledRL + playerStats[x].KilledLG);
                    //teamplay += 100 * playerStats[x].KilledQuad;
                    //teamplay += 15 * playerStats[x].ItemCountRA;
                    //teamplay += 10 * playerStats[x].ItemCountYA;
                    //teamplay += 5 * playerStats[x].ItemCountGA;
                    //teamplay += 50 * playerStats[x].ItemCountQuad;
                    //teamplay += 200 * playerStats[x].ItemCountPent;
                    //teamplay += 50 * playerStats[x].ItemCountEyes;
                    //teamplay -= 50 * (playerStats[x].ItemCountRLLost + playerStats[x].ItemCountLGLost);
                    //teamplay += 50 * (playerStats[x].ItemCountRLDonated + playerStats[x].ItemCountLGDonated);
                    //teamplay += 40 * ((uint)playerStats[x].TotalTimeWithRL / 60);


        public int TP_Rating
        {
            get
            {
                return (int)(this.TP_AvgPower
                    //+ this.TP_KillRL
                    //+ this.TP_KillLG
                    + this.TP_KillQuad
                    + this.TP_Armors
                    + this.TP_Powerups
                    + this.TP_RLLGLost
                    + this.TP_RLLGDonated
                    + this.TP_RLTime
                    + this.TP_WeaponPerformance);
            }
        }

        public uint TP_AvgPower { get { return (uint)this.AveragePower; } }
        public int TP_KillRL { get { return 50 * ((int)this.KilledRL); } }
        public int TP_KillLG { get { return 50 * ((int)this.KilledLG); } }
        public uint TP_KillQuad { get { return 100 * this.KilledQuad; } }
        public uint TP_RA { get { return 30 * this.ItemCountRA; } }
        public uint TP_YA { get { return 20 * this.ItemCountYA; } }
        public uint TP_GA { get { return 10 * this.ItemCountGA; } }
        public uint TP_Quad { get { return 75 * this.ItemCountQuad; } }
        public uint TP_Pent { get { return 100 * this.ItemCountPent; } }
        public uint TP_Eyes { get { return 50 * this.ItemCountEyes; } }
        public int TP_RLLGLost { get { return -50 * ((int)this.ItemCountRLLost + (int)this.ItemCountLGLost); } }
        public uint TP_RLLGDonated { get { return 50 * (this.ItemCountRLDonated + this.ItemCountLGDonated); } }
        public uint TP_RLTime { get { return (40 * (uint)this.TotalTimeWithRL / 60); } }

        public uint TP_Armors { get { return this.TP_RA + this.TP_YA + this.TP_GA; } }
        public uint TP_Powerups { get { return this.TP_Quad + this.TP_Pent + this.TP_Eyes; } }
        public uint TP_RLDropPercent 
        { 
            get 
            {
                if (this.ItemCountRL == 0)
                {
                    return 100;
                }
                else
                {
                    return (uint)(100d * ((double)this.ItemCountRLDropped / (double)this.ItemCountRL));
                }
                //return 50 * this.ItemCountRLDropped; 
            } 
        }

        public float TP_WeaponPerformance
        {
            get
            {
                return RLvsXPercent + RLvsXPercent + XvsRLPercent + XvsXPercent;
            }
        }

        public uint RLvsRLWon;
        public uint RLvsRLLost;
        public uint RLvsXWon;
        public uint RLvsXLost;
        public uint XvsRLWon;
        public uint XvsRLLost;
        public uint XvsXWon;
        public uint XvsXLost;

        public float RLvsRLPercent
        {
            get
            {
                uint sum = RLvsRLWon + RLvsRLLost;
                if (sum == 0) return 0f;
                return 100f * (float)RLvsRLWon / (float)sum;
            }
        }
        public float RLvsXPercent
        {
            get
            {
                uint sum = RLvsXWon + RLvsXLost;
                if (sum == 0) return 0f;
                return 100f * (float)RLvsXWon / (float)sum;
            }
        }
        public float XvsRLPercent
        {
            get
            {
                uint sum = XvsRLWon + XvsRLLost;
                if (sum == 0) return 0f;
                return 100f * (float)XvsRLWon / (float)sum;
            }
        }
        public float XvsXPercent
        {
            get
            {
                uint sum = XvsXWon + XvsXLost;
                if (sum == 0) return 0f;
                return 100f * (float)XvsXWon / (float)sum;
            }
        }
        //public void UpdateMatchTimeline(double matchTime)
        //{
        //    int index = (int)matchTime;
        //    if (this.IsSpectator == false && index < this.MatchPower.Length)
        //    {
        //        uint multiplier = 1;
        //        if ((this.CurrentWeapons & Weapon.IT_ROCKET_LAUNCHER) == Weapon.IT_ROCKET_LAUNCHER
        //            || (this.CurrentWeapons & Weapon.IT_LIGHTNING) == Weapon.IT_LIGHTNING)
        //        {
        //            multiplier *= 2;
        //        }
        //        if ((this.CurrentPowerups & Powerup.IT_QUAD) == Powerup.IT_QUAD
        //            || (this.CurrentPowerups & Powerup.IT_INVULNERABILITY) == Powerup.IT_INVULNERABILITY)
        //        {
        //            multiplier *= 2;
        //        }
        //        this.MatchPower[index] = multiplier * (CurrentValueHealth + CurrentValueArmor);

        //        this.MatchScore[index] = CurrentScore;
        //    }
        //}
    }
}
