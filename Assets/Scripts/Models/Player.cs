using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models
{
    public enum PlayerType
    {
        LocalHuman = 0,
        Computer = 1,
        NetworkHuman = 2
    }

    public class Player
    {
        public int Id;
        public string Name;
        public CharacterRole Role;
        public PlayerType Type;

        public override string ToString()
        {
            return Name;
        }
    }
}
