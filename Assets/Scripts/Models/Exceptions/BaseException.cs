using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Exceptions
{
    public abstract class BaseException : Exception
    {
        public BaseException (String msg) : base(msg) { }
        public BaseException () : base () { }
    }
}
