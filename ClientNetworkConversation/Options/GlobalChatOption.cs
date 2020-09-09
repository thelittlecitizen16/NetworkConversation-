﻿using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common.Enums;
using System.Threading.Tasks;

namespace ClientNetworkConversation.Options
{
    public class GlobalChatOption : IOption
    {
        public string OptionMessage => "Enter To Global Chat";
        private static TcpClient _client;
        private HandleServer _handleServer;
        private ISystem _system;
        private string message;

        private bool endConnection = false;
        public GlobalChatOption(TcpClient client, HandleServer handleServer, ISystem system)
        {
            _handleServer = handleServer;
            _client = client;
            _system = system;
        }

        public void Run()
        {
            _handleServer.SendMessageToServer(_client, ClientOptions.GLOBAL_CHAT.ToString());

            try
            {
                Thread thread = new Thread(GetMessage);
                thread.Start();

                while (!endConnection)
                {
                    _system.Write("enter message, if you wand to exist global chat enter: 0");
                    message = _system.ReadString();

                    if (message == "0")
                    {
                        endConnection = true;
                        _handleServer.SendMessageToServer(_client, message);
                        break;
                    }
                    else
                    {
                        _handleServer.SendMessageToServer(_client, message);
                    }
                }


            }
            catch (Exception e)
            {
            }

            endConnection = false;
            Console.WriteLine("out of chat");
        }
        private void GetMessage()
        {
            string message = "";

            while (message != "0")
            {
                message = _handleServer.GetMessageFromServer(_client);
                _system.Write(message);
            }
        }
    }
}
