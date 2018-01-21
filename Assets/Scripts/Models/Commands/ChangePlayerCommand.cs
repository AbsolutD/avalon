using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalon.Models.Exceptions;

namespace Avalon.Models.Commands
{
    public class ChangePlayerCommand : BaseCommand
    {
        public int PlayerId;
        public PlayerType PlayerType;
        public String PlayerName;

        public ChangePlayerCommand(int playerId, PlayerType playerType, String playerName)
        {
            PlayerId = playerId;
            PlayerType = playerType;
            PlayerName = playerName;
        }

        public override string CmdToString()
        {
            return "ChangePlayer " + PlayerId + " " + (int)PlayerType + " " + PlayerName;
        }

        public override void ExecuteCommand(GameModel model)
        {
            ValidateCommand(model);

            Player player = model.GetPlayer(PlayerId);
            player.Name = PlayerName;
            player.Type = PlayerType;
        }

        public override string LogConsole(GameModel model)
        {
            return "Player " + PlayerId +" has changed (" + PlayerType + "," + PlayerName + ")";
        }

        public override void ValidateCommand(GameModel model)
        {
            if (PlayerId > model.Players.Length || PlayerId < 0)
            {
                throw new NotPartOfModelException("PlayerId: " + PlayerId);
            }

            if (PlayerName.Length > Player.NameMaxLength)
            {
                throw new ArgumentException("PlayerName length: " + PlayerName.Length);
            }
        }
    }
}
