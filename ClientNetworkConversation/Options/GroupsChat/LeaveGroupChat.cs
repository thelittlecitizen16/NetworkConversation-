using Common;
using Common.Enums;
using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ClientNetworkConversation.Options.GroupsChat
{
    public class LeaveGroupChat :  IOption
    {
        public string OptionMessage => "Leave Group Chat";
        private static TcpClient _client;
        private HandleServer _handleServer;

        public LeaveGroupChat(TcpClient client, HandleServer handleServer)
        {
            _handleServer = handleServer;
            _client = client;
        }

        public void Run()
        {
            _handleServer.SendMessageToServer(_client, ClientOptions.LEAVE_GROUP_CHAT.ToString());

            try
            {
                AllGroupChat allGroupChat = (AllGroupChat)_handleServer.GetFromServer(_client);
                PrintAllGroups(allGroupChat);

                Console.WriteLine("enter group name");
                string userResponse = Console.ReadLine();

                if (CheckGroupName(userResponse, allGroupChat))
                {
                    _handleServer.SendMessageToServer(_client, userResponse);
                    Console.WriteLine("you leave group");
                   
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

    }
}
