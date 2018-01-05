using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Exceptions
{
    public class TeamNumberException : BaseException
    {
        public TeamNumberException() : base() { }
        public TeamNumberException(String msg) : base(msg) { }
    }
}
