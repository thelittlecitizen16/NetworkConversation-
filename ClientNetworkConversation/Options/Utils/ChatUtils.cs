using Common.Enums;
using Common.HandleRequests;
using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ClientNetworkConversation.Options.Utils
{
    public static class ChatUtils
    {
        public static void GetMessage(IRequests requests, ISystem system, TcpClient client)
        {
            string message = requests.GetStringMessage(client);
            while (message != "0")
            {
                system.Write(message);
                message = requests.GetStringMessage(client);
            }
        }
        public static bool SendMessageByType(IRequests requests, TcpClient client,string message)
        {
            var messageType = CheckMessageType.GetMessageType(message);
           
            if (messageType == MessageType.STRING)
            {
                requests.SendStringMessage(client, MessageType.STRING.ToString());
                requests.SendStringMessage(client, message);
                return true;

            }
            else if(messageType == MessageType.PIC)
            {
                string urlPic = CheckMessageType.GetPictureUrl(message);

                if (File.Exists(urlPic))
                {
                    requests.SendStringMessage(client, MessageType.PIC.ToString());
                    requests.SendPictureMessage(client, urlPic);
                    return true;
                }
                else
                {
                    requests.SendStringMessage(client,"0");
                    return false;
                }
            }
            else
            {
                requests.SendStringMessage(client, "0");
                return false;
            }

        }
    }
}
