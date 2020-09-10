using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientNetworkConversation
{
    public static class CheckMessageType
    {
        public static MessageType GetMessageType(string message)
        {
            if (message.Contains("picture"))
            {
                return MessageType.PIC;
            }

            return MessageType.STRING;
        }
        public static string GetPictureUrl(string message)
        {
            string url = message.Split(';')[1];
            return url;
        }
    }
}
