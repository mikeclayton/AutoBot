using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoBot.Chat
{

    public sealed class MessageReceivedEventArgs : EventArgs
    {

        public MessageReceivedEventArgs(IChatMessage message, IChatResponse response)
        {
            this.Message = message;
            this.Response = response;
        }

        public IChatMessage Message
        {
            get;
            private set;
        }

        public IChatResponse Response
        {
            get;
            private set;
        }

    }

}
