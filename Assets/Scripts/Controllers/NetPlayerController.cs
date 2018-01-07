using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using Avalon.Models;
using Avalon.Models.Commands;

namespace Avalon.Controllers
{
    public class NetPlayerController : NetworkBehaviour
    {
        public int PlayerId;
        public GameModel Model;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
        }

        public void ReloadModel(GameModel model)
        {

        }

        public void CommandAdded(BaseCommand command, GameModel model)
        {

        }
    }
}
