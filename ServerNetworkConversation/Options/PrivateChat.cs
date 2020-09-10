using Common.Enums;
using Common.HandleRequests;
using Common.Models;
using Microsoft.Extensions.Logging;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Options.Interfaces;
using ServerNetworkConversation.Options.Utils;
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
        private Thread _thread;
        private ILogger<Worker> _logger;
        private IRequests _requests;

        public PrivateChat(Data data, TcpClient inClientSocket,  ILogger<Worker> logger, IRequests requests)
        {
            _client = inClientSocket;
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
                    MessageRequest messageRequest = new MessageRequest(MessageKey.STRING, message);
                    _requests.SendModelMessage(_client, messageRequest);
                    //_requests.SendStringMessage(_client, message);
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

           // MessageRequest messageRequest = new MessageRequest(MessageKey.STRING, message);
           // _requests.SendModelMessage(_client, messageRequest);
            _requests.SendStringMessage(_client, message);
        }

        private void AddPrivateChat(Guid clientGuid, Guid guidToSend)
        {
            string message = $"success";
            //MessageRequest messageRequest = new MessageRequest(MessageKey.STRING, message);
            //_requests.SendModelMessage(_client, messageRequest);
            _requests.SendStringMessage(_client, message);
            _data.ClientsConnectedInChat.Add(clientGuid, guidToSend);
            _logger.LogInformation($"client {clientGuid} send {message} to client {message}");
        }

        private void ExistChat(Guid clientGuid, Guid guidToSend)
        {
            MessageRequest messageRequest = new MessageRequest(MessageKey.Exit, "0");
            _requests.SendModelMessage(_client, messageRequest);
            // _requests.SendStringMessage(_client, "0");
            _data.ClientsConnectedInChat.Remove(clientGuid, guidToSend);
            _logger.LogInformation($"client {clientGuid} leave chat with client {guidToSend}");
        }

        private void SendMessage(string dataReceived, Guid clientGuid, Guid guidToSend, TcpClient clientSend)
        {
            _logger.LogInformation($"Received from {clientGuid} and Sending to {guidToSend} : {dataReceived}");

            if (_data.ClientsConnectedInChat.HaveConversition(clientGuid, guidToSend))
            {
                MessageRequest messageRequest = new MessageRequest(MessageKey.STRING, dataReceived);
                _requests.SendModelMessage(clientSend, messageRequest);
                // _requests.SendStringMessage(clientSend, dataReceived);
                _logger.LogInformation($"client {clientSend} get message {dataReceived}");
            }

            Alert alert = new Alert(AlertOptions.NEW_MESSAGE, $"new message from {clientGuid}");
            MessageRequest messageRequestAlert = new MessageRequest(MessageKey.ALERT, alert);
            _requests.SendModelMessage(clientSend, messageRequestAlert);
            _data.ClientsConnectedInChat.AddMessagesToHistory(clientGuid, guidToSend, dataReceived);
        }
    }

}
