using OpenAI.Chat;

namespace BigShotCore.Data.Services
{
    internal class ChatRequest
    {
        public string Model { get; set; }
        public List<ChatMessage> Messages { get; set; }
        public double Temperature { get; set; }
        public int MaxTokens { get; set; }
    }
}