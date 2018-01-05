using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models
{
    public enum VoteNumber
    {
        First,
        Second,
        Third,
        Fourth,
        Last
    }

    public enum MissionNumber
    {
        First,
        Second,
        Third,
        Fourth,
        Last
    }

    public static class GameRules
    {
        public static int[] GetMissionTeamLength(int playerNumber)
        {
            switch (playerNumber)
            {
                case 5:
                    return new int[] { 2, 3, 2, 3, 3 };
                case 6:
                    return new int[] { 2, 3, 4, 3, 4 };
                case 7:
                    return new int[] { 2, 3, 3, -4, 4 };
                case 8:
                    return new int[] { 3, 4, 4, -5, 5 };
                case 9:
                    return new int[] { 3, 4, 4, -5, 5 };
                default:
                    throw new ArgumentException("playernumber");
            }
        }

        public static List<CharacterRole> GetCharacterRoles(int playerNumber)
        {
            switch (playerNumber)
            {
                case 5:
                    return new List<CharacterRole>()
                    {
                        CharacterRole.Good,
                        CharacterRole.Good,
                        CharacterRole.Good,
                        CharacterRole.Evil,
                        CharacterRole.Evil
                    };
                case 6:
                    return new List<CharacterRole>()
                    {
                        CharacterRole.Good,
                        CharacterRole.Good,
                        CharacterRole.Good,
                        CharacterRole.Good,
                        CharacterRole.Evil,
                        CharacterRole.Evil
                    };
                default:
                    throw new ArgumentException("playernumber");
            }
        }

        public static int GetEvilCount(int playerNumber)
        {
            switch (playerNumber)
            {
                case 5:
                case 6:
                    return 2;
                case 7:
                case 8:
                case 9:
                    return 3;
                case 10:
                    return 4;
                default:
                    throw new ArgumentException("playernumber");
            }
        }

        public static int GetGoodCount(int playerNum)
        {
            return playerNum - GetEvilCount(playerNum);
        }

        public static int VoteCount = (int)VoteNumber.Last + 1;

        public static readonly String[] PlayerNames = new string[]
            { "Mercur", "Venus", "Earth", "Mars", "Jupiter", "Saturnus", "Neptunus", "Uranus", "Pluto"};

        #region AiModel Chances for logical model picking team

        public static readonly int[] LM_OnPick =    { 25, 33, 75 };
        public static readonly int[] LM_Offpick =   { 25, 33, 0 };

        public static readonly int[] LM_OnApprove = { 5, 25, 75 };
        public static readonly int[] LM_OffReject = { 1, 1, 1 };

        public static readonly int[] LM_OnReject =  { 25, 50, 50 };
        public static readonly int[] LM_Offapprove= { 25, 75, 95 };

        public static readonly int LM_MerlinDirty = 5; // chances of Merlin doing sth misleading

        //attention: hammer Offapprove!!!

        #endregion
    }
}
