using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoBot.Core.Chat
{

    public interface IChatSession
    {

        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        void Connect();
        void Disconnect();

    }

}
