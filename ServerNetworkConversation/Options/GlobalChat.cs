﻿using System;
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

namespace ServerNetworkConversation.Options
{
    public class GlobalChat : IClientOption
    {
        private TcpClient clientSocket;
        private Data _data;
        private HandleClient _handleClient;
        private RemoveClient _removeClient;
        private Thread _thread;
        private ILogger<Worker> _logger;

        public GlobalChat(Data data, TcpClient inClientSocket, HandleClient handleClient, RemoveClient removeClient, ILogger<Worker> logger)
        {
            clientSocket = inClientSocket;
            _data = data;
            _handleClient = handleClient;
            _removeClient = removeClient;
            _logger = logger;
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
            var clientGuid = _data.ClientsInGlobalChat.GetClient(clientSocket);
            SendAllAboutEnter(clientGuid);


            while (!end)
            {
                try
                {
                    string dataReceived = _handleClient.GetMessageFromClient(clientSocket);

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
                    _removeClient.RemoveClientWhenOut(clientSocket,clientGuid);
                    _data.ClientsInGlobalChat.Remove(clientGuid);
                }
            }
        }
        private void SendMessageToEachClient(string message)
        {
            foreach (var client in _data.ClientsInGlobalChat.Clients)
            {
                if (client.Value.Connected)
                {
                    _handleClient.SendMessageToClient(client.Value, message);
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

            _handleClient.SendMessageToClient(clientSocket, "0");
            _logger.LogInformation($"client {clientGuid} exit global chat");
        }

        private void SendAllMessage(Guid clientGuid, string dataReceived)
        {
          _logger.LogInformation($"send {dataReceived} from {clientGuid} in globalchat");

            string message = $"{clientGuid} send: {dataReceived}";
            SendMessageToEachClient(message);
            _logger.LogInformation($"client {clientGuid} send {message} in global chat");
        }
    }

}
