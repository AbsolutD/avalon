using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models
{
    public enum VoteType
    {
        Unknown,
        Approved,
        Rejected
    }

    

    public class Vote
    {
        public Player Leader;
        public HashSet<Player> Team;
        public Player[] Players;
        public VoteType VoteResult;
        public Dictionary<Player, VoteType> VoteOfPlayer;
        public bool IsInitialized
        {
            get; private set;
        }

        public Vote(Player leader, HashSet<Player> team, Player[] players)
        {
            Leader = leader;
            Team = team;
            Players = players;
            VoteOfPlayer = new Dictionary<Player, VoteType>();
            foreach (Player player in players)
            {
                VoteOfPlayer.Add(player, VoteType.Unknown);
            }
            IsInitialized = true;
        }

        public Vote()
        {
            Leader = null;
            Team = null;
            Players = null;
            IsInitialized = false;
        }

        public void InitializeVote(Player leader, HashSet<Player> team, Player[] players)
        {
            if (VoteOfPlayer != null)
            {
                throw new InvalidOperationException("Vote already initialized.");
            }

            Leader = leader;
            Team = team;
            Players = players;
            VoteOfPlayer = new Dictionary<Player, VoteType>();
            foreach (Player player in players)
            {
                VoteOfPlayer.Add(player, VoteType.Unknown);
            }
            IsInitialized = true;
        }

        public void AddTeamVote(Player player, VoteType vote)
        {
            if (VoteOfPlayer == null)
            {
                throw new InvalidOperationException("Vote uninitialized.");
            }

            if (vote == VoteType.Unknown)
            {
                throw new ArgumentException("vote");
            }

            if (!Players.Contains(player))
            {
                throw new ArgumentException("player not in team");
            }

            if (VoteOfPlayer[player] != VoteType.Unknown)
            {
                throw new ArgumentException("player alrdy vote");
            }

            VoteOfPlayer[player] = vote;
        }

        public void ValuateTeamVote()
        {
            if (VoteOfPlayer == null)
            {
                throw new InvalidOperationException("Vote uninitialized");
            }

            if (VoteResult != VoteType.Unknown)
            {
                throw new InvalidOperationException("Vote already valuated.");
            }

            VoteResult = CountVoteResult();
        }

        private VoteType CountVoteResult()
        {
            if (Players.Any(plr => VoteOfPlayer[plr] == VoteType.Unknown))
            {
                return VoteType.Unknown;
            }

            int Approves = Players.Count(plr => VoteOfPlayer[plr] == VoteType.Approved);
            int Rejects = Players.Length - Approves;

            if (Approves > Rejects)
            {
                return VoteType.Approved;
            }
            else
            {
                return VoteType.Rejected;
            }
        }
    }

}
