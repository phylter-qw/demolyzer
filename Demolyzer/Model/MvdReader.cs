//#define LOGFRAGS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Media3D;
using System.Threading;
using System.Text.RegularExpressions;

namespace Demolyzer.Model
{
    public class MvdReader
    {
        private const byte DemoMessageMultiple = 3;
        private const byte DemoMessageSingle = 4;
        private const byte DemoMessageStats = 5;
        private const byte DemoMessageAll = 6;

        private double _demoTime;
        private MsgType _lastMsg;
        private CommandIds _lastCommand;
        private uint _lastPlayer;
#if LOG
        private StringBuilder _log;
#endif
        private bool _isMatchStarted;

        public enum MsgType : byte
        {
            Cmd = 0,
            Read = 1,
            Set = 2,
            Multiple = 3,
            Single = 4,
            Stats = 5,
            All = 6
        };

        public MvdReader()
        {
        }

        private string[] _playerNames;
        private List<string> _modelList;

        private DemoContent _demoDeltaContent;

        private int _packetIndex;
        private bool _isCanceled;

        public void Cancel()
        {
            this._isCanceled = true;
        }

        public bool IsCanceled
        {
            get
            {
                return this._isCanceled;
            }
        }

        private bool _isGrabContent;

        public DemoContent Read(string fileName, bool isGrabContent)
        {
            this._isGrabContent = isGrabContent;
            //this._match = match;
            //this._players = match.Players;
            this._demoDeltaContent = new DemoContent();
            FileInfo info = new FileInfo(fileName);
            this._demoDeltaContent.MvdName = info.Name;
            this._demoDeltaContent.MvdFullName = info.FullName;

#if LOG
            if (File.Exists("out.log"))
            {
                File.Delete("out.log");
            }

            this._log = new StringBuilder(10000000);
#endif

            try
            {
                this._playerNames = new string[32];
                this._fragInfo = new List<FragInfo>();
                this._modelList = new List<string>();

                byte[] data = File.ReadAllBytes(fileName);

                using (MemoryStream ms = new MemoryStream(data))
                using (BinaryReader br = new BinaryReader(ms))
                {
                    while (this._isCanceled == false && br.BaseStream.Position < br.BaseStream.Length)
                    {
                        byte demoTime = br.ReadByte();
                        this._demoTime += (double)demoTime * 0.001d;
                        byte msg = br.ReadByte();
                        MsgType msgType = (MsgType)(msg & 7);
                        this._lastMsg = msgType;

                        if (msgType == MsgType.Multiple)
                        {
                            ms.Seek(4, SeekOrigin.Current);
                            uint count = br.ReadUInt32();
                            ms.Seek(count, SeekOrigin.Current);
                            continue;
                        }

                        switch (msgType)
                        {
                            //case MsgType.Cmd:
                            //    throw new InvalidOperationException("Only in QWD");
                            case MsgType.Read:
                                break;
                                //throw new InvalidOperationException("TODO");
                            //case MsgType.Set:
                            //    throw new InvalidOperationException("TODO");
                            //case MsgType.Multiple:
                            //    uint playerBits = br.ReadUInt32();
                            //    break;
                            case MsgType.Single:
                            case MsgType.Stats:
                                this._lastPlayer = (uint)(msg >> 3);
                                msgType = MsgType.Read;
                                break;
                            case MsgType.All:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("msgType", msgType, "Invalid msg type");

                        }

                        //if (msgType == MsgType.Read)
                        {
                            uint msgSize = br.ReadUInt32();
                            ReadPacket(br, msgSize);
                        }

                        if (this._lastCommand == CommandIds.svc_disconnect)
                        {
                            break;
                        }
                    }
                }
                this._demoDeltaContent.PostProcess();
            }
            catch (Exception ex)
            {
#if LOG
                this._log.AppendLine(ex.ToString());
#endif

                //this._demoDeltaContent.IsInvalidMap
                this._demoDeltaContent.IsError = true;
            }
#if LOG
            File.WriteAllText("out.log", this._log.ToString());
#endif

            return this._demoDeltaContent;
        }

        private void ReadPacket(BinaryReader packetReader, uint len)
        {
            uint endPos = (uint)packetReader.BaseStream.Position + len;

            //try to read more if there is data left in the current packet
            do
            {
                //String str = ASCIIEncoding.ASCII.GetString(packet);
                ReadNext(packetReader);
                this._packetIndex++;
            }
            while (packetReader.BaseStream.Position < endPos && this._lastCommand != CommandIds.svc_disconnect);
        }

        private void ReadNext(BinaryReader packetReader)
        {
            CommandIds cmdId = (CommandIds)packetReader.ReadByte();
            this._lastCommand = cmdId;

            switch (cmdId)
            {
                case CommandIds.svc_playerinfo:
                    ReadPlayerInfo(packetReader);
                    break;
                case CommandIds.svc_packetentities:
                    ReadPacketEntities(packetReader, false);
                    break;
                case CommandIds.svc_updatestat:
                    ReadUpdateStat(packetReader);
                    break;
                case CommandIds.svc_temp_entity:
                    ReadTempEntity(packetReader);
                    break;
                case CommandIds.svc_serverdata:
                    ReadServerData(packetReader);
                    break;
                case CommandIds.svc_soundlist:
                    ReadSoundList(packetReader);
                    break;
                case CommandIds.svc_modellist:
                    ReadModelList(packetReader, false);
                    break;
                case CommandIds.svc_modellistshort:
                    ReadModelList(packetReader, true);
                    break;
                case CommandIds.svc_spawnstaticsound:
                    ReadSpawnStaticSound(packetReader);
                    break;
                case CommandIds.svc_spawnbaseline:
                    ReadSpawnBaseline(packetReader);
                    break;
                case CommandIds.svc_spawnbaseline2:
                    ReadSpawnBaseline2(packetReader);
                    break;
                case CommandIds.svc_spawnstatic:
                    ReadSpawnStatic(packetReader);
                    break;
                case CommandIds.svc_spawnstatic2:
                    ReadSpawnStatic2(packetReader);
                    break;
                case CommandIds.svc_updatefrags:
                    ReadUpdateFrags(packetReader);
                    break;
                case CommandIds.svc_updateping:
                    ReadUpdatePing(packetReader);
                    break;
                case CommandIds.svc_updatepl:
                    ReadUpdatePl(packetReader);
                    break;
                case CommandIds.svc_updatestatlong:
                    ReadUpdateStatLong(packetReader);
                    break;
                case CommandIds.svc_lightstyle:
                    ReadLightStyle(packetReader);
                    break;
                case CommandIds.svc_deltapacketentities:
                    ReadPacketEntities(packetReader, true);
                    break;
                case CommandIds.svc_centerprint:
                    ReadCenterPrint(packetReader);
                    break;
                case CommandIds.svc_print:
                    ReadPrint(packetReader);
                    break;
                case CommandIds.svc_stufftext:
                    ReadStuffText(packetReader);
                    break;
                case CommandIds.svc_sound:
                    ReadSound(packetReader);
                    break;
                case CommandIds.svc_setinfo:
                    ReadSetInfo(packetReader);
                    break;
                case CommandIds.nq_svc_cutscene:
                    //TODO
                    break;
                case CommandIds.svc_foundsecret:
                    //nothing to read
                    break;
                case CommandIds.svc_damage:
                    ReadDamage(packetReader);
                    break;
                case CommandIds.svc_bigkick:
                    ReadBigKick(packetReader);
                    break;
                case CommandIds.svc_serverinfo:
                    ReadServerInfo(packetReader);
                    break;
                case CommandIds.svc_chokecount:
                    ReadChokeCount(packetReader);
                    break;
                case CommandIds.svc_updateentertime:
                    ReadUpdateEnterTime(packetReader);
                    break;
                case CommandIds.svc_cdtrack:
                    ReadCDTrack(packetReader);
                    break;
                case CommandIds.svc_updateuserinfo:
                    ReadUpdateUserInfo(packetReader);
                    break;
                case CommandIds.svc_setangle:
                    ReadSetAngle(packetReader);
                    break;
                case CommandIds.svc_muzzleflash:
                    ReadMuzzleFlash(packetReader);
                    break;
                case CommandIds.svc_intermission:
                    ReadIntermission(packetReader);
                    break;
                case CommandIds.svc_disconnect:
                    ProcessEndGameMessages();
                    break;
                default:
                    break;
            }
        }

