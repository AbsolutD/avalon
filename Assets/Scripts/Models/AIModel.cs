using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalon.Models.Commands;
using Avalon.Models.Exceptions;

namespace Avalon.Models
{
    public static class AIModel
    {
        public static BaseCommand TryGenerateAICommand(GameModel model, Player player)
        {
            try
            {
                return GenerateAICommand(model, player);
            }
            catch (BaseException)
            {
                return null;
            }
        }

        public static BaseCommand GenerateAICommand(GameModel model, Player player)
        {
            switch (model.GamePhase)
            {
                case GamePhase.TeamPicking:
                    return GeneratePickTeamCommand(model, player);
                case GamePhase.TeamVoting:
                    return GenerateVoteTeamCommand(model, player);
                case GamePhase.MissionVoting:
                    return GenerateVoteMissionCommand(model, player);
                case GamePhase.Assassination:
                    break;
                case GamePhase.GameOver:
                    break;
                default:
                    break;
            }
            return null;
        }

        private static BaseCommand GenerateVoteMissionCommand(GameModel model, Player player)
        {
            if (model.GamePhase != GamePhase.MissionVoting)
            {
                throw new GamePhaseException(model.GamePhase.ToString());
            }

            if (!model.CurrentMission.Team.Contains(player))
            {
                throw new NotOnTeamException();
            }

            if (model.CurrentMission.MissionVoteOf[player] != MissionResult.Unknown)
            {
                throw new TwiceVoteException();
            }

            MissionResult missionVote =
                (Character.IsGoodRole(player.Role)) ?
                MissionResult.Succeed :
                MissionResult.Fail;

            VoteMissionCommand command = new VoteMissionCommand(player.Id, missionVote);

            return command;
        }

        private static VoteTeamCommand GenerateVoteTeamCommand(GameModel model, Player player)
        {
            if (model.GamePhase != GamePhase.TeamVoting)
            {
                throw new GamePhaseException(model.GamePhase.ToString());
            }

            if (!model.Players.Contains(player))
            {
                throw new NotPartOfModelException("player");
            }

            if (model.CurrentVote.VoteOfPlayer[player] != VoteType.Unknown)
            {
                throw new TwiceVoteException();
            }

            //if (model.CurrentMission == null)
            //{
            //    Utilities.LogToFile("currentmission null");
            //}

            //if (model.CurrentMission.Team == null)
            //{
            //    Utilities.LogToFile("currentmission team null");
            //}

            HashSet<Player> pickedTeam = PickATeam(model, player);

            //Utilities.LogToFile(player.Name + "picked the team: " + Persistence.PlayerTeamToString(pickedTeam, model.Players));

            bool offapprove = Utilities.random.Next(10) < 5;
            bool onreject = Utilities.random.Next(10) < 5;

            //Utilities.LogToFile(player.Name + " decided to " +
            //    ((offapprove) ? "OFFAPPROVE" : "not offapprove") + ", " +
            //    ((onreject) ? "ONREJECT" : "not onreject") + ", " +
            //    ((model.IsHammer) ? "HAMMER" : "not hammer"));

            VoteType voteType =
                (   (model.IsHammer)
                    ||
                    (   model.CurrentVote.Team.IsSubsetOf(pickedTeam) 
                        &&
                        (   model.CurrentVote.Team.Contains(player) 
                            ||
                            offapprove)) 
                    ||
                    (   model.CurrentVote.Team.Contains(player) 
                        &&
                        onreject)) 
                ?
                VoteType.Approved :
                VoteType.Rejected;

            VoteTeamCommand command = new VoteTeamCommand(player.Id, voteType);

            return command;
        }

        public static PickTeamCommand GeneratePickTeamCommand(GameModel model, Player leader)
        {
            if (model.GamePhase != GamePhase.TeamPicking)
            {
                throw new GamePhaseException();
            }
            if (model.CurrentLeader != leader)
            {
                throw new NotLeaderException();
            }
            HashSet<Player> pickedTeam = PickATeam(model, leader);

            LogicalModel<Player> logicalModel = new LogicalModel<Player>(pickedTeam, model.CurrentTeamLength);

            HashSet<Player> finalTeam = 
                Utilities.PickRandom(logicalModel.PossibleTeams);

            HashSet<int> finalTeamIds = new HashSet<int>(finalTeam.Select(plr => plr.Id));

            PickTeamCommand command = new PickTeamCommand(leader.Id, finalTeamIds, model.Players.Length);

            return command;
        }

        private static HashSet<Player> PickATeam(GameModel model, Player player)
        {
            LogicalModel<Player> logicalModel =
                new LogicalModel<Player>(
                    new HashSet<Player>(model.Players),
                    GameRules.GetGoodCount(model.Players.Length));

            logicalModel.FilterContainsElement(player);

            foreach (Mission mission in model.Missions)
            {
                if (mission != null && mission.MissionResult == MissionResult.Fail)
                {
                    logicalModel.AddDirtySet(mission.Team);
                }
            }
            //Mission[] reverseMissions = model.Missions.Reverse()
            foreach (Mission mission in model.Missions)
            {
                if (mission != null && mission.MissionResult == MissionResult.Succeed
                    && !model.Missions.Any(ms => ms != null && ms.MissionResult == MissionResult.Fail
                        && mission.Team.IsProperSubsetOf(ms.Team))
                    && logicalModel.PossibleTeams.Any(pt => mission.Team.IsSubsetOf(pt)))
                {
                    logicalModel.AddCleanSet(mission.Team);
                }
            }

            HashSet<Player> pickedTeam =
                Utilities.PickRandom(logicalModel.PossibleTeams);

            return pickedTeam;
        }
    }
}
