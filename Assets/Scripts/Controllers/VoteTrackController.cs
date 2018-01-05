using Avalon.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteTrackController : MonoBehaviour
{
    public Image VoteTrackImage;
    public Text VoteTrackText;
    public Color CurrentVoteTrackColor = Color.red;
    public Color NormalVoteTrackColor = Color.white;
    private bool _isCurrentVoteTrack;
    private int _voteTrackNumber;

    public bool IsCurrentVoteTrack
    {
        get
        {
            return _isCurrentVoteTrack;
        }

        set
        {
            _isCurrentVoteTrack = value;
            VoteTrackImage.color =
                _isCurrentVoteTrack ?
                    CurrentVoteTrackColor :
                    NormalVoteTrackColor;
        }
    }

    public int VoteTrackNumber
    {
        get
        {
            return _voteTrackNumber;
        }

        set
        {
            _voteTrackNumber = value;
            VoteTrackText.text = _voteTrackNumber.ToString();
        }
    }
}
