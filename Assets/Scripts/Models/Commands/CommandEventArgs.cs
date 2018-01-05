using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Commands
{
    public class CommandEventArgs : EventArgs
    {
        public BaseCommand Command { get; private set; }

        public CommandEventArgs(BaseCommand command) { Command = command; }
    }
}