        // When a user drops, the following happens:
        // If player #8 drops, then a ghost is created in spot #9 with the same name
        // when the player rejoins with same name, he goes back into spot #8
        // then spot #9's name is cleared
        private void ReadUpdateUserInfo(BinaryReader reader)
        {
            uint playerNum = reader.ReadByte();
            uint userId = reader.ReadUInt32();
            string userInfo = reader.ReadStringSimple();

            //if (this._packetIndex > 164900)
            //{
            //    int asdf = 0;
            //}

            if (String.IsNullOrEmpty(userInfo) || userInfo[0] == '\0')
            {
                return;
            }

            //remove initial \
            userInfo = userInfo.Substring(userInfo.IndexOf("\\") + 1);

            string[] infos = userInfo.Split('\\');
            Dictionary<string, string> infoDictionary = new Dictionary<string, string>();
            for (int i = 0; i < infos.Length; i += 2)
            {
                infoDictionary[infos[i]] = infos[i + 1];
            }

#if GHOSTLOGIC
            //if this is a player rejoining
            if (this._isMatchStarted == true && userInfo.Contains("spectator") == false && this._ghostPlayer != -1)
            {
                this._ghostOldPlayerNum = (uint)this._ghostPlayer;
                this._ghostPlayer = (int)playerNum;
                return;
            }
#else
            //if this is a player dropping or rejoining mid game, do nothing with the info
            if (this._isMatchStarted == true)
            {
                return;
            }
#endif

            if (infoDictionary.ContainsKey("name") == true)
            {
                this._playerNames[playerNum] = infoDictionary["name"];
                this._demoDeltaContent.AddUserInfo(this._demoTime, playerNum, infoDictionary);
            }

            //foreach (var kvp in infoDictionary)
            //{
            //    this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Key:{4} Value:{5}\r\n", this._demoTime, this._lastMsg, this._lastCommand, playerNum, kvp.Key, kvp.Value));
            //}

            //// A player just timed out, this userinfo will be the ghost that is created.
            //// HACK: Checking for ghosts this way is somewaht of a hack, but I can't come up with a better solution, don't have enough info :(
            //if (mvd->serverinfo.player_timed_out)
            //{
            //    Sys_PrintDebug(1, "svc_updateuserinfo: Saving ghost info\n");
        
            //    if (mvd->serverinfo.player_timout_frame != mvd->frame_count)
            //    {
            //        Sys_PrintDebug(1, "svc_updateuserinfo: WARNING: Ghost userinfo was not sent the same frame as the user left, might label player as a ghost incorrectly!\n");
            //    }

            //    mvd->serverinfo.player_timed_out = false;

            //    player->is_ghost = true;
            //}
        }

        private uint AdjustForGhost(uint playerNum)
        {
#if GHOSTLOGIC
            if (playerNum == this._ghostPlayer)
            {
                return (uint)this._ghostOldPlayerNum;
            }
#endif
            return playerNum;
        }

        private void ReadSetInfo(BinaryReader reader)
        {
            uint playerNum = AdjustForGhost(reader.ReadByte());
            string key = reader.ReadStringSimple();
            string value = reader.ReadStringSimple();

            Dictionary<string, string> infoDictionary = new Dictionary<string, string>();
            infoDictionary[key] = value;

            //if the user changes his name before match started
            if (key == "name")
            {
                this._playerNames[playerNum] = value;
            }

            this._demoDeltaContent.AddSetInfo(this._demoTime, playerNum, infoDictionary);

            //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Key:{4} Value:{5}\r\n", this._demoTime, this._lastMsg, this._lastCommand, playerNum, key, value));
        }

        private void ReadUpdateStatLong(BinaryReader reader)
        {
            //if (this._isMatchStarted == false)
            //{
            //    reader.SeekToEnd();
            //    return;
            //}
            uint stat = reader.ReadByte();
            uint value = reader.ReadUInt32();
            if (this._isMatchStarted == true)
            {
                HandleUpdateStat((Stat)stat, value);
            }
        }

        private void ReadUpdateStat(BinaryReader reader)
        {
            //if (this._isMatchStarted == false)
            //{
            //    reader.SeekToEnd();
            //    return;
            //}
            uint stat = reader.ReadByte();
            uint value = reader.ReadByte();
            if (this._isMatchStarted == true)
            {
                HandleUpdateStat((Stat)stat, value);
            }
        }

        private void HandleUpdateStat(Stat stat, uint value)
        {
            uint player = AdjustForGhost(this._lastPlayer);

            // NOTE: Since ammo stats can come before 'active weapon' is changed (for people that use weapon scripts), 
            // we will make an assumption  on the active weapon based upon the type of ammo decremented. 
            // For example, if rockets decreases, we will assume player has RL selected. If the assumption is wrong it
            // just means our data will be 'incorrect' by 1 shot. The next packet will properly set ActiveWeapon.
            // In a nutshell, everytime a player shoots a GL using a script, it is possible these shots will be counted
            // as RL shots.
            switch (stat)
            {
                case Stat.STAT_HEALTH:
                    int health = (int)value;
                    if (health < 0)
                    {
                        health = 0;
                        FragInfo fragInfo = DequeueFragInfoDeath(player);
                        if (fragInfo != null)
                        {
                            this._demoDeltaContent.AddFragInfo(this._demoTime, fragInfo);
                        }

#if LOGFRAGS
                        StatValue statValue = (StatValue)value;
                        this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Stat:{4} Value:{5}\r\n", this._demoTime, this._lastMsg, this._lastCommand, this._lastPlayer, stat, statValue));
#endif
                    }
                    //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Stat:{4} Value:{5}\r\n", this._demoTime, this._lastMsg, this._lastCommand, this._lastPlayer, stat, value));
                    this._demoDeltaContent.AddHealth(this._demoTime, player, health);
                    break;
                case Stat.STAT_ARMOR:
                    //if the armor is higher, then the player picked up armor
                    //this method is used instead of using STAT_ITEMS since STAT_ITEMS does not
                    //give us updates if the user picks up the same type of armor as what they already have
                    this._demoDeltaContent.AddArmorAcquired(this._demoTime, player, value);
                    //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Stat:{4} Value:{5}\r\n", this._demoTime, this._lastMsg, this._lastCommand, this._lastPlayer, stat, value));

                    break;
                case Stat.STAT_SHELLS:
                case Stat.STAT_NAILS:
                case Stat.STAT_ROCKETS:
                case Stat.STAT_CELLS:
                case Stat.STAT_ACTIVEWEAPON:
                case Stat.STAT_ITEMS:
                    this._demoDeltaContent.AddStat(this._demoTime, player, stat, value);
                    break;
                case Stat.STAT_AMMO:
                case Stat.STAT_WEAPON:
                case Stat.STAT_WEAPONFRAME:
                case Stat.STAT_TOTALSECRETS:
                case Stat.STAT_TOTALMONSTERS:
                case Stat.STAT_SECRETS:
                case Stat.STAT_MONSTERS:
                case Stat.STAT_VIEWHEIGHT:
                case Stat.STAT_TIME:
                default:
                    return;
            }
            //if (stat == Stat.STAT_WEAPON || stat == Stat.STAT_ACTIVEWEAPON || stat == Stat.STAT_ITEMS)
            //{
            //    StatValue statValue = (StatValue)value;
            //    this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Stat:{4} Value:{5}\r\n", this._demoTime, this._lastMsg, this._lastCommand, this._lastPlayer, stat, statValue));
            //}
            //else
            //{
            //    this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Stat:{4} Value:{5}\r\n", this._demoTime, this._lastMsg, this._lastCommand, this._lastPlayer, stat, value));
            //}
        }

        private FragInfo DequeueFragInfoDeath(uint player)
        {
            FragInfo fragInfo = null;
            foreach (FragInfo info in this._fragInfo)
            {
                if (info.Death == player)
                {
                    fragInfo = info;
                    break;
                }
            }
            //if the person that died was unable to be found, it means there was a teammate kill
            //Which means the fragInfo's Death property will be -1 since the teammate that died was
            //unknown when the frag happened "XXX mows down a teammate"  (teammate is unknown)
            if (fragInfo == null)
            {
                //so just get the first fraginfo with a death of -1 since that should be a teammate
                //NOTE: there could be multiple! but that is ok just handle one at a time
                foreach (FragInfo info in this._fragInfo)
                {
                    if (info.Death == -1)
                    {
                        fragInfo = info;
                        fragInfo.Death = (int)player; //Update the fraginfo with the playernum that died
                        break;
                    }
                }
            }

            this._fragInfo.Remove(fragInfo);

            return fragInfo;
        }

        private void ReadDamage(BinaryReader reader)
        {
            byte armor = reader.ReadByte();
            byte blood = reader.ReadByte();

            this._demoDeltaContent.AddDamage(this._demoTime, AdjustForGhost(this._lastPlayer), blood, armor);

            Vector3D from = new Vector3D();
            from.X = reader.ReadCoord();
            from.Y = reader.ReadCoord();
            from.Z = reader.ReadCoord();

            //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Armor:{4} Blood:{5} Coord:{6}\r\n", this._demoTime, this._lastMsg, this._lastCommand, this._lastPlayer, armor, blood, from.ToString()));
        }

