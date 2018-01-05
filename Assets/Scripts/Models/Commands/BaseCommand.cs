using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalon.Models.Exceptions;

namespace Avalon.Models.Commands
{
    public abstract class BaseCommand
    {
        public abstract void ValidateCommand(GameModel model);

        public abstract void ExecuteCommand(GameModel model);

        public abstract String LogConsole(GameModel model);

        public abstract String CmdToString();

        public BaseCommand()
        { }

        //public BaseCommand(String strCommand, GameModel model)
        //{
            
        //}

        public BaseCommand InvokedCommand = null;

        public int CommandId;

        


    }



}
