using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common.Enums;
using System.Threading.Tasks;
using Common.HandleRequests;
using ClientNetworkConversation.Options.Utils;

namespace ClientNetworkConversation.Options
{
    public class GlobalChatOption : IOption
    {
        public string OptionMessage => "Enter To Global Chat";
        private static TcpClient _client;
        private IRequests _requests;
        private ISystem _system;
        private string message;

        private bool endConnection = false;
        public GlobalChatOption(TcpClient client, IRequests requests, ISystem system)
        {
            _client = client;
            _requests = requests;
            _system = system;
        }

        public void Run()
        {
            _requests.SendStringMessage(_client, ClientOptions.GLOBAL_CHAT.ToString());

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

                        _requests.SendStringMessage(_client, message);
                        break;
                    }
                    else
                    {
                        _requests.SendStringMessage(_client, message);
                    }
                }


            }
            catch (Exception e)
            {
            }

            endConnection = false;
        }
        private void GetMessage()
        {
            ChatUtils.GetMessage(_requests, _system, _client);
        }
    }
}
