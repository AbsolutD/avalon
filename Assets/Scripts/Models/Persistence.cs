using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Avalon.Models.Commands;

namespace Avalon.Models
{
    public class Persistence
    {
        private static string newLine = Environment.NewLine;
        private static string endLine = newLine;
        private static string shortEnd = "---";
        private static string longEnd = "------";
        private static string shortEndN = shortEnd + endLine;
        private static string longEndN = longEnd + endLine;

        private static string playerLabel = "Players";
        private static string gameStateLabel = "Game State";
        private static string missionLabel = "Missions";
        private static string voteLabel = "\tVotes";
        private static string commandLabel = "Commands";
        
        
        public const string DefaultPath = "C:\\temp\\log\\Avalon";

        public static String ModelToString (GameModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            String lines = "Created: " + DateTime.Now + endLine;
            lines += longEndN;
            lines += playerLabel + endLine;
            for (int i = 0; i < model.Players.Length; i++)
            {
                lines += shortEndN;
                lines += "Id: " + model.Players[i].Id + endLine;
                lines += "Name: " + model.Players[i].Name + endLine;
                lines += "Role: " + model.Players[i].Role + endLine;
                lines += "Type: " + model.Players[i].Type + endLine;
            }
            lines += longEndN;
            lines += gameStateLabel + endLine;
            lines += shortEndN;
            lines += "Current Phase: " + model.GamePhase + endLine;
            lines += "Game Result: " + model.GameResult + endLine;
            lines += "Defeat Reason: " + model.DefeatReason + endLine;
            lines += "Current Leader: " + model.CurrentLeader + endLine;
            lines += longEndN;


            #region Missions

            lines += missionLabel + endLine;
            Utilities.LogToFile("MissionNumber: " + model.MissionNumber);

            for (int i = 0; i <= (int)model.MissionNumber; i++)
            {
                if (model.Missions == null)
                {
                    throw new NullReferenceException("model.missions");
                }
                Utilities.LogToFile("i = " + i + "missions.length: " + model.Missions.Length);
                Mission mission = model.Missions[i];
                lines += shortEndN;
                HashSet<Player> playerTeam =
                    mission.Team != null ?
                    mission.Team :
                    new HashSet<Player>();
                string team =
                    PlayerTeamToString(playerTeam, model.Players);
                lines += "Team: " + team + endLine;
                lines += "MissionResult: " + mission.MissionResult + endLine;
                string mResVotes = 
                    MissionResultVotesToStr(mission.MissionVoteOf, model.Players);
                lines += "Missionresult votes: " + mResVotes + endLine;
                lines += voteLabel + endLine;
                
                for (int j = 0; j <= (int)mission.VoteNumber && mission.Votes[j].IsInitialized; j++)
                {
                    Vote vote = mission.Votes[j];
                    lines += "\t" + shortEndN;
                    lines += "\tLeader: " + vote.Leader + endLine;
                    string voteTeam = PlayerTeamToString(vote.Team, model.Players);
                    lines += "\tTeam: " + voteTeam + endLine;
                    lines += "\tVoteResult: " + vote.VoteResult + endLine;
                    string playerVotes = PlayerVotesToString(vote.VoteOfPlayer, model.Players);
                    lines += "\tPlayerVotes: " + playerVotes + endLine;
                }
                
            }

            lines += longEndN;

            #endregion

            #region Commands

            lines += commandLabel + endLine;
            lines += " " + endLine;
            //lines += shortEndN;

            BaseCommand[] commands = model.CommandQueue.ToArray();

            for (int i = 0; i < commands.Length; i++)
            {
                lines += commands[i].CmdToString() + endLine;
            }

            lines += longEndN;

            #endregion

            return lines;
        }

