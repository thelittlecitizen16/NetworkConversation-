using ClientNetworkConversation.Options.Utils;
using Common;
using Common.Enums;
using Common.HandleRequests;
using Common.Models;
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
        private IRequests _requests;
        private ISystem _system;

        public LeaveGroupChat(TcpClient client, IRequests requests, ISystem system)
        {
            _client = client;
            _requests = requests;
            _system = system;
        }

        public void Run()
        {
            _requests.SendStringMessage(_client, ClientOptions.LEAVE_GROUP_CHAT.ToString());

            try
            {
                AllGroupChat allGroupChat = (AllGroupChat)_requests.GetModelMessage(_client);
                PrintAllGroups(allGroupChat);

                if (allGroupChat.GroupsName.Count> 0)
                {
                    _system.Write("enter group name");
                    string userResponse = _system.ReadString();

                    if (CheckGroupName(userResponse, allGroupChat))
                    {
                        _requests.SendStringMessage(_client, userResponse);
                        _system.Write("you leave group");

                    }
                    else
                    {
                        _requests.SendStringMessage(_client, "0");
                        _system.Write("the group not exist");
                    }
                }
                else
                {
                    _requests.SendStringMessage(_client, "0");
                    _system.Write("you dont have groups to leave");
                }

            }
            catch (Exception e)
            {
            }
        }

        private void PrintAllGroups(AllGroupChat allGroupChat)
        {
            GruopUtils.PrintString(allGroupChat.GroupsName, _system);
        }
        private bool CheckGroupName(string userResponse, AllGroupChat allGroupChat)
        {
            return GruopUtils.CheckGroupName(userResponse, allGroupChat);
        }

    }
}
