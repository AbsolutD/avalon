using Avalon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Avalon.Models.Commands;
using System.Collections;

namespace Avalon.Controllers
{
    public class NetGameController : NetworkBehaviour
    {
        public GameModel Model = null;
        public HashSet<int> TakenId; // Players - TakenId = AiPlayers
        public bool LockDown;
        public AvalonNetManager netManager;

        public override void OnStartServer()
        {
            Model = new GameModel(5);
            TakenId = new HashSet<int>();
            Subscribe();
        }

        void OnDestroy()
        {
            Unsubscribe();
        }

        //public override void 

        [ClientRpc]
        public void RpcCommandExecuted(string strCommand, string strModel)
        {
            NetPlayerController[] players =
                netManager.client.connection.playerControllers.
                Select(x => x.gameObject.GetComponent<NetPlayerController>()).ToArray();
            NetPlayerController localPlayer = players.FirstOrDefault(x => x != null && x.isLocalPlayer);
            if (localPlayer != null)
            {
                GameModel model = Persistence.ModelFromString(strModel);
                BaseCommand command = ConvertedCommand.CommandFromString(strCommand, model);
                localPlayer.CommandAdded(command, model);
            }
        }

        public float AiWaitSec = 3f;

        #region Subscription

        private void Subscribe()
        {
            if (Model != null)
            {
                Model.CommandExecuted += Model_CommandExecuted;
            }
        }

        private void Unsubscribe()
        {
            if (Model != null)
            {
                Model.CommandExecuted -= Model_CommandExecuted;
            }
        }

        #endregion

        private void Model_CommandExecuted(object sender, CommandEventArgs e)
        {
            if (!isServer)
            {
                Debug.LogError("Model_CommandExecuted called on client.");
                return;
            }
            String strCommand = e.Command.CmdToString();
            String strModel = Persistence.ModelToString(Model);
            RpcCommandExecuted(strCommand, strModel);
            StartCoroutine(GenerateAICommands());
        }

        private IEnumerator GenerateAICommands()
        {
            if (Model.GamePhase == GamePhase.TeamPicking || Model.GamePhase == GamePhase.MissionVoting)
            {
                yield return new WaitForSeconds(AiWaitSec);
            }
            List<BaseCommand> commands = new List<BaseCommand>();
            Model.Players.Where(player => !TakenId.Contains(player.Id)).ToList().
                ForEach(player => commands.Add(AIModel.TryGenerateAICommand(Model, player)));
            commands.RemoveAll(command => command == null);
            commands.ForEach(command => Model.AddCommand(command));
        }

        
    }
}
