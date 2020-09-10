using Common.HandleRequests;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ServerNetworkConversation.HandleData;
using System.Text;

namespace ServerNetworkConversation.Options.Utils
{
    public static class ChatUtils
    {
        public static void SendMessagesHistory(List<string> messagesInHistory, TcpClient client, IRequests requests)
        {
            string allMessages = "";
            foreach (var message in messagesInHistory)
            {
                allMessages += message + "\n";
            }
            if (allMessages != "")
            {
                requests.SendStringMessage(client, allMessages);
            }
        }

        public static void SendMessageToEachClient(string message, List<TcpClient> clients, IRequests requests)
        {
            foreach (var client in clients)
            {
                if (client.Connected)
                {
                    requests.SendStringMessage(client, message);
                }
            }
        }
        public static void RemoveClientWhenOut(TcpClient client, Guid clientGuid, Data data)
        {
            client.Close();
            data.ClientsConnectedInServer.Remove(clientGuid);
        }
    }
}
