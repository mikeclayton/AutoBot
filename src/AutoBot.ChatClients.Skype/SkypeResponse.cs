using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoBot.Core.Chat;
using SKYPE4COMLib;

namespace AutoBot.ChatClients.Skype
{

    /// <summary>
    /// Represents a response channel that can be used to write
    /// response text while processing Skype chat messages.
    /// </summary>
    public class SkypeResponse : IChatResponse
    {

        #region Constructors

        public SkypeResponse(Chat chat)
        {
            this.Chat = chat;
        }

        #endregion

        #region Properties

        private Chat Chat
        {
            get;
            set;
        }

        #endregion

        #region IChatResponse Interface

        /// <summary>
        /// Writes a message to the chat client response channel.
        /// </summary>
        /// <param name="text">The text to write to the chat client.</param>
        public void Write(string text)
        {
            this.Chat.SendMessage(text);
        }

        #endregion

    }

}
