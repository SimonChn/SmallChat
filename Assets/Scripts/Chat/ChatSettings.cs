namespace SmallChat
{
    public struct ChatSettings
    {
        public string Nickname { get; private set; }
        public string Color { get; private set; }

        public ChatSettings(string nickname, string color)
        {
            Nickname = nickname;
            Color = color;
        }
    }
}