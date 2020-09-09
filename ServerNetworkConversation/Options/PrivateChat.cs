﻿using Microsoft.Extensions.Logging;
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
        private TcpClient clientSocket;
        private Data _data;
        private HandleClient _handleClient;
        private RemoveClient _removeClient;
        private Thread _thread;
        private ILogger<Worker> _logger;

        public PrivateChat(Data data, TcpClient inClientSocket, HandleClient handleClient, RemoveClient removeClient, ILogger<Worker> logger)
        {
            clientSocket = inClientSocket;
            _data = data;
            _handleClient = handleClient;
            _removeClient = removeClient;
            _logger = logger;
        }
        public Thread Run()
        {
            _thread = new Thread(StartPrivateChat);
            _thread.Start();
            return _thread;
        }

        private void StartPrivateChat()
        {
            Guid clientGuid = _data.ClientsConnectedInServer.GetGuid(clientSocket);
            Guid guidToSend;

            try
            {
                SendAllClientsConnected(clientGuid);

                string dataReceived = _handleClient.GetMessageFromClient(clientSocket);
                TcpClient clientSend;
              
                Guid.TryParse(dataReceived, out guidToSend);

                if (_data.ClientsConnectedInServer.Clients.TryGetValue(guidToSend, out clientSend))
                {      
                    bool end = false;
                    AddPrivateChat(clientGuid, guidToSend);

                    while (!end)
                    while (!end)
                    {
                        dataReceived = _handleClient.GetMessageFromClient(clientSocket);

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
                    string  message = $"fail";
                    _handleClient.SendMessageToClient(clientSocket, message);
                }
            }
            catch (Exception)
            {
                _removeClient.RemoveClientWhenOut(clientSocket,clientGuid);
            }
        }

        private void SendAllClientsConnected(Guid clientGuid)
        {
            string message = $"the clients you can chat with: \n";

            foreach (var clientConnected in _data.ClientsConnectedInServer.Clients)
            {
                var guidClient = _data.ClientsConnectedInServer.GetGuid(clientConnected.Value);

                if (guidClient!= clientGuid)
                {
                    message += $"{guidClient} \n";
                }
            }

            _handleClient.SendMessageToClient(clientSocket, message);
        }

        private void AddPrivateChat(Guid clientGuid, Guid guidToSend)
        {
            string message = $"success";
            _handleClient.SendMessageToClient(clientSocket, message);
            _data.ClientsConnectedInChat.Add(clientGuid, guidToSend);
            _logger.LogInformation($"client {clientGuid} send {message} to client {message}");
        }

        private void ExistChat(Guid clientGuid, Guid guidToSend)
        {
            _handleClient.SendMessageToClient(clientSocket, "0");
            _data.ClientsConnectedInChat.Remove(clientGuid, guidToSend);
            _logger.LogInformation($"client {clientGuid} leave chat with client {guidToSend}");
        }

        private void SendMessage(string dataReceived,Guid clientGuid, Guid guidToSend, TcpClient clientSend)
        {
           _logger.LogInformation($"Received from {clientGuid} and Sending to {guidToSend} : {dataReceived}");

            if (_data.ClientsConnectedInChat.HaveConversition(clientGuid, guidToSend))
            {
                _handleClient.SendMessageToClient(clientSend, dataReceived);
                _logger.LogInformation($"client {clientSend} get message {dataReceived}");
            }
        }
    }

}
