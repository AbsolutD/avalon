using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models
{
    public enum PlayerType
    {
        Human = 0,
        Computer = 1,
    }

    public class Player
    {
        public int Id;
        public string Name;
        public CharacterRole Role;
        public PlayerType Type;
        public const int NameMaxLength = 100;

        public override string ToString()
        {
            return Name;
        }
    }
}
