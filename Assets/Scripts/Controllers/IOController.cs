﻿using Avalon.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Avalon.Models.Commands;
using Avalon.Models.Exceptions;
using System;

namespace Avalon.Controllers
{
    public class IOController : MonoBehaviour
    {
        
        public Button SucceedButton;
        public Button FailButton;
        public Text SucceedButtonText;
        public Text FailButtonText;
        public Text InfoText;
        public GameModel GameModel
        {
            get
            {
                if (GameController != null)
                {
                    return GameController.GameModel;
                }
                else
                {
                    Utilities.LogToFile("GameModel NULL in IOController!");
                    return null;
                }
            }
        }
        public GameController GameController;
        public Player LocalPlayer
        {
            get
            {
                if (LocalPlayerCtr != null && GameModel != null)
                {
                    return GameModel.Players[LocalPlayerCtr.PlayerId];
                }
                else
                {
                    Utilities.LogToFile("GameModel/LocalPlayerController NULL in IOController!");
                    return null;
                }
            }
        }
        public BasePlayerController LocalPlayerCtr;

        #region Click Event Handler

        public void SucceedButtonClicked()
        {
            switch (GameModel.GamePhase)
            {
                case GamePhase.TeamPicking:
                    CreatePickTeamCmd();
                    break;
                case GamePhase.TeamVoting:
                    CreateVoteTeamCmd(VoteType.Approved);
                    break;
                case GamePhase.MissionVoting:
                    CreateMissionVoteCmd(MissionResult.Succeed);
                    break;
                case GamePhase.Assassination:
                    break;
                default:
                    break;
            }
        }

        public void FailButtonClicked()
        {
            switch (GameModel.GamePhase)
            {
                case GamePhase.TeamVoting:
                    CreateVoteTeamCmd(VoteType.Rejected);
                    break;
                case GamePhase.MissionVoting:
                    CreateMissionVoteCmd(MissionResult.Fail);
                    break;
            }
        }

        #endregion

        #region Create Commands

        private void CreateMissionVoteCmd(MissionResult missionVote)
        {
            VoteMissionCommand command = new VoteMissionCommand(LocalPlayer.Id, missionVote);
            try
            {
                command.ValidateCommand(GameModel);
            }
            catch (BaseException ex)
            {
                if (ex is VoteTypeException)
                {
                    InfoText.text = "You character alignment is Good. You must vote with Succeed!";
                }
                return;
            }
            GameController.AddCommand(command);
        }

        private void CreateVoteTeamCmd(VoteType teamVote)
        {
            VoteTeamCommand command = new VoteTeamCommand(LocalPlayer.Id, teamVote);
            try
            {
                command.ValidateCommand(GameModel);
            }
            catch (BaseException)
            {
                return;
            }
            GameController.AddCommand(command);
        }

        private void CreatePickTeamCmd()
        {
            HashSet<int> team = 
                new HashSet<int>(GameController.SelectedPlayers.
                Select(plCtr => plCtr.PlayerModel.Id));
            PickTeamCommand command = new PickTeamCommand(LocalPlayer.Id, team, GameModel.Players.Length);
            try
            {
                command.ValidateCommand(GameModel);
            }
            catch (BaseException ex)
            {
                if (ex is TeamNumberException)
                {
                    InfoText.text = "You have to pick EXACTLY " + GameModel.CurrentTeamLength + "players for the mission!";
                }
                return;
            }
            GameController.AddCommand(command);
        }

        #endregion

        #region Subscription

        private void Subscribe()
        {
            if (LocalPlayerCtr != null)
            {
                LocalPlayerCtr.CommandAdded += PlayerController_CommandAdded;
            }
        }

        private void UnSubscribe()
        {
            if (LocalPlayerCtr != null)
            {
                LocalPlayerCtr.CommandAdded -= PlayerController_CommandAdded;
            }
        }

        private void PlayerController_CommandAdded(object sender, ExecCommandEventArgs e)
        {
            Refresh();
        }

        

        #endregion

        private void Start()
        {

        }

