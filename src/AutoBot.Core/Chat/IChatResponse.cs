using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoBot.Core.Chat
{
    
    public interface IChatResponse
    {

        /// <summary>
        /// Writes a message to the chat client response channel.
        /// </summary>
        /// <param name="text"></param>
        void Write(string text);

    }

}
