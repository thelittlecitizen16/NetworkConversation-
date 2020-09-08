using MenuBuilder.Interfaces;
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
        private string message;

        private bool endConnection = false;
        public GlobalChatOption(TcpClient client, HandleServer handleServer)
        {
            _handleServer = handleServer;
            _client = client;
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
                    Console.WriteLine("enter message, if you wand to exist global chat enter: 0");
                    message = Console.ReadLine();

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
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
            }
            catch (ArgumentNullException ane)
            {

                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }

            catch (SocketException se)
            {

                Console.WriteLine("SocketException : {0}", se.ToString());
            }

            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            endConnection = false;
            Console.WriteLine("out of chat");
        }
        private void GetMessage()
        {
            string s = "";

            while (s != "0")
            {
                s = _handleServer.GetMessageFromServer(_client);
                Console.WriteLine(s);
            }
        }
    }
}
