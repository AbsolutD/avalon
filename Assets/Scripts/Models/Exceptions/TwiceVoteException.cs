using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Exceptions
{
    public class TwiceVoteException : BaseException
    {
        public TwiceVoteException() : base() { }
        public TwiceVoteException(String msg) : base(msg) { }
    }
}
