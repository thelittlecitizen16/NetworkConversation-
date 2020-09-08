using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientNetworkConversation.Options
{
    public class GlobalChatOption : IOption
    {
        public string OptionMessage => "Enter To Global Chat";
        private static TcpClient _client;
        private HandleServer _handleServer;

        private bool endConnection = false;
        public GlobalChatOption(TcpClient client, HandleServer handleServer)
        {
            _handleServer = handleServer;
            _client = client;
        }
        public void Run()
        {
            _handleServer.SendMessageToServer(_client,"1");

            try
            {

                endConnection = false;
                Thread ctThread = new Thread(GetMessage);
                ctThread.Start();

                while (!endConnection)
                {
                    Console.WriteLine("enter message, if you wand to exist global chat enter: 0");
                    string message = Console.ReadLine();

                    if (message == "0")
                    {
                        endConnection = true;
                    }

                    _handleServer.SendMessageToServer(_client, message);
                }

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
            Console.WriteLine("out of chat");
        }
        private void GetMessage()
        {
            try
            {
                while (!endConnection)
                {
                    string returndata = _handleServer.GetMessageFromServer(_client);

                    if (returndata != "")
                    {
                        Console.WriteLine(returndata);
                        returndata = "";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
    }
}
