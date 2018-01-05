using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalon.Models.Exceptions;

namespace Avalon.Models.Commands
{
    public class EvaluateMissionVote : BaseCommand
    {
        public MissionNumber MissionNumber;

        public EvaluateMissionVote(MissionNumber missionNum)
        {
            MissionNumber = missionNum;
        }

        public override void ExecuteCommand(GameModel model)
        {
            ValidateCommand(model);

            model.CurrentMission.ValuateMissionVote();

            int succeedCount = model.Missions.
                Where(mis => mis != null).
                Count(mis => mis.MissionResult == MissionResult.Succeed);

            int failCount = model.Missions.
                Where(mis => mis != null).
                Count(mis => mis.MissionResult == MissionResult.Fail);

            if (succeedCount < 3 && failCount < 3)
            {
                model.CurrentLeader = 
                    Utilities.NextOrFirst<Player>(model.CurrentLeader, model.Players);
                ++model.MissionNumber;
                model.GamePhase = GamePhase.TeamPicking;
            }
            else
            {
                InvokedCommand = new EvaluateGameResult();
            }
            
        }

        

        public override void ValidateCommand(GameModel model)
        {
            if (model.GamePhase != GamePhase.MissionVoting)
            {
                throw new GamePhaseException(model.GamePhase.ToString());
            }

            if (model.MissionNumber != MissionNumber)
            {
                throw new ArgumentException("Mission not current.");
            }

            if (model.Missions.Length <= (int)MissionNumber)
            {
                throw new IndexOutOfRangeException("Mission length <= missionNumber");
            }

            Mission mission = model.Missions[(int)MissionNumber];

            if (mission.Team.Any(plr =>
                mission.MissionVoteOf[plr] == MissionResult.Unknown))
            {
                throw new MissingVoteException();
            }
        }

        public override string LogConsole(GameModel model)
        {
            String result = "";
            Mission mission = model.Missions[(int)MissionNumber];
            switch (mission.MissionResult)
            {
                case MissionResult.Succeed:
                    result += "The mission is SUCCEEDED!\n";
                    break;
                case MissionResult.Fail:
                    result += "The mission is FAILED!\n";
                    break;
            }
            return result;
        }

        public override string CmdToString()
        {
            return "EvaluateMissionVote " + (int)MissionNumber;
        }
    }
}
