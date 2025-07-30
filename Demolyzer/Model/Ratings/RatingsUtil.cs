using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Demolyzer.Model.Ratings
{
    public class RatingsUtil
    {
        public static float ELOStartPoints = 1200;
        public static float ELOCoefficient = 15;

//        public static ParticipantModel[] CalculateELOPlayerRatings(MatchResult[] matches, bool teamOnly)
//        {
//            Dictionary<string, ParticipantModel> allPlayers = CreateDictionary(matches, teamOnly);

//            for (int i = 0; i < matches.Length; ++i)
//            {
//                MatchResult match = matches[i];

//                if (match.PlayerCount > 1 && teamOnly == false)
//                {

//                    int totalFrags = match.TeamAScore + match.TeamBScore;
//                    int teamPlayerCount = match.TeamAPlayerNames.Length;

//                    float[] teamAExpectedScores = new float[teamPlayerCount];
//                    float[] teamBExpectedScores = new float[teamPlayerCount];
//                    ParticipantModel[] participantTeamA = new ParticipantModel[teamPlayerCount];
//                    ParticipantModel[] participantTeamB = new ParticipantModel[teamPlayerCount];

//                    for (int indexA = 0; indexA < teamPlayerCount; ++indexA)
//                    {
//                        string playerA = match.TeamAPlayerNames[indexA];
//                        participantTeamA[indexA] = allPlayers[playerA];

//                        for (int indexB = 0; indexB < teamPlayerCount; ++indexB)
//                        {
//                            string playerB = match.TeamBPlayerNames[indexB];
//                            if (indexA == 0)
//                            {
//                                participantTeamB[indexB] = allPlayers[playerB];
//                            }
//                        }
//                    }

//                    //Debug.WriteLine(String.Format("#########################################################"));

//                    float eloAverageTeamA = 0;
//                    float eloAverageTeamB = 0;

//                    for (int index = 0; index < teamPlayerCount; ++index)
//                    {
//                        eloAverageTeamA += participantTeamA[index].ELO;
//                        eloAverageTeamB += participantTeamB[index].ELO;
//                    }
//                    eloAverageTeamA /= teamPlayerCount;
//                    eloAverageTeamB /= teamPlayerCount;

//                    float expectedTeamA = GetExpectedScore(eloAverageTeamA - eloAverageTeamB);
//                    float expectedTeamB = GetExpectedScore(eloAverageTeamB - eloAverageTeamA);

//                    int aaa = 0;
//                    //Debug.WriteLine(String.Format("TeamAELO={0:F2}", eloAverageTeamA));
//                    //Debug.WriteLine(String.Format("TeamAExpected={0:F2}", expectedTeamA));
//                    //Debug.WriteLine(String.Format("TeamAActual={0:F2}", match.TeamAPercent));
//                    ////Debug.WriteLine(String.Format("TeamAPoints={0:F2}", eloPointsTeamA));
//                    //Debug.WriteLine(String.Format("TeamBELO={0:F2}", eloAverageTeamB));
//                    //Debug.WriteLine(String.Format("TeamBExpected={0:F2}", expectedTeamB));
//                    //Debug.WriteLine(String.Format("TeamBActual={0:F2}", match.TeamBPercent));
//                    ////Debug.WriteLine(String.Format("TeamBPoints={0:F2}", eloPointsTeamB));

//                    float[] teamATeammateFactors = new float[teamPlayerCount];

//                    for (int x = 0; x < teamPlayerCount; ++x)
//                    {
//                        for (int y = 0; y < teamPlayerCount; ++y)
//                        {
//                            float eloSelf = participantTeamA[x].ELO;
//                            float eloTeammate = participantTeamA[y].ELO;

//                            float expectedSelfvsTeammate = GetExpectedScore(eloSelf - eloTeammate);
//                            teamATeammateFactors[x] += expectedSelfvsTeammate;
//                        }
//                        teamATeammateFactors[x] /= teamPlayerCount;
//                        teamATeammateFactors[x] *= 2;
//                        teamATeammateFactors[x] /= teamPlayerCount;
//                    }

//                    float[] teamBTeammateFactors = new float[teamPlayerCount];

//                    for (int x = 0; x < teamPlayerCount; ++x)
//                    {
//                        for (int y = 0; y < teamPlayerCount; ++y)
//                        {
//                            float eloSelf = participantTeamB[x].ELO;
//                            float eloTeammate = participantTeamB[y].ELO;

//                            float expectedSelfvsTeammate = GetExpectedScore(eloSelf - eloTeammate);
//                            teamBTeammateFactors[x] += expectedSelfvsTeammate;
//                        }
//                        teamBTeammateFactors[x] /= teamPlayerCount;
//                        teamBTeammateFactors[x] *= 2;
//                        teamBTeammateFactors[x] /= teamPlayerCount;
//                    }
//                    int asdf = 0;

//                    for (int index = 0; index < teamPlayerCount; ++index)
//                    {
//                        teamAExpectedScores[index] = teamATeammateFactors[index] * expectedTeamA;
//                        teamBExpectedScores[index] = teamBTeammateFactors[index] * expectedTeamB;
//                    }
//                    //for (int index = 0; index < teamPlayerCount; ++index)
//                    //{
//                    //    Debug.WriteLine(String.Format("teamATeammateFactors[{0}]={1:F2}", index, teamATeammateFactors[index]));
//                    //}
//                    //for (int index = 0; index < teamPlayerCount; ++index)
//                    //{
//                    //    Debug.WriteLine(String.Format("teamBTeammateFactors[{0}]={1:F2}", index, teamBTeammateFactors[index]));
//                    //}

//                    //for (int index = 0; index < teamPlayerCount; ++index)
//                    //{
//                    //    float eloPoints = ELOCoefficient * (match.TeamAPercents[index] - teamAExpectedScores[index]);
//                    //    Debug.WriteLine(String.Format("PlayerA[{0}] Expected={1:F2} Actual={2:F2} Points={3:F2}", index, teamAExpectedScores[index], match.TeamAPercents[index], eloPoints));
//                    //}
//                    //for (int index = 0; index < teamPlayerCount; ++index)
//                    //{
//                    //    float eloPoints = ELOCoefficient * (match.TeamBPercents[index] - teamBExpectedScores[index]);
//                    //    Debug.WriteLine(String.Format("PlayerB[{0}] Expected={1:F2} Actual={2:F2} Points={3:F2}", index, teamBExpectedScores[index], match.TeamBPercents[index], eloPoints));
//                    //}

//                    //Debug.WriteLine(String.Format("#########################################################"));

//                    int qwer = 0;
//                    //calculate team A's players' new ELO and match information
//                    for (int index = 0; index < teamPlayerCount; ++index)
//                    {
//                        float eloPoints = ELOCoefficient * (match.TeamAPercents[index] - teamAExpectedScores[index]);
//                        float newELO = participantTeamA[index].ELO + eloPoints;
//                        participantTeamA[index].UpdateELO(newELO);

//                        //update player's match information
//                        ParticipantMatchModel matchA = participantTeamA[index].Matches[participantTeamA[index].MatchCount];
//                        matchA.TeamPlayerCount = teamPlayerCount;
//                        matchA.ELOPointsGained = eloPoints;
//                        matchA.ELOPointsCurrent = newELO;
//                        matchA.ExpectedScore = teamAExpectedScores[index] * totalFrags;
//                        //matchA.ExpectedScoreText = String.Format("{0:F0}", teamAExpectedScores[index] * totalFrags / (float)teamPlayerCount); //example: 0.5 * 100 / 4 = 12.5 frags
//                        matchA.IsShowBothExpected = 0;
//                        matchA.ExpectedScoreA = (short)(teamAExpectedScores[index] * totalFrags);
//                        //matchA.ScoreText = String.Format("{0}", matchA.ScorePlayer);
//                        matchA.IsShowBothScore = 0;
//                        matchA.ScoreA = matchA.ScorePlayer;
//                        matchA.IsWin = match.TeamAScore >= match.TeamBScore;
//                        matchA.Percent = teamPlayerCount * match.TeamAScores[index] / (float)totalFrags;
//                        participantTeamA[index].AddTeam(match.TeamA);
//                    }

//                    //calculate team B's players' new ELO and match information
//                    for (int index = 0; index < teamPlayerCount; ++index)
//                    {
//                        float eloPoints = ELOCoefficient * (match.TeamBPercents[index] - teamBExpectedScores[index]);

//                        float newELO = participantTeamB[index].ELO + eloPoints;
//                        participantTeamB[index].UpdateELO(newELO);

//                        //update player's match information
//                        ParticipantMatchModel matchB = participantTeamB[index].Matches[participantTeamB[index].MatchCount];
//                        matchB.TeamPlayerCount = teamPlayerCount;
//                        matchB.ELOPointsGained = eloPoints;
//                        matchB.ELOPointsCurrent = newELO;
//                        matchB.ExpectedScore = teamBExpectedScores[index] * totalFrags;
//                        //matchB.ExpectedScoreText = String.Format("{0:F0}", teamBExpectedScores[index] * totalFrags / (float)teamPlayerCount); //example: 0.5 * 100 / 4 = 12.5 frags
//                        matchB.IsShowBothExpected = 0;
//                        matchB.ExpectedScoreA = (short)(teamBExpectedScores[index] * totalFrags);
//                        //matchB.ScoreText = String.Format("{0}", matchB.ScorePlayer);
//                        matchB.IsShowBothScore = 0;
//                        matchB.ScoreA = matchB.ScorePlayer;
//                        matchB.IsWin = match.TeamBScore >= match.TeamAScore;
//                        matchB.Percent = teamPlayerCount * match.TeamBScores[index] / (float)totalFrags;
//                        participantTeamB[index].AddTeam(match.TeamB);
//                    }

//                    //update win/loss counts for each player
//                    if (match.TeamAScore >= match.TeamBScore)
//                    {
//                        for (int index = 0; index < participantTeamA.Length; ++index)
//                        {
//                            participantTeamA[index].WinCount++;
//                            participantTeamB[index].LossCount++;
//                        }
//                    }
//                    else
//                    {
//                        for (int index = 0; index < participantTeamA.Length; ++index)
//                        {
//                            participantTeamA[index].LossCount++;
//                            participantTeamB[index].WinCount++;
//                        }
//                    }
//                }
//                else
//                {
//                    int totalFrags = match.TeamAScore + match.TeamBScore;

//                    ParticipantModel resultA = allPlayers[match.TeamA];
//                    ParticipantModel resultB = allPlayers[match.TeamB];

//                    float teamAELO = resultA.ELO;
//                    float teamBELO = resultB.ELO;

//                    float teamAExpectedScore = GetExpectedScore(teamAELO - teamBELO);
//                    float teamBExpectedScore = GetExpectedScore(teamBELO - teamAELO);

//                    float eloPoints = ELOCoefficient * (match.TeamAPercent - teamAExpectedScore);
//                    eloPoints = eloPoints * ELOAdjustmentFactor(resultA.MatchCount, resultB.MatchCount, teamAELO, teamBELO);
//                    float newELO = resultA.ELO + eloPoints;
//                    resultA.UpdateELO(newELO);

//                    //update player's match information
//                    ParticipantMatchModel matchA = resultA.Matches[resultA.MatchCount];
//                    matchA.TeamPlayerCount = 1;
//                    matchA.ELOPointsGained = eloPoints;
//                    matchA.ELOPointsCurrent = newELO;
//                    matchA.ExpectedScore = teamAExpectedScore * totalFrags;
//                    //matchA.ExpectedScoreText = String.Format("{0:F0}:{1:F0}", teamAExpectedScore * totalFrags, teamBExpectedScore * totalFrags);
//                    matchA.IsShowBothExpected = 1;
//                    matchA.ExpectedScoreA = (short)(teamAExpectedScore * totalFrags);
//                    matchA.ExpectedScoreB = (short)(teamBExpectedScore * totalFrags);
//                    //matchA.ScoreText = String.Format("{0}:{1}", match.TeamAScore, match.TeamBScore);
//                    matchA.IsShowBothScore = 1;
//                    matchA.ScoreA = match.TeamAScore;
//                    matchA.ScoreB = match.TeamBScore;
//                    matchA.IsWin = match.TeamAScore >= match.TeamBScore;
//                    matchA.Percent = match.TeamAScore / (float)totalFrags;
//                    resultA.AddTeam(match.TeamA);

//                    //calculate team B's players' new ELO and match information
//                    eloPoints = ELOCoefficient * (match.TeamBPercent - teamBExpectedScore);
//                    eloPoints = eloPoints * ELOAdjustmentFactor(resultB.MatchCount, resultA.MatchCount, teamBELO, teamAELO);
//                    newELO = resultB.ELO + eloPoints;
//                    resultB.UpdateELO(newELO);

//                    //update player's match information
//                    ParticipantMatchModel matchB = resultB.Matches[resultB.MatchCount];
//                    matchB.TeamPlayerCount = 1;
//                    matchB.ELOPointsGained = eloPoints;
//                    matchB.ELOPointsCurrent = newELO;
//                    matchB.ExpectedScore = teamBExpectedScore * totalFrags;
//                    //matchB.ExpectedScoreText = String.Format("{0:F0}:{1:F0}", teamBExpectedScore * totalFrags, teamAExpectedScore * totalFrags);
//                    matchB.IsShowBothExpected = 1;
//                    matchB.ExpectedScoreA = (short)(teamBExpectedScore * totalFrags);
//                    matchB.ExpectedScoreB = (short)(teamAExpectedScore * totalFrags);
//                    //matchB.ScoreText = String.Format("{0}:{1}", match.TeamBScore, match.TeamAScore);
//                    matchB.IsShowBothScore = 1;
//                    matchB.ScoreA = match.TeamBScore;
//                    matchB.ScoreB = match.TeamAScore;
//                    matchB.IsWin = match.TeamBScore >= match.TeamAScore;
//                    matchB.Percent = match.TeamBScore / (float)totalFrags;
//                    resultB.AddTeam(match.TeamB);

//                    //update win/loss counts for each player
//                    if (match.TeamAScore >= match.TeamBScore)
//                    {
//                        resultA.WinCount++;
//                        resultB.LossCount++;
//                    }
//                    else
//                    {
//                        resultA.LossCount++;
//                        resultB.WinCount++;
//                    }
//                }
//            }

//#if EXPERIMENTAL_PERFORMANCE_RATING
//            if ((matches.Length > 0 && matches[0].PlayerCount == 1) || teamOnly == true)
//            {
//                Dictionary<string, float> currentELOs = new Dictionary<string, float>();
//                foreach (var kvp in allPlayers)
//                {
//                    currentELOs[kvp.Key] = kvp.Value.ELO;
//                }

//                int lastMatchCount = 30;
//                foreach (var kvp in allPlayers)
//                {
//                    int index = Math.Max(kvp.Value.Matches.Count - lastMatchCount, 0);
//                    int count = kvp.Value.Matches.Count - index;
//                    var recentMatches = kvp.Value.Matches.GetRange(index, count);
//                    float currentELO = kvp.Value.ELO;

//                    for (int x = 0; x < 10; ++x)
//                    {
//                        foreach (var match in recentMatches)
//                        {
//                            float teamAELO = currentELO;
//                            float teamBELO = currentELOs[match.Opponent];
//                            float expectedScore = GetExpectedScore(teamAELO - teamBELO);
//                            float percent = (float)Math.Min(1d, match.Percent); //prevent negative scores from inflating data
//                            percent = (float)Math.Max(0d, percent); //prevent negative scores from inflating data
//                            float eloPoints = ELOCoefficient * (percent - expectedScore);

//                            currentELO += eloPoints;
//                        }
//                    }
//                    kvp.Value.UpdateELO(currentELO);
//                }
//            }
//#endif

//            return allPlayers.Values.ToArray();
//        }

//        public static float GetExpectedScore(float eloDifference)
//        {
//            float x = (float)Math.Pow(10, (-eloDifference) / 400f);
//            return 1 / (1 + x);
//        }

//        private static float ELOAdjustmentFactor(int matchCountA, int matchCountB, float participantAELO, float participantBELO)
//        {
//            //if A has few matches but B has many, make A's change more significant
//            if (matchCountA <= 5 && matchCountB > 5)
//            {
//                return 6f / (matchCountA + 1);
//            }

//            //if the opponent is new to the system, do not make A change by much
//            if (matchCountA > 5 && matchCountB <= 5)
//            {
//                return matchCountB / 5f;
//            }

//            float eloDiff = participantAELO - participantBELO;

//#if POINTS_RUN_UP_FILTER
//            //prevent skilled players from gaining points by beating up on non-skilled players
//            if (eloDiff > 100)
//            {
//                //if elo diff is 0, return adjustment of 1
//                //if elo diff is 100 through 200, return adjustment of 0.5 to 0
//                return (float)Math.Max(0d, (200 - eloDiff) / 200);
//            }
//#endif

//            return 1f;
//        }

//        private static Dictionary<string, ParticipantModel> CreateDictionary(MatchResult[] matches, bool teamOnly)
//        {
//            Dictionary<string, ParticipantModel> playerResults = new Dictionary<string, ParticipantModel>();
//            Dictionary<int, string> playerNameLookup = new Dictionary<int, string>();
//            Dictionary<string, int> playerIdLookup = new Dictionary<string, int>();
//            HashSet<string> players = new HashSet<string>();

//            for (int i = 0; i < matches.Length; ++i)
//            {
//                MatchResult match = matches[i];

//                //HANDLE 4v4 data
//                if (match.TeamAPlayerNames != null)
//                {
//                    for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
//                    {
//                        string playerA = match.TeamAPlayerNames[x];
//                        string playerB = match.TeamBPlayerNames[x];

//                        if (players.Add(playerA) == true)
//                        {
//                            playerIdLookup[playerA] = playerIdLookup.Count;
//                            playerNameLookup[playerNameLookup.Count] = playerA;
//                        }
//                        if (players.Add(playerB) == true)
//                        {
//                            playerIdLookup[playerB] = playerIdLookup.Count;
//                            playerNameLookup[playerNameLookup.Count] = playerB;
//                        }
//                    }
//                }
//            }

//            for (int i = 0; i < matches.Length; ++i)
//            {
//                MatchResult match = matches[i];

//                int[] teamAPlayerNames = null;
//                int[] teamBPlayerNames = null;

//                if (match.TeamAPlayerNames != null)
//                {
//                    teamAPlayerNames = new int[match.TeamAPlayerNames.Length];
//                    teamBPlayerNames = new int[match.TeamBPlayerNames.Length];

//                    for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
//                    {
//                        string playerA = match.TeamAPlayerNames[x];
//                        string playerB = match.TeamBPlayerNames[x];

//                        teamAPlayerNames[x] = playerIdLookup[playerA];
//                        teamBPlayerNames[x] = playerIdLookup[playerB];
//                    }
//                }

//                //HANDLE 4v4 data
//                if (match.PlayerCount > 1 && teamOnly == false)
//                {
//                    for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
//                    {
//                        string playerA = match.TeamAPlayerNames[x];
//                        string playerB = match.TeamBPlayerNames[x];

//                        ParticipantModel participantA = null;
//                        ParticipantModel participantB = null;
//                        if (playerResults.TryGetValue(playerA, out participantA) == false)
//                        {
//                            participantA = new ParticipantModel(playerA);
//                            playerResults.Add(playerA, participantA);
//                        }
//                        if (playerResults.TryGetValue(playerB, out participantB) == false)
//                        {
//                            participantB = new ParticipantModel(playerB);
//                            playerResults.Add(playerB, participantB);
//                        }

//                        participantA.Matches.Add(new ParticipantMatchModel
//                        {
//                            Player = match.TeamA,
//                            Opponent = match.TeamB,
//                            ScorePlayer = match.TeamAScores[x],
//                            ScoreTeam = match.TeamAScore,
//                            ScoreOpponent = match.TeamBScore,
//                            Map = match.Map,
//                            Date = match.DateTime,
//                            TeamAPlayerNames = teamAPlayerNames,
//                            TeamBPlayerNames = teamBPlayerNames,
//                            TeamAScores = match.TeamAScores,
//                            TeamBScores = match.TeamBScores,
//                            PlayerNameLookupTable = playerNameLookup,
//                        });

//                        participantB.Matches.Add(new ParticipantMatchModel
//                        {
//                            Player = match.TeamB,
//                            Opponent = match.TeamA,
//                            ScorePlayer = match.TeamBScores[x],
//                            ScoreTeam = match.TeamBScore,
//                            ScoreOpponent = match.TeamAScore,
//                            Map = match.Map,
//                            Date = match.DateTime,
//                            TeamAPlayerNames = teamBPlayerNames,
//                            TeamBPlayerNames = teamAPlayerNames,
//                            TeamAScores = match.TeamBScores,
//                            TeamBScores = match.TeamAScores,
//                            PlayerNameLookupTable = playerNameLookup,
//                        });
//                    }
//                }
//                else //otherwise 1on1 or Team data
//                {
//                    ParticipantModel participantA = null;
//                    ParticipantModel participantB = null;
//                    if (playerResults.TryGetValue(match.TeamA, out participantA) == false)
//                    {
//                        participantA = new ParticipantModel(match.TeamA);
//                        playerResults.Add(match.TeamA, participantA);
//                    }
//                    if (playerResults.TryGetValue(match.TeamB, out participantB) == false)
//                    {
//                        participantB = new ParticipantModel(match.TeamB);
//                        playerResults.Add(match.TeamB, participantB);
//                    }

//                    participantA.Matches.Add(new ParticipantMatchModel
//                    {
//                        Percent = match.TeamAPercent,
//                        Player = match.TeamA,
//                        Opponent = match.TeamB,
//                        ScorePlayer = match.TeamAScore,
//                        ScoreTeam = match.TeamAScore,
//                        ScoreOpponent = match.TeamBScore,
//                        Map = match.Map,
//                        Date = match.DateTime,
//                        TeamAPlayerNames = teamAPlayerNames,
//                        TeamBPlayerNames = teamBPlayerNames,
//                        TeamAScores = match.TeamAScores,
//                        TeamBScores = match.TeamBScores,
//                        PlayerNameLookupTable = playerNameLookup,
//                    });

//                    participantB.Matches.Add(new ParticipantMatchModel
//                    {
//                        Percent = match.TeamBPercent,
//                        Player = match.TeamB,
//                        Opponent = match.TeamA,
//                        ScorePlayer = match.TeamBScore,
//                        ScoreTeam = match.TeamBScore,
//                        ScoreOpponent = match.TeamAScore,
//                        Map = match.Map,
//                        Date = match.DateTime,
//                        TeamAPlayerNames = teamBPlayerNames,
//                        TeamBPlayerNames = teamAPlayerNames,
//                        TeamAScores = match.TeamBScores,
//                        TeamBScores = match.TeamAScores,
//                        PlayerNameLookupTable = playerNameLookup,
//                    });
//                }
//            }

//            return playerResults;
//        }

//        public static MatchResult[] FilterMatches(MatchResult[] matches, bool teamOnly, int minMatchCount)
//        {
//            Dictionary<string, int> playerMatchCounts = new Dictionary<string, int>();

//            List<MatchResult> filteredMatches = new List<MatchResult>();

//            for (int i = 0; i < matches.Length; ++i)
//            {
//                MatchResult match = matches[i];

//                //HANDLE 4v4 data
//                if (match.PlayerCount > 1 && teamOnly == false)
//                {
//                    for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
//                    {
//                        AddPlayerMatch(playerMatchCounts, match.TeamAPlayerNames[x]);
//                        AddPlayerMatch(playerMatchCounts, match.TeamBPlayerNames[x]);
//                    }
//                }
//                else //otherwise 1on1 or Team data
//                {
//                    AddPlayerMatch(playerMatchCounts, match.TeamA);
//                    AddPlayerMatch(playerMatchCounts, match.TeamB);
//                }
//            }

//            for (int i = 0; i < matches.Length; ++i)
//            {
//                MatchResult match = matches[i];

//                bool addMatch = true;

//                //HANDLE 4v4 data
//                if (match.PlayerCount > 1 && teamOnly == false)
//                {
//                    for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
//                    {
//                        if (playerMatchCounts[match.TeamAPlayerNames[x]] < minMatchCount
//                            || playerMatchCounts[match.TeamBPlayerNames[x]] < minMatchCount)
//                        {
//                            addMatch = false;
//                            break;
//                        }
//                    }
//                }
//                else //otherwise 1on1 or Team data
//                {
//                    if (playerMatchCounts[match.TeamA] < minMatchCount
//                        || playerMatchCounts[match.TeamB] < minMatchCount)
//                    {
//                        addMatch = false;
//                    }
//                }

//                if (addMatch == true)
//                {
//                    filteredMatches.Add(match);
//                }
//            }

//            return filteredMatches.ToArray();
//        }

//        private static void AddPlayerMatch(Dictionary<string, int> playerMatchCounts, string playerName)
//        {
//            if (playerMatchCounts.ContainsKey(playerName) == false)
//            {
//                playerMatchCounts[playerName] = 0;
//            }
//            playerMatchCounts[playerName]++;
//        }
    }
}
