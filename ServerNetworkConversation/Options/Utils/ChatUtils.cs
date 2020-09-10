using Common.HandleRequests;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ServerNetworkConversation.HandleData;
using System.Text;
using Common.Models;
using Common.Enums;

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
                MessageRequest messageRequest = new MessageRequest(MessageKey.STRING, allMessages);
                requests.SendModelMessage(client,messageRequest);
            }
        }

        public static void SendMessageToEachClient(string message, List<TcpClient> clients, IRequests requests)
        {
            foreach (var client in clients)
            {
                if (client.Connected)
                {
                    MessageRequest messageRequest = new MessageRequest(MessageKey.STRING, message);
                    requests.SendModelMessage(client, messageRequest);
                }
            }
        }
        public static void RemoveClientWhenOut(TcpClient client, Guid clientGuid, Data data)
        {
            client.Close();
            data.ClientsConnectedInServer.Remove(clientGuid);
        }
        public static void SandStringMessage(TcpClient client, IRequests requests, string message)
        {
            MessageRequest messageRequest = new MessageRequest(MessageKey.STRING, message);
            requests.SendModelMessage(client, messageRequest);
        }
        public static void CreateAlertMessage(TcpClient client,Data data, AlertOptions alertOptions, string message)
        {
            Alert alert = new Alert(alertOptions, message);
            MessageRequest messageRequestAlert = new MessageRequest(MessageKey.ALERT, alert);
            data.ClientsAlerts.AddNewAlert(client, messageRequestAlert);
        }
    }
}
