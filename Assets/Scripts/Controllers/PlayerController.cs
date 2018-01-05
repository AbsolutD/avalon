using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Avalon.Models;

namespace Avalon.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        public Text PlayerNameText;
        public Text PlayerRoleText;
        public Text PlayerVoteText;
        public Color LeaderTextColor;
        public Color NormalTextColor;
        public Image PickedImage;
        public GameController GameController;

        private Player _playerModel;
        public Player PlayerModel
        {
            get
            {
                return _playerModel;
            }
            set
            {
                _playerModel = value;
                PlayerNameText.text = _playerModel.Name;
                //PlayerRoleText.text = _playerModel.Role.ToString();
                IsLeader = false;
                IsPicked = false;
                PlayerVote = VoteType.Unknown;
            }
        }

        public bool IsLeader
        {
            get
            {
                return _isLeader;
            }

            set
            {
                _isLeader = value;
                PlayerNameText.color =
                    _isLeader ? LeaderTextColor : NormalTextColor;
            }
        }

        public bool IsPicked
        {
            get
            {
                return _isPicked;
            }

            set
            {
                _isPicked = value;
                PickedImage.color = new Color(0.1f, 0.1f, 0.1f);
                PickedImage.gameObject.SetActive(_isPicked);
            }
        }

        public VoteType PlayerVote
        {
            get
            {
                return _playerVote;
            }

            set
            {
                _playerVote = value;
                switch (_playerVote)
                {
                    case VoteType.Approved:
                        PlayerVoteText.text = "✓";
                        PlayerVoteText.color = Color.green;
                        break;
                    case VoteType.Rejected:
                        PlayerVoteText.text = "✗";
                        PlayerVoteText.color = Color.red;
                        break;
                    case VoteType.Unknown:
                        PlayerVoteText.text = "";
                        PlayerVoteText.color = Color.grey;
                        break;
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
                PickedImage.color = new Color(0.5f, 0.5f, 0.5f);
                PickedImage.gameObject.SetActive(_isSelected);
            }
        }

        public void OnPlayerClicked()
        {
            if (GameController != null && GameController.SelectionEnabled)
            {
                if (!GameController.SelectedPlayers.Contains(this))
                {
                    GameController.SelectedPlayers.Add(this);
                    IsSelected = true;
                }
                else
                {
                    GameController.SelectedPlayers.Remove(this);
                    IsSelected = false;
                }
            }
        }

        private bool _isLeader;
        private bool _isPicked;
        private VoteType _playerVote;
        private bool _isSelected;

    }
}
