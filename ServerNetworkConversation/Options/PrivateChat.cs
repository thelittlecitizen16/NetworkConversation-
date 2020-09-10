using Common.Enums;
using Common.HandleRequests;
using Common.Models;
using Microsoft.Extensions.Logging;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Options.Interfaces;
using ServerNetworkConversation.Options.Utils;
using System;
using System.Net.Sockets;
using System.Threading;

namespace ServerNetworkConversation.Options
{
    public class PrivateChat : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private Thread _thread;
        private ILogger<Worker> _logger;
        private IRequests _requests;

        public PrivateChat(Data data, TcpClient client,  ILogger<Worker> logger, IRequests requests)
        {
            _client = client;
            _data = data;
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
                    ChatUtils.SandStringMessage(_client,_requests, message);
                }
            }
            catch (Exception)
            {
                ChatUtils.RemoveClientWhenOut(_client, clientGuid, _data);
            }
        }
        private void SendMessagesHistory(Guid clientGuid, Guid guidToSend)
        {
            ChatUtils.SendMessagesHistory(_data.ClientsConnectedInChat.GetMessagesToHistory(clientGuid, guidToSend), _client, _requests);
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
            MessageRequest messageRequest = new MessageRequest(MessageKey.Exit, "0");
            _requests.SendModelMessage(_client, messageRequest);

            _data.ClientsConnectedInChat.Remove(clientGuid, guidToSend);
            _logger.LogInformation($"client {clientGuid} leave chat with client {guidToSend}");
        }
        private void SendMessage(string dataReceived, Guid clientGuid, Guid guidToSend, TcpClient clientSend)
        {
            _logger.LogInformation($"Received from {clientGuid} and Sending to {guidToSend} : {dataReceived}");

            if (_data.ClientsConnectedInChat.HaveConversition(clientGuid, guidToSend))
            {
                ChatUtils.SandStringMessage(clientSend, _requests, dataReceived);
                _logger.LogInformation($"client {clientSend} get message {dataReceived}");
            }
            else
            {
                ChatUtils.CreateAlertMessage(clientSend,_data, AlertOptions.NEW_MESSAGE, $"new message from {clientGuid}");       
            }        

            _data.ClientsConnectedInChat.AddMessagesToHistory(clientGuid, guidToSend, dataReceived);
        }
    }
}
