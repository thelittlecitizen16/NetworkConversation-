using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using ServerNetworkConversation.Options.Interfaces;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Options.HandleOptions;
using Microsoft.Extensions.Logging;
using Common.Enums;
using Common.HandleRequests;
using Common.HandleRequests.HandleMessages;

namespace ServerNetworkConversation.Options
{
    public class GlobalChat : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private RemoveClient _removeClient;
        private Thread _thread;
        private ILogger<Worker> _logger;
        private IRequests _requests;

        public GlobalChat(Data data, TcpClient inClientSocket,  RemoveClient removeClient, ILogger<Worker> logger, IRequests requests)
        {
            _client = inClientSocket;
            _data = data;
            _removeClient = removeClient;
            _logger = logger;
            _requests = requests;
        }
        public Thread Run()
        {
            _thread = new Thread(StartGlobalChat);
            _thread.Start();
            return _thread;

        }

        private void StartGlobalChat()
        {
            bool end = false;
            var clientGuid = _data.ClientsInGlobalChat.GetClient(_client);

            SendMessagesHistory();
            SendAllAboutEnter(clientGuid);

            while (!end)
            {
                try
                {

                    string dataReceived = _requests.GetStringMessage(_client);

                    if (dataReceived == "0")
                    {
                        _data.ClientsInGlobalChat.Remove(clientGuid);
                        SendAllAboutExist(clientGuid);
                        end = true;
                    }
                    else
                    {
                        SendAllMessage(clientGuid, dataReceived);
                    }
                }
                catch (Exception)
                {
                    end = true;
                    _removeClient.RemoveClientWhenOut(_client,clientGuid);
                    _data.ClientsInGlobalChat.Remove(clientGuid);
                }
            }
        }
        private void SendMessagesHistory()
        {
            string allMessages = "";
            foreach (var message in _data.ClientsInGlobalChat.MessagesHistory)
            {
                allMessages += message + "\n";
            }
            if (allMessages != "")
            {
                _requests.SendStringMessage(_client, allMessages);
            }
        }
        private void SendMessageToEachClient(string message)
        {
            foreach (var client in _data.ClientsInGlobalChat.Clients)
            {
                if (client.Value.Connected)
                {
                    _requests.SendStringMessage(client.Value, message);
                }
            }
        }

        private void SendAllAboutEnter(Guid clientGuid)
        {
            string message = $"{clientGuid} enter to global chat";
            SendMessageToEachClient(message);
            _logger.LogInformation($"client {clientGuid} enter global chat");
        }

        private void SendAllAboutExist(Guid clientGuid)
        {
            string message = $"{clientGuid} exist the global chat";
            SendMessageToEachClient(message);

            _requests.SendStringMessage(_client, "0");
            _logger.LogInformation($"client {clientGuid} exit global chat");
        }

        private void SendAllMessage(Guid clientGuid, string dataReceived)
        {
          _logger.LogInformation($"send {dataReceived} from {clientGuid} in globalchat");

            string message = $"{clientGuid} send: {dataReceived}";
            SendMessageToEachClient(message);
           _data.ClientsInGlobalChat.MessagesHistory.Add(message);

            _logger.LogInformation($"client {clientGuid} send {message} in global chat");
        }
    }

}
