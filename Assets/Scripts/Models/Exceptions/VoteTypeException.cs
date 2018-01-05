using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Exceptions
{
    public class VoteTypeException : BaseException
    {
        public VoteTypeException() : base() { }
        public VoteTypeException(String msg) : base(msg) { }
    }
}
