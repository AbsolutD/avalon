using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Exceptions
{
    public class MissingVoteException : BaseException
    {
        public MissingVoteException() : base() { }
        public MissingVoteException(String msg) : base(msg) { }
    }
}
