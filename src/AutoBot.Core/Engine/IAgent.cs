using AutoBot.Core.Chat;

namespace AutoBot.Core.Engine
{

    public interface IAgent
    {

        void Execute(IChatMessage message, IChatResponse response);

    }

}
