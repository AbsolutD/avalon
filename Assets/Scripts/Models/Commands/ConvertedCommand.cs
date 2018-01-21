using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Commands
{
    public class ConvertedCommand
    {
        public static BaseCommand CommandFromString (String strCommand, GameModel model)
        {
            if (strCommand.StartsWith("EvaluateGameResult "))
            {
                String[] split = strCommand.Split(' ');
                MissionResult gameResult = (MissionResult)Int32.Parse(split[1]);
                DefeatType defeatType = (DefeatType)Int32.Parse(split[2]);

                EvaluateGameResult command = new EvaluateGameResult();
                command.GameResult = gameResult;
                command.DefeatReason = defeatType;

                return command;
            }

            if (strCommand.StartsWith("EvaluateMissionVote "))
            {
                String[] split = strCommand.Split(' ');
                MissionNumber missionNumber = (MissionNumber)Int32.Parse(split[1]);
                EvaluateMissionVote command = new EvaluateMissionVote(missionNumber);
                return command;
            }

            if (strCommand.StartsWith("EvaluateTeamVote "))
            {
                String[] split = strCommand.Split(' ');
                MissionNumber missionNumber = (MissionNumber)Int32.Parse(split[1]);
                VoteNumber voteNumber = (VoteNumber)Int32.Parse(split[2]);
                EvaluateTeamVote command = new EvaluateTeamVote(missionNumber, voteNumber);
                return command;
            }

            if (strCommand.StartsWith("PickTeam "))
            {
                String[] split = strCommand.Split(' ');
                int leaderId = Int32.Parse(split[1]);
                string teamCode = split[2];
                int playerNum = teamCode.Length;

                HashSet<int> playerTeamIds = new HashSet<int>();
                for (int i = 0; i < teamCode.Length; i++)
                {
                    if (teamCode[i] == '1')
                    {
                        playerTeamIds.Add(i);
                    }
                }

                PickTeamCommand command = new PickTeamCommand(leaderId, playerTeamIds, playerNum);
                return command;
            }

            if (strCommand.StartsWith("VoteMission "))
            {
                String[] split = strCommand.Split(' ');
                int playerId = Int32.Parse(split[1]);
                MissionResult missionVote = (MissionResult)Int32.Parse(split[2]);
                VoteMissionCommand command = new VoteMissionCommand(playerId, missionVote);
                return command;
            }

            if (strCommand.StartsWith("VoteTeam "))
            {
                String[] split = strCommand.Split(' ');
                int playerId = Int32.Parse(split[1]);
                VoteType teamVote = (VoteType)Int32.Parse(split[2]);
                VoteTeamCommand command = new VoteTeamCommand(playerId, teamVote);
                return command;
            }

            if (strCommand.StartsWith("ChangePlayer "))
            {
                String[] split = strCommand.Split(' ');
                int playerId = Int32.Parse(split[1]);
                PlayerType playerType = (PlayerType)Int32.Parse(split[2]);
                String playerName = split[3];
                ChangePlayerCommand command = new ChangePlayerCommand(playerId, playerType, playerName);
                return command;
            }

            throw new ArgumentException("Command string not found: " + strCommand);
        }
    }
}
