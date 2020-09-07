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
        private bool endConnection = false;
        public GlobalChatOption(TcpClient client)
        {
            _client = client;
        }
        public void Run()
        {
            NetworkStream nwStream = _client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("1");
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);

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
                        nwStream = _client.GetStream();
                        bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);

                        nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                    }
                    else
                    {
                        nwStream = _client.GetStream();
                        bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);

                        nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                    }

                }

                ctThread.Abort();

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
            Console.WriteLine("enter to chat");
        }
        private void GetMessage()
        {
            try
            {
                while (!endConnection)
                {
                    NetworkStream serverStream = _client.GetStream();
                    byte[] bytesToRead = new byte[_client.ReceiveBufferSize];
                    int bytesRead = serverStream.Read(bytesToRead, 0, _client.ReceiveBufferSize);
                    string returndata = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

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
