using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalon.Models.Commands
{
    public class ExecCommandEventArgs : EventArgs
    {
        public BaseCommand Command
        { get; private set; }

        public GameModel Model
        { get; private set; }

        public ExecCommandEventArgs (BaseCommand command, GameModel model)
        {
            Command = command;
            Model = model;
        }
    }
}
