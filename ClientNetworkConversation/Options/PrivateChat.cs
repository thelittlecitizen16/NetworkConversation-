using Common.Enums;
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
        private HandleServer _handleServer;

        private bool endConnection = false;
        public PrivateChat(TcpClient client, HandleServer handleServer)
        {
            _handleServer = handleServer;
            _client = client;
        }

        public void Run()
        {
            _handleServer.SendMessageToServer(_client, ClientOptions.PRIVATE_CHAT.ToString());


            try
            {
                string messageRecive;
                messageRecive = _handleServer.GetMessageFromServer(_client);
                Console.WriteLine(messageRecive);

                string messageToSend = Console.ReadLine();
                _handleServer.SendMessageToServer(_client, messageToSend);
                messageRecive = _handleServer.GetMessageFromServer(_client);

                if (messageRecive == "success")
                {
                    endConnection = false;
                    Thread ctThread = new Thread(GetMessage);
                    ctThread.Start();

                    while (!endConnection)
                    {
                        Console.WriteLine("enter message, if you wand to exist chat enter: 0");
                        string message = Console.ReadLine();

                        if (message == "0")
                        {
                            endConnection = true;
                        }

                        _handleServer.SendMessageToServer(_client, message);
                    }
                }
                else
                {
                    Console.WriteLine("you enter guid that not exist");
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
                string returndata = "";
                ; while (returndata != "0")
                {
                    returndata = _handleServer.GetMessageFromServer(_client);
                    Console.WriteLine(returndata);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
    }
}