        private void ReadUpdateFrags(BinaryReader reader)
        {
            uint playerNum = AdjustForGhost(reader.ReadByte());
            int score = reader.ReadInt16();

            //if (this._packetIndex > 164900)
            //{
            //    int asdf = 0;
            //}

#if GHOSTLOGIC
            //do not handle 0 score packets for when a player drops/rejoins
            if (playerNum == this._ghostOldPlayerNum && score == 0)
            {
                return;
            }
#endif
            if (this._isGameOver == false)
            {
                this._demoDeltaContent.AddScore(this._demoTime, playerNum, score);
            }
#if LOGFRAGS
            this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Frags:{4}\r\n", this._demoTime, this._lastMsg, this._lastCommand, playerNum, score));
#endif
        }

        private void ReadUpdatePing(BinaryReader reader)
        {
            uint playerNum = AdjustForGhost(reader.ReadByte());
            uint ping = reader.ReadUInt16();

            //ignore pings higher than 900 (such as 999 or bad readings from ghost players)
            if (ping > 900)
            {
                return;
            }
            this._demoDeltaContent.AddPing(this._demoTime, playerNum, ping);
            LogSimpleCommand();
        }

        private void ReadUpdatePl(BinaryReader reader)
        {
            uint playerNum = AdjustForGhost(reader.ReadByte());
            uint pl = reader.ReadByte();
            this._demoDeltaContent.AddPL(this._demoTime, playerNum, pl);
            LogSimpleCommand();
        }

        private readonly string[] SupportedMaps =
        {
            "dm2",
            "dm3",
            "dm4",
            "dm6",
            "aerowalk",
            "ztndm3",
            "e1m2",
            "a2",
            "cmt3",
            "cmt4",
            "schloss"
        };

        private void ReadStuffText(BinaryReader reader)
        {
            string stuffText = reader.ReadStringSimple();

            if (stuffText.Contains("fullserverinfo") == true)
            {
                string fullServerInfo = stuffText.Substring(stuffText.IndexOf("\\") + 1);
                string[] infos = fullServerInfo.Split('\\');
                Dictionary<string, string> infoDictionary = new Dictionary<string, string>();
                for (int i = 0; i < infos.Length; i += 2)
                {
                    infoDictionary[infos[i]] = infos[i + 1];
                }
                string map = infoDictionary.GetValue("map");
                if (!SupportedMaps.Contains(map))
                {
                    this._demoDeltaContent.ServerInfo.IsInvalidMap = true;
                }
                Func<string, string> GetInfo = key => infoDictionary.ContainsKey(key) ? infoDictionary[key] : "Unavailable";
                this._demoDeltaContent.ServerInfo.Map = map;
                this._demoDeltaContent.ServerInfo.Fraglimit = GetInfo("fraglimit");
                this._demoDeltaContent.ServerInfo.Watervis = GetInfo("watervis");
                this._demoDeltaContent.ServerInfo.Antilag = GetInfo("sv_antilag");
                this._demoDeltaContent.ServerInfo.Version = GetInfo("*version");
                this._demoDeltaContent.ServerInfo.Timelimit = GetInfo("timelimit");
                this._demoDeltaContent.ServerInfo.Teamplay = GetInfo("teamplay");
                int deathmatch = 1;
                Int32.TryParse(infoDictionary.GetValue("deathmatch"), out deathmatch);
                this._demoDeltaContent.ServerInfo.Deathmatch = deathmatch;
                this._demoDeltaContent.ServerInfo.MaxClients = GetInfo("maxclients");
                this._demoDeltaContent.ServerInfo.MaxSpectators = GetInfo("maxspectators");
                this._demoDeltaContent.ServerInfo.GameDir = GetInfo("*gamedir");
                this._demoDeltaContent.ServerInfo.MaxFPS = GetInfo("maxfps");
                this._demoDeltaContent.ServerInfo.Hostname = GetInfo("hostname");
                this._demoDeltaContent.ServerInfo.KTXVersion = GetInfo("ktxver");
                this._demoDeltaContent.ServerInfo.KTXBuild = GetInfo("ktxbuild");
                this._demoDeltaContent.ServerInfo.ServerDemo = GetInfo("serverdemo").TrimEnd("\"_".ToCharArray());
                this._demoDeltaContent.AddServerInfo(this._demoTime, this._demoDeltaContent.ServerInfo);
                if (this._demoDeltaContent.ServerInfo.IsInvalidMap == true)
                {
                    throw new InvalidOperationException("Unsupported map");
                }
            }
            //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Text:{4}\r\n", this._demoTime, this._lastMsg, this._lastCommand, this._lastPlayer, stuffText));
        }

// Print flags
        public enum PrintLevel
        {
            PRINT_LOW = 0,		// Pickup messages
            PRINT_MEDIUM = 1,		// Death messages
            PRINT_HIGH = 2,		// Critical messages
            PRINT_CHAT = 3,		// Chat messages
        };

        private void ReadCenterPrint(BinaryReader reader)
        {
            string str = reader.ReadStringSimple();
            //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Text:{4}\r\n", this._demoTime, this._lastMsg, this._lastCommand, this._lastPlayer, str));
        }

        private List<string> _endGameMesages;
        private bool _isGameOver;
#if GHOSTLOGIC
        private int _ghostPlayer = -1;
        private uint _ghostOldPlayerNum;
#endif

        private double GetWeaponPercent(string msg, string weapon)
        {
            double weaponPercent = 0d;
            int index = msg.IndexOf(weapon);
            if (index >= 0)
            {
                index += weapon.Length;
                int indexEnd = msg.IndexOf("%", index);
                Double.TryParse(msg.Substring(index, indexEnd - index), out weaponPercent);
            }
            return weaponPercent;
        }

        private void ReadPrint(BinaryReader reader)
        {
            PrintLevel level = (PrintLevel)reader.ReadByte();
            string str = reader.ReadStringSimple();
            //System.Diagnostics.Debug.WriteLine(str);
#if LOG
            this._log.AppendLine(str);
#endif

            if (str.Contains("matchdate") == true)
            {
                this._demoDeltaContent.MatchStarted(this._demoTime);
                this._isMatchStarted = true;
            }
            if (level == PrintLevel.PRINT_MEDIUM)
            {
                ParseFragMessage(str);
#if LOGFRAGS
                this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Level:{3} Print:{4}\r\n", this._demoTime, this._lastMsg, this._lastCommand, level, str));
#endif
            }
            if (level == PrintLevel.PRINT_HIGH)
            {
                //if (str.Contains("timed out") == true)
                //{
                //    int asdf = 0;
                //}
                //left the game
                if (str.Contains("rejoins the game") == true)
                {
                    int asdf = 0;
                }
                if (str.Contains("left the game") == true && str.Contains("Spectator") == false)
                {
#if GHOSTLOGIC
                    if (this._ghostPlayer != -1)
                    {
                        this._demoDeltaContent.IsPlayerDropped = true;
                        throw new InvalidOperationException("More than 1 player dropped mid-game. Unable to analyze demo");
                    }
                    for (int i = 0; i < this._playerNames.Length; ++i)
                    {
                        if (this._playerNames[i] != null && str.Contains(this._playerNames[i]) == true)
                        {
                            this._ghostPlayer = i;
                            break;
                        }
                    }
#endif
                }
                if (str.Contains("The match is over") == true)
                {
                    this._isGameOver = true;
                    this._endGameMesages = new List<string>();
                }
                if (this._endGameMesages != null)
                {
                    this._endGameMesages.Add(str);
                }
                if (str.Contains("match statistics") == true)
                {
                }
                //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Level:{3} Print:{4}\r\n", this._demoTime, this._lastMsg, this._lastCommand, level, str));
            }

            //reader.SeekToEnd();


            //int level = MSG_ReadByte();
            //char str[MAX_INFO_STRING];

            //strlcpy(str, MSG_ReadString(), sizeof(str)); 

            //// TODO : Check for frag messages spread over several svc_prints older mods/servers does this crap :(

            //Sys_PrintDebug(5, "svc_print: (%s) RAW: %s\n", print_strings[level], str);
            //Sys_PrintDebug(1, "svc_print: (%s) %s\n", print_strings[level], Sys_RedToWhite(str));

            //// Parse frags.
            //Frags_Parse(mvd, str, level);

            //if (level == PRINT_HIGH)
            //{
            //    if (!strncmp(str, "matchdate:", 10))
            //    {
            //        // KTX
            //        struct tm t;

            //        // matchdate: Fri Nov 23, 16:33:46 2007
            //        // matchdate: 2007-11-23 17:12:44 CET
            //        // TODO: Parse timezone.
            //        if (strptime(str, "matchdate: %a %b %d, %X %Y", &t) || strptime(str, "matchdate: %Y-%m-%d %X ", &t))
            //        {
            //            mvd->match_start_date_full	= t;
            //            mvd->match_start_year		= t.tm_year + 1900;
            //            mvd->match_start_month		= t.tm_mon + 1;
            //            mvd->match_start_date		= t.tm_mday;
            //            mvd->match_start_hour		= t.tm_hour;
            //            mvd->match_start_minute		= t.tm_min;
            //        }
            //    }
            //    else if (!strncmp(str, "matchkey:", 9))
            //    {
            //        // KTPro
            //        struct tm t;
            //        char *s = str;

            //        // matchkey: 177-2006-3-19:23-27-20
            //        while (*s && (*s != '-'))
            //        {
            //            s++;
            //        }

            //        if (strptime(s, "-%Y-%m-%d:%H-%M", &t))
            //        {
            //            mvd->match_start_date_full	= t;
            //            mvd->match_start_year		= t.tm_year + 1900;
            //            mvd->match_start_month		= t.tm_mon + 1;
            //            mvd->match_start_date		= t.tm_mday;
            //            mvd->match_start_hour		= t.tm_hour;
            //            mvd->match_start_minute		= t.tm_min;
            //        }
            //    }
            //    else if (!strncmp(str, "The match is over", 17))
            //    {
            //        int i;

            //        mvd->serverinfo.match_ended = true;
            //        Log_Event(&logger, mvd, LOG_MATCHEND, -1);

            //        // HACK :(
            //        for (i = 0; i < MAX_PLAYERS; i++)
            //        {
            //            if (!PLAYER_ISVALID(&mvd->players[i]))
            //            {
            //                continue;
            //            }

            //            Log_Event(&logger, mvd, LOG_MATCHEND_ALL, i);
            //        }
            //    }
            //    else if (strstr(str, "overtime follows"))
            //    {
            //        char *s = str;
            //        while (*s && (*s < '0') && (*s > '9'))
            //        {
            //            s++;
            //        }

            //        mvd->serverinfo.overtime_minutes = atoi(str + 1);
            //    }
            //    else if (!strncmp(str, "time over, the game is a draw", 29))
            //    {
            //        mvd->serverinfo.match_overtime = true;
            //    }
            //    else if (strstr(str, "left the game with"))
            //    {
            //        // A player left the game in the middle of a match, we need to keep track
            //        // of the ghost player that is created straight after this event.
            //        mvd->serverinfo.player_timed_out = true;
            //        mvd->serverinfo.player_timout_frame	= mvd->frame_count;
            //        Sys_PrintDebug(1, "Warning: Player timed out!");
            //    }
            //}
        }

