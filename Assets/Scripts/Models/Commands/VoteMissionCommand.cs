using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalon.Models.Exceptions;

namespace Avalon.Models.Commands
{
    public class VoteMissionCommand : BaseCommand
    {
        public int PlayerId;

        public MissionResult MissionVote;

        public VoteMissionCommand(int playerId, MissionResult missionVote)
        {
            PlayerId = playerId;
            MissionVote = missionVote;
        }

        public override void ExecuteCommand(GameModel model)
        {
            ValidateCommand(model);

            Player player = model.GetPlayer(PlayerId);

            model.CurrentMission.AddMissionVote(player, MissionVote);

            InvokedCommand = new EvaluateMissionVote(model.MissionNumber);
        }

        public override void ValidateCommand(GameModel model)
        {
            if (model.Players.Length <= PlayerId)
            {
                throw new IndexOutOfRangeException("playerId");
            }

            Player player = model.GetPlayer(PlayerId);

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

            if (MissionVote == MissionResult.Unknown)
            {
                throw new VoteTypeException("Unkown");
            }

            if (MissionVote == MissionResult.Fail && Character.IsGoodRole(player.Role))
            {
                throw new VoteTypeException("Good Failed");
            }
        }

        public override string LogConsole(GameModel model)
        {
            Player player = model.GetPlayer(PlayerId);

            return player.Name + " has voted on the mission success. (" + MissionVote + ")";
        }

        public override string CmdToString()
        {
            return "VoteMission " + PlayerId + " " + (int)MissionVote;
        }

    }
}
