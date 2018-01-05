using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalon.Models.Exceptions;

namespace Avalon.Models.Commands
{
    public class PickTeamCommand : BaseCommand
    {
        public int LeaderId;
        public HashSet<int> TeamPlayerIds;
        public int PlayerNumber;

        public PickTeamCommand(int leaderId, HashSet<int> teamIds, int playerNumber)
        {
            LeaderId = leaderId;
            TeamPlayerIds = teamIds;
            PlayerNumber = playerNumber;
        }

        public override void ExecuteCommand(GameModel model)
        {
            ValidateCommand(model);

            Player leader = model.GetPlayer(LeaderId);
            HashSet<Player> team =
                new HashSet<Player>(TeamPlayerIds.Select(id => model.GetPlayer(id)).ToArray());

            model.CurrentVote.InitializeVote(leader, team, model.Players);
            model.GamePhase = GamePhase.TeamVoting;
        }

        public override void ValidateCommand(GameModel model)
        {
            if (model.GamePhase != GamePhase.TeamPicking)
            {
                throw new GamePhaseException(model.GamePhase.ToString());
            }

            if (model.Players.Length <= LeaderId)
            {
                throw new IndexOutOfRangeException("LeaderId");
            }

            Player leader = model.GetPlayer(LeaderId);

            if (leader != model.CurrentLeader)
            {
                throw new NotLeaderException(leader.Name);
            }

            if (model.CurrentVote.VoteOfPlayer != null)
            {
                throw new InvalidOperationException("CurrentVote is already initialized");
            }

            if (TeamPlayerIds.Any(id => model.Players.Length <= id))
            {
                throw new NotPartOfModelException("Team");
            }

            HashSet<Player> team = 
                new HashSet<Player>(TeamPlayerIds.Select(id => model.GetPlayer(id)).ToArray());

            if (team.Count != (model.CurrentTeamLength))
            {
                throw new TeamNumberException(team.Count.ToString());
            }
        }

        public override string LogConsole(GameModel model)
        {
            Player leader = model.GetPlayer(LeaderId);
            HashSet<Player> playerTeam =
                new HashSet<Player>(TeamPlayerIds.Select(id => model.GetPlayer(id)).ToArray());

            String result = leader.Name + " picked his mission team:";

            foreach (Player player in playerTeam)
            {
                result += " " + player.Name;
            }
            return result;
        }

        public override string CmdToString()
        {
            String result = "PickTeam " + LeaderId + " ";
            for (int i = 0; i < PlayerNumber; i++)
            {
                result += TeamPlayerIds.Contains(i) ? 1 : 0;
            }
            return result;
        }

        public Player GetLeader(GameModel model)
        {
            throw new NotImplementedException();
        }

        public HashSet<Player> GetTeam (GameModel model)
        {
            throw new NotImplementedException();
        }
    }
}
