using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models
{
    public enum CharacterRole
    {
        Good,
        Evil,
        Merlin, 
        Assassin, 
        Percival,
        Morgana,
        Mordred,
        Oberon
    }

    

    public class Character
    {
        public static bool IsGoodRole(CharacterRole role)
        {
            switch (role)
            {
                case CharacterRole.Good:
                case CharacterRole.Percival:
                case CharacterRole.Merlin:
                    return true;
                case CharacterRole.Evil:
                case CharacterRole.Assassin:
                case CharacterRole.Morgana:
                case CharacterRole.Mordred:
                case CharacterRole.Oberon:
                default:
                    return false;
            }
        }
    }
}