        private void ProcessEndGameMessages()
        {
            //if (this._endGameMesages == null)
            //{
            //    //clan arena
            //    return;
            //}
            //if they are cleared or processed previously, nothing will be done here
            for (int x = 0; x < this._endGameMesages.Count; ++x)
            {
                for (uint i = 0; i < this._playerNames.Length; ++i)
                {
                    if (this._playerNames[i] != null && this._endGameMesages[x].Contains(this._playerNames[i]) == true)
                    {
                        try
                        {
                            List<string> playerStats = this._endGameMesages.GetRange(x, 9);

                            double lgPercent = GetWeaponPercent(playerStats[1], "lg");
                            double rlPercent = GetWeaponPercent(playerStats[1], "rl");
                            double glPercent = GetWeaponPercent(playerStats[1], "gl");
                            double sgPercent = GetWeaponPercent(playerStats[1], "sg");
                            double ssgPercent = GetWeaponPercent(playerStats[1], "ssg");

                            double rlAvgDmg = 0d;
                            uint rlDirectHits = 0;

                            {
                                string msg = playerStats[2];
                                int indexAd = msg.IndexOf("ad:");
                                int indexdh = msg.IndexOf("dh:");
                                Double.TryParse(msg.Substring(indexAd + 3, indexdh - indexAd - 4), out rlAvgDmg);
                                UInt32.TryParse(msg.Substring(indexdh + 3, msg.Length - indexdh - 4), out rlDirectHits);
                            }
                            uint damageGiven = 0;
                            uint damageTaken = 0;
                            uint damageTeam = 0;
                            foreach (string msg in playerStats)
                            {
                                if (msg.Contains("Damage") == true)
                                {
                                    Match match;

                                    match = Regex.Match(msg, @"Tkn:\s*(\d+)");
                                    if (match.Success)
                                    {
                                        damageTaken = uint.Parse(match.Groups[1].Value);
                                    }

                                    match = Regex.Match(msg, @"Gvn:\s*(\d+)");
                                    if (match.Success)
                                    {
                                        damageGiven = uint.Parse(match.Groups[1].Value);
                                    }

                                    match = Regex.Match(msg, @"Tm:\s*(\d+)");
                                    if (match.Success)
                                    {
                                        damageTeam = uint.Parse(match.Groups[1].Value);
                                    }

                                    /*
                                    int indexTkn = msg.IndexOf("Tkn");
                                    int indexGvn = msg.IndexOf("Gvn");
                                    int indexTm = msg.IndexOf("Tm");
                                    UInt32.TryParse(msg.Substring(indexTkn + 4, indexGvn - indexTkn - 5), out damageTaken);
                                    UInt32.TryParse(msg.Substring(indexGvn + 4, indexTm - indexGvn - 5), out damageGiven);
                                    UInt32.TryParse(msg.Substring(indexTm + 3, msg.Length - indexTm - 4), out damageTeam);
                                    */

                                    this._demoDeltaContent.AddPlayerEndGameStats(this._demoTime, i, damageGiven, damageTaken, damageTeam, lgPercent, rlPercent, glPercent, sgPercent, ssgPercent, rlAvgDmg, rlDirectHits);
                                    break;
                                }
                            }
                        }
                        catch
                        {
#warning TODO (short names)
                        }
                    }
                }

            }
            //parse messages

            //sometimes "match statistics" is not at end of demo, so to detect that the endgame messages
            //were never processed, we will clear this list so that in the scenario where the message is missing, this list
            //will not be cleared, and we can thus process it at the end of the demo
            this._endGameMesages.Clear();
        }

        private List<FragInfo> _fragInfo;

