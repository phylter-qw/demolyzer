using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model.Ratings
{
    public class EloStatsResult
    {
        public int TeamATotal { get; set; }
        public int TeamBTotal { get; set; }
        public float TeamAExpectedTotal { get; set; }
        public float TeamBExpectedTotal { get; set; }
        public int TeamAPerformanceTotal { get; set; }
        public int TeamBPerformanceTotal { get; set; }

        //public float[] TeamAElo { get; set; }
        //public float[] TeamBElo { get; set; }
        public float[] TeamAExpected { get; set; }
        public float[] TeamBExpected { get; set; }
        public int[] TeamA { get; set; }
        public int[] TeamB { get; set; }
        public int[] TeamAPerformance { get; set; }
        public int[] TeamBPerformance { get; set; }
        //public float[] TeamAEloGained { get; set; }
        //public float[] TeamBEloGained { get; set; }

        public void PostProcess()
        {
            this.TeamAPerformance = new int[this.TeamA.Length];
            this.TeamBPerformance = new int[this.TeamB.Length];

            for (int x = 0; x < this.TeamAPerformance.Length; ++x)
            {
                this.TeamATotal += this.TeamA[x];
                this.TeamBTotal += this.TeamB[x];

                this.TeamAExpectedTotal += this.TeamAExpected[x];
                this.TeamBExpectedTotal += this.TeamBExpected[x];

                this.TeamAPerformance[x] = (short)(this.TeamA[x] - this.TeamAExpected[x]);
                this.TeamBPerformance[x] = (short)(this.TeamB[x] - this.TeamBExpected[x]);

                this.TeamAPerformanceTotal += this.TeamAPerformance[x];
                this.TeamBPerformanceTotal += this.TeamBPerformance[x];
            }
        }
    }

    public class MatchEloResult
    {
        public string Player { get; set; }
        public string Opponent { get; set; }
        public string Map { get; set; }
        public DateTime Date { get; set; }

        public EloStatsResult Frags { get; set; }
        public EloStatsResult DamageGiven { get; set; }
        public EloStatsResult Efficiency { get; set; }
        public EloStatsResult Teamplay { get; set; }

        public Dictionary<string, EloStatsResult> Stats { get; set; }

        public string[] TeamAPlayerNames { get; set; }
        public string[] TeamBPlayerNames { get; set; }

        public MatchEloResult()
        {
            this.Stats = new Dictionary<string, EloStatsResult>();
        }

        public void PostProcess()
        {
            this.Frags.PostProcess();
            if (this.DamageGiven != null)
            {
                this.DamageGiven.PostProcess();
            }
            if (this.Efficiency != null)
            {
                this.Efficiency.PostProcess();
            }
            if (this.Teamplay != null)
            {
                this.Teamplay.PostProcess();
            }
            foreach (var kvp in this.Stats)
            {
                kvp.Value.PostProcess();
            }
        }
    }

    public static class StatNamesAverages
    {
        public const string FPM = "FPM";
        public const string FPMwithRL = "FPMwithRL";
        public const string FPMwithNoRL = "FPMwithNoRL";
        public const string DPM = "DPM";
        public const string HighFragStreak = "HighFragStreak";
        public const string TeamDamage = "TeamDamage";
        public const string KillSpawns = "KillSpawns";
        public const string KillRL = "KillRL";
        public const string KillQuad = "KillQuad";
        public const string KillTeam = "KillTeam";
        public const string Quads = "Quads";

        public static string[] All = new string[]
        {
            FPM,
            FPMwithRL,
            FPMwithNoRL,
            DPM,
            HighFragStreak,
            TeamDamage,
            KillSpawns,
            KillRL,
            KillQuad,
            KillTeam,
            Quads,
        };
    }

    public static class StatNamesTotalPercents
    {
        public const string RLDrop = "RLDrop";
        public const string KillsPerRL = "KillsPerRL";
        public const string LivesToGetRL = "LivesToGetRL";
        public const string TimePerRL = "TimePerRL";
        public const string TimeToGetRL = "TimeToGetRL";
        public const string TimeWithRL = "TimeWithRL";
        public const string RLvsRL = "RLvsRL";
        public const string RLvsX = "RLvsX";
        public const string XvsRL = "XvsRL";
        public const string XvsX = "XvsX";

        public static string[] All = new string[]
        {
            RLDrop,
            KillsPerRL,
            LivesToGetRL,
            TimePerRL,
            TimeToGetRL,
            TimeWithRL,
            RLvsRL,
            RLvsX,
            XvsRL,
            XvsX,
        };
    }

    public class iELOUtil
    {
        public static bool IsProcessAsTeamsEnabled;

        public static float ELOStartPoints = 1200;
        public static float ELOCoefficient = 100;

        public const string RLsKilled = "RLsKilled";
        public const string Armors = "Armors";
        public const string Powerups = "Powerups";
        public const string RLsDropped = "RLsDropped";
        public const string TimeWithRL = "TimeWithRL";
        public const string AveragePower = "AveragePower";
        public const string RLvsRL = "RLvsRL";
        public const string RLvsX = "RLvsX";
        public const string XvsRL = "XvsRL";
        public const string XvsX = "XvsX";

        public static List<MatchEloResult> CalculateiELO(List<MatchResult> matches, ParticipantLookupTable participants, int repeatFactor, bool isTeamOnly)
        {
            for (int x = 0; x < repeatFactor; ++x)
            {
                CalculateELO(matches, participants, isTeamOnly);
            }

            return CalculateMatchELOs(matches, participants, isTeamOnly);
        }

        private static List<MatchEloResult> CalculateMatchELOs(List<MatchResult> matches, ParticipantLookupTable participants, bool isTeamOnly)
        {
            List<MatchEloResult> eloResults = new List<MatchEloResult>();
            
            for (int i = 0; i < matches.Count; ++i)
            {
                MatchResult match = matches[i];

                int totalFrags = match.TeamAScore + match.TeamBScore;
                int teamPlayerCount = match.TeamAPlayerNames.Length;

                ParticipantModel[] participantTeamA = new ParticipantModel[teamPlayerCount];
                ParticipantModel[] participantTeamB = new ParticipantModel[teamPlayerCount];

                for (int x = 0; x < teamPlayerCount; ++x)
                {
                    participantTeamA[x] = participants.GetParticipant(match.TeamAPlayerNames[x]);
                    participantTeamB[x] = participants.GetParticipant(match.TeamBPlayerNames[x]);
                }

                //calculate WIN/LOSS
                if (match.TeamAScore > match.TeamBScore)
                {
                    for (int x = 0; x < teamPlayerCount; ++x)
                    {
                        participantTeamA[x].WinCount++;
                        participantTeamB[x].LossCount++;
                    }
                }
                else
                {
                    for (int x = 0; x < teamPlayerCount; ++x)
                    {
                        participantTeamA[x].LossCount++;
                        participantTeamB[x].WinCount++;
                    }
                }

                float[] elosTeamA = new float[teamPlayerCount];
                float[] elosTeamB = new float[teamPlayerCount];
                float[] teamAExpectedScores = null;
                float[] teamBExpectedScores = null;
                float[] eloPointsTeamA = null;
                float[] eloPointsTeamB = null;

                MatchEloResult matchEloResult = new MatchEloResult();
                matchEloResult.Date = match.DateTime;
                matchEloResult.Map = match.Map;
                matchEloResult.TeamAPlayerNames = match.TeamAPlayerNames;
                matchEloResult.TeamBPlayerNames = match.TeamBPlayerNames;

                for (int index = 0; index < teamPlayerCount; ++index)
                {
                    elosTeamA[index] = participantTeamA[index].EloFrags.ELO;
                    elosTeamB[index] = participantTeamB[index].EloFrags.ELO;
                }

                CalculateELOPoints(elosTeamA, elosTeamB, match.TeamAScores, match.TeamBScores, out eloPointsTeamA, out eloPointsTeamB, out teamAExpectedScores, out teamBExpectedScores);

                matchEloResult.Frags = new EloStatsResult
                {
                    TeamAExpected = teamAExpectedScores,
                    TeamBExpected = teamBExpectedScores,
                    TeamA = match.TeamAScores,
                    TeamB = match.TeamBScores,
                };

                //if this is 2on2 or 4on4
                if (teamPlayerCount > 1)
                {
                    for (int index = 0; index < teamPlayerCount; ++index)
                    {
                        elosTeamA[index] = participantTeamA[index].EloDamageGiven.ELO;
                        elosTeamB[index] = participantTeamB[index].EloDamageGiven.ELO;
                    }

                    CalculateELOPoints(elosTeamA, elosTeamB, match.TeamADamagesGiven, match.TeamBDamagesGiven, out eloPointsTeamA, out eloPointsTeamB, out teamAExpectedScores, out teamBExpectedScores);

                    matchEloResult.DamageGiven = new EloStatsResult
                    {
                        TeamAExpected = teamAExpectedScores,
                        TeamBExpected = teamBExpectedScores,
                        TeamA = match.TeamADamagesGiven,
                        TeamB = match.TeamBDamagesGiven,
                    };


                    for (int index = 0; index < teamPlayerCount; ++index)
                    {
                        elosTeamA[index] = participantTeamA[index].EloEfficiency.ELO;
                        elosTeamB[index] = participantTeamB[index].EloEfficiency.ELO;
                    }

                    CalculateELOPoints(elosTeamA, elosTeamB, match.TeamAEfficiencies, match.TeamBEfficiencies, out eloPointsTeamA, out eloPointsTeamB, out teamAExpectedScores, out teamBExpectedScores);

                    matchEloResult.Efficiency = new EloStatsResult
                    {
                        TeamAExpected = teamAExpectedScores,
                        TeamBExpected = teamBExpectedScores,
                        TeamA = match.TeamAEfficiencies,
                        TeamB = match.TeamBEfficiencies,
                    };

                    for (int index = 0; index < teamPlayerCount; ++index)
                    {
                        elosTeamA[index] = participantTeamA[index].EloTeamplay.ELO;
                        elosTeamB[index] = participantTeamB[index].EloTeamplay.ELO;
                    }

                    CalculateELOPoints(elosTeamA, elosTeamB, match.TeamATeamplays, match.TeamBTeamplays, out eloPointsTeamA, out eloPointsTeamB, out teamAExpectedScores, out teamBExpectedScores);

                    matchEloResult.Teamplay = new EloStatsResult
                    {
                        TeamAExpected = teamAExpectedScores,
                        TeamBExpected = teamBExpectedScores,
                        TeamA = match.TeamATeamplays,
                        TeamB = match.TeamBTeamplays,
                    };
                }
                else
                {
                    participantTeamA[0].Is1on1Matches = true;
                    participantTeamB[0].Is1on1Matches = true;
                }

                //-------------------------------------------------------------------------------
                matchEloResult.Stats[iELOUtil.RLsKilled] = Calc(teamPlayerCount, iELOUtil.RLsKilled, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.RLsKilled], match.TeamBStats[iELOUtil.RLsKilled]);
                matchEloResult.Stats[iELOUtil.Armors] = Calc(teamPlayerCount, iELOUtil.Armors, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.Armors], match.TeamBStats[iELOUtil.Armors]);
                matchEloResult.Stats[iELOUtil.Powerups] = Calc(teamPlayerCount, iELOUtil.Powerups, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.Powerups], match.TeamBStats[iELOUtil.Powerups]);
                matchEloResult.Stats[iELOUtil.RLsDropped] = Calc(teamPlayerCount, iELOUtil.RLsDropped, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.RLsDropped], match.TeamBStats[iELOUtil.RLsDropped]);
                matchEloResult.Stats[iELOUtil.TimeWithRL] = Calc(teamPlayerCount, iELOUtil.TimeWithRL, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.TimeWithRL], match.TeamBStats[iELOUtil.TimeWithRL]);
                matchEloResult.Stats[iELOUtil.AveragePower] = Calc(teamPlayerCount, iELOUtil.AveragePower, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.AveragePower], match.TeamBStats[iELOUtil.AveragePower]);
                matchEloResult.Stats[iELOUtil.RLvsRL] = Calc(teamPlayerCount, iELOUtil.RLvsRL, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.RLvsRL], match.TeamBStats[iELOUtil.RLvsRL]);
                matchEloResult.Stats[iELOUtil.RLvsX] = Calc(teamPlayerCount, iELOUtil.RLvsX, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.RLvsX], match.TeamBStats[iELOUtil.RLvsX]);
                matchEloResult.Stats[iELOUtil.XvsRL] = Calc(teamPlayerCount, iELOUtil.XvsRL, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.XvsRL], match.TeamBStats[iELOUtil.XvsRL]);
                matchEloResult.Stats[iELOUtil.XvsX] = Calc(teamPlayerCount, iELOUtil.XvsX, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.XvsX], match.TeamBStats[iELOUtil.XvsX]);
                //-------------------------------------------------------------------------------

                matchEloResult.PostProcess();
                eloResults.Add(matchEloResult);

                foreach (string statName in StatNamesAverages.All)
                {
                    for (int index = 0; index < teamPlayerCount; ++index)
                    {
                        participantTeamA[index].StatsAverages[statName].Add(match.TeamAStatsAverages[statName][index]);
                        participantTeamB[index].StatsAverages[statName].Add(match.TeamBStatsAverages[statName][index]);
                    }
                }

                foreach (string statName in StatNamesTotalPercents.All)
                {
                    for (int index = 0; index < teamPlayerCount; ++index)
                    {
                        participantTeamA[index].StatsTotalPercents[statName].Add(match.TeamAStatsTotalPercents[statName][index].Item1, match.TeamAStatsTotalPercents[statName][index].Item2);
                        participantTeamB[index].StatsTotalPercents[statName].Add(match.TeamBStatsTotalPercents[statName][index].Item1, match.TeamBStatsTotalPercents[statName][index].Item2);
                    }
                }

                for (int index = 0; index < teamPlayerCount; ++index)
                {
                    elosTeamA[index] = participantTeamA[index].EloCombinedNormalized;
                    elosTeamB[index] = participantTeamB[index].EloCombinedNormalized;
                }

                for (int x = 0; x < teamPlayerCount; ++x)
                {
                    participantTeamA[x].ParticipationPoints += 10;
                    participantTeamB[x].ParticipationPoints += 10;
                }

                float teamAELO = elosTeamA.Sum();
                float teamBELO = elosTeamB.Sum();

                //if A won
                if (match.TeamAScore > match.TeamBScore)
                {
                    //if A is higher rated than B, just give 25 points
                    if (teamAELO > teamBELO)
                    {
                        for (int x = 0; x < teamPlayerCount; ++x)
                        {
                            participantTeamA[x].WinPoints += 50;
                        }
                    }
                    else //otherwise A upset B, so give 75 points
                    {
                        for (int x = 0; x < teamPlayerCount; ++x)
                        {
                            participantTeamA[x].WinPoints += 100;
                        }
                    }
                }
                else //otherwise B won
                {
                    //if B is higher rated than A, just give 25 points
                    if (teamBELO > teamAELO)
                    {
                        for (int x = 0; x < teamPlayerCount; ++x)
                        {
                            participantTeamB[x].WinPoints += 50;
                        }
                    }
                    else //otherwise B upset A, so give 75 points
                    {
                        for (int x = 0; x < teamPlayerCount; ++x)
                        {
                            participantTeamB[x].WinPoints += 100;
                        }
                    }
                }
            }
            return eloResults;
        }

        private static EloStatsResult Calc(int playerCount, string stat, ParticipantModel[] a, ParticipantModel[] b, int[] statsA, int[] statsB)
        {
            float[] elosTeamA = new float[playerCount];
            float[] elosTeamB = new float[playerCount];
            float[] teamAExpectedScores = null;
            float[] teamBExpectedScores = null;
            float[] eloPointsTeamA = null;
            float[] eloPointsTeamB = null;

            for (int index = 0; index < playerCount; ++index)
            {
                elosTeamA[index] = a[index].EloStats[stat].ELO;
                elosTeamB[index] = b[index].EloStats[stat].ELO;
            }

            CalculateELOPoints(elosTeamA, elosTeamB, statsA, statsB, out eloPointsTeamA, out eloPointsTeamB, out teamAExpectedScores, out teamBExpectedScores);

            return new EloStatsResult
            {
                TeamAExpected = teamAExpectedScores,
                TeamBExpected = teamBExpectedScores,
                TeamA = statsA,
                TeamB = statsB,
            };
        }

        public static void CalculateELO(List<MatchResult> matches, ParticipantLookupTable participants, bool isTeamOnly)
        {
            if (isTeamOnly == true)
            {
                for (int i = 0; i < matches.Count; ++i)
                {
                    MatchResult match = matches[i];

                    int totalFrags = match.TeamAScore + match.TeamBScore;

                    ParticipantModel resultA = participants.GetParticipant(match.TeamA);
                    ParticipantModel resultB = participants.GetParticipant(match.TeamB);

                    //float teamAELO = resultA.ELO;
                    //float teamBELO = resultB.ELO;

                    //float teamAExpectedScore = GetExpectedScore(teamAELO - teamBELO);
                    //float teamBExpectedScore = GetExpectedScore(teamBELO - teamAELO);

                    //float eloPoints = ELOCoefficient * (match.TeamAPercent - teamAExpectedScore);
                    //float newELO = resultA.ELO + eloPoints;
                    //resultA.UpdateELO(newELO);

                    ////calculate team B's players' new ELO and match information
                    //eloPoints = ELOCoefficient * (match.TeamBPercent - teamBExpectedScore);
                    //newELO = resultB.ELO + eloPoints;
                    //resultB.UpdateELO(newELO);
                }
            }
            else
            {
                for (int i = 0; i < matches.Count; ++i)
                {
                    MatchResult match = matches[i];

                    int totalFrags = match.TeamAScore + match.TeamBScore;
                    int teamPlayerCount = match.TeamAPlayerNames.Length;

                    float[] teamAExpectedScores = new float[teamPlayerCount];
                    float[] teamBExpectedScores = new float[teamPlayerCount];
                    ParticipantModel[] participantTeamA = new ParticipantModel[teamPlayerCount];
                    ParticipantModel[] participantTeamB = new ParticipantModel[teamPlayerCount];

                    for (int x = 0; x < teamPlayerCount; ++x)
                    {
                        participantTeamA[x] = participants.GetParticipant(match.TeamAPlayerNames[x]);
                        participantTeamB[x] = participants.GetParticipant(match.TeamBPlayerNames[x]);
                    }

                    float[] eloPointsTeamA = null;
                    float[] eloPointsTeamB = null;
                    float[] elosTeamA = new float[teamPlayerCount];
                    float[] elosTeamB = new float[teamPlayerCount];


                    // -------------- ELO Frags -----------------------------------
                    for (int index = 0; index < teamPlayerCount; ++index)
                    {
                        elosTeamA[index] = participantTeamA[index].EloFrags.ELO;
                        elosTeamB[index] = participantTeamB[index].EloFrags.ELO;
                    }

                    CalculateELOPoints(elosTeamA, elosTeamB, match.TeamAScores, match.TeamBScores, out eloPointsTeamA, out eloPointsTeamB, out teamAExpectedScores, out teamBExpectedScores);

                    for (int index = 0; index < teamPlayerCount; ++index)
                    {
                        participantTeamA[index].EloFrags.UpdateELO(elosTeamA[index] + eloPointsTeamA[index]);
                        participantTeamB[index].EloFrags.UpdateELO(elosTeamB[index] + eloPointsTeamB[index]);
                    }
                    // -----------------------------------------------------------

                    if (teamPlayerCount > 1)
                    {
                        // -------------- ELO Damage Given -----------------------------------
                        for (int index = 0; index < teamPlayerCount; ++index)
                        {
                            elosTeamA[index] = participantTeamA[index].EloDamageGiven.ELO;
                            elosTeamB[index] = participantTeamB[index].EloDamageGiven.ELO;
                        }

                        CalculateELOPoints(elosTeamA, elosTeamB, match.TeamADamagesGiven, match.TeamBDamagesGiven, out eloPointsTeamA, out eloPointsTeamB, out teamAExpectedScores, out teamBExpectedScores);

                        for (int index = 0; index < teamPlayerCount; ++index)
                        {
                            participantTeamA[index].EloDamageGiven.UpdateELO(elosTeamA[index] + eloPointsTeamA[index]);
                            participantTeamB[index].EloDamageGiven.UpdateELO(elosTeamB[index] + eloPointsTeamB[index]);
                        }
                        // -----------------------------------------------------------

                        // -------------- ELO Efficiency -----------------------------------
                        for (int index = 0; index < teamPlayerCount; ++index)
                        {
                            elosTeamA[index] = participantTeamA[index].EloEfficiency.ELO;
                            elosTeamB[index] = participantTeamB[index].EloEfficiency.ELO;
                        }

                        CalculateELOPoints(elosTeamA, elosTeamB, match.TeamAEfficiencies, match.TeamBEfficiencies, out eloPointsTeamA, out eloPointsTeamB, out teamAExpectedScores, out teamBExpectedScores);

                        for (int index = 0; index < teamPlayerCount; ++index)
                        {
                            participantTeamA[index].EloEfficiency.UpdateELO(elosTeamA[index] + eloPointsTeamA[index]);
                            participantTeamB[index].EloEfficiency.UpdateELO(elosTeamB[index] + eloPointsTeamB[index]);
                        }
                        // -----------------------------------------------------------

                        // -------------- ELO Teamplay -----------------------------------
                        for (int index = 0; index < teamPlayerCount; ++index)
                        {
                            elosTeamA[index] = participantTeamA[index].EloTeamplay.ELO;
                            elosTeamB[index] = participantTeamB[index].EloTeamplay.ELO;
                        }

                        CalculateELOPoints(elosTeamA, elosTeamB, match.TeamATeamplays, match.TeamBTeamplays, out eloPointsTeamA, out eloPointsTeamB, out teamAExpectedScores, out teamBExpectedScores);

                        for (int index = 0; index < teamPlayerCount; ++index)
                        {
                            participantTeamA[index].EloTeamplay.UpdateELO(elosTeamA[index] + eloPointsTeamA[index]);
                            participantTeamB[index].EloTeamplay.UpdateELO(elosTeamB[index] + eloPointsTeamB[index]);
                        }
                        // -----------------------------------------------------------
                    }

                    // -------------- ELO Stats -----------------------------------
                    CalcEloStats(teamPlayerCount, iELOUtil.RLsKilled, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.RLsKilled], match.TeamBStats[iELOUtil.RLsKilled]);
                    CalcEloStats(teamPlayerCount, iELOUtil.Armors, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.Armors], match.TeamBStats[iELOUtil.Armors]);
                    CalcEloStats(teamPlayerCount, iELOUtil.Powerups, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.Powerups], match.TeamBStats[iELOUtil.Powerups]);
                    CalcEloStats(teamPlayerCount, iELOUtil.RLsDropped, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.RLsDropped], match.TeamBStats[iELOUtil.RLsDropped]);
                    CalcEloStats(teamPlayerCount, iELOUtil.TimeWithRL, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.TimeWithRL], match.TeamBStats[iELOUtil.TimeWithRL]);
                    CalcEloStats(teamPlayerCount, iELOUtil.AveragePower, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.AveragePower], match.TeamBStats[iELOUtil.AveragePower]);
                    CalcEloStats(teamPlayerCount, iELOUtil.RLvsRL, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.RLvsRL], match.TeamBStats[iELOUtil.RLvsRL]);
                    CalcEloStats(teamPlayerCount, iELOUtil.RLvsX, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.RLvsX], match.TeamBStats[iELOUtil.RLvsX]);
                    CalcEloStats(teamPlayerCount, iELOUtil.XvsRL, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.XvsRL], match.TeamBStats[iELOUtil.XvsRL]);
                    CalcEloStats(teamPlayerCount, iELOUtil.XvsX, participantTeamA, participantTeamB, match.TeamAStats[iELOUtil.XvsX], match.TeamBStats[iELOUtil.XvsX]);
                    // -----------------------------------------------------------
                }
            }
        }

        private static void CalcEloStats(int playerCount, string stat, ParticipantModel[] a, ParticipantModel[] b, int[] statsA, int[] statsB)
        {
            float[] elosTeamA = new float[playerCount];
            float[] elosTeamB = new float[playerCount];
            float[] teamAExpectedScores = null;
            float[] teamBExpectedScores = null;
            float[] eloPointsTeamA = null;
            float[] eloPointsTeamB = null;

            for (int index = 0; index < playerCount; ++index)
            {
                elosTeamA[index] = a[index].EloStats[stat].ELO;
                elosTeamB[index] = b[index].EloStats[stat].ELO;
            }

            CalculateELOPoints(elosTeamA, elosTeamB, statsA, statsB, out eloPointsTeamA, out eloPointsTeamB, out teamAExpectedScores, out teamBExpectedScores);

            for (int index = 0; index < playerCount; ++index)
            {
                a[index].EloStats[stat].UpdateELO(elosTeamA[index] + eloPointsTeamA[index]);
                b[index].EloStats[stat].UpdateELO(elosTeamB[index] + eloPointsTeamB[index]);
            }
        }

        private static void CalculateELOPoints(float[] elosTeamA, float[] elosTeamB, int[] scoresTeamA, int[] scoresTeamB, out float[] eloPointsTeamA, out float[] eloPointsTeamB, out float[] teamAExpectedPerformanceScore, out float[] teamBExpectedPerformanceScore)
        {
            int teamPlayerCount = elosTeamA.Length;

            float eloAverageTeamA = 0;
            float eloAverageTeamB = 0;

            for (int index = 0; index < teamPlayerCount; ++index)
            {
                eloAverageTeamA += elosTeamA[index];
                eloAverageTeamB += elosTeamB[index];
            }
            eloAverageTeamA /= teamPlayerCount;
            eloAverageTeamB /= teamPlayerCount;

            float expectedTeamA = GetExpectedScore(eloAverageTeamA - eloAverageTeamB);
            float expectedTeamB = GetExpectedScore(eloAverageTeamB - eloAverageTeamA);

            float[] teamATeammateFactors = new float[teamPlayerCount];

            for (int x = 0; x < teamPlayerCount; ++x)
            {
                for (int y = 0; y < teamPlayerCount; ++y)
                {
                    float eloSelf = elosTeamA[x];
                    float eloTeammate = elosTeamA[y];

                    float expectedSelfvsTeammate = GetExpectedScore(eloSelf - eloTeammate);
                    teamATeammateFactors[x] += expectedSelfvsTeammate;
                }
                teamATeammateFactors[x] /= teamPlayerCount;
                teamATeammateFactors[x] *= 2;
                teamATeammateFactors[x] /= teamPlayerCount;
            }

            float[] teamBTeammateFactors = new float[teamPlayerCount];

            for (int x = 0; x < teamPlayerCount; ++x)
            {
                for (int y = 0; y < teamPlayerCount; ++y)
                {
                    float eloSelf = elosTeamB[x];
                    float eloTeammate = elosTeamB[y];

                    float expectedSelfvsTeammate = GetExpectedScore(eloSelf - eloTeammate);
                    teamBTeammateFactors[x] += expectedSelfvsTeammate;
                }
                teamBTeammateFactors[x] /= teamPlayerCount;
                teamBTeammateFactors[x] *= 2;
                teamBTeammateFactors[x] /= teamPlayerCount;
            }

            float[] teamAExpectedPerformance = new float[teamPlayerCount];
            float[] teamBExpectedPerformance = new float[teamPlayerCount];
            teamAExpectedPerformanceScore = new float[teamPlayerCount];
            teamBExpectedPerformanceScore = new float[teamPlayerCount];

            //get the total expected % for each player (example, if team A should win by 80%, and each player on team A should have 25% of total score,
            //then each player on team A should have a total expected % of 20% (0.25 * 0.80))
            for (int index = 0; index < teamPlayerCount; ++index)
            {
                teamAExpectedPerformance[index] = teamATeammateFactors[index] * expectedTeamA;
                teamBExpectedPerformance[index] = teamBTeammateFactors[index] * expectedTeamB;
            }

            float totalScore = 0;
            for (int i = 0; i < teamPlayerCount; ++i)
            {
                totalScore += scoresTeamA[i];
                totalScore += scoresTeamB[i];
            }

            float[] teamAPercents = new float[teamPlayerCount];
            float[] teamBPercents = new float[teamPlayerCount];

            for (int i = 0; i < teamPlayerCount; ++i)
            {
                teamAPercents[i] = (float)scoresTeamA[i] / totalScore;
                teamBPercents[i] = (float)scoresTeamB[i] / totalScore;
            }

            eloPointsTeamA = new float[teamPlayerCount];
            eloPointsTeamB = new float[teamPlayerCount];

            for (int index = 0; index < teamPlayerCount; ++index)
            {
                eloPointsTeamA[index] = ELOCoefficient * (teamAPercents[index] - teamAExpectedPerformance[index]);
                eloPointsTeamB[index] = ELOCoefficient * (teamBPercents[index] - teamBExpectedPerformance[index]);
            }

            for (int index = 0; index < teamPlayerCount; ++index)
            {
                teamAExpectedPerformanceScore[index] = teamAExpectedPerformance[index] * totalScore;
                teamBExpectedPerformanceScore[index] = teamBExpectedPerformance[index] * totalScore;
            }
        }

        public static float GetExpectedScore(float eloDifference)
        {
            float x = (float)Math.Pow(10, (-eloDifference) / 400f);
            return 1 / (1 + x);
        }
    }
}

