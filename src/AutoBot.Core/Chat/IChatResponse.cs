namespace AutoBot.Core.Chat
{

    /// <summary>
    /// Represents a response channel that can be used to write
    /// response text while processing chat messages.
    /// </summary>
    public interface IChatResponse
    {

        /// <summary>
        /// Writes a message to the chat client response channel.
        /// </summary>
        /// <param name="text">The text to write to the chat client.</param>
        void Write(string text);

    }

}
