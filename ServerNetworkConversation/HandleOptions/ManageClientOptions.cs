using Common.Enums;
using Common.HandleRequests;
using Microsoft.Extensions.Logging;
using ServerNetworkConversation.HandleData;
using ServerNetworkConversation.Interfaces;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ServerNetworkConversation.HandleOptions
{
    public class ManageClientOptions
    {
        private TcpClient _client;
        private Data _data;
        private IClientOptionsFactory _clientOptionsFactory;
        private ILogger<Worker> _logger;
        private IRequests _requests;
        public ManageClientOptions(Data data, TcpClient client, IClientOptionsFactory clientOptionsFactory, ILogger<Worker> logger, IRequests requests)
        {
            _client = client;
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
                if (!_client.Connected)
                {
                    break;
                }
                else
                {
                    try
                    {
                        string dataReceived = _requests.GetStringMessage(_client); 
                        ClientOptions choice;

                        if (Enum.TryParse(dataReceived, out choice))
                        {
                            switch (choice)
                            {
                                case ClientOptions.GLOBAL_CHAT:
                                    _data.ClientsInGlobalChat.Add(_data.ClientsConnectedInServer.GetGuid(_client), _client);
                                    Thread globalChat = _clientOptionsFactory.AddClientOptions(ClientOptions.GLOBAL_CHAT, _data, _client, _logger, _requests).Run();
                                    globalChat.Join();
                                    break;

                                case ClientOptions.PRIVATE_CHAT:
                                    Thread privateChat = _clientOptionsFactory.AddClientOptions(ClientOptions.PRIVATE_CHAT, _data, _client, _logger, _requests).Run();
                                    privateChat.Join();
                                    break;

                                case ClientOptions.CREATE_GROUP_CHAT:
                                    Thread CreateGroupChat = _clientOptionsFactory.AddClientOptions(ClientOptions.CREATE_GROUP_CHAT, _data, _client, _logger, _requests).Run();
                                    CreateGroupChat.Join();
                                    break;

                                case ClientOptions.GROUP_CHAT:
                                    Thread EnterGroupChat = _clientOptionsFactory.AddClientOptions(ClientOptions.GROUP_CHAT, _data, _client, _logger, _requests).Run();
                                    EnterGroupChat.Join();
                                    break;

                                case ClientOptions.MANAGER_SETTINGS:
                                    Thread managerSettings = _clientOptionsFactory.AddClientOptions(ClientOptions.MANAGER_SETTINGS, _data, _client, _logger, _requests).Run();
                                    managerSettings.Join();
                                    break;

                                case ClientOptions.LEAVE_GROUP_CHAT:
                                    Thread leavrGroupChat = _clientOptionsFactory.AddClientOptions(ClientOptions.LEAVE_GROUP_CHAT, _data, _client, _logger, _requests).Run();
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
