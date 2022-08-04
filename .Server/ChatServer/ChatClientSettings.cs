namespace SmallChatServer
{
    public struct ChatClientSettings
    {
        public string Nickname { get; private set; }
        public string Color { get; private set; }

        public ChatClientSettings(string nickname, string color)
        {
            Nickname = nickname;
            Color = color;
        }
    }
}