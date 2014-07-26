using AutoBot.Core.Chat;
using System;

namespace AutoBot.ChatClients.Skype
{

    /// <summary>
    /// Represents the text in a chat message received from a Skype chat client.
    /// </summary>
    public sealed class SkypeMessage : IChatMessage
    {

        #region Constructors

        public SkypeMessage(string originalText, string commandText)
            : base()
        {
            this.OriginalText = originalText;
            this.CommandText = commandText;
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
