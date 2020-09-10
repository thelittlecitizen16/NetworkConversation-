using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using ServerNetworkConversation.Options.Interfaces;
using ServerNetworkConversation.HandleData;
using Microsoft.Extensions.Logging;
using Common.Enums;
using Common.HandleRequests;
using ServerNetworkConversation.Options.Utils;
using System.IO;
using Common.Models;

namespace ServerNetworkConversation.Options
{
    public class GlobalChat : IClientOption
    {
        private TcpClient _client;
        private Data _data;
        private Thread _thread;
        private ILogger<Worker> _logger;
        private IRequests _requests;

        public GlobalChat(Data data, TcpClient inClientSocket,  ILogger<Worker> logger, IRequests requests)
        {
            _client = inClientSocket;
            _data = data;
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

                    string dataReceivedType = _requests.GetStringMessage(_client);

                    if (dataReceivedType == "0")
                    {
                        _data.ClientsInGlobalChat.Remove(clientGuid);
                        SendAllAboutExist(clientGuid);
                        end = true;
                    }
                    else
                    {
                        if (dataReceivedType == MessageType.STRING.ToString())
                        {
                            string dataReceived = _requests.GetStringMessage(_client);
                            SendAllStringMessage(clientGuid, dataReceived);
                        }
                        else if (dataReceivedType == MessageType.PIC.ToString())
                        {
                            _requests.GetPictureMessage(_client);
                            SendAllPicMessage(clientGuid);
                        }   
                        else
                        {
                            _data.ClientsInGlobalChat.Remove(clientGuid);
                            SendAllAboutExist(clientGuid);
                            end = true;
                        }
                    }
                }
                catch (Exception)
                {
                    end = true;
                    ChatUtils.RemoveClientWhenOut(_client,clientGuid, _data);
                    _data.ClientsInGlobalChat.Remove(clientGuid);
                }
            }
        }
        private void SendMessagesHistory()
        {
            ChatUtils.SendMessagesHistory(_data.ClientsInGlobalChat.MessagesHistory,_client, _requests);
        }
        private void SendMessageToEachClient(string message)
        {
            ChatUtils.SendMessageToEachClient(message, _data.ClientsInGlobalChat.Clients.Values.ToList(), _requests);
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
            MessageRequest messageRequest = new MessageRequest(MessageKey.Exit, "0");
            _requests.SendModelMessage(_client, messageRequest);
            //_requests.SendStringMessage(_client, "0");
            _logger.LogInformation($"client {clientGuid} exit global chat");
        }

        private void SendAllStringMessage(Guid clientGuid, string dataReceived)
        {
          _logger.LogInformation($"send {dataReceived} from {clientGuid} in globalchat");

            string message = $"{clientGuid} send: {dataReceived}";
            SendMessageToEachClient(message);
           _data.ClientsInGlobalChat.MessagesHistory.Add(message);
        }
        private void SendAllPicMessage(Guid clientGuid)
        {
            _logger.LogInformation($"{clientGuid} send picture in globalchat");
            string message = $"{clientGuid} send picture";
            SendMessageToEachClient(message);
            _data.ClientsInGlobalChat.MessagesHistory.Add(message);

        }
    }
}
