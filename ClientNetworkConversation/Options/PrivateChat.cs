using Common.Enums;
using Common.HandleRequests;
using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientNetworkConversation.Options
{
    public class PrivateChat : IOption
    {
        public string OptionMessage => "Enter To private Chats";
        private static TcpClient _client;
        private IRequests _requests;
        private ISystem _system;


        private bool endConnection = false;
        public PrivateChat(TcpClient client,  IRequests requests, ISystem system)
        {
            _client = client;
            _requests = requests;
            _system = system;
        }

        public void Run()
        {
            _requests.SendStringMessage(_client, ClientOptions.PRIVATE_CHAT.ToString());


            try
            {
                string messageRecive;
                messageRecive = _requests.GetStringMessage(_client);
                _system.Write(messageRecive);

                string messageToSend = _system.ReadString();
                _requests.SendStringMessage(_client, messageToSend);
                messageRecive = _requests.GetStringMessage(_client);

                if (messageRecive == "success")
                {
                    endConnection = false;
                    Thread ctThread = new Thread(GetMessage);
                    ctThread.Start();

                    while (!endConnection)
                    {
                        _system.Write("enter message, if you wand to exist chat enter: 0");
                        string message = _system.ReadString();

                        if (message == "0")
                        {
                            endConnection = true;
                        }

                        _requests.SendStringMessage(_client, message);
                    }
                }
                else
                {
                    _system.Write("you enter guid that not exist");
                }
            }
            catch (Exception e)
            {
            }
        }
        private void GetMessage()
        {
            string message = "";

            while (message != "0")
            {
                message = _requests.GetStringMessage(_client);
                _system.Write(message);
            }
        }
    }
}
