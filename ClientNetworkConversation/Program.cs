using Common.HandleRequests;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ClientNetworkConversation
{
    class Program
    {
        public static TcpClient client;

        static void Main(string[] args)
        { 
            Requests requests = new Requests();

            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];

                client = new TcpClient();
                client.Connect(ipAddr, 7777);
            }
            catch (Exception e)
            {
            }

            Menu menuOption1 = new Menu(client, requests);
            menuOption1.RunMenu();
            Console.ReadLine();
        }
    }
}
