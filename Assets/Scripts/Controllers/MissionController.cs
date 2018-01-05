using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalon.Models;
using UnityEngine.UI;
using UnityEngine;

namespace Avalon.Controllers
{
    public class MissionController : MonoBehaviour
    {
        public Image MissionImage;
        public Color SucceedColor = Color.blue;
        public Color FailColor = Color.red;
        public Color UnknownColor = Color.white;
        public Text MissionText;
        private int _missionTeamLength;
        private MissionResult _missionResult;


        public int MissionTeamLength
        {
            get
            {
                return _missionTeamLength;
            }

            set
            {
                _missionTeamLength = value;
                MissionText.text = _missionTeamLength.ToString();
            }
        }

        public MissionResult MissionResult
        {
            get
            {
                return _missionResult;
            }

            set
            {
                _missionResult = value;
                switch (_missionResult)
                {
                    case MissionResult.Unknown:
                        MissionImage.color = UnknownColor;
                        break;
                    case MissionResult.Succeed:
                        MissionImage.color = SucceedColor;
                        break;
                    case MissionResult.Fail:
                        MissionImage.color = FailColor;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
