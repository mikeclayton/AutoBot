using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoBot.Core.Chat;

namespace AutoBot.ChatClients.Skype
{

    public sealed class SkypeMessage : IChatMessage
    {

        public SkypeMessage(string originalText, string commandText)
            : base()
        {
            this.OriginalText = originalText;
            this.CommandText = commandText;
        }

        public string OriginalText
        {
            get;
            private set;
        }

        public string CommandText
        {
            get;
            private set;
        }

    }

}
