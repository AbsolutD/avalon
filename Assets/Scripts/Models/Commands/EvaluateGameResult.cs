using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalon.Models.Exceptions;

namespace Avalon.Models.Commands
{
    public class EvaluateGameResult : BaseCommand
    {
        public MissionResult GameResult;
        public DefeatType DefeatReason;

        public EvaluateGameResult()
        {

        }

        public override void ValidateCommand(GameModel model)
        {
            if
                ((model.Missions.Count(ms => ms != null && ms.MissionResult == MissionResult.Fail) < 3)
                &&
                (model.Missions.Count(ms => ms !=null && ms.MissionResult == MissionResult.Succeed) < 3)
                &&
                (!(model.CurrentMission.VoteNumber >= VoteNumber.Last)
                && model.CurrentVote.VoteResult == VoteType.Rejected))
            {
                throw new NotEnoughMissionException();
            }
            
        }

        public override void ExecuteCommand(GameModel model)
        {
            ValidateCommand(model);

            model.GamePhase = GamePhase.GameOver;
            
            if (model.Missions.Count(ms => ms != null && ms.MissionResult == MissionResult.Succeed) >= 3)
            {
                model.GameResult = MissionResult.Succeed;
                model.DefeatReason = DefeatType.NotDefeated;
            }
            else if (model.Missions.Count(ms => ms != null && ms.MissionResult == MissionResult.Fail) >= 3)
            {
                model.GameResult = MissionResult.Fail;
                model.DefeatReason = DefeatType.FailedMission;
            }
            else
            {
                model.GameResult = MissionResult.Fail;
                model.DefeatReason = DefeatType.HammerRejected;
            }

            GameResult = model.GameResult;
            DefeatReason = model.DefeatReason;
            
        }

        public override string LogConsole(GameModel model)
        {
            if (GameResult == MissionResult.Succeed)
            {
                return "This was third succesful mission. Forces of GOOD are VICTORIOUS!";
            }
            else if (GameResult == MissionResult.Fail)
            {
                switch (DefeatReason)
                {
                    case DefeatType.FailedMission:
                        return "This was the third failed mission. Forces of EVIL are VICTORIOUS!";
                    case DefeatType.MerlinKilled:
                        return "This assassin killed Merlin. Forces of EVIL are VICTORIOUS!";
                    case DefeatType.HammerRejected:
                        return "This was the 5th rejected mission team. Forces of EVIL are VICTORIOUS!";
                    default:
                        throw new NotImplementedException();
                }
            }
            return "Game result unkown.";
        }

        public override string CmdToString()
        {
            return "EvaluateGameResult " + (int)GameResult + " " + (int)DefeatReason;
        }
    }
}
