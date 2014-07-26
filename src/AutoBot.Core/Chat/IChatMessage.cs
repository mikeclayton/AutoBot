namespace AutoBot.Core.Chat
{

    /// <summary>
    /// Represents the text in a chat message received from a chat client.
    /// </summary>
    public interface IChatMessage
    {

        /// <summary>
        /// Gets the original text of the message received by the chat client.
        /// </summary>
        string OriginalText
        {
            get;
        }

        /// <summary>
        /// Gets the pre-processed command text to execute by the engine.
        /// </summary>
        string CommandText
        {
            get;
        }

    }

}