#if FALSE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model.Ratings
{
    public class iELOUtil
    {
        public static float ELOStartPoints = 1200;
        public static float ELOCoefficient = 15;

        public static void CalculateiELO(MatchResult[] matches, ParticipantLookupTable participants, int repeatFactor, bool isTeamOnly)
        {
            for (int x = 0; x < repeatFactor; ++x)
            {
                CalculateELO(matches, participants, isTeamOnly);
            }
        }

        public static void CalculateELO(MatchResult[] matches, ParticipantLookupTable participants, bool isTeamOnly)
        {
            if (isTeamOnly == true)
            {
                for (int i = 0; i < matches.Length; ++i)
                {
                    MatchResult match = matches[i];

                    int totalFrags = match.TeamAScore + match.TeamBScore;

                    ParticipantModel resultA = participants.GetParticipant(match.TeamA);
                    ParticipantModel resultB = participants.GetParticipant(match.TeamB);

                    float teamAELO = resultA.ELO;
                    float teamBELO = resultB.ELO;

                    float teamAExpectedScore = GetExpectedScore(teamAELO - teamBELO);
                    float teamBExpectedScore = GetExpectedScore(teamBELO - teamAELO);

                    float eloPoints = ELOCoefficient * (match.TeamAPercent - teamAExpectedScore);
                    float newELO = resultA.ELO + eloPoints;
                    resultA.UpdateELO(newELO);

                    //calculate team B's players' new ELO and match information
                    eloPoints = ELOCoefficient * (match.TeamBPercent - teamBExpectedScore);
                    newELO = resultB.ELO + eloPoints;
                    resultB.UpdateELO(newELO);
                }
            }
            else
            {
                for (int i = 0; i < matches.Length; ++i)
                {
                    MatchResult match = matches[i];

                    int totalFrags = match.TeamAScore + match.TeamBScore;
                    int teamPlayerCount = match.TeamAPlayerNames.Length;

                    float[] teamAExpectedScores = new float[teamPlayerCount];
                    float[] teamBExpectedScores = new float[teamPlayerCount];
                    ParticipantModel[] participantTeamA = new ParticipantModel[teamPlayerCount];
                    ParticipantModel[] participantTeamB = new ParticipantModel[teamPlayerCount];

                    for (int x = 0; x < teamPlayerCount; ++x)
                    {
                        string playerA = match.TeamAPlayerNames[x];
                        participantTeamA[x] = participants.GetParticipant(playerA);

                        for (int y = 0; y < teamPlayerCount; ++y)
                        {
                            string playerB = match.TeamBPlayerNames[y];
                            if (x == 0)
                            {
                                participantTeamB[y] = participants.GetParticipant(playerB);
                            }

                            float teamAELO = participantTeamA[x].ELO;
                            float teamBELO = participantTeamB[y].ELO;

                            teamAExpectedScores[x] += GetExpectedScore(teamAELO - teamBELO);
                            teamBExpectedScores[y] += GetExpectedScore(teamBELO - teamAELO);
                        }
                    }

                    //get each player's expected score
                    for (int index = 0; index < teamPlayerCount; ++index)
                    {
                        teamAExpectedScores[index] = teamAExpectedScores[index] / teamPlayerCount;
                        teamBExpectedScores[index] = teamBExpectedScores[index] / teamPlayerCount;
                    }

                    //calculate team A's players' new ELO and match information
                    for (int index = 0; index < teamPlayerCount; ++index)
                    {
                        float eloPoints = ELOCoefficient * (match.TeamAPercents[index] * teamPlayerCount - teamAExpectedScores[index]);
                        float newELO = participantTeamA[index].ELO + eloPoints;
                        participantTeamA[index].UpdateELO(newELO);
                    }

                    //calculate team B's players' new ELO and match information
                    for (int index = 0; index < teamPlayerCount; ++index)
                    {
                        float eloPoints = ELOCoefficient * (match.TeamBPercents[index] * teamPlayerCount - teamBExpectedScores[index]);
                        float newELO = participantTeamB[index].ELO + eloPoints;
                        participantTeamB[index].UpdateELO(newELO);
                    }
                }
            }
        }

        public static float GetExpectedScore(float eloDifference)
        {
            float x = (float)Math.Pow(10, (-eloDifference) / 400f);
            return 1 / (1 + x);
        }
    }
}

#endif