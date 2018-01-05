using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models
{
    public enum MissionResult
    {
        Unknown,
        Succeed,
        Fail
    }


    public class Mission
    {
        public MissionResult MissionResult;
        public HashSet<Player> Team;
        public Dictionary<Player, MissionResult> MissionVoteOf;
        public Vote[] Votes;
        public VoteNumber VoteNumber;
        public bool IsInitialized
        { get; private set; }
        
        public Vote CurrentVote
        {
            get
            {
                return Votes[(int)VoteNumber];
            }
        }

        public Mission(HashSet<Player> team)
        {
            InitializeVotes();
            Team = team;
            MissionVoteOf = new Dictionary<Player, MissionResult>();
            foreach (Player player in Team)
            {
                MissionVoteOf.Add(player, MissionResult.Unknown);
            }
            IsInitialized = true;
        }

        public Mission()
        {
            IsInitialized = false;
            InitializeVotes();
        }

        public void SetTeam(HashSet<Player> team)
        {
            if (Team != null)
            {
                throw new InvalidOperationException("Team already set!");
            }

            Team = team;
            MissionVoteOf = new Dictionary<Player, MissionResult>();
            foreach (Player player in Team)
            {
                MissionVoteOf.Add(player, MissionResult.Unknown);
            }
            IsInitialized = true;
        }

        public void AddMissionVote(Player player, MissionResult missionVote)
        {
            if (Team == null)
            {
                throw new InvalidOperationException("Missionteam uninitialized.");
            }

            if (missionVote == MissionResult.Unknown)
            {
                throw new ArgumentException("vote");
            }

            if (!Team.Contains(player))
            {
                throw new ArgumentException("player not in team");
            }

            if (MissionVoteOf[player] != MissionResult.Unknown)
            {
                throw new ArgumentException("player alrdy vote");
            }

            MissionVoteOf[player] = missionVote;
        }

        public void ValuateMissionVote()
        {
            if (Team == null)
            {
                throw new InvalidOperationException("Missionteam unitialized.");
            }

            if (MissionResult != MissionResult.Unknown)
            {
                throw new InvalidOperationException("MissionResult already valuated!");
            }

            MissionResult = CountMissionResult();
        }



        private MissionResult CountMissionResult()
        {
            if (Team.Any(plr => MissionVoteOf[plr] == MissionResult.Unknown))
            {
                return MissionResult.Unknown;
            }
            else if (Team.All(plr => MissionVoteOf[plr] == MissionResult.Succeed))
            {
                return MissionResult.Succeed;
            }
            else
            {
                return MissionResult.Fail;
            }
        }

        private void InitializeVotes()
        {
            Votes = new Vote[(int)VoteNumber.Last + 1];
            for (int i = 0; i < Votes.Length; i++)
            {
                Votes[i] = new Vote();
            }
            VoteNumber = VoteNumber.First;
        }
    }
}
