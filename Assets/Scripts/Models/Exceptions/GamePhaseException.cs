using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Exceptions
{
    class GamePhaseException : BaseException
    {
        public GamePhaseException() : base() { }
        public GamePhaseException(String msg) : base(msg) { }
    }
}
