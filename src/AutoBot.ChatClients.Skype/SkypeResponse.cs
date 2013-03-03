using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoBot.Core.Chat;
using SKYPE4COMLib;

namespace AutoBot.ChatClients.Skype
{

    public class SkypeResponse : IChatResponse
    {

        public SkypeResponse(Chat chat)
        {
            this.Chat = chat;
        }

        private Chat Chat
        {
            get;
            set;
        }

        public void Write(string text)
        {
            this.Chat.SendMessage(text);
        }

    }

}
