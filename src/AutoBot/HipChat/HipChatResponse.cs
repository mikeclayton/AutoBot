using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoBot.Chat;
using jabber;
using jabber.protocol.client;

namespace AutoBot.HipChat
{

    internal sealed class HipChatResponse : IChatResponse
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

        public void Write(string text)
        {
            this.Session.SendMessage(this.MessageType, this.ReplyTo, text);
        }

        #endregion

    }

}
