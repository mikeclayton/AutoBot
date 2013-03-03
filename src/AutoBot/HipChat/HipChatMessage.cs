using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoBot.Chat;
using jabber.protocol.client;

namespace AutoBot.HipChat
{

    public sealed class HipChatMessage : IChatMessage
    {

        public HipChatMessage(MessageType type, string originalText, string commandText)
            : base()
        {
            this.Type = type;
            this.OriginalText = originalText;
            this.CommandText = commandText;
        }

        public MessageType Type
        {
            get;
            private set;
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
