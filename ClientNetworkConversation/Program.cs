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
            //var ts = new CancellationTokenSource();
            //CancellationToken ct = ts.Token;

            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        Console.WriteLine("aaa");

            //        if (ct.IsCancellationRequested)
            //        {
            //            // another thread decided to cancel
            //            Console.WriteLine("task canceled");
            //            break;
            //        }
            //    }
            //}, ct);

            //// Simulate waiting 3s for the task to complete
            //Thread.Sleep(3000);

            //// Can't wait anymore => cancel this task 
            //ts.Cancel();
            //Console.ReadLine();

            HandleServer handleServer = new HandleServer();
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];

                client = new TcpClient();
                client.Connect(ipAddr, 7777);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }

            Menu menuOption1 = new Menu(client, handleServer);
            menuOption1.RunMenu();
            Console.ReadLine();
        }
    }
}
