using AutoBot.Core.Chat;

namespace AutoBot.Core.Engine
{

    public interface IAutoBotAgent
    {

        void ProcessMessage(IChatMessage message, IChatResponse response);

    }

}
