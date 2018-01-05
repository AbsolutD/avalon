using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalon.Models.Exceptions;

namespace Avalon.Models.Commands
{
    public class EvaluateTeamVote : BaseCommand
    {
        public MissionNumber MissionNumber;
        public VoteNumber VoteNumber;

        public EvaluateTeamVote(MissionNumber missNum, VoteNumber voteNum)
        {
            MissionNumber = missNum;
            VoteNumber = voteNum;
        }

        public override void ExecuteCommand(GameModel model)
        {
            ValidateCommand(model);

            model.CurrentVote.ValuateTeamVote();

            Vote vote = model.Missions[(int)MissionNumber].Votes[(int)VoteNumber];

            if (vote.VoteResult == VoteType.Approved)
            {
                model.CurrentMission.SetTeam(vote.Team);
                model.GamePhase = GamePhase.MissionVoting;
                //ConsoleLog = "The team is approved. \n Now the mission team should vote on mission success.";
            }
            else if (vote.VoteResult == VoteType.Rejected && 
                model.CurrentMission.VoteNumber < VoteNumber.Last)
            {
                ++model.CurrentMission.VoteNumber;
                model.CurrentLeader = Utilities.NextOrFirst<Player>(model.CurrentLeader, model.Players);
                model.GamePhase = GamePhase.TeamPicking;
                //ConsoleLog = "The team is rejected. \n Now " + model.CurrentLeader + "can pick a mission team.";
            }
            else
            {
                model.GamePhase = GamePhase.GameOver;
                model.GameResult = MissionResult.Fail;

                InvokedCommand = new EvaluateGameResult();

                //string ConsoleLog = "The team is rejected \n" +
                //            "Forces of Good failed to choose a mission team. Forces of Evil prevail.";
                //Utilities.LogToFile(ConsoleLog);
            }
            
        }

        public override string LogConsole(GameModel model)
        {
            Vote vote = model.Missions[(int)MissionNumber].Votes[(int)VoteNumber];
            switch (vote.VoteResult)
            {
                case VoteType.Approved:
                    return "The mission team is approved.";
                case VoteType.Rejected:
                    return "The mission team is rejected.";
                default:
                    return "The team vote is not complete yet.";
            }
        }

        public override void ValidateCommand(GameModel model)
        {
            if (model.GamePhase != GamePhase.TeamVoting)
            {
                throw new GamePhaseException(model.GamePhase.ToString());
            }

            if (model.MissionNumber != MissionNumber)
            {
                throw new ArgumentException("Not current mission");
            }

            Mission mission = model.Missions[(int)MissionNumber];

            if (mission.VoteNumber != VoteNumber)
            {
                throw new ArgumentException("Not current vote");
            }

            if (mission.Votes.Length <= (int)VoteNumber)
            {
                throw new IndexOutOfRangeException("VoteNumber");
            }

            Vote vote = mission.Votes[(int)VoteNumber];

            if (vote.Players.Any(plr => vote.VoteOfPlayer[plr] == VoteType.Unknown))
            {
                throw new MissingVoteException();
            }
        }

        public override string CmdToString()
        {
            return "EvaluateTeamVote " + (int)MissionNumber + " " + (int)VoteNumber;
        }
    }
}
