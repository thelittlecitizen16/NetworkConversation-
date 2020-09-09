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
        private ISystem _system;

        public LeaveGroupChat(TcpClient client, HandleServer handleServer, ISystem system)
        {
            _handleServer = handleServer;
            _client = client;
            _system = system;
        }

        public void Run()
        {
            _handleServer.SendMessageToServer(_client, ClientOptions.LEAVE_GROUP_CHAT.ToString());

            try
            {
                AllGroupChat allGroupChat = (AllGroupChat)_handleServer.GetFromServer(_client);
                PrintAllGroups(allGroupChat);

                _system.Write("enter group name");
                string userResponse = _system.ReadString();

                if (CheckGroupName(userResponse, allGroupChat))
                {
                    _handleServer.SendMessageToServer(_client, userResponse);
                    _system.Write("you leave group");
                   
                }
                else
                {
                    _handleServer.SendMessageToServer(_client, "0");
                    _system.Write("the group  not exist");
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
                _system.Write(groupName);
            }
        }
        private bool CheckGroupName(string userResponse, AllGroupChat allGroupChat)
        {
            return allGroupChat.GroupsName.Contains(userResponse);
        }

    }
}
