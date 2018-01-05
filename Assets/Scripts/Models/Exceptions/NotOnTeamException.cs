using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Exceptions
{
    public class NotOnTeamException : BaseException
    {
        public NotOnTeamException() : base() { }
        public NotOnTeamException(String msg) : base(msg) { }
    }
}
