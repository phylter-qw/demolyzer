using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demolyzer.Model.Ratings;

namespace Demolyzer.Model
{
    public enum MatchType
    {
        Unknown,
        Match1on1,
        Match2on2,
        Match4on4,
    };

    public class MatchResults
    {
        public MatchType MatchType { get; set; }
        public ParticipantInfo[] PlayerStats { get; set; }
        public MatchResults(MatchType matchType, ParticipantInfo[] playerStats)
        {
            this.MatchType = matchType;
            this.PlayerStats = playerStats;
        }
    }
    //frags per minute (team/player)
    //damage per minute (team/player)
    //lead (team)
    public class DemoContent
    {
        private List<DemoDelta> _demoPackets;
        private ParticipantInfo[] _playerCache;
        private bool _isMatchStarted;
        private double _lastDemoTime;
        private double _matchTime;
        private List<Entity> _entities;
        private ParticipantInfo[] _playerStats;
        private ParticipantInfo[] _teamPower;

        public Player[] Players { get; private set; }

        public ServerInfo ServerInfo { get; private set; }
        public bool IsError { get; set; }
        public string MvdName { get; set; }
        public string MvdFullName { get; set; }
        public DateTime Date { get; set; }

        public DemoContent()
        {
            this._demoPackets = new List<DemoDelta>();
            this._playerCache = new ParticipantInfo[32];
            for (int i = 0; i < this._playerCache.Length; ++i)
            {
                this._playerCache[i] = new ParticipantInfo();
            }
            this.ServerInfo = new ServerInfo();
            this._entities = new List<Entity>();
        }

        public void PostProcess()
        {
            ReorderPackets();
            ProcessAllPackets();
        }

        private void ProcessAllPackets()
        {
            MatchProcessor processor = new MatchProcessor(this._entities.ToArray());
            for (int i = 0; i < this._demoPackets.Count; ++i)
            {
                processor.Process(this._demoPackets[i]);
            }
            processor.PostProcess();
            this._playerStats = processor.Players.Select(p => p.CurrentStats).ToArray();
            this._teamPower = processor.TeamPower;
        }

        //public MatchResults GetMatchResults()
        //{
        //    ParticipantInfo[] playerStats = this._playerStats.Where(p => p.IsSpectator == false).ToArray();
        //    MatchType matchType = MatchType.Unknown;
        //    switch(playerStats.Length)
        //    {
        //        case 2:
        //            matchType = MatchType.Match1on1;
        //            break;
        //        case 4:
        //            matchType = MatchType.Match2on2;
        //            break;
        //        case 8:
        //            matchType = MatchType.Match4on4;
        //            break;
        //        default:
        //            break;
        //    }
        //    return new MatchResults(matchType, playerStats);
        //}

        public static ParticipantInfo[] ConvertToTeams(ParticipantInfo[] playerStats)
        {
            ParticipantInfo[] teams = new ParticipantInfo[2];
            int playerCount = playerStats.Length / 2;

            string teamA = null;
            string teamB = null;

            List<ParticipantInfo> teamAStats = new List<ParticipantInfo>();
            List<ParticipantInfo> teamBStats = new List<ParticipantInfo>();

            for (int x = 0; x < playerStats.Length; ++x)
            {
                if (teamA == null)
                {
                    teamA = playerStats[x].Team;
                }
                else
                {
                    if (teamB == null && playerStats[x].Team != teamA)
                    {
                        teamB = playerStats[x].Team;
                    }
                }
                if (playerStats[x].Team == teamA)
                {
                    teamAStats.Add(playerStats[x]);
                }
                if (playerStats[x].Team == teamB)
                {
                    teamBStats.Add(playerStats[x]);
                }
            }
            teams[0] = ParticipantInfo.Sum(teamAStats.ToArray());
            teams[0].Name = teamA;
            teams[1] = ParticipantInfo.Sum(teamBStats.ToArray());
            teams[1].Name = teamB;
            return teams;
        }

