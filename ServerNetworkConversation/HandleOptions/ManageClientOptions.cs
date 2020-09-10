using ServerNetworkConversation.Options;
using ServerNetworkConversation.Options.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Enums;
using ServerNetworkConversation.HandleData;
using Microsoft.Extensions.Logging;
using ServerNetworkConversation.Interfaces;
using Common.HandleRequests;

namespace ServerNetworkConversation.HandleOptions
{
    public class ManageClientOptions
    {
        private TcpClient _clientSocket;
        private Data _data;
        private IClientOptionsFactory _clientOptionsFactory;
        private ILogger<Worker> _logger;
        private IRequests _requests;
        public ManageClientOptions(Data data, TcpClient inClientSocket, IClientOptionsFactory clientOptionsFactory, ILogger<Worker> logger, IRequests requests)
        {
            _clientSocket = inClientSocket;
            _data = data;
            _clientOptionsFactory = clientOptionsFactory;
            _logger = logger;
            _requests = requests;
        }

        public void Run()
        {
            Task.Run(() =>
             GetClientChoice()
            );
        }

        private void GetClientChoice()
        {
            while (true)
            {
                if (!_clientSocket.Connected)
                {
                    break;
                }
                else
                {
                    try
                    {
                        string dataReceived = _requests.GetStringMessage(_clientSocket); //_handleClient.GetMessageFromClient(_clientSocket);
                        ClientOptions choice;

                        if (Enum.TryParse(dataReceived, out choice))
                        {
                            switch (choice)
                            {
                                case ClientOptions.GLOBAL_CHAT:
                                    _data.ClientsInGlobalChat.Add(_data.ClientsConnectedInServer.GetGuid(_clientSocket), _clientSocket);
                                    Thread globalChat = _clientOptionsFactory.AddClientOptions(ClientOptions.GLOBAL_CHAT, _data, _clientSocket, _logger, _requests).Run();
                                    globalChat.Join();
                                    break;

                                case ClientOptions.PRIVATE_CHAT:
                                    Thread privateChat = _clientOptionsFactory.AddClientOptions(ClientOptions.PRIVATE_CHAT, _data, _clientSocket, _logger, _requests).Run();
                                    privateChat.Join();
                                    break;

                                case ClientOptions.CREATE_GROUP_CHAT:
                                    Thread CreateGroupChat = _clientOptionsFactory.AddClientOptions(ClientOptions.CREATE_GROUP_CHAT, _data, _clientSocket, _logger, _requests).Run();
                                    CreateGroupChat.Join();
                                    break;

                                case ClientOptions.GROUP_CHAT:
                                    Thread EnterGroupChat = _clientOptionsFactory.AddClientOptions(ClientOptions.GROUP_CHAT, _data, _clientSocket, _logger, _requests).Run();
                                    EnterGroupChat.Join();
                                    break;

                                case ClientOptions.MANAGER_SETTINGS:
                                    Thread managerSettings = _clientOptionsFactory.AddClientOptions(ClientOptions.MANAGER_SETTINGS, _data, _clientSocket, _logger, _requests).Run();
                                    managerSettings.Join();
                                    break;

                                case ClientOptions.LEAVE_GROUP_CHAT:
                                    Thread leavrGroupChat = _clientOptionsFactory.AddClientOptions(ClientOptions.LEAVE_GROUP_CHAT, _data, _clientSocket, _logger, _requests).Run();
                                    leavrGroupChat.Join();
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
