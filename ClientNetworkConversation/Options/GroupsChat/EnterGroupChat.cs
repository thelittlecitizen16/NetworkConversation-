using Common;
using Common.Enums;
using Common.HandleRequests;
using Common.Models;
using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientNetworkConversation.Options.GroupsChat
{
    public class EnterGroupChat : IOption
    {
        public string OptionMessage => "Enter To Open Group Chat";
        private static TcpClient _client;
        private IRequests _requests;
        private ISystem _system;
        private bool endConnection = false;
        public EnterGroupChat(TcpClient client, IRequests requests, ISystem system)
        {
            _client = client;
            _requests = requests;
            _system = system;
        }

        public void Run()
        {
            _requests.SendStringMessage(_client, ClientOptions.GROUP_CHAT.ToString());
            endConnection = false;

            try
            {
                AllGroupChat allGroupChat = (AllGroupChat)_requests.GetModelMessage(_client);
              
                PrintAllGroups(allGroupChat);

                if (allGroupChat.GroupsName.Count>0)
                {
                    _system.Write("enter group name");
                    string userResponse = _system.ReadString();

                    if (CheckGroupName(userResponse, allGroupChat))
                    {
                        _requests.SendStringMessage(_client, userResponse);
                        ListenToServer();

                        while (!endConnection)
                        {
                            _system.Write("enter message, if you wand to exist chat enter: 0");
                            string message = _system.ReadString();

                            if (message == "0")
                            {
                                endConnection = true;
                            }

                            _requests.SendStringMessage(_client, message);
                        }
                    }
                    else
                    {
                        _requests.SendStringMessage(_client, "0");
                        _system.Write("the group  not exist");
                    }
                }
                else
                {
                    _requests.SendStringMessage(_client, "0");
                    _system.Write("you dont have group to enter");
                }

            }
            catch (Exception e)
            {
            }
        }
        private void GetMessage()
        {
            try
            {
                string returndata = "";
                while (returndata != "0")
                {
                    returndata = _requests.GetStringMessage(_client);
                    _system.Write(returndata);
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
        private void ListenToServer()
        {
            Thread ctThread = new Thread(GetMessage);
            ctThread.Start();
        }
    }
}
