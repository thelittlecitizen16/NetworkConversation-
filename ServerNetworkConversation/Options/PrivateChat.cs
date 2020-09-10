using Common.HandleRequests;
using Microsoft.Extensions.Logging;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Options.HandleOptions;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerNetworkConversation.Options
{
    public class PrivateChat : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private RemoveClient _removeClient;
        private Thread _thread;
        private ILogger<Worker> _logger;
        private IRequests _requests;

        public PrivateChat(Data data, TcpClient inClientSocket, RemoveClient removeClient, ILogger<Worker> logger, IRequests requests)
        {
            _client = inClientSocket;
            _data = data;
            _removeClient = removeClient;
            _logger = logger;
            _requests = requests;
        }
        public Thread Run()
        {
            _thread = new Thread(StartPrivateChat);
            _thread.Start();
            return _thread;
        }

        private void StartPrivateChat()
        {
            Guid clientGuid = _data.ClientsConnectedInServer.GetGuid(_client);
            Guid guidToSend;

            try
            {
                SendAllClientsConnected(clientGuid);

                string dataReceived = _requests.GetStringMessage(_client);
                TcpClient clientSend;

                Guid.TryParse(dataReceived, out guidToSend);

                if (_data.ClientsConnectedInServer.Clients.TryGetValue(guidToSend, out clientSend))
                {
                    bool end = false;
                    AddPrivateChat(clientGuid, guidToSend);
                    SendMessagesHistory(clientGuid, guidToSend);

                    while (!end)
                    {
                        dataReceived = _requests.GetStringMessage(_client);

                        if (dataReceived == "0")
                        {
                            ExistChat(clientGuid, guidToSend);
                            end = true;
                        }
                        else
                        {
                            SendMessage(dataReceived, clientGuid, guidToSend, clientSend);
                        }
                    }
                }
                else
                {
                    string message = $"fail";
                    _requests.SendStringMessage(_client, message);
                }
            }
            catch (Exception)
            {
                _removeClient.RemoveClientWhenOut(_client, clientGuid);
            }
        }
        private void SendMessagesHistory(Guid clientGuid, Guid guidToSend)
        {
            string allMessages = "";
            foreach (var message in _data.ClientsConnectedInChat.GetMessagesToHistory(clientGuid, guidToSend))
            {
                allMessages += message + "\n";
            }
            if (allMessages != "")
            {
                _requests.SendStringMessage(_client, allMessages);
            }
        }
        private void SendAllClientsConnected(Guid clientGuid)
        {
            string message = $"the clients you can chat with: \n";

            foreach (var clientConnected in _data.ClientsConnectedInServer.Clients)
            {
                var guidClient = _data.ClientsConnectedInServer.GetGuid(clientConnected.Value);

                if (guidClient != clientGuid)
                {
                    message += $"{guidClient} \n";
                }
            }

            _requests.SendStringMessage(_client, message);
        }

        private void AddPrivateChat(Guid clientGuid, Guid guidToSend)
        {
            string message = $"success";
            _requests.SendStringMessage(_client, message);
            _data.ClientsConnectedInChat.Add(clientGuid, guidToSend);
            _logger.LogInformation($"client {clientGuid} send {message} to client {message}");
        }

        private void ExistChat(Guid clientGuid, Guid guidToSend)
        {
            _requests.SendStringMessage(_client, "0");
            _data.ClientsConnectedInChat.Remove(clientGuid, guidToSend);
            _logger.LogInformation($"client {clientGuid} leave chat with client {guidToSend}");
        }

        private void SendMessage(string dataReceived, Guid clientGuid, Guid guidToSend, TcpClient clientSend)
        {
            _logger.LogInformation($"Received from {clientGuid} and Sending to {guidToSend} : {dataReceived}");

            if (_data.ClientsConnectedInChat.HaveConversition(clientGuid, guidToSend))
            {
                _requests.SendStringMessage(clientSend, dataReceived);
                _logger.LogInformation($"client {clientSend} get message {dataReceived}");
            }

            _data.ClientsConnectedInChat.AddMessagesToHistory(clientGuid, guidToSend, dataReceived);
        }
    }

}
