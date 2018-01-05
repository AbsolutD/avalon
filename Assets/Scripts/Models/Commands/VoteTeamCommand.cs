using Avalon.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Commands
{
    public class VoteTeamCommand : BaseCommand
    {
        public VoteType VoteType
        {
            get; private set;
        }

        public int PlayerId;

        public VoteTeamCommand(int playerId, VoteType voteType)
        {
            PlayerId = playerId;
            VoteType = voteType;
        }

        public override void ExecuteCommand(GameModel model)
        {
            ValidateCommand(model);

            Player player = model.GetPlayer(PlayerId);

            model.CurrentVote.AddTeamVote(player, VoteType);

            InvokedCommand = new EvaluateTeamVote(model.MissionNumber, model.CurrentMission.VoteNumber);
        }

        public override void ValidateCommand(GameModel model)
        {
            if (model.GamePhase != GamePhase.TeamVoting)
            {
                throw new GamePhaseException(model.GamePhase.ToString());
            }

            if (model.Players.Length <= PlayerId)
            {
                throw new IndexOutOfRangeException("playerId");
            }

            Player player = model.GetPlayer(PlayerId);

            if (model.CurrentVote.VoteOfPlayer[player] != VoteType.Unknown)
            {
                throw new TwiceVoteException(player.Name);
            }

            if (VoteType == VoteType.Unknown)
            {
                throw new VoteTypeException("Unkown");
            }
        }

        public override string LogConsole(GameModel model)
        {
            Player player = model.GetPlayer(PlayerId);

            switch (VoteType)
            {
                case VoteType.Approved:
                    return player.Name + " approved.";
                case VoteType.Rejected:
                    return player.Name + " rejected.";
                default:
                    return "";
            }
        }

        public override string CmdToString()
        {
            return "VoteTeam " + PlayerId + " " + (int)VoteType;
        }
    }
}
