using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SmallChat
{
    public class ChatUserSettings
    {
        private static string savePath = string.Empty;

        public string Id { get;private set; }

        public string Nickname { get; private set; }
        public string Color { get; private set; }

        public ChatUserSettings(string nickname, string color, string id = Constants.Client.DefaultUserId)
        {
            Nickname = nickname;
            Color = color;

            Id = id;
        }

        public static ChatUserSettings TryLoad(string savePath)
        {
            ChatUserSettings.savePath = savePath;

            ChatUserSettings settings = null;

            try
            {
                using (FileStream fstream = new FileStream(GetFullFilePath(savePath), FileMode.Open))
                {
                    byte[] buffer = new byte[fstream.Length];

                    fstream.Read(buffer, 0, buffer.Length);
                    string json = Encoding.UTF8.GetString(buffer);
                    settings = JsonConvert.DeserializeObject<ChatUserSettings>(json);
                }
            }
            catch (Exception e)
            when (e is FileNotFoundException || e is JsonReaderException)
            {
                
            }

            return settings;
        }

        private static string GetFullFilePath(string loadPath)
        {
            if (!Directory.Exists(loadPath))
            {
                Directory.CreateDirectory(loadPath);
            }

            return @$"{loadPath}\{Constants.SaveFileName}";
        }

        public void Save()
        {
            if(string.IsNullOrEmpty(savePath))
            {
                return;
            }

            string json = JsonConvert.SerializeObject(this);
            using (FileStream fstream = new FileStream(GetFullFilePath(savePath), FileMode.Create))
            {
                byte[] input = Encoding.UTF8.GetBytes(json);
                fstream.Write(input, 0, input.Length);
            }
        }
    }
}