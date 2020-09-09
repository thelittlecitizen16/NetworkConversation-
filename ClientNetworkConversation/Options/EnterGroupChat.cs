using Common;
using Common.Enums;
using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientNetworkConversation.Options
{
    public class EnterGroupChat : IOption
    {
        public string OptionMessage => "Enter To Open Group Chat";
        private static TcpClient _client;
        private HandleServer _handleServer;

        private bool endConnection = false;
        public EnterGroupChat(TcpClient client, HandleServer handleServer)
        {
            _handleServer = handleServer;
            _client = client;
        }

        public void Run()
        {
            _handleServer.SendMessageToServer(_client, ClientOptions.GROUP_CHAT.ToString());
            endConnection = false;

            try
            {
                AllGroupChat allGroupChat = (AllGroupChat)_handleServer.GetFromServer(_client);
                PrintAllGroups(allGroupChat);

                Console.WriteLine("enter group name");
                string userResponse = Console.ReadLine();

                if (CheckGroupName(userResponse, allGroupChat))
                {
                    _handleServer.SendMessageToServer(_client, userResponse);
                    ListenToServer();

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
                    _handleServer.SendMessageToServer(_client, "0");
                    Console.WriteLine("the group  not exist");
                }
            }
            catch (Exception e)
            {
            }
            Console.WriteLine("out of chat");
        }
        private void GetMessage()
        {
            try
            {
                string returndata = "";
                while (returndata != "0")
                {
                    returndata = _handleServer.GetMessageFromServer(_client);
                    Console.WriteLine(returndata);
                }
            }
            catch (Exception e)
            {
            }
        }
        private void PrintAllGroups(AllGroupChat allGroupChat)
        {
            foreach (var groupName in allGroupChat.GroupsName)
            {
                Console.WriteLine(groupName);
            }
        }
        private bool CheckGroupName(string userResponse, AllGroupChat allGroupChat)
        {
            return allGroupChat.GroupsName.Contains(userResponse);
        }
        private void ListenToServer()
        {
            Thread ctThread = new Thread(GetMessage);
            ctThread.Start();
        }
    }
}
