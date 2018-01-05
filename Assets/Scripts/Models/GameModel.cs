using Avalon.Models.Commands;
using Avalon.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalon.Models
{
    public enum GamePhase
    {
        TeamPicking,
        TeamVoting,
        MissionVoting,
        Assassination,
        GameOver
    }

    public class GameModel
    {
        public Player[] Players;
        public Mission[] Missions;
        public MissionNumber MissionNumber;
        public Player CurrentLeader;
        public GamePhase GamePhase;
        public MissionResult GameResult;
        public DefeatType DefeatReason;
        public int[] MissionTeamLengths;
        public Queue<BaseCommand> CommandQueue = new Queue<BaseCommand>();
        public event EventHandler<CommandEventArgs> CommandExecuted;

        public Player LocalPlayer
        {
            get
            {
                return Players.FirstOrDefault(player => player.Type == PlayerType.LocalHuman);
            }
        }
        public Mission CurrentMission
        {
            get
            {
                return Missions[(int)MissionNumber];
            }
        }
        public Vote CurrentVote
        {
            get
            {
                return (CurrentMission.Votes[(int)CurrentMission.VoteNumber]);
            }
        }

        //public Player[] CurrentTeam
        //{
        //    get
        //    {
        //        return 
        //    }
        //}

        public GameModel()
        {

        }

        public GameModel(int playerNumber)
        {
            Players = new Player[playerNumber];
            List<CharacterRole> roleList = GameRules.GetCharacterRoles(playerNumber);
            roleList = (Utilities.Shuffle<CharacterRole>(roleList));
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i] = new Player();
                Players[i].Type = (i == 0 ? PlayerType.LocalHuman : PlayerType.Computer);
                Players[i].Id = i;
                Players[i].Role = roleList[i];
                Players[i].Name = GameRules.PlayerNames[i];
                if (i != 0)
                {
                    Players[i].Name += " (AI)";
                }
            }
            Players = (Utilities.Shuffle<Player>(Players.ToList())).ToArray();
            MissionTeamLengths = GameRules.GetMissionTeamLength(playerNumber);
            Missions = new Mission[MissionTeamLengths.Length];
            for (int i = 0; i < Missions.Length; i++)
            {
                Missions[i] = new Mission();
            }
            MissionNumber = MissionNumber.First;
            CurrentLeader = Players[0];
            GamePhase = GamePhase.TeamPicking;
        }

        public void AddCommand(BaseCommand command)
        {
            command.ValidateCommand(this);
            command.CommandId = CommandQueue.Count + 1;
            command.ExecuteCommand(this);
            CommandQueue.Enqueue(command);
            OnCommandExecuted(command);

            #region Invoke Command

            if (command.InvokedCommand != null)
            {
                try
                {
                    command.InvokedCommand.ValidateCommand(this);
                }
                catch (BaseException)
                {
                    return;
                }
                AddCommand(command.InvokedCommand);
            }

            #endregion
        }

        private void OnCommandExecuted(BaseCommand command)
        {
            if (CommandExecuted != null)
            {
                CommandExecuted(this, new CommandEventArgs(command));
            }
        }
		
        public int CurrentTeamLength
        {
            get
            {
                return (GameRules.GetMissionTeamLength(Players.Length))[(int)MissionNumber];
            }
        }

        public bool IsHammer
        {
            get
            {
                return (CurrentMission.VoteNumber == VoteNumber.Last);
            }
        }

        public string InitialLog
        {
            get
            {
                String log  = "The game has started with " + Players.Length + " players: \n ";
                foreach (Player player in Players)
                {
                    log += player.Name + " ";
                }
                return log;
            }
        }

        public CharacterRole? VisibleRole(Player player)
        {
            if (player == LocalPlayer || GamePhase == GamePhase.GameOver)
            {
                return player.Role;
            }
            switch (LocalPlayer.Role)
            {
                case CharacterRole.Good:
                case CharacterRole.Oberon:
                    return null;

                case CharacterRole.Merlin:
                case CharacterRole.Evil:
                case CharacterRole.Assassin:
                case CharacterRole.Morgana:
                case CharacterRole.Mordred:
                    return Character.IsGoodRole(player.Role) ? CharacterRole.Good : CharacterRole.Evil;

                case CharacterRole.Percival:
                    if (player.Role == CharacterRole.Merlin
                            || player.Role == CharacterRole.Morgana)
                        return CharacterRole.Merlin;
                    else
                        return null;
                default:
                    return null;
            }
        }

        public override string ToString()
        {
            
            return base.ToString();
        }

        public void FromString (String str)
        {

        }

        public Player GetPlayer (int playerId)
        {
            Player player = Players.FirstOrDefault(plr => plr.Id == playerId);
            if (player != null)
            {
                return player;
            }
            else
            {
                throw new NotPartOfModelException("playerId: " + playerId);
            }
        }
    }

    public enum DefeatType
    {
        NotDefeated = 0,
        FailedMission = 1,
        MerlinKilled = 2,
        HammerRejected = 3
    }
}


