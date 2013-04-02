using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoBot.Core.Engine
{

    internal sealed class PowerShellCommand
    {

        public PowerShellCommand(string command)
        {
            this.Command = command;
            this.Parameters = null;
        }

        public PowerShellCommand(string command, string parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }
    
        public string Command
        {
            get;
            set;
        }

        public string Parameters
        {
            get;
            set;
        }

    }

}