        public MatchResult GetMatchResult()
        {
            ParticipantInfo[] playerStats = this._playerStats.Where(p => p.IsSpectator == false).ToArray();

            if (iELOUtil.IsProcessAsTeamsEnabled == true)
            {
                playerStats = ConvertToTeams(playerStats);
            }

            MatchResult result = new MatchResult();
            result.MvdFullName = this.MvdFullName;
            result.DateTime = this.Date;
            result.Map = this.ServerInfo.Map;

            int playerCount = playerStats.Length / 2;

            result.TeamAPercents = new float[playerCount];
            result.TeamAPlayerNames = new string[playerCount];
            result.TeamAScores = new int[playerCount];
            result.TeamBPercents = new float[playerCount];
            result.TeamBPlayerNames = new string[playerCount];
            result.TeamBScores = new int[playerCount];
            result.TeamADamagesGiven = new int[playerCount];
            result.TeamAEfficiencies = new int[playerCount];
            result.TeamBDamagesGiven = new int[playerCount];
            result.TeamBEfficiencies = new int[playerCount];
            result.TeamATeamplays = new int[playerCount];
            result.TeamBTeamplays = new int[playerCount];

            result.TeamAStats[iELOUtil.RLsKilled] = new int[playerCount];
            result.TeamBStats[iELOUtil.RLsKilled] = new int[playerCount];

            result.TeamAStats[iELOUtil.Armors] = new int[playerCount];
            result.TeamBStats[iELOUtil.Armors] = new int[playerCount];
            result.TeamAStats[iELOUtil.Powerups] = new int[playerCount];
            result.TeamBStats[iELOUtil.Powerups] = new int[playerCount];
            result.TeamAStats[iELOUtil.RLsDropped] = new int[playerCount];
            result.TeamBStats[iELOUtil.RLsDropped] = new int[playerCount];
            result.TeamAStats[iELOUtil.TimeWithRL] = new int[playerCount];
            result.TeamBStats[iELOUtil.TimeWithRL] = new int[playerCount];
            result.TeamAStats[iELOUtil.AveragePower] = new int[playerCount];
            result.TeamBStats[iELOUtil.AveragePower] = new int[playerCount];

            result.TeamAStats[iELOUtil.RLvsRL] = new int[playerCount];
            result.TeamBStats[iELOUtil.RLvsRL] = new int[playerCount];
            result.TeamAStats[iELOUtil.RLvsX] = new int[playerCount];
            result.TeamBStats[iELOUtil.RLvsX] = new int[playerCount];
            result.TeamAStats[iELOUtil.XvsRL] = new int[playerCount];
            result.TeamBStats[iELOUtil.XvsRL] = new int[playerCount];
            result.TeamAStats[iELOUtil.XvsX] = new int[playerCount];
            result.TeamBStats[iELOUtil.XvsX] = new int[playerCount];

            foreach (string statname in StatNamesAverages.All)
            {
                result.TeamAStatsAverages[statname] = new float[playerCount];
                result.TeamBStatsAverages[statname] = new float[playerCount];
            }
            foreach (string statname in StatNamesTotalPercents.All)
            {
                result.TeamAStatsTotalPercents[statname] = new Tuple<float, float>[playerCount];
                result.TeamBStatsTotalPercents[statname] = new Tuple<float, float>[playerCount];
            }


            //if 1on1
            if (playerCount == 1)
            {
                for (int x = 0; x < playerStats.Length; ++x)
                {
                    playerStats[x].Team = playerStats[x].Name;
                }
            }

            int teamAIndex = 0;
            int teamBIndex = 0;
            for (int x = 0; x < playerStats.Length; ++x)
            {
                if (result.TeamA == null)
                {
                    result.TeamA = playerStats[x].Team;
                }
                else
                {
                    if (result.TeamB == null && playerStats[x].Team != result.TeamA)
                    {
                        result.TeamB = playerStats[x].Team;
                    }
                }
                if (playerStats[x].Team == result.TeamA)
                {
                    result.TeamAPlayerNames[teamAIndex] = playerStats[x].Name;
                    result.TeamAScores[teamAIndex] = (short)playerStats[x].CurrentScore;
                    result.TeamADamagesGiven[teamAIndex] = (short)playerStats[x].DamageGiven;
                    result.TeamAEfficiencies[teamAIndex] = (short)playerStats[x].EfficiencyPercent;
                    result.TeamATeamplays[teamAIndex] = playerStats[x].TP_Rating;
                    result.TeamAStats[iELOUtil.RLsKilled][teamAIndex] = playerStats[x].TP_KillRL;
                    result.TeamAStats[iELOUtil.Armors][teamAIndex] = (int)playerStats[x].TP_Armors;
                    result.TeamAStats[iELOUtil.Powerups][teamAIndex] = (int)playerStats[x].TP_Powerups;
                    result.TeamAStats[iELOUtil.RLsDropped][teamAIndex] = (int)playerStats[x].TP_RLDropPercent;
                    result.TeamAStats[iELOUtil.TimeWithRL][teamAIndex] = (int)playerStats[x].TP_RLTime;
                    result.TeamAStats[iELOUtil.AveragePower][teamAIndex] = (int)playerStats[x].TP_AvgPower;
                    result.TeamAStats[iELOUtil.RLvsRL][teamAIndex] = (int)playerStats[x].RLvsRLPercent;
                    result.TeamAStats[iELOUtil.RLvsX][teamAIndex] = (int)playerStats[x].RLvsXPercent;
                    result.TeamAStats[iELOUtil.XvsRL][teamAIndex] = (int)playerStats[x].XvsRLPercent;
                    result.TeamAStats[iELOUtil.XvsX][teamAIndex] = (int)playerStats[x].XvsXPercent;
                    result.TeamAStatsAverages[StatNamesAverages.FPM][teamAIndex] = (float)playerStats[x].KillsEnemy / 20f;
                    result.TeamAStatsAverages[StatNamesAverages.FPMwithRL][teamAIndex] = (float)playerStats[x].KillsWithRL / ((float)playerStats[x].TotalTimeWithRL / 60f);
                    result.TeamAStatsAverages[StatNamesAverages.FPMwithNoRL][teamAIndex] = (float)playerStats[x].KillsWithoutRL / ((1200 - (float)playerStats[x].TotalTimeWithRL) / 60f);
                    result.TeamAStatsAverages[StatNamesAverages.DPM][teamAIndex] = (float)playerStats[x].Deaths / 20f;
                    result.TeamAStatsAverages[StatNamesAverages.HighFragStreak][teamAIndex] = (float)playerStats[x].MaxFragStreak;
                    result.TeamAStatsAverages[StatNamesAverages.TeamDamage][teamAIndex] = (float)playerStats[x].DamageTeam;
                    result.TeamAStatsAverages[StatNamesAverages.KillSpawns][teamAIndex] = (float)playerStats[x].KillsSpawn;
                    result.TeamAStatsAverages[StatNamesAverages.KillRL][teamAIndex] = (float)playerStats[x].KilledRL;
                    result.TeamAStatsAverages[StatNamesAverages.KillQuad][teamAIndex] = (float)playerStats[x].KilledQuad;
                    result.TeamAStatsAverages[StatNamesAverages.KillTeam][teamAIndex] = (float)playerStats[x].KillsTeam;
                    result.TeamAStatsAverages[StatNamesAverages.Quads][teamAIndex] = (float)playerStats[x].ItemCountQuad;

                    result.TeamAStatsTotalPercents[StatNamesTotalPercents.RLDrop][teamAIndex] = new Tuple<float, float>(playerStats[x].ItemCountRLDropped, playerStats[x].ItemCountRL);
                    result.TeamAStatsTotalPercents[StatNamesTotalPercents.KillsPerRL][teamAIndex] = new Tuple<float, float>(playerStats[x].KillsWithRL, playerStats[x].ItemCountRL);
                    result.TeamAStatsTotalPercents[StatNamesTotalPercents.LivesToGetRL][teamAIndex] = new Tuple<float, float>(playerStats[x].RLTakeAttempts, playerStats[x].ItemCountRL);
                    result.TeamAStatsTotalPercents[StatNamesTotalPercents.TimePerRL][teamAIndex] = new Tuple<float, float>((float)playerStats[x].TotalTimeWithRL, playerStats[x].ItemCountRL);
                    result.TeamAStatsTotalPercents[StatNamesTotalPercents.TimeToGetRL][teamAIndex] = new Tuple<float, float>((float)playerStats[x].TotalTimeToGetRL, playerStats[x].ItemCountRL);
                    result.TeamAStatsTotalPercents[StatNamesTotalPercents.TimeWithRL][teamAIndex] = new Tuple<float, float>((float)playerStats[x].TotalTimeWithRL, 20 * 60);
                    result.TeamAStatsTotalPercents[StatNamesTotalPercents.RLvsRL][teamAIndex] = new Tuple<float, float>(playerStats[x].RLvsRLWon, playerStats[x].RLvsRLWon + playerStats[x].RLvsRLLost);
                    result.TeamAStatsTotalPercents[StatNamesTotalPercents.RLvsX][teamAIndex] = new Tuple<float, float>(playerStats[x].RLvsXWon, playerStats[x].RLvsXWon + playerStats[x].RLvsXLost);
                    result.TeamAStatsTotalPercents[StatNamesTotalPercents.XvsRL][teamAIndex] = new Tuple<float, float>(playerStats[x].XvsRLWon, playerStats[x].XvsRLWon + playerStats[x].XvsRLLost);
                    result.TeamAStatsTotalPercents[StatNamesTotalPercents.XvsX][teamAIndex] = new Tuple<float, float>(playerStats[x].XvsXWon, playerStats[x].XvsXWon + playerStats[x].XvsXLost);

                    teamAIndex++;
                }
                if (playerStats[x].Team == result.TeamB)
                {
                    result.TeamBPlayerNames[teamBIndex] = playerStats[x].Name;
                    result.TeamBScores[teamBIndex] = (short)playerStats[x].CurrentScore;
                    result.TeamBDamagesGiven[teamBIndex] = (short)playerStats[x].DamageGiven;
                    result.TeamBEfficiencies[teamBIndex] = (short)playerStats[x].EfficiencyPercent;
                    result.TeamBTeamplays[teamBIndex] = playerStats[x].TP_Rating;
                    result.TeamBStats[iELOUtil.RLsKilled][teamBIndex] = playerStats[x].TP_KillRL;
                    result.TeamBStats[iELOUtil.Armors][teamBIndex] = (int)playerStats[x].TP_Armors;
                    result.TeamBStats[iELOUtil.Powerups][teamBIndex] = (int)playerStats[x].TP_Powerups;
                    result.TeamBStats[iELOUtil.RLsDropped][teamBIndex] = (int)playerStats[x].TP_RLDropPercent;
                    result.TeamBStats[iELOUtil.TimeWithRL][teamBIndex] = (int)playerStats[x].TP_RLTime;
                    result.TeamBStats[iELOUtil.AveragePower][teamBIndex] = (int)playerStats[x].TP_AvgPower;
                    result.TeamBStats[iELOUtil.RLvsRL][teamBIndex] = (int)playerStats[x].RLvsRLPercent;
                    result.TeamBStats[iELOUtil.RLvsX][teamBIndex] = (int)playerStats[x].RLvsXPercent;
                    result.TeamBStats[iELOUtil.XvsRL][teamBIndex] = (int)playerStats[x].XvsRLPercent;
                    result.TeamBStats[iELOUtil.XvsX][teamBIndex] = (int)playerStats[x].XvsXPercent;
                    result.TeamBStatsAverages[StatNamesAverages.FPM][teamBIndex] = (float)playerStats[x].KillsEnemy / 20f;
                    result.TeamBStatsAverages[StatNamesAverages.FPMwithRL][teamBIndex] = (float)playerStats[x].KillsWithRL / ((float)playerStats[x].TotalTimeWithRL / 60f);
                    result.TeamBStatsAverages[StatNamesAverages.FPMwithNoRL][teamBIndex] = (float)playerStats[x].KillsWithoutRL / ((1200 - (float)playerStats[x].TotalTimeWithRL) / 60f);
                    result.TeamBStatsAverages[StatNamesAverages.DPM][teamBIndex] = (float)playerStats[x].Deaths / 20f;
                    result.TeamBStatsAverages[StatNamesAverages.HighFragStreak][teamBIndex] = (float)playerStats[x].MaxFragStreak;
                    result.TeamBStatsAverages[StatNamesAverages.TeamDamage][teamBIndex] = (float)playerStats[x].DamageTeam;
                    result.TeamBStatsAverages[StatNamesAverages.KillSpawns][teamBIndex] = (float)playerStats[x].KillsSpawn;
                    result.TeamBStatsAverages[StatNamesAverages.KillRL][teamBIndex] = (float)playerStats[x].KilledRL;
                    result.TeamBStatsAverages[StatNamesAverages.KillQuad][teamBIndex] = (float)playerStats[x].KilledQuad;
                    result.TeamBStatsAverages[StatNamesAverages.KillTeam][teamBIndex] = (float)playerStats[x].KillsTeam;
                    result.TeamBStatsAverages[StatNamesAverages.Quads][teamBIndex] = (float)playerStats[x].ItemCountQuad;

                    result.TeamBStatsTotalPercents[StatNamesTotalPercents.RLDrop][teamBIndex] = new Tuple<float, float>(playerStats[x].ItemCountRLDropped, playerStats[x].ItemCountRL);
                    result.TeamBStatsTotalPercents[StatNamesTotalPercents.KillsPerRL][teamBIndex] = new Tuple<float, float>(playerStats[x].KillsWithRL, playerStats[x].ItemCountRL);
                    result.TeamBStatsTotalPercents[StatNamesTotalPercents.LivesToGetRL][teamBIndex] = new Tuple<float, float>(playerStats[x].RLTakeAttempts, playerStats[x].ItemCountRL);
                    result.TeamBStatsTotalPercents[StatNamesTotalPercents.TimePerRL][teamBIndex] = new Tuple<float, float>((float)playerStats[x].TotalTimeWithRL, playerStats[x].ItemCountRL);
                    result.TeamBStatsTotalPercents[StatNamesTotalPercents.TimeToGetRL][teamBIndex] = new Tuple<float, float>((float)playerStats[x].TotalTimeToGetRL, playerStats[x].ItemCountRL);
                    result.TeamBStatsTotalPercents[StatNamesTotalPercents.TimeWithRL][teamBIndex] = new Tuple<float, float>((float)playerStats[x].TotalTimeWithRL, 20 * 60);
                    result.TeamBStatsTotalPercents[StatNamesTotalPercents.RLvsRL][teamBIndex] = new Tuple<float, float>(playerStats[x].RLvsRLWon, playerStats[x].RLvsRLWon + playerStats[x].RLvsRLLost);
                    result.TeamBStatsTotalPercents[StatNamesTotalPercents.RLvsX][teamBIndex] = new Tuple<float, float>(playerStats[x].RLvsXWon, playerStats[x].RLvsXWon + playerStats[x].RLvsXLost);
                    result.TeamBStatsTotalPercents[StatNamesTotalPercents.XvsRL][teamBIndex] = new Tuple<float, float>(playerStats[x].XvsRLWon, playerStats[x].XvsRLWon + playerStats[x].XvsRLLost);
                    result.TeamBStatsTotalPercents[StatNamesTotalPercents.XvsX][teamBIndex] = new Tuple<float, float>(playerStats[x].XvsXWon, playerStats[x].XvsXWon + playerStats[x].XvsXLost);

                    teamBIndex++;
                }
            }

            //result.TeamAScores[0] = 100;
            //result.TeamAScores[1] = 50;
            //result.TeamAScores[2] = 50;
            //result.TeamAScores[3] = 50;
            //result.TeamBScores[0] = 10;
            //result.TeamBScores[1] = 10;
            //result.TeamBScores[2] = 10;
            //result.TeamBScores[3] = 10;

            float totalFrags = 0;
            for (int i = 0; i < playerCount; ++i)
            {
                totalFrags += Math.Max(0, (float)result.TeamAScores[i]);
                totalFrags += Math.Max(0, (float)result.TeamBScores[i]);

                result.TeamAScore += result.TeamAScores[i];
                result.TeamBScore += result.TeamBScores[i];
            }

            if ((playerCount == 4 && totalFrags < 50) || (playerCount == 2 && totalFrags < 20) || (playerCount == 1 && totalFrags <= 0))
            {
                return null;
            }

            for (int i = 0; i < playerCount; ++i)
            {
                result.TeamAPercents[i] = (float)result.TeamAScores[i] / totalFrags;
                result.TeamBPercents[i] = (float)result.TeamBScores[i] / totalFrags;
            }

            result.TeamAPercent = (float)result.TeamAScore / totalFrags;
            result.TeamBPercent = (float)result.TeamBScore / totalFrags;

            AdjustPlayerNames(result.TeamAPlayerNames);
            AdjustPlayerNames(result.TeamBPlayerNames);

            return result;
        }

