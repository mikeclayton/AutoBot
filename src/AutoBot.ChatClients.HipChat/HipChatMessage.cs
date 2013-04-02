using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoBot.Core.Chat;
using jabber.protocol.client;

namespace AutoBot.ChatClients.HipChat
{

    /// <summary>
    /// Represents the text in a chat message received from a HipChat chat client.
    /// </summary>
    public sealed class HipChatMessage : IChatMessage
    {

        #region Constructors

        public HipChatMessage(MessageType type, string originalText, string commandText)
            : base()
        {
            this.Type = type;
            this.OriginalText = originalText;
            this.CommandText = commandText;
        }

        #endregion

        #region Properties

        public MessageType Type
        {
            get;
            private set;
        }

        #endregion

        #region IChatMessage Interface

        /// <summary>
        /// Gets the original text of the message received by the chat client.
        /// </summary>
        public string OriginalText
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the pre-processed command text to execute by the engine.
        /// </summary>
        public string CommandText
        {
            get;
            private set;
        }

        #endregion

    }

}
