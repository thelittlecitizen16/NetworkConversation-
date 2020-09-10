using ClientNetworkConversation.HandleRequest;
using Common.Enums;
using Common.HandleRequests;
using Common.Models;
using MenuBuilder.Interfaces;
using System.IO;
using System.Net.Sockets;

namespace ClientNetworkConversation.Options.Utils
{
    public static class ChatUtils
    {
        public static void GetMessage(IRequests requests, ISystem system, TcpClient client)
        {
            MessageRequest messageRequest = (MessageRequest)requests.GetModelMessage(client);
            while (messageRequest.Key != MessageKey.Exit)
            {
                MessageRequestFactory.GetMessageOptionByKey(messageRequest.Key).HandleMessage(system, messageRequest.Value);
                messageRequest = (MessageRequest)requests.GetModelMessage(client);
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
