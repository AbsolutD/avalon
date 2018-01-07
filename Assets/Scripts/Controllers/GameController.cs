using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Avalon.Models;
using Avalon.Models.Commands;
using UnityEngine.UI;
using System.Collections;
using Avalon.Models.Exceptions;

namespace Avalon.Controllers
{

    public class GameController : MonoBehaviour
    {
        //public InputField InputField;
        //public Button SendButton;
        //public ScrollRect ScrollView;
        //public GameObject ScrollTextGO;
        //public GameObject ScrollContent;
        public GameObject PlayerGO;
        public GameObject MissionGO;
        public GameObject VoteTrackPrefab;
        public RectTransform PlayerPanel;
        public RectTransform MissionPanel;
        public RectTransform VoteTrackPanel;
        public HashSet<PlayerController> SelectedPlayers = new HashSet<PlayerController>();
        public IOController IOController;
        public MissionController[] MissionControllers;
        public Dictionary<Player, PlayerController> PlayerToController =
            new Dictionary<Player, PlayerController>();
        public bool SelectionEnabled = false;
        public float AiWaitSec = 3f;

        public NetPlayerController NetPlayerCtr;

        public int LocalPlayerId;
        public GameModel GameModel;


        private VoteTrackController[] voteTrackControllers;

        public void AddCommand(BaseCommand command)
        {
            NetPlayerCtr.CmdExecuteCommand(command.CmdToString());
        }


        #region Initialize methods

        private void InitializePlayers()
        {
            foreach (Player player in GameModel.Players)
            {
                GameObject newPlayer = Instantiate(PlayerGO, PlayerPanel);
                PlayerController playerCtr = newPlayer.GetComponent<PlayerController>();
                playerCtr.PlayerModel = player;
                playerCtr.PlayerNameText.text = player.Name;
                playerCtr.PlayerRoleText.text = GetPlayerRoleText(player);
                playerCtr.IsLeader = GameModel.CurrentLeader == player;
                playerCtr.IsPicked =
                    (GameModel.GamePhase != GamePhase.TeamPicking ?
                    GameModel.CurrentVote.Team.Contains(player) :
                    false
                    );
                playerCtr.GameController = this;
                PlayerToController.Add(player, playerCtr);
            }
        }

        private string GetPlayerRoleText(Player player)
        {
            CharacterRole? visibleRole = GameModel.VisibleRole(player);
            if (visibleRole != null)
            {
                return visibleRole.ToString();
            }
            else
            {
                return "Unknown";
            }
        }

        private void InitializeMissions()
        {
            int missionNumber = GameModel.MissionTeamLengths.Length;
            MissionControllers = new MissionController[missionNumber];
            for (int i = 0; i < missionNumber; i++)
            {
                GameObject newMissionGO = Instantiate(MissionGO, MissionPanel);
                MissionController missionCtr = newMissionGO.GetComponent<MissionController>();
                missionCtr.MissionResult = GameModel.Missions[i].MissionResult;
                missionCtr.MissionTeamLength = GameModel.MissionTeamLengths[i];
                MissionControllers[i] = missionCtr;
            }
        }

        private void InitializeVoteTracks()
        {
            voteTrackControllers = new VoteTrackController[GameRules.VoteCount];
            for (int i = 0; i < GameRules.VoteCount; i++)
            {
                GameObject VoteTrackGO = Instantiate(VoteTrackPrefab, VoteTrackPanel);
                VoteTrackController voteTrackCtr = VoteTrackGO.GetComponent<VoteTrackController>();
                voteTrackCtr.VoteTrackNumber = i+1;
                voteTrackCtr.IsCurrentVoteTrack = false;
                voteTrackControllers[i] = voteTrackCtr;
            }
        }

        #endregion


        #region Update methods

        private void UpdateView()
        {
            UpdateVoteTracks();
            switch (GameModel.GamePhase)
            {
                case GamePhase.TeamPicking:
                    foreach (Player player in GameModel.Players)
                    {
                        PlayerToController[player].IsPicked = false;
                        PlayerToController[player].IsLeader = player == GameModel.CurrentLeader;
                    }
                    SelectionEnabled = GameModel.CurrentLeader.Type == PlayerType.LocalHuman;
                    break;
                case GamePhase.TeamVoting:
                    break;
                case GamePhase.MissionVoting:
                    break;
                case GamePhase.Assassination:
                    break;
                case GamePhase.GameOver:
                    break;
                default:
                    break;
            }
        }

        private void UpdateVoteTracks()
        {
            if (voteTrackControllers == null)
            {
                return;
            }
            for (int i = 0; i < voteTrackControllers.Length; i++)
            {
                voteTrackControllers[i].IsCurrentVoteTrack =
                    (int)GameModel.CurrentMission.VoteNumber == i;
            }
        }

        private void UpdatePlayers()
        {
            foreach (Player player in GameModel.Players)
            {
                PlayerController playerCtr = PlayerToController[player];
                playerCtr.PlayerRoleText.text = GetPlayerRoleText(player);
            }
        }

        #endregion

        private void AnimateCommand(BaseCommand command)
        {
            Write_ScrollView(command.LogConsole(GameModel));
            Persistence.Save(GameModel, "Last.save");

            if (command is PickTeamCommand)
            {
                foreach (Player player in GameModel.Players)
                {
                    PlayerToController[player].PlayerVote =
                        VoteType.Unknown;
                    PlayerToController[player].IsPicked =
                        ((PickTeamCommand)command).TeamPlayerIds.Contains(player.Id);
                }
            }
            if (command is EvaluateTeamVote)
            {
                foreach (Player player in GameModel.Players)
                {
                    MissionNumber missNum = ((EvaluateTeamVote)command).MissionNumber;
                    VoteNumber voteNum = ((EvaluateTeamVote)command).VoteNumber;
                    Vote vote = GameModel.Missions[(int)missNum].Votes[(int)voteNum];
                    PlayerToController[player].PlayerVote =
                        vote.VoteOfPlayer[player];
                    PlayerToController[player].IsLeader =
                        GameModel.CurrentLeader == player;
                }
            }
            if (command is EvaluateMissionVote)
            {
                //Debug.Log("Not null missions: " + MissionControllers.Count(a => a != null));
                //Debug.Log("CurrentMission is " + ((GameModel.CurrentMission == null) ? "null" : "not null"));
                MissionNumber missionNumber = ((EvaluateMissionVote)command).MissionNumber;
                Mission mission = GameModel.Missions[(int)missionNumber];
                MissionControllers[(int)missionNumber].MissionResult =
                     mission.MissionResult;
            }
            if (command is EvaluateGameResult)
            {
                UpdatePlayers();
            }
            IOController.Refresh();
            UpdateView();
        }

        private void ReloadModel(String strModel)
        {
            IOController.GameController = this;
            InitializePlayers();
            InitializeMissions();
            InitializeVoteTracks();
            IOController.Refresh();
            UpdateView();
            Write_ScrollView(GameModel.InitialLog);
        }

        public void Write_ScrollView(String message)
        {
            //Utilities.LogToFile(message);
            return;

            //GameObject newScrollTextGO =
            //    Instantiate(ScrollTextGO, Vector3.zero, Quaternion.identity, ScrollContent.transform);

            //Text textCmp = newScrollTextGO.GetComponent<Text>();
            //newScrollTextGO.GetComponent<Text>().text = message;

            //Canvas.ForceUpdateCanvases();
            //ScrollView.verticalNormalizedPosition = 0f;
        }
    }
}