        public void Refresh()
        {
            switch (GameModel.GamePhase)
            {
                case GamePhase.TeamPicking:
                    if (LocalPlayer == GameModel.CurrentLeader)
                    {
                        InfoText.text = "You are the Leader. Pick a mission team!";
                        SucceedButton.gameObject.SetActive(true);
                        SucceedButtonText.text = "Pick";
                        FailButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        InfoText.text = GameModel.CurrentLeader.Name + " is picking a mission team...";
                        SucceedButton.gameObject.SetActive(false);
                        FailButton.gameObject.SetActive(false);
                    }
                    break;
                case GamePhase.TeamVoting:
                    //Debug.Log("LocalPlayer is " + ((LocalPlayer == null) ? "null" : "not null"));
                    //Debug.Log(Utilities.LogList<PlayerType>(GameModel.Players.Select(pl => pl.Type).ToList()));
                    //Debug.Log("LocalPlayerVote: " + GameModel.CurrentVote.VoteOfPlayer[LocalPlayer]);
                    //Debug.Log("Player votes: " + (GameModel.CurrentVote.VoteOfPlayer.Values).ToList());
                    //Vote cVote = GameModel.CurrentVote;
                    //Debug.Log("MissionNumber: " + GameModel.MissionNumber + ", VoteNumber: " + GameModel.CurrentMission.VoteNumber);

                    if (GameModel.CurrentVote.VoteOfPlayer[LocalPlayer] == VoteType.Unknown)
                    {
                        InfoText.text = "A mission team was picked by " + GameModel.CurrentLeader.Name + ". Do you approve or reject his team?";
                        SucceedButton.gameObject.SetActive(true);
                        SucceedButtonText.text = "Approve";
                        FailButton.gameObject.SetActive(true);
                        FailButtonText.text = "Reject";
                    }
                    else
                    {
                        Dictionary<Player, VoteType> voteOfPlayer = GameModel.CurrentVote.VoteOfPlayer;
                        InfoText.text = voteOfPlayer.Keys.Count(player => voteOfPlayer[player] == VoteType.Unknown) 
                            + " player is still voting on the mission team...";
                        SucceedButton.gameObject.SetActive(false);
                        FailButton.gameObject.SetActive(false);
                    }
                    break;
                case GamePhase.MissionVoting:
                    if (GameModel.CurrentMission.Team.Contains(LocalPlayer) && 
                        GameModel.CurrentMission.MissionVoteOf[LocalPlayer] == MissionResult.Unknown)
                    {
                        InfoText.text = "Vote on a mission result!";
                        SucceedButton.gameObject.SetActive(true);
                        SucceedButtonText.text = "Success";
                        FailButton.gameObject.SetActive(true);
                        FailButtonText.text = "Failure";
                    }
                    else
                    {
                        Dictionary<Player, MissionResult> voteOfPlayer = GameModel.CurrentMission.MissionVoteOf;
                        InfoText.text = voteOfPlayer.Keys.Count(player => voteOfPlayer[player] == MissionResult.Unknown)
                            + " player is still voting on the mission success...";
                        SucceedButton.gameObject.SetActive(false);
                        FailButton.gameObject.SetActive(false);
                    }
                    break;
                case GamePhase.Assassination:
                    if (LocalPlayer.Role == CharacterRole.Assassin)
                    {
                        InfoText.text = "You are the assassin. Assassinate a player!";
                        SucceedButton.gameObject.SetActive(true);
                        SucceedButtonText.text = "Assassinate";
                        FailButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        Player assassin = GameModel.Players.FirstOrDefault(pl => pl.Role == CharacterRole.Assassin);
                        InfoText.text = assassin.Name + " is assassinating a player...";
                        SucceedButton.gameObject.SetActive(false);
                        FailButton.gameObject.SetActive(false);
                    }
                    break;
                case GamePhase.GameOver:
                    switch (GameModel.GameResult)
                    {
                        case MissionResult.Succeed:
                            InfoText.text = "Forces of GOOD are victorious!";
                            break;
                        case MissionResult.Fail:
                            InfoText.text = "Forces of EVIL are victorious";
                            break;
                        default:
                            InfoText.text = "Game over. (Unkown result)";
                            break;
                    }
                    SucceedButton.gameObject.SetActive(false);
                    FailButton.gameObject.SetActive(false);
                    break;
                default:
                    InfoText.text = "";
                    SucceedButton.gameObject.SetActive(false);
                    FailButton.gameObject.SetActive(false);
                    break;
            }
        }
    }
}
