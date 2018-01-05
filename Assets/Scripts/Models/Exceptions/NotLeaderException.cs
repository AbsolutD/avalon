using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Exceptions
{
    public class NotLeaderException : BaseException
    {
        public NotLeaderException() : base() { }
        public NotLeaderException(String msg) : base(msg) { }
    }
}
