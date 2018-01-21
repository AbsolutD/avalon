using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Avalon.Models;
using UnityEngine.Networking;
using Avalon.Controllers;
using System.Linq;
using Avalon.Models.Commands;

public class AvalonNetManager : NetworkManager {


    public NetGameController NetGameCtr;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        int playerId = -1;
        for (int i = 0; i < NetGameCtr.Model.Players.Length; i++)
        {
            if (!NetGameCtr.TakenId.Contains(i))
            {
                playerId = i;
                break;
            }
        }

        if (playerId >= 0)
        {
            GameObject playerGO = Instantiate(playerPrefab);
            NetPlayerController netPlayerCtr = playerGO.GetComponent<NetPlayerController>();
            NetGameCtr.TakenId.Add(playerId);
            netPlayerCtr.PlayerId = playerId;
            netPlayerCtr.Model = NetGameCtr.Model;
            NetworkServer.AddPlayerForConnection(conn, playerGO, playerControllerId);
        }
    }

}