        public static GameModel ModelFromString (String lines)
        {
            GameModel model = new GameModel();
            String[] split = lines.Split(new string[] { newLine },
                StringSplitOptions.RemoveEmptyEntries);

            Utilities.LogToFile("MapFromString started.");

            //Utilities.LogToFile("Array length: " + split.Length);

            Queue<String> queue = new Queue<String>(split);

            //Utilities.LogToFile("Queue length: " + queue.Count);

            String line = "";
            String stringToRead;



            while (line != longEnd)
            {
                line = queue.Dequeue();
            }
            //Utilities.LogToFile("New Line: '" + newLine + "'");

            #region Players

            line = queue.Dequeue();

            List<Player> playerList = new List<Player>();

            if (line == playerLabel)
            {
                line = queue.Dequeue();
                while (line != longEnd)
                {
                    Player player = new Player();
                    line = queue.Dequeue();

                    stringToRead = "Id: ";
                    if (line.StartsWith(stringToRead))
                    {
                        String stringValue = line.Substring(stringToRead.Length);
                        player.Id = Int32.Parse(stringValue);
                        line = queue.Dequeue();
                    }

                    stringToRead = "Name: ";
                    if (line.StartsWith(stringToRead))
                    {
                        String stringValue = line.Substring(stringToRead.Length);
                        player.Name = stringValue;
                        line = queue.Dequeue();
                    }

                    stringToRead = "Role: ";
                    if (line.StartsWith(stringToRead))
                    {
                        String stringValue = line.Substring(stringToRead.Length);
                        player.Role = (CharacterRole)Enum.Parse(typeof(CharacterRole), stringValue);
                        line = queue.Dequeue();
                    }

                    stringToRead = "Type: ";
                    if (line.StartsWith(stringToRead))
                    {
                        String stringValue = line.Substring(stringToRead.Length);
                        player.Type = (PlayerType)Enum.Parse(typeof(PlayerType), stringValue);
                        line = queue.Dequeue();
                    }

                    playerList.Add(player);
                }

                model.Players = playerList.ToArray();

                model.MissionTeamLengths = GameRules.GetMissionTeamLength(model.Players.Length);
            }
            else
            {
                throw new ArgumentException("ModelFromString: No player info found!");
            }

            #endregion

            #region Game State

            if (queue.Dequeue() == gameStateLabel)
            {
                line = queue.Dequeue();
                line = queue.Dequeue();

                stringToRead = "Current Phase: ";
                if (line.StartsWith(stringToRead))
                {
                    String stringValue = line.Substring(stringToRead.Length);
                    model.GamePhase = (GamePhase)Enum.Parse(typeof(GamePhase), stringValue);
                    line = queue.Dequeue();
                }
                else
                {
                    throw new ArgumentException("MapFromString: No current phase found.");
                }

                stringToRead = "Game Result: ";
                if (line.StartsWith(stringToRead))
                {
                    String stringValue = line.Substring(stringToRead.Length);
                    model.GameResult = (MissionResult)Enum.Parse(typeof(MissionResult), stringValue);
                    line = queue.Dequeue();
                }
                else
                {
                    throw new ArgumentException("MapFromString: No game result found.");
                }

                stringToRead = "Defeat Reason: ";
                if (line.StartsWith(stringToRead))
                {
                    String stringValue = line.Substring(stringToRead.Length);
                    model.DefeatReason = (DefeatType)Enum.Parse(typeof(DefeatType), stringValue);
                    line = queue.Dequeue();
                }
                else
                {
                    throw new ArgumentException("MapFromString: No defeat reason found.");
                }

                stringToRead = "Current Leader: ";
                if (line.StartsWith(stringToRead))
                {
                    String stringValue = line.Substring(stringToRead.Length);
                    model.CurrentLeader = model.Players.FirstOrDefault(plr => plr.Name == stringValue);
                    //int playerId = Int32.Parse(stringValue);
                    //model.CurrentLeader = model.Players.FirstOrDefault(plr => plr.Id == playerId);
                    if (model.CurrentLeader == null)
                    {
                        throw new ArgumentException("MapFromString: Leader not found.");
                    }
                    line = queue.Dequeue();
                }
            }
            else
            {
                throw new ArgumentException("MapFromString: no info on GameState");
            }

            #endregion

            #region Missions

            line = queue.Dequeue(); // Missions
            

            if (line == missionLabel)
            {
                int missionCounter = 0;
                model.Missions = new Mission[(int)MissionNumber.Last + 1];
                for (int i = 0; i < model.Missions.Length; i++)
                {
                    model.Missions[i] = new Mission();
                }
                Mission currentMission = null;
                line = queue.Dequeue(); // shortEnd

                while (line != longEnd && missionCounter <= (int)MissionNumber.Last)
                {
                    line = queue.Dequeue();

                    currentMission = new Mission();

                    stringToRead = "Team: ";
                    if (line.StartsWith(stringToRead))
                    {
                        String stringValue = line.Substring(stringToRead.Length);
                        HashSet<Player> playerTeam = PlayerTeamFromStr(stringValue, model.Players);
                        if (playerTeam.Count > 0)
                        {
                            currentMission.SetTeam(playerTeam);
                        }
                        line = queue.Dequeue();
                    }
                    else
                    {
                        throw new ArgumentException("MapFromString: no mission team found");
                    }

                    stringToRead = "MissionResult: ";
                    if (line.StartsWith(stringToRead))
                    {
                        String stringValue = line.Substring(stringToRead.Length);
                        MissionResult missionResult = 
                            (MissionResult)Enum.Parse(typeof(MissionResult), stringValue);
                        currentMission.MissionResult = missionResult;
                        line = queue.Dequeue();
                    }
                    else
                    {
                        throw new ArgumentException("MapFromString: no mission result found");
                    }

                    stringToRead = "Missionresult votes: ";
                    if (line.StartsWith(stringToRead))
                    {
                        String stringValue = line.Substring(stringToRead.Length);
                        currentMission.MissionVoteOf =
                            currentMission.IsInitialized ?
                            MResultVotesFromStr(stringValue, model.Players, currentMission.Team) :
                            null;
                        line = queue.Dequeue();
                    }
                    else
                    {
                        throw new ArgumentException("MapFromString: no mission result votes found");
                    }

                    #region Votes

                    int voteCounter = 0;

                    Utilities.LogToFile("Current line: '" + line + "', Rows left: " + queue.Count);

                    currentMission.Votes = new Vote[(int)VoteNumber.Last + 1];
                    for (int i = 0; i < currentMission.Votes.Length; i++)
                    {
                        currentMission.Votes[i] = new Vote();
                    }

                    if (line == voteLabel) // \tVotes
                    {
                        line = queue.Dequeue(); // shortEnd

                        while (line != longEnd && line != shortEnd && voteCounter <= (int)VoteNumber.Last)
                        {
                            Vote vote = currentMission.Votes[voteCounter];
                            Player leader = null;
                            HashSet<Player> team = null;

                            //Utilities.LogToFile("1. Current line: '" + line + "', Rows left: " + queue.Count);

                            line = queue.Dequeue(); // shortEnd

                            stringToRead = "\tLeader: ";

                            //Utilities.LogToFile("2. Current line: '" + line + "', Rows left: " + queue.Count);

                            if (line.StartsWith(stringToRead))
                            {
                                String stringValue = line.Substring(stringToRead.Length);
                                //int leaderId = Int32.Parse(stringValue);
                                //if (!model.Players.Any(plr => plr.Id == leaderId))
                                //{
                                //    throw new Exception("Persistence: Leader not found");
                                //}
                                //leader = model.Players[leaderId];
                                leader = model.Players.FirstOrDefault(plr => plr.Name == stringValue);
                                line = queue.Dequeue();
                            }
                            else
                            {
                                throw new ArgumentException("MapFromString: Vote leader not found.");
                            }

                            stringToRead = "\tTeam: ";
                            if (line.StartsWith(stringToRead))
                            {
                                String stringValue = line.Substring(stringToRead.Length);
                                team = PlayerTeamFromStr(stringValue, model.Players);
                                line = queue.Dequeue();
                            }
                            else
                            {
                                throw new Exception("Persistence: Team undefined.");
                            }

                            vote.InitializeVote(leader, team, model.Players);

                            stringToRead = "\tVoteResult: ";
                            if (line.StartsWith(stringToRead))
                            {
                                String stringValue = line.Substring(stringToRead.Length);
                                vote.VoteResult = (VoteType)Enum.Parse(typeof(VoteType), stringValue);
                                line = queue.Dequeue();
                            }

                            stringToRead = "\tPlayerVotes: ";
                            if (line.StartsWith(stringToRead))
                            {
                                String stringValue = line.Substring(stringToRead.Length);
                                vote.VoteOfPlayer = PlayerVotesFromStr(stringValue, model.Players);
                                line = queue.Dequeue();
                            }

                            ++voteCounter;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("MapFromString: No mission votes found.");
                    }

                    #endregion

                    currentMission.VoteNumber = 
                        (VoteNumber)(voteCounter != 0 ? voteCounter - 1 : 0);
                    model.Missions[missionCounter] = currentMission;
                    ++missionCounter;
                    Utilities.LogToFile("Current line: " + line);
                }

                model.MissionNumber =
                    (MissionNumber)(missionCounter != 0 ? missionCounter - 1 : 0);
                Utilities.LogToFile("MissionNumber wrote: " + (int)model.MissionNumber);
            }
            else
            {
                throw new ArgumentException("MapFromString: no mission info");
            }

            #endregion

            #region Commands

            line = queue.Dequeue(); // Commands

            if (line == commandLabel)
            {
                Queue<BaseCommand> commandQueue = new Queue<BaseCommand>();
                queue.Dequeue(); // endLine
                while ((line = queue.Dequeue()) != longEnd)
                {
                    BaseCommand command = ConvertedCommand.CommandFromString(line, model);
                    commandQueue.Enqueue(command);
                }
                model.CommandQueue = commandQueue;
            }

            #endregion

            return model;
        }

        public static void Save (GameModel model, String fileName, String filePath = null)
        {
            if (filePath == null)
            {
                filePath = DefaultPath;
            }

            String modelString = ModelToString(model);

            String[] lines = modelString.Split('\n');

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            using (StreamWriter file =
                new StreamWriter(filePath + "\\" + fileName, false))
            {
                file.Write(modelString);
            }
        }

        public static GameModel Load (String fileName, String filePath = null)
        {
            if (filePath == null)
            {
                filePath = DefaultPath;
            }

            String lines;

            using (StreamReader file = new StreamReader(filePath + "\\" + fileName))
            {
                lines = file.ReadToEnd();
            }

            return ModelFromString(lines);
        }

        public static int PlayerTeamToInt (HashSet<Player> team, Player[] players)
        {
            int result = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (team.Contains(players[i]))
                {
                    result += (int)Math.Pow(2, i);
                }
            }
            return result;
        }

