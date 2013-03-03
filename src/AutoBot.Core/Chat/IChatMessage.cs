using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoBot.Core.Chat
{

    public interface IChatMessage
    {

        /// <summary>
        /// Gets the original text received by the chat client.
        /// </summary>
        string OriginalText { get; }
        
        /// <summary>
        /// Gets the pre-processed command to execute by the engine.
        /// </summary>
        string CommandText { get; }

    }

}