        private void ParseFragMessage(string message)
        {
            /* HACK EXAMPLE DATA 
             * In the below data, the STAT_HEALTH death message is never received, so to hack around it,
             * we will check the age of any existing frag infos. If the fraginfo has 'expired' we will 
   653.621 All       svc_print           Level:PRINT_MEDIUM Print:whitey was gibbed by furbison's rocket_
   653.621 All       svc_updatefrags     Player:7 Frags:21
   653.621 Stats     svc_updatestatlong  Player:1 Stat:STAT_HEALTH Value:-45
   653.724 All       svc_print           Level:PRINT_MEDIUM Print:furbison chewed on BLooD_DoG(D_P)'s boomstick_
   653.724 All       svc_updatefrags     Player:2 Frags:14
   653.724 Stats     svc_updatestatlong  Player:7 Stat:STAT_HEALTH Value:-10
   657.178 All       svc_print           Level:PRINT_MEDIUM Print:furbison was gibbed by BLooD_DoG(D_P)'s rocket_
   657.178 All       svc_updatefrags     Player:2 Frags:15
   661.371 All       svc_print           Level:PRINT_MEDIUM Print:cyan1de was gibbed by serp's rocket_
   661.371 All       svc_updatefrags     Player:0 Frags:19
   662.412 All       svc_print           Level:PRINT_MEDIUM Print:flash was gibbed by Thump4's rocket_
   662.412 All       svc_updatefrags     Player:6 Frags:45
   662.412 Stats     svc_updatestatlong  Player:8 Stat:STAT_HEALTH Value:-73
   663.337 All       svc_print           Level:PRINT_MEDIUM Print:cyan1de rides up2's rocket_
   663.337 All       svc_updatefrags     Player:5 Frags:24
   663.337 Stats     svc_updatestatlong  Player:3 Stat:STAT_HEALTH Value:-10
             * */
            FragInfo expiredFragInfo = DequeueExpiredFragInfo();
            while (expiredFragInfo != null)
            {
                this._demoDeltaContent.AddFragInfo(this._demoTime, expiredFragInfo);
                expiredFragInfo = DequeueExpiredFragInfo();
            }

            int player1 = -1;
            int player2 = -1;
            for (int i = 0; i < this._playerNames.Length; ++i)
            {
                if (this._playerNames[i] != null)
                {
                    int index = message.IndexOf(this._playerNames[i]);
                    if (index >=0)
                    {
                        if (index == 0)
                        {
                            player1 = i;
                        }
                        else
                        {
                            player2 = i;
                        }
                    }
                    if (player1 != -1 && player2 != -1)
                    {
                        break;
                    }
                }
            }
            if (player1 == -1 && player2 == -1)
            {
                return;
                throw new InvalidOperationException("Unable to parse frag message");
            }

            FragInfo fragInfo = new FragInfo();
            if (message.Contains("chewed") == true) //whitey chewed on mikefails's boomstick_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_SHOTGUN;
            }
            else if (message.Contains("gibbed") == true) 
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                if (message.Contains("rocket") == true) //mikefails was gibbed by STEEK's rocket_
                {
                    fragInfo.Weapon = Weapon.IT_ROCKET_LAUNCHER;
                }
                if (message.Contains("grenade") == true) //kwong was gibbed by BLooD_DoG(D_P)'s grenade_
                {
                    fragInfo.Weapon = Weapon.IT_GRENADE_LAUNCHER;
                }
            }
            else if (message.Contains("rides")) //mikefails rides BLooD_DoG(D_P)'s rocket_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_ROCKET_LAUNCHER;
            }
            else if (message.Contains("squishes")) //Fastidious squishes whitey_
            {
                fragInfo.Fragger = player1;
                fragInfo.Death = player2;
            }
            else if (message.Contains("ax-murdered")) //flash was ax-murdered by veg3ta_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_AXE;
            }
            else if (message.Contains("lead")) //kwong was lead poisoned by BLooD_DoG(D_P)_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_SHOTGUN;
            }
            else if (message.Contains("loads")) //BLooD_DoG(D_P) ate 2 loads of 6. vegeta's buckshot_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                //fragInfo.Weapon = Weapon.IT_SUPER_SHOTGUN;
                fragInfo.Weapon = Weapon.IT_SHOTGUN;
            }
            else if (message.Contains("eats")) //mikefails eats cyan1de's pineapple_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_GRENADE_LAUNCHER;
            }
            else if (message.Contains("telefragged"))
            {
                if (message.Contains("teammate") == true) //kwong was telefragged by his teammate_
                {
#warning TODO Somehow find a way to figure out who did the telefrag
                    fragInfo.Death = player1;
                }
                else //Fastidious was telefragged by whitey_
                {
                    fragInfo.Fragger = player2;
                    fragInfo.Death = player1;
                }
            }
            else if (message.Contains("jumped"))
            {
                if (message.Contains("teammate") == true) //kwong was jumped by his teammate_
                {
#warning TODO Somehow find a way to figure out who did the jump
                    fragInfo.Death = player1;
                }
            }
            else if (message.Contains("pierced")) //6. vegeta was body pierced by BLooD_DoG(D_P)_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_NAILGUN;
            }
            else if (message.Contains("brutalized")) //6. vegeta was brutalized by STEEK's quad rocket_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_ROCKET_LAUNCHER;
            }
            else if (message.Contains("smeared")) //Fastidious was smeared by STEEK's quad rocket_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_ROCKET_LAUNCHER;
            }
            else if (message.Contains("rips")) //cyan1de rips 6. vegeta a new one_
            {
                fragInfo.Fragger = player1;
                fragInfo.Death = player2;
                fragInfo.Weapon = Weapon.IT_ROCKET_LAUNCHER;
            }
            else if (message.Contains("perforated")) //"rook was perforated by BLooD_DoG(D_P)_"
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_NAILGUN;
            }
            else if (message.Contains("nailed")) //STEEK was nailed by Fastidious_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_NAILGUN;
            }
            else if (message.Contains("punctured")) //STEEK was punctured by kwong_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_NAILGUN;
            }
            else if (message.Contains("accepts")) //whitey accepts Fastidious's shaft_
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_LIGHTNING;
            }
            else if (message.Contains("drains")) //"BLooD_DoG(D_P) drains rook's batteries_"
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_LIGHTNING;
            }
            else if (message.Contains("natural")) //"BLooD_DoG(D_P) gets a natural disaster from 6. vegeta_"
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_LIGHTNING;
            }
            else if (message.Contains("ventilated")) //"6. vegeta was ventilated by cyan1de_"
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_NAILGUN;
            }
            else if (message.Contains("straw")) //"BLooD_DoG(D_P) was straw-cuttered by Thump4_"
            {
                fragInfo.Fragger = player2;
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_NAILGUN;
            }
            

            // ########### TEAM KILLS ##################
            else if (message.Contains("mows")) //kwong mows down a teammate_
            {
                fragInfo.Fragger = player1;
#warning is this correct?
                fragInfo.Weapon = Weapon.IT_ROCKET_LAUNCHER;
            }
            else if (message.Contains("gets a frag")) //mikefails gets a frag for the other team_
            {
                fragInfo.Fragger = player1;
            }
            else if (message.Contains("loses")) //STEEK loses another friend_
            {
                fragInfo.Fragger = player1;
            }
            else if (message.Contains("checks")) //6. vegeta checks his glasses_
            {
                fragInfo.Fragger = player1;
            }
            else if (message.Contains("squished a teammate")) //"flash squished a teammate_"
            {
                fragInfo.Fragger = player1;
            }

            //################  SUICIDES/DEATHS ##############
            else if (message.Contains("discovers")) //STEEK discovers blast radius_
            {
                fragInfo.Death = player1;
                fragInfo.Weapon = Weapon.IT_ROCKET_LAUNCHER;
            }
            else if (message.Contains("burst")) //mikefails burst into flames_
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("visits")) //mikefails visits the Volcano God_
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("turned")) //Fastidious turned into hot slag_
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("tries")) //mikefails tries to put the pin back in_
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("bored")) //6. vegeta becomes bored with life_
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("discharges")) //"rook discharges into the water_"
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("was squished")) //"flash was squished_"
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("spiked")) //"cyan1de was spiked_"
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("cratered")) //"STEEK cratered_"
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("fell")) //"[tVS]Ihminen fell to his death_"
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("sleeps")) //"slabi sleeps with the fishes_"
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("sucks")) //"[tVS]XantoM sucks it down_"
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("suicides")) //"Rusty suicides_"
            {
                fragInfo.Death = player1;
            }
            else if (message.Contains("tried to leave")) //"kwong tried to leave_"
            {
                fragInfo.Death = player1;
            }

            else if (message.Contains("Satan")) //"Satan's power deflects [tVS]XantoM's telefrag_"
            {
                fragInfo.Death = player2;
            }
            
            else
            {
                //File.AppendAllText("frag.log", message + "\r\n");
                //comment out for clan arena
                throw new InvalidOperationException("Unable to parse frag message"); 
            }
            fragInfo.DemoTime = this._demoTime;
            this._fragInfo.Add(fragInfo);
#if MSG
    {WEAPON, 10, 1, "(.*) sleeps with the fishes", false},
    {WEAPON, 10, 1, "(.*) sucks it down", false},
    {WEAPON, 10, 1, "(.*) gulped a load of slime", false},
    {WEAPON, 10, 1, "(.*) can't exist on slime alone", false},
    {WEAPON, 10, 1, "(.*) burst into flames", false},
    {WEAPON, 10, 1, "(.*) turned into hot slag", false},
    {WEAPON, 10, 1, "(.*) visits the Volcano God", false},
    {WEAPON, 15, 1, "(.*) cratered", false},
    {WEAPON, 15, 1, "(.*) fell to his death", false},
    {WEAPON, 15, 1, "(.*) fell to her death", false},
    {WEAPON, 11, 1, "(.*) blew up", false},
    {WEAPON, 11, 1, "(.*) was spiked", false},
    {WEAPON, 11, 1, "(.*) was zapped", false},
    {WEAPON, 11, 1, "(.*) ate a lavaball", false},
    {WEAPON, 12, 1, "(.*) was telefragged by his teammate", false},
    {WEAPON, 12, 1, "(.*) was telefragged by her teammate", false},
    {WEAPON,  0, 1, "(.*) died", false},
    {WEAPON,  0, 1, "(.*) tried to leave", false},
    {WEAPON, 14, 1, "(.*) was squished", false},
    {WEAPON,  0, 1, "(.*) suicides", false},
    {WEAPON,  6, 1, "(.*) tries to put the pin back in", false},
    {WEAPON,  7, 1, "(.*) becomes bored with life", false},
    {WEAPON,  7, 1, "(.*) discovers blast radius", false},
    {WEAPON, 13, 1, "(.*) electrocutes himself.", false},
    {WEAPON, 13, 1, "(.*) electrocutes herself.", false},
    {WEAPON, 13, 1, "(.*) discharges into the slime", false},
    {WEAPON, 13, 1, "(.*) discharges into the lava", false},
    {WEAPON, 13, 1, "(.*) discharges into the water", false},
    {WEAPON, 13, 1, "(.*) heats up the water", false},
    {WEAPON, 16, 1, "(.*) squished a teammate", false},
    {WEAPON, 16, 1, "(.*) mows down a teammate", false},
    {WEAPON, 16, 1, "(.*) checks his glasses", false},
    {WEAPON, 16, 1, "(.*) checks her glasses", false},
    {WEAPON, 16, 1, "(.*) gets a frag for the other team", false},
    {WEAPON, 16, 1, "(.*) loses another friend", false},
    {WEAPON,  1, 2, "(.*) was ax-murdered by (.*)", false},
    {WEAPON,  2, 2, "(.*) was lead poisoned by (.*)", false},
    {WEAPON,  2, 2, "(.*) chewed on (.*)'s boomstick", false},
    {WEAPON,  3, 2, "(.*) ate 8 loads of (.*)'s buckshot", false},
    {WEAPON,  3, 2, "(.*) ate 2 loads of (.*)'s buckshot", false},
    {WEAPON,  4, 2, "(.*) was body pierced by (.*)", false},
    {WEAPON,  4, 2, "(.*) was nailed by (.*)", false},
    {WEAPON,  5, 2, "(.*) was perforated by (.*)", false},
    {WEAPON,  5, 2, "(.*) was punctured by (.*)", false},
    {WEAPON,  5, 2, "(.*) was ventilated by (.*)", false},
    {WEAPON,  5, 2, "(.*) was straw-cuttered by (.*)", false},
    {WEAPON,  6, 2, "(.*) eats (.*)'s pineapple", false},
    {WEAPON,  6, 2, "(.*) was gibbed by (.*)'s grenade", false},
    {WEAPON,  7, 2, "(.*) was smeared by (.*)'s quad rocket", false},
    {WEAPON,  7, 2, "(.*) was brutalized by (.*)'s quad rocket", false},
    {WEAPON,  7, 2, "(.*) rips (.*) a new one", true},
    {WEAPON,  7, 2, "(.*) was gibbed by (.*)'s rocket", false},
    {WEAPON,  7, 2, "(.*) rides (.*)'s rocket", false},
    {WEAPON,  8, 2, "(.*) accepts (.*)'s shaft", false},
    {WEAPON,  9, 2, "(.*) was railed by (.*)", false},
    {WEAPON, 12, 2, "(.*) was telefragged by (.*)", false},
    {WEAPON, 14, 2, "(.*) squishes (.*)", true},
    {WEAPON, 13, 2, "(.*) accepts (.*)'s discharge", false},
    {WEAPON, 13, 2, "(.*) drains (.*)'s batteries", false},
    {WEAPON,  8, 2, "(.*) gets a natural disaster from (.*)", false},
