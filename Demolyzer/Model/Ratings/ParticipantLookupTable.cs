using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model.Ratings
{
    public class ParticipantLookupTable
    {
        //the player lookup table
        private Dictionary<string, ParticipantModel> _playerResults;
        
        //table for looking up a player name via Id
        private Dictionary<int, string> _playerNameLookup;

        //table for looking up a player Id via name
        private Dictionary<string, int> _playerIdLookup;

        //set of all players
        private HashSet<string> _players;

        public ParticipantLookupTable()
        {
            this._playerResults = new Dictionary<string, ParticipantModel>();
            this._playerNameLookup = new Dictionary<int, string>();
            this._playerIdLookup = new Dictionary<string, int>();
            this._players = new HashSet<string>();
        }

        public ParticipantModel GetParticipant(string name)
        {
            return this._playerResults[name];
        }

        public ParticipantModel[] GetParticipants()
        {
            return this._playerResults.Values.ToArray();
        }

        private void Initialize(List<MatchResult> matches, bool teamOnly)
        {
            for (int i = 0; i < matches.Count; ++i)
            {
                MatchResult match = matches[i];

                //if (match.TeamAPlayerNames != null)
                {
                    for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                    {
                        string playerA = match.TeamAPlayerNames[x];
                        string playerB = match.TeamBPlayerNames[x];

                        //if a player has not yet been added to the set, then we can add them to the name and Id lookup
                        if (this._players.Add(playerA) == true)
                        {
                            this._playerIdLookup[playerA] = this._playerIdLookup.Count;
                            this._playerNameLookup[this._playerNameLookup.Count] = playerA;
                        }

                        //if a player has not yet been added to the set, then we can add them to the name and Id lookup
                        if (this._players.Add(playerB) == true)
                        {
                            this._playerIdLookup[playerB] = this._playerIdLookup.Count;
                            this._playerNameLookup[this._playerNameLookup.Count] = playerB;
                        }
                    }
                }
            }

            //build participant model lookup

            //if participants models are teams
            if (teamOnly == true)
            {
                for (int i = 0; i < matches.Count; ++i)
                {
                    MatchResult match = matches[i];

                    ParticipantModel participantA = null;
                    ParticipantModel participantB = null;
                    if (this._playerResults.TryGetValue(match.TeamA, out participantA) == false)
                    {
                        participantA = new ParticipantModel(match.TeamA);
                        this._playerResults.Add(match.TeamA, participantA);
                    }
                    if (this._playerResults.TryGetValue(match.TeamB, out participantB) == false)
                    {
                        participantB = new ParticipantModel(match.TeamB);
                        this._playerResults.Add(match.TeamB, participantB);
                    }
                }
            }
            else //otherwise participants are players
            {
                for (int i = 0; i < matches.Count; ++i)
                {
                    MatchResult match = matches[i];

                    for (int x = 0; x < match.TeamAPlayerNames.Length; ++x)
                    {
                        string playerA = match.TeamAPlayerNames[x];
                        string playerB = match.TeamBPlayerNames[x];

                        ParticipantModel participantA = null;
                        ParticipantModel participantB = null;
                        if (this._playerResults.TryGetValue(playerA, out participantA) == false)
                        {
                            participantA = new ParticipantModel(playerA);
                            this._playerResults.Add(playerA, participantA);
                        }
                        if (this._playerResults.TryGetValue(playerB, out participantB) == false)
                        {
                            participantB = new ParticipantModel(playerB);
                            this._playerResults.Add(playerB, participantB);
                        }
                    }
                }
            }
        }

        public static ParticipantLookupTable Create(List<MatchResult> matches, bool teamOnly)
        {
            ParticipantLookupTable lookupTable = new ParticipantLookupTable();
            lookupTable.Initialize(matches, teamOnly);
            return lookupTable;
        }
    }
}
