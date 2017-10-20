/*
 * Author: Shon Verch
 * File Name: TextToSpeechChatCommand.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/20/2017
 * Modified Date: 10/20/2017
 * Description: Text-to-speech command for user messages.
 */

using System.Speech.Synthesis;

namespace NetworkCryptography.Core.ChatCommands
{
    /// <summary>
    /// Text-to-speech command for user messages.
    /// </summary>
    [UseChatCommand]
    public class TextToSpeechChatCommand : ChatCommand
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        public override string Name => "/tts";

        /// <summary>
        /// TTS speech synthesizer.
        /// </summary>
        private readonly SpeechSynthesizer speechSynthesizer;

        /// <summary>
        /// Initializes the <see cref="TextToSpeechChatCommand"/>
        /// </summary>
        public TextToSpeechChatCommand()
        {
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SetOutputToDefaultAudioDevice();
        }

        /// <summary>
        /// The execution logic of the command.
        /// </summary>
        /// <param name="chatMessage"></param>
        /// <param name="execute"></param>
        public override ChatMessage Execute(ChatMessage chatMessage, bool execute)
        {
            if (!execute) return chatMessage;

            string ttsMessage = $"{chatMessage.User.Name} said {chatMessage.Message}";
            speechSynthesizer.SpeakAsync(ttsMessage);

            return chatMessage;
        }
    }
}