#endif
        }

        /// <summary>
        /// get a fraginfo that has been in the queue too long (means we never received a health 
        /// message indicating a player died
        /// </summary>
        private FragInfo DequeueExpiredFragInfo()
        {
            FragInfo fragInfo = null;
            foreach (FragInfo info in this._fragInfo)
            {
                if (this._demoTime - info.DemoTime > 0.200)
                {
                    fragInfo = info;
                    break;
                }
            }
            if (fragInfo != null)
            {
                this._fragInfo.Remove(fragInfo);
            }
            return fragInfo;
        }

        private void ReadPlayerInfo(BinaryReader reader)
        {
            uint playerNum = AdjustForGhost(reader.ReadByte());

            int flags = reader.ReadInt16();
            int frame = reader.ReadByte();

            Vector3D origin = new Vector3D();
            Vector3D viewAngle = new Vector3D();

            byte modelIndex = 0;
            byte skinNum = 0;
            byte effects = 0;
            byte weaponFrame = 0;

            if ((flags & (DF_ORIGIN << 0)) != 0)
            {
                origin.X = reader.ReadCoord();
            }
            if ((flags & (DF_ORIGIN << 1)) != 0)
            {
                origin.Y = reader.ReadCoord();
            }
            if ((flags & (DF_ORIGIN << 2)) != 0)
            {
                origin.Z = reader.ReadCoord();
            }

            //view angles
            if ((flags & (DF_ANGLES << 0)) != 0)
            {
                viewAngle.X = reader.ReadAngle16();
            }
            if ((flags & (DF_ANGLES << 1)) != 0)
            {
                viewAngle.Y = reader.ReadAngle16();
            }
            if ((flags & (DF_ANGLES << 2)) != 0)
            {
                viewAngle.Z = reader.ReadAngle16();
            }

            if ((flags & (DF_MODEL)) != 0)
            {
                modelIndex = reader.ReadByte();
            }

            if ((flags & (DF_SKINNUM)) != 0)
            {
                skinNum = reader.ReadByte();
            }

            if ((flags & (DF_EFFECTS)) != 0)
            {
                effects = reader.ReadByte();
            }

            if ((flags & (DF_WEAPONFRAME)) != 0)
            {
                weaponFrame = reader.ReadByte();
            }

            this._demoDeltaContent.AddPlayerMovement(this._demoTime, playerNum, origin.X, origin.Y);

            //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3} Pos:{4:F0},{5:F0},{6:F0} Angle:{7:F0},{8:F0}\r\n", this._demoTime, this._lastMsg, this._lastCommand, playerNum, origin.X, origin.Y, origin.Z, viewAngle.X, viewAngle.Y));
        }

        private void ReadServerData(BinaryReader reader)
        {
            uint version = reader.ReadUInt32();
            uint serverCount = reader.ReadUInt32();
            string gameDir = reader.ReadStringSimple();
            float demoTime = reader.ReadSingle();
            string mapName = reader.ReadStringSimple();

            float gravity = reader.ReadSingle();
            float stopspeed = reader.ReadSingle();
            float maxspeed = reader.ReadSingle();
            float spectatormaxspeed = reader.ReadSingle();
            float accelerate = reader.ReadSingle();
            float airaccelerate = reader.ReadSingle();
            float wateraccelerate = reader.ReadSingle();
            float friction = reader.ReadSingle();
            float waterfriction = reader.ReadSingle();
            float entgravity = reader.ReadSingle();

            //LogSimpleCommand();
        }

        private void ReadServerInfo(BinaryReader reader)
        {
            string key = reader.ReadStringSimple();
            string value = reader.ReadStringSimple();
            if (key == "serverdemo")
            {
                //try
                {
                    int index = value.IndexOf(".mvd") - 11;
                    //"4on4_mili_vs_-0-[dm3]091212-2150.mvd"
                    int day = Convert.ToInt32(value.Substring(index, 2));
                    int month = Convert.ToInt32(value.Substring(index + 2, 2));
                    int year = 2000 + Convert.ToInt32(value.Substring(index + 4, 2));
                    int hour = Convert.ToInt32(value.Substring(index + 7, 2));
                    int minute = Convert.ToInt32(value.Substring(index + 9, 2));
                    DateTime date = new DateTime(year, month, day, hour, minute, 0);
                    this._demoDeltaContent.Date = date;
                }
                //catch { } //clan arena
            }
            //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Key:{3} Value:{4}\r\n", this._demoTime, this._lastMsg, this._lastCommand, key, value));
        }

        public enum TempEntity
        {
            TE_SPIKE = 0,
            TE_SUPERSPIKE = 1,
            TE_GUNSHOT = 2,
            TE_EXPLOSION = 3,
            TE_TAREXPLOSION = 4,
            TE_LIGHTNING1 = 5,
            TE_LIGHTNING2 = 6,
            TE_WIZSPIKE = 7,
            TE_KNIGHTSPIKE = 8,
            TE_LIGHTNING3 = 9,
            TE_LAVASPLASH = 10,
            TE_TELEPORT = 11,
            TE_BLOOD = 12,
            TE_LIGHTNINGBLOOD = 13,
        };

        private void ReadTempEntity(BinaryReader reader)
        {
            TempEntity tempEntity = (TempEntity)reader.ReadByte();

            if (tempEntity == TempEntity.TE_GUNSHOT || tempEntity == TempEntity.TE_BLOOD)
            {
                reader.ReadByte();
            }

            if (tempEntity == TempEntity.TE_LIGHTNING1 || tempEntity == TempEntity.TE_LIGHTNING2 || tempEntity == TempEntity.TE_LIGHTNING3)
            {
                int entityNumber = reader.ReadUInt16(); // Entity number.

                // Start position (the other position is the end of the beam).
                Vector3D from = new Vector3D();
                from.X = reader.ReadCoord();
                from.Y = reader.ReadCoord();
                from.Z = reader.ReadCoord();
            }

            //entity coord
            Vector3D coord = new Vector3D();
            coord.X = reader.ReadCoord();
            coord.Y = reader.ReadCoord();
            coord.Z = reader.ReadCoord();

            //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Entity:{3} Pos:{4:F0},{5:F0},{6:F0}\r\n", this._demoTime, this._lastMsg, this._lastCommand, tempEntity, coord.X, coord.Y, coord.Z));
        }

        private void ReadIntermission(BinaryReader reader)
        {
            // Position.
            Vector3D coord = new Vector3D();
            coord.X = reader.ReadCoord();
            coord.Y = reader.ReadCoord();
            coord.Z = reader.ReadCoord();

            // View angle.
            Vector3D angle = new Vector3D();
            angle.X = reader.ReadAngle();
            angle.Y = reader.ReadAngle();
            angle.Z = reader.ReadAngle();

            this._demoDeltaContent.MatchComplete(this._demoTime);
        }

        private void ReadMuzzleFlash(BinaryReader reader)
        {
            uint playerNum = reader.ReadUInt16(); // Playernum.
            //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Player:{3}\r\n", this._demoTime, this._lastMsg, this._lastCommand, playerNum));
        }

        private void ReadSetAngle(BinaryReader reader)
        {
            uint playerNum = AdjustForGhost(reader.ReadByte());

            // View angles.
            Vector3D coord = new Vector3D();
            coord.X = reader.ReadAngle();
            coord.Y = reader.ReadAngle();
            coord.Z = reader.ReadAngle();
        }

        private void ReadCDTrack(BinaryReader reader)
        {
            reader.ReadByte();
        }

        private void ReadBigKick(BinaryReader reader)
        {
            // Nothing.

            //LogSimpleCommand();
        }

        private void ReadSpawnStatic(BinaryReader reader)
        {
            byte modelIndex = reader.ReadByte();
            byte frame = reader.ReadByte();
            byte colorMap = reader.ReadByte();
            byte skinNum = reader.ReadByte();

            Vector3D coord = new Vector3D();
            Vector3D angle = new Vector3D();
            coord.X = reader.ReadCoord();
            angle.X = reader.ReadAngle();
            coord.Y = reader.ReadCoord();
            angle.Y = reader.ReadAngle();
            coord.Z = reader.ReadCoord();
            angle.Z = reader.ReadAngle();
            //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}ModelIndex:{3} Coord:({4:F0},{5:F0},{6:F0})\r\n", this._demoTime, this._lastMsg, this._lastCommand, modelIndex, coord.X, coord.Y, coord.Z));
        }

        private void ReadSpawnStatic2(BinaryReader reader)
        {
            (_, uint bits, uint morebits) = ReadEntityNum(reader);
            ReadEntityDelta(reader, bits, morebits);            
        }

        private void ReadSpawnBaseline2(BinaryReader reader)
        {
            (uint entnum, uint bits, uint morebits) = ReadEntityNum(reader);
            if (entnum == 0)
            {
                return;
            }
            EntityDelta delta = ReadEntityDelta(reader, bits, morebits);
            Entity entity = new Entity();
            string model = this._modelList[(int)delta.model-1];
            switch (model)
            {
                case "progs/quaddama.mdl":
                    entity.Type = EntityType.Quad;
                    break;
                case "progs/invulner.mdl":
                    entity.Type = EntityType.Pent;
                    break;
                case "progs/g_shot.mdl":
                    entity.Type = EntityType.SuperShotgun;
                    break;
                //case "progs/g_nail.mdl":
                //    entity.Type = EntityType.NailGun;
                //    break;
                case "progs/g_rock.mdl":
                    entity.Type = EntityType.GrenadeLauncher;
                    break;
                case "progs/g_rock2.mdl":
                    entity.Type = EntityType.RocketLauncher;
                    break;
                case "progs/invisibl.mdl":
                    entity.Type = EntityType.Eyes;
                    break;
                case "progs/armor.mdl":
                    switch (delta.skin)
                    {
                        case 0:
                            entity.Type = EntityType.ArmorGA;
                            break;
                        case 1:
                            entity.Type = EntityType.ArmorYA;
                            break;
                        case 2:
                            entity.Type = EntityType.ArmorRA;
                            break;
                        default:
                            break;
                    }
                    break;
                case "maps/b_rock1.bsp":
                    entity.Type = EntityType.Rockets10;
                    break;
                case "maps/b_bh10.bsp":
                    entity.Type = EntityType.Health10;
                    break;
                case "maps/b_bh25.bsp":
                    entity.Type = EntityType.Health25;
                    break;
                case "maps/b_bh100.bsp":
                    entity.Type = EntityType.Mega;
                    break;
                case "maps/b_rock0.bsp":
                    entity.Type = EntityType.Rockets5;
                    break;
                case "progs/g_nail2.mdl":
                    entity.Type = EntityType.SuperNailgun;
                    break;
                case "progs/g_light.mdl":
                    entity.Type = EntityType.Lightning;
                    break;
                case "maps/b_batt0.bsp":
                    entity.Type = EntityType.Cells;
                    break;
                //case "maps/b_nail0.bsp":
                //    entity.Type = EntityType.Nails;
                //    break;
                default:
                    //do not add a model if it is not one of the ones we want to keep track of
                    return;
            }

            entity.DeathMatch = this._demoDeltaContent.ServerInfo.Deathmatch;
            entity.OffsetX = (double)delta.origin1;
            entity.OffsetY = (double)-delta.origin2; //NOTE: Must inverse Y since WPF (UI) uses higher numbers when going down screen

            this._demoDeltaContent.AddEntity(entity);
        }

        private void ReadSpawnBaseline(BinaryReader reader)
        {
            short entityIndex = reader.ReadInt16();
            int modelIndex = reader.ReadByte() - 1;
            byte frame = reader.ReadByte();
            byte colorMap = reader.ReadByte();
            byte skinNum = reader.ReadByte();

            Vector3D coord = new Vector3D();
            Vector3D angle = new Vector3D();
            coord.X = reader.ReadCoord();
            angle.X = reader.ReadAngle();
            coord.Y = reader.ReadCoord();
            angle.Y = reader.ReadAngle();
            coord.Z = reader.ReadCoord();
            angle.Z = reader.ReadAngle();

            //if (coord.X != 0 && coord.Y != 0 && coord.Z != 0)
            {
                Entity entity = new Entity();
                entity.OffsetX = coord.X;
                entity.OffsetY = -coord.Y; //NOTE: Must inverse Y since WPF (UI) uses higher numbers when going down screen
                entity.DeathMatch = this._demoDeltaContent.ServerInfo.Deathmatch;

                //0.000 All       svc_modellist       Index:70 Model:progs/quaddama.mdl      QUAD
                //0.000 All       svc_modellist       Index:71 Model:progs/invulner.mdl      PENT
                //0.000 All       svc_modellist       Index:72 Model:progs/g_shot.mdl	      SSG
                //0.000 All       svc_modellist       Index:73 Model:progs/g_nail.mdl        NAIL GUN
                //0.000 All       svc_modellist       Index:75 Model:progs/g_rock.mdl         GRENADE LAUNCHER
                //0.000 All       svc_modellist       Index:76 Model:progs/g_rock2.mdl        ROCKET LAUNCHER
                //0.000 All       svc_modellist       Index:78 Model:progs/invisibl.mdl       EYES
                //0.000 All       svc_modellist       Index:79 Model:progs/armor.mdl          ARMOR
                //0.000 All       svc_modellist       Index:80 Model:maps/b_rock1.bsp          ROCKETS 10
                //0.000 All       svc_modellist       Index:82 Model:maps/b_bh10.bsp          HEALTH 10
                //0.000 All       svc_modellist       Index:83 Model:maps/b_bh25.bsp          HEALTH 25
                //0.000 All       svc_modellist       Index:84 Model:maps/b_bh100.bsp         MEGA
                //0.000 All       svc_modellist       Index:85 Model:maps/b_rock0.bsp         ROCKETS 5
                //0.000 All       svc_modellist       Index:42 Model:progs/g_nail2.mdl      SUPER NAILGUN
                //0.000 All       svc_modellist       Index:45 Model:progs/g_light.mdl      LIGHTNING GUN
                //0.000 All       svc_modellist       Index:49 Model:maps/b_batt0.bsp       CELLS(6)
                //0.000 All       svc_modellist       Index:52 Model:maps/b_nail0.bsp       NAIL (5?)

                string model = this._modelList[modelIndex];
                switch (model)
                {
                    case "progs/quaddama.mdl":
                        entity.Type = EntityType.Quad;
                        break;
                    case "progs/invulner.mdl":
                        entity.Type = EntityType.Pent;
                        break;
                    case "progs/g_shot.mdl":
                        entity.Type = EntityType.SuperShotgun;
                        break;
                    //case "progs/g_nail.mdl":
                    //    entity.Type = EntityType.NailGun;
                    //    break;
                    case "progs/g_rock.mdl":
                        entity.Type = EntityType.GrenadeLauncher;
                        break;
                    case "progs/g_rock2.mdl":
                        entity.Type = EntityType.RocketLauncher;
                        break;
                    case "progs/invisibl.mdl":
                        entity.Type = EntityType.Eyes;
                        break;
                    case "progs/armor.mdl":
                        switch (skinNum)
                        {
                            case 0:
                                entity.Type = EntityType.ArmorGA;
                                break;
                            case 1:
                                entity.Type = EntityType.ArmorYA;
                                break;
                            case 2:
                                entity.Type = EntityType.ArmorRA;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "maps/b_rock1.bsp":
                        entity.Type = EntityType.Rockets10;
                        break;
                    case "maps/b_bh10.bsp":
                        entity.Type = EntityType.Health10;
                        break;
                    case "maps/b_bh25.bsp":
                        entity.Type = EntityType.Health25;
                        break;
                    case "maps/b_bh100.bsp":
                        entity.Type = EntityType.Mega;
                        break;
                    case "maps/b_rock0.bsp":
                        entity.Type = EntityType.Rockets5;
                        break;
                    case "progs/g_nail2.mdl":
                        entity.Type = EntityType.SuperNailgun;
                        break;
                    case "progs/g_light.mdl":
                        entity.Type = EntityType.Lightning;
                        break;
                    case "maps/b_batt0.bsp":
                        entity.Type = EntityType.Cells;
                        break;
                    //case "maps/b_nail0.bsp":
                    //    entity.Type = EntityType.Nails;
                    //    break;
                    default:
                        //do not add a model if it is not one of the ones we want to keep track of
                        return;
                }

                this._demoDeltaContent.AddEntity(entity);
            }

            //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Entity:{3} ModelIndex:{4} Coord:({5:F0},{6:F0},{7:F0})\r\n", this._demoTime, this._lastMsg, this._lastCommand, entityIndex, modelIndex, coord.X, coord.Y, coord.Z));
        }

        private void ReadSpawnStaticSound(BinaryReader reader)
        {
            Vector3D origin = new Vector3D();
            origin.X = reader.ReadCoord();
            origin.Y = reader.ReadCoord();
            origin.Z = reader.ReadCoord();

            byte number = reader.ReadByte();
            byte volume = reader.ReadByte();
            byte attenuation = reader.ReadByte();
            //LogSimpleCommand();
        }

        private void ReadSoundList(BinaryReader reader)
        {
            int soundIndex = reader.ReadByte();

            while (true)
            {
                string soundName = reader.ReadStringSimple();
                if (soundName.Length == 0)
                {
                    break;
                }
                soundIndex++;
            }
            reader.ReadByte();
            //LogSimpleCommand();
        }

        private void ReadModelList(BinaryReader reader, bool extended)
        {
            int val = extended ? reader.ReadUInt16() : reader.ReadByte();

            List<string> models = new List<string>();
            while (true)
            {
                string model = reader.ReadStringSimple();
                if (model.Length == 0)
                {
                    break;
                }
                else
                {
                    //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}Index:{3} Model:{4}\r\n", this._demoTime, this._lastMsg, this._lastCommand, models.Count, model));
                    this._modelList.Add(model);
                }
            }
            reader.ReadByte();

            //LogSimpleCommand();
        }

        private void ReadChokeCount(BinaryReader reader)
        {
            byte val = reader.ReadByte();
        }

        private void ReadLightStyle(BinaryReader reader)
        {
            int count = reader.ReadByte();
            string str = reader.ReadStringSimple();
            //MSG_ReadByte();		// Lightstyle count.
            //MSG_ReadString();	// Lightstyle string.

            //LogSimpleCommand();
            //reader.SeekToEnd();
        }

        // the first 16 bits of a packetentities update holds 9 bits of entity number and 7 bits of flags
        private const int U_ORIGIN1 = (1 << 9);
        private const int U_ORIGIN2 = (1 << 10);
        private const int U_ORIGIN3 = (1 << 11);
        private const int U_ANGLE2 = (1 << 12);
        private const int U_FRAME = (1 << 13);
        private const int U_REMOVE = (1 << 14);		// REMOVE this entity, don't add it
        private const int U_MOREBITS = (1 << 15);
        private const int U_ANGLE1 = (1 << 0);
        private const int U_ANGLE3 = (1 << 1);
        private const int U_MODEL = (1 << 2);
        private const int U_COLORMAP = (1 << 3);
        private const int U_SKIN = (1 << 4);
        private const int U_EFFECTS = (1 << 5);
        private const int U_SOLID = (1 << 6);		// the entity should be solid for prediction

        private const int DF_ORIGIN = 1;
        private const int DF_ANGLES = (1 << 3);
        private const int DF_EFFECTS = (1 << 6);
        private const int DF_SKINNUM = (1 << 7);
        private const int DF_DEAD = (1 << 8);
        private const int DF_GIB = (1 << 9);
        private const int DF_WEAPONFRAME = (1 << 10);
        private const int DF_MODEL = (1 << 11);

        private const int U_FTE_TRANS = (1 << 1);
        private const int U_FTE_MODELDBL = (1 << 3);
        private const int U_FTE_ENTITYDBL = (1 << 5);
        private const int U_FTE_ENTITYDBL2 = (1 << 6);
        private const int U_FTE_YETMORE = (1 << 7);
        private const int U_FTE_EVENMORE = (1 << 7);

        private const int U_FTE_COLOURMOD = (1 << 10);

        private (uint, uint, uint) ReadEntityNum(BinaryReader reader)
        {
            uint bits = reader.ReadUInt16();
            uint entnum = bits & 0x1FF;
            bits &= ~0x1FFU;
            uint morebits = 0;
            if ((bits & U_MOREBITS) != 0)
            {
                bits |= reader.ReadByte();
                if ((bits & U_FTE_EVENMORE) != 0)
                {
                    morebits = reader.ReadByte();
                    if ((morebits & U_FTE_YETMORE) != 0)
                    {
                        morebits |= (uint)reader.ReadByte() << 8;
                    }
                    if ((morebits & U_FTE_ENTITYDBL) != 0)
                    {
                        entnum += 512;
                    }
                    if ((morebits & U_FTE_ENTITYDBL2) != 0)
                    {
                        entnum += 1024;
                    }
                }
            }
            return (entnum, bits, morebits);
        }

        private struct EntityDelta
        {
            public ushort? model;
            public byte? frame;
            public byte? colormap;
            public byte? skin;
            public byte? effects;
            public float? origin1;
            public float? origin2;
            public float? origin3;
            public float? angle1;
            public float? angle2;
            public float? angle3;
            public byte? trans;
            public byte[] colourmod;
        }

        private EntityDelta ReadEntityDelta(BinaryReader reader, uint bits, uint morebits)
        {
            EntityDelta delta = new EntityDelta();

            if ((bits & U_MODEL) != 0)
                delta.model = reader.ReadByte();
            else if ((morebits & U_FTE_MODELDBL) != 0)
                delta.model = reader.ReadUInt16();

            if ((bits & U_FRAME) != 0)
                delta.frame = reader.ReadByte();
            if ((bits & U_COLORMAP) != 0)
                delta.colormap = reader.ReadByte();
            if ((bits & U_SKIN) != 0)
                delta.skin = reader.ReadByte();
            if ((bits & U_EFFECTS) != 0)
                delta.effects = reader.ReadByte();
            if ((bits & U_ORIGIN1) != 0)
                delta.origin1 = reader.ReadCoord();
            if ((bits & U_ORIGIN2) != 0)
                delta.origin2 = reader.ReadCoord();
            if ((bits & U_ORIGIN3) != 0)
                delta.origin3 = reader.ReadCoord();
            if ((bits & U_ANGLE1) != 0)
                delta.angle1 = reader.ReadAngle();
            if ((bits & U_ANGLE2) != 0)
                delta.angle2 = reader.ReadAngle();
            if ((bits & U_ANGLE3) != 0)
                delta.angle3 = reader.ReadAngle();

            if ((morebits & U_FTE_TRANS) != 0)
                delta.trans = reader.ReadByte();

            if ((morebits & U_FTE_COLOURMOD) != 0)
            {
                delta.colourmod = reader.ReadBytes(3);
            }

            return delta;
        }

        private void ReadPacketEntities(BinaryReader reader, bool delta)
        {
            if (delta == true)
            {
                reader.ReadByte();
            }
            try
            {
                while (true)
                {
                    (uint entnum, uint bits, uint morebits) = ReadEntityNum(reader);
                    if (entnum == 0)
                    {
                        return;
                    }
                    ReadEntityDelta(reader, bits, morebits);
                }
            }
            catch { }
            //this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}From:{3}\r\n", this._demoTime, this._lastMsg, this._lastCommand, from));
        }

        private void ReadUpdateEnterTime(BinaryReader reader)
        {
            uint playerNum = AdjustForGhost(reader.ReadByte());
            float time = reader.ReadSingle();	// Time (sent as seconds ago).
            LogSimpleCommand();

            //mvd->players[pnum].entertime = mvd->demotime - time;
            //// TODO: Hmmm??? wtf is this, gives values like 1019269.8
            //Sys_PrintDebug(4, "svc_updateentertime: %s %f\n", Sys_RedToWhite(mvd->players[pnum].name), mvd->players[pnum].entertime);
        }

        private const ushort SND_VOLUME = (1 << 15);
        private const ushort SND_ATTENUATION = (1 << 14);

        private void ReadSound(BinaryReader reader)
        {
            int channel = reader.ReadUInt16();

            if ((ushort)(channel & SND_VOLUME) == SND_VOLUME)
            {
                reader.ReadByte();
            }
            if ((ushort)(channel & SND_ATTENUATION) == SND_ATTENUATION)
            {
                reader.ReadByte();
            }
            int soundNum = reader.ReadByte();

            Vector3D from = new Vector3D();
            from.X = reader.ReadCoord();
            from.Y = reader.ReadCoord();
            from.Z = reader.ReadCoord();
        }

        private void LogSimpleCommand()
        {
#if LOGSIMPLE
            this._log.Append(String.Format("{0,10:F3} {1,-10}{2,-20}\r\n", this._demoTime, this._lastMsg, this._lastCommand));
#endif
        }
    }
}