        public static int MissionResultVotesToInt (Dictionary<Player,MissionResult> mResultVotes, Player[] players)
        {
            int result = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (mResultVotes.ContainsKey(players[i]))
                {
                    result += (int)Math.Pow(3, i) * (int)mResultVotes[players[i]];
                }
            }
            return result;
        }

        public static int PlayerVotesToInt (Dictionary<Player, VoteType> playerVotes, Player[] players)
        {
            int result = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (playerVotes.ContainsKey(players[i]))
                {
                    result += (int)Math.Pow(3, i) * (int)playerVotes[players[i]];
                }
            }
            return result;
        }

        public static string PlayerTeamToString (HashSet<Player> team, Player[] players)
        {
            String result = "";

            if (team == null)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    result += "0";
                }
                return result;
            }

            for (int i = 0; i < players.Length; i++)
            {
                result += (team.Contains(players[i])) ? "1" : "0";
            }
            return result;
        }

        public static string MissionResultVotesToStr(Dictionary<Player, MissionResult> mResultVotes, Player[] players)
        {
            string result = "";

            if (mResultVotes == null)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    result += "0";
                }
                return result;
            }

            for (int i = 0; i < players.Length; i++)
            {
                if (mResultVotes.ContainsKey(players[i]))
                {
                    result += (int)mResultVotes[players[i]];
                }
                else
                {
                    result += "0";
                }
            }
            return result;
        }

        public static string PlayerVotesToString(Dictionary<Player, VoteType> playerVotes, Player[] players)
        {
            string result = "";

            if (playerVotes == null)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    result += "0";
                }
                return result;
            }

            for (int i = 0; i < players.Length; i++)
            {
                if (playerVotes.ContainsKey(players[i]))
                {
                    result += (int)playerVotes[players[i]];
                }
                else
                {
                    result += "0";
                }
            }
            return result;
        }

        public static HashSet<Player> PlayerTeamFromStr (String line, Player[] players)
        {
            HashSet<Player> playerTeam = new HashSet<Player>();

            if (line.Length != players.Length)
            {
                throw new ArgumentException("string length");
            }

            for (int i = 0; i < players.Length; i++)
            {
                if (line[i] == '1')
                {
                    playerTeam.Add(players[i]);
                }
            }

            return playerTeam;
        }

        public static Dictionary<Player, MissionResult> MResultVotesFromStr(String line, Player[] players, HashSet<Player> team)
        {
            Dictionary<Player, MissionResult> mResultVotes = new Dictionary<Player, MissionResult>();

            if (line.Length != players.Length)
            {
                throw new ArgumentException("string length");
            }

            for (int i = 0; i < players.Length; i++)
            {
                if (team.Contains(players[i]))
                {
                    switch (line[i])
                    {
                        case '0':
                            mResultVotes[players[i]] = MissionResult.Unknown;
                            break;
                        case '1':
                            mResultVotes[players[i]] = MissionResult.Succeed;
                            break;
                        case '2':
                            mResultVotes[players[i]] = MissionResult.Fail;
                            break;
                        default:
                            break;
                    }
                }
            }

            return mResultVotes;

        }

        public static Dictionary<Player, VoteType> PlayerVotesFromStr(String line, Player[] players)
        {
            Dictionary<Player, VoteType> playerVotes = new Dictionary<Player, VoteType>();

            if (line.Length != players.Length)
            {
                throw new ArgumentException("string length");
            }

            for (int i = 0; i < players.Length; i++)
            {
                if (line[i] == '0')
                {
                    playerVotes[players[i]] = VoteType.Unknown;
                }

                if (line[i] == '1')
                {
                    playerVotes[players[i]] = VoteType.Approved;
                }

                if (line[i] == '2')
                {
                    playerVotes[players[i]] = VoteType.Rejected;
                }
            }

            return playerVotes;
        }

        
            
    }
}
