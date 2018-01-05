using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace Avalon.Controllers
{
    public class NetworkPlayerController : NetworkBehaviour
    {
        public int PlayerId;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
        }
    }
}