        private void AdjustPlayerNames(string[] names)
        {
            for (int x = 0; x < names.Length; ++x)
            {
                string name = names[x];
                if (name.Contains("_ ") == true)
                {
                    name = name.Replace("_ ", "");
                }
                if (name.Contains("6. ") == true)
                {
                    name = name.Replace("6. ", "");
                }
                name = name.ToLower();
                names[x] = name;
            }
        }

        public ParticipantInfo[] FinalPlayerStats
        {
            get
            {
                return this._playerStats;
            }
        }

        public ParticipantInfo[] TeamPower
        {
            get
            {
                return this._teamPower;
            }
        }

        private void ReorderPackets()
        {
            PacketComparer packetComparer = new PacketComparer();
            int index = 0;
            while (this._demoPackets[index].Type != DemoDeltaType.MatchStarted)
            {
                index++;
            }
            index++;
            while (index < this._demoPackets.Count - 1)
            {
                int count = 1;
                while (index + count < this._demoPackets.Count && this._demoPackets[index + count].DemoTime == 0)
                {
                    count++;
                }
                if (count > 1)
                {
                    this._demoPackets.Sort(index, count, packetComparer);
                    index += count;
                }
                else
                {
                    index++;
                }
            }
        }

        public ParticipantInfo[] PlayerInfos
        {
            get
            {
                return this._playerCache;
            }
        }

