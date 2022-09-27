namespace TcpApp_Classes
{
    public class ChatObject
    {
        public string Message { get; set; }
        public string SourceName { get; set; }
        public string TimeStamp { get; set; } 

        public ChatObject(string message, string sourceName)
        {
            Message = message;
            SourceName = sourceName;
            TimeStamp = DateTimeOffset.UtcNow
                        .ToLocalTime().ToString("T");
        }
    }
}