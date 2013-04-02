using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoBot.Core.Chat;
using jabber;
using jabber.protocol.client;

namespace AutoBot.ChatClients.HipChat
{

    /// <summary>
    /// Represents a response channel that can be used to write
    /// response text while processing HipChat chat messages.
    /// </summary>
    public sealed class HipChatResponse : IChatResponse
    {

        #region Constructors

        public HipChatResponse(HipChatSession session, JID replyTo, MessageType messageType)
        {
            this.Session = session;
            this.ReplyTo = replyTo;
            this.MessageType = messageType;
        }

        #endregion

        #region Properties

        private HipChatSession Session
        {
            get;
            set;
        }

        private JID ReplyTo
        {
            get;
            set;
        }

        private MessageType MessageType
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
            this.Session.SendResponse(this.MessageType, this.ReplyTo, text);
        }

        #endregion

    }

}
