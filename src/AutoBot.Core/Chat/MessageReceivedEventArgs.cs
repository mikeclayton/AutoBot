using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoBot.Core.Chat
{

    public sealed class MessageReceivedEventArgs : EventArgs
    {

        public MessageReceivedEventArgs(IChatMessage message, IChatResponse response)
        {
            this.Message = message;
            this.Response = response;
        }

        /// <summary>
        /// Gets a reference to the message object received by the chat session.
        /// </summary>
        public IChatMessage Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a reference to the response object to use when sending text
        /// back to the chat session while processing the message.
        /// </summary>
        public IChatResponse Response
        {
            get;
            private set;
        }

    }

}