        public List<Entity> Entities
        {
            get
            {
                return this._entities;
            }
        }

        public void AddUserInfo(double demoTime, uint player, Dictionary<string, string> userInfo)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);
            UserInfo info = new UserInfo(deltaDemoTime, player, userInfo);
            if (info.Spectator == null)
            {
                this._playerCache[player].Name = info.Name;
                this._playerCache[player].Team = info.Team;
            }
            else
            {
                this._playerCache[player].IsSpectator = true;
            }
            this._demoPackets.Add(info);
        }

        public void AddSetInfo(double demoTime, uint player, Dictionary<string, string> userInfo)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);
            SetInfo setInfo = new SetInfo(deltaDemoTime, player, userInfo);
            string name = setInfo.Name;
            //if the user changed his name
            if (name != null)
            {
                this._playerCache[player].Name = name;
            }
            this._demoPackets.Add(setInfo);
        }

        public void MatchStarted(double demoTime)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);
            this._isMatchStarted = true;
            this._demoPackets.Add(new MatchStarted(deltaDemoTime));
        }

        public void MatchComplete(double demoTime)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);
            this._demoPackets.Add(new MatchComplete(deltaDemoTime));
        }

        public void AddPlayerMovement(double demoTime, uint player, double offsetX, double offsetY)
        {
            if (this._isMatchStarted == false)
            {
                return;
            }
            double deltaX = 0d;
            double deltaY = 0d;
            if (offsetX != 0)
            {
                deltaX = offsetX - this._playerCache[player].OffsetX;
                this._playerCache[player].OffsetX = offsetX;
            }
            if (offsetY != 0)
            {
                deltaY = offsetY - this._playerCache[player].OffsetY;
                this._playerCache[player].OffsetY = offsetY;
            }
            if (deltaX != 0 || deltaY != 0)
            {
                double deltaDemoTime = UpdateDemoTime(demoTime);
                this._demoPackets.Add(new PlayerMovement(deltaDemoTime, player, deltaX, deltaY));
            }
        }

        public void AddEntity(Entity entity)
        {
            this._entities.Add(entity);
        }

        public List<DemoDelta> Packets
        {
            get
            {
                return this._demoPackets;
            }
        }

        private double UpdateDemoTime(double demoTime)
        {
            double deltaDemoTime = demoTime - this._lastDemoTime;
            this._lastDemoTime = demoTime;
            if (this._isMatchStarted == true)
            {
                this._matchTime += deltaDemoTime;
            }
            return deltaDemoTime;
        }

        internal void AddFragInfo(double demoTime, FragInfo fragInfo)
        {
            if (fragInfo == null)
            {
                //return;
            }
            double deltaDemoTime = UpdateDemoTime(demoTime);

            if (fragInfo.Fragger == -1)
            {
                this._demoPackets.Add(new Suicide(deltaDemoTime, (uint)fragInfo.Death));
            }
            else
            {
                uint death = (uint)fragInfo.Death;
                if (fragInfo.Death < 0)
                {
#if DEBUG
                    //throw new InvalidOperationException("TODO: DequeueFragInfoDeath was not called in this scenario");
#else
                    return;
#endif
                }
                this._demoPackets.Add(new Kill(deltaDemoTime, (uint)fragInfo.Fragger, (uint)fragInfo.Death, fragInfo.Weapon));
            }
        }

        internal void AddHealth(double demoTime, uint player, int health)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);

            int deltaHealth = health - (int)this._playerCache[player].CurrentValueHealth;
            this._demoPackets.Add(new Health(deltaDemoTime, player, deltaHealth));

            //if this is a death
            if (health == 0)
            {
                this._demoPackets.Add(new Death(0, player));
            }

            //if the player is respawning, reset 'previous health'
            if (this._playerCache[player].CurrentValueHealth == 0 && health == 100)
            {
                this._playerCache[player].PreviousValueHealth = 100;
            }
            else
            {
                this._playerCache[player].PreviousValueHealth = this._playerCache[player].CurrentValueHealth;
            }
            this._playerCache[player].CurrentValueHealth = (byte)health;
        }

        internal void AddServerInfo(double demoTime, ServerInfo serverInfo)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);
            this._demoPackets.Add(new ServerInfoDemoDelta(deltaDemoTime, serverInfo));
        }

        internal void AddScore(double demoTime, uint player, int score)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);

            int deltaScore = score - (int)this._playerCache[player].CurrentScore;
            this._playerCache[player].CurrentScore = score;

            this._demoPackets.Add(new Score(deltaDemoTime, player, deltaScore));
        }

        internal void AddPing(double demoTime, uint player, uint ping)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);
            this._demoPackets.Add(new PlayerPing(deltaDemoTime, player, ping));
        }

        internal void AddPL(double demoTime, uint player, uint pl)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);
            this._demoPackets.Add(new PlayerPL(deltaDemoTime, player, pl));
        }

        internal void AddPlayerEndGameStats(double demoTime, uint player, uint damageGiven, uint damageTaken, uint damageTeam, double lgPercent, double rlPercent, double glPercent, double sgPercent, double ssgPercent, double rlAvgDmg, uint rlDirectHits)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);
            this._demoPackets.Add(new PlayerEndGameStats(deltaDemoTime, player, damageGiven, damageTaken, damageTeam, lgPercent, rlPercent, glPercent, sgPercent, ssgPercent, rlAvgDmg, rlDirectHits));
        }

        internal void AddDamage(double demoTime, uint player, byte damageHealth, byte damageArmor)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);

            if (this._isMatchStarted == true)
            {
                if (damageHealth > this._playerCache[player].PreviousValueHealth)
                {
                    damageHealth = (byte)this._playerCache[player].PreviousValueHealth;
                }

                this._demoPackets.Add(new Damage(deltaDemoTime, player, damageHealth, damageArmor));
            }
        }

        internal void AddArmorAcquired(double demoTime, uint player, uint armorValue)
        {
            double deltaDemoTime = UpdateDemoTime(demoTime);

            Armor newArmor = Armor.None;
            int deltaArmor = (int)armorValue - (int)this._playerCache[player].CurrentValueArmor;
            bool isPickup = false;
            if (deltaArmor > 0)
            {
                switch (armorValue)
                {
                    case 100:
                        newArmor = Armor.IT_ARMOR1;
                        isPickup = true;
                        break;
                    case 150:
                        newArmor = Armor.IT_ARMOR2;
                        isPickup = true;
                        break;
                    case 200:
                        newArmor = Armor.IT_ARMOR3;
                        isPickup = true;
                        break;
                }
            }
            this._demoPackets.Add(new ArmorChanged(deltaDemoTime, player, newArmor, deltaArmor, isPickup));

            this._playerCache[player].CurrentValueArmor = (byte)armorValue;
            this._playerCache[player].CurrentArmor = newArmor;
        }

        internal void AddStat(double demoTime, uint player, Stat stat, uint value)
        {
            //double deltaDemoTime = UpdateDemoTime(demoTime);
            switch (stat)
            {
                case Stat.STAT_SHELLS:
                    {
                        int deltaValue = (int)value - (int)this._playerCache[player].CurrentValueShells;
                        this._playerCache[player].CurrentValueShells = (byte)value;
                        this._demoPackets.Add(new StatChanged(0, player, stat, deltaValue));
                    }
                    break;
                case Stat.STAT_NAILS:
                    {
                        int deltaValue = (int)value - (int)this._playerCache[player].CurrentValueNails;
                        this._playerCache[player].CurrentValueNails = (byte)value;
                        this._demoPackets.Add(new StatChanged(0, player, stat, deltaValue));
                    }
                    break;
                case Stat.STAT_ROCKETS:
                    {
                        int deltaValue = (int)value - (int)this._playerCache[player].CurrentValueRockets;
                        this._playerCache[player].CurrentValueRockets = (byte)value;
                        this._demoPackets.Add(new StatChanged(0, player, stat, deltaValue));
                    }
                    break;
                case Stat.STAT_CELLS:
                    {
                        int deltaValue = (int)value - (int)this._playerCache[player].CurrentValueCells;
                        this._playerCache[player].CurrentValueCells = (byte)value;
                        this._demoPackets.Add(new StatChanged(0, player, stat, deltaValue));
                    }
                    break;
                case Stat.STAT_ACTIVEWEAPON:
                    {
                        Weapon oldWeapon = this._playerCache[player].ActiveWeapon;
                        Weapon newWeapon = (Weapon)value;
                        this._playerCache[player].ActiveWeapon = newWeapon;
                        if (newWeapon != oldWeapon)
                        {
                            this._demoPackets.Add(new ActiveWeaponChanged(0, player, oldWeapon, newWeapon));
                        }
                    }
                    break;
                case Stat.STAT_ITEMS:
                    {
                        Weapon currentWeapons = this._playerCache[player].CurrentWeapons;
                        Weapon newWeapons = (Weapon)value & Weapon.All;
                        if (currentWeapons != newWeapons)
                        {
                            CheckForWeaponPickup(player, currentWeapons, newWeapons, Weapon.IT_SUPER_SHOTGUN);
                            CheckForWeaponPickup(player, currentWeapons, newWeapons, Weapon.IT_NAILGUN);
                            CheckForWeaponPickup(player, currentWeapons, newWeapons, Weapon.IT_SUPER_NAILGUN);
                            CheckForWeaponPickup(player, currentWeapons, newWeapons, Weapon.IT_GRENADE_LAUNCHER);
                            CheckForWeaponPickup(player, currentWeapons, newWeapons, Weapon.IT_ROCKET_LAUNCHER);
                            CheckForWeaponPickup(player, currentWeapons, newWeapons, Weapon.IT_LIGHTNING);
                        }
                        this._playerCache[player].CurrentWeapons = newWeapons;

                        Powerup currentPowerups = this._playerCache[player].CurrentPowerups;
                        Powerup newPowerups = (Powerup)value & Powerup.All;
                        if (currentPowerups != newPowerups)
                        {
                            CheckForPowerupPickup(player, currentPowerups, newPowerups, Powerup.IT_QUAD);
                            CheckForPowerupPickup(player, currentPowerups, newPowerups, Powerup.IT_INVISIBILITY);
                            CheckForPowerupPickup(player, currentPowerups, newPowerups, Powerup.IT_INVULNERABILITY);

                            this._playerCache[player].CurrentPowerups = newPowerups;
                        }
                    }
                    break;
            }
        }

        private void CheckForWeaponPickup(uint player, Weapon currentWeapons, Weapon newWeapons, Weapon weapon)
        {
            if ((newWeapons & weapon) == weapon
                && (currentWeapons & weapon) == Weapon.None)
            {
                this._demoPackets.Add(new WeaponPickup(0, player, weapon));
            }
        }

        private void CheckForPowerupPickup(uint player, Powerup currentPowerups, Powerup newPowerups, Powerup powerup)
        {
            if ((newPowerups & powerup) == powerup
                && (currentPowerups & powerup) == Powerup.None)
            {
                this._demoPackets.Add(new PowerupPickup(0, player, powerup, false));
                return;
            }
            //if the player has the powerup then loses it
            if ((currentPowerups & powerup) == powerup
                && (newPowerups & powerup) != powerup)
            {
                this._demoPackets.Add(new PowerupPickup(0, player, powerup, true));
                return;
            }
        }

        public bool IsPlayerDropped { get; set; }
    }
}
