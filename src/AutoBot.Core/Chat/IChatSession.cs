using System;

namespace AutoBot.Core.Chat
{

    public interface IChatSession
    {

        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        void Connect();
        void Disconnect();

    }

}
