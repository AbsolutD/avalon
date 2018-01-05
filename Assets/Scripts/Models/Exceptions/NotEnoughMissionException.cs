using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Exceptions
{
    public class NotEnoughMissionException : BaseException
    {
        public NotEnoughMissionException() : base() { }
        public NotEnoughMissionException(String msg) : base(msg) { }
    }
}
