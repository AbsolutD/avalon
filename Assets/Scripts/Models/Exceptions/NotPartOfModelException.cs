using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Exceptions
{
    public class NotPartOfModelException : BaseException
    {
        public NotPartOfModelException() : base() { }
        public NotPartOfModelException(String msg) : base(msg) { }
    }
}
