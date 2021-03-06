﻿using ClientNetworkConversation.Options.Utils;
using Common.Enums;
using Common.HandleRequests;
using Common.Models;
using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace ClientNetworkConversation.Options.GroupsChat
{
    public class ManagerSettings : IOption
    {
        public string OptionMessage => "Manager Settings";
        private static TcpClient _client;
        private IRequests _requests;
        private ISystem _system;
        public ManagerSettings(TcpClient client,  IRequests requests, ISystem system)
        {
            _client = client;
            _requests = requests;
            _system = system;
        }

        public void Run()
        {
            try
            {
                _requests.SendStringMessage(_client, ClientOptions.MANAGER_SETTINGS.ToString());

                AllGroupChat allGroupChat = (AllGroupChat)_requests.GetModelMessage(_client);
                PrintAllGroups(allGroupChat);

                if (allGroupChat.GroupsName.Count>0)
                {
                    _system.Write("enter group name");
                    string userResponse = _system.ReadString();

                    if (CheckGroupName(userResponse, allGroupChat))
                    {
                        _requests.SendStringMessage(_client, userResponse);
                        GroupChat groupChat = (GroupChat)_requests.GetModelMessage(_client);
                        Participants participants = (Participants)_requests.GetModelMessage(_client);


                        _system.Write("enter names you want to remove from group, when end enter o");
                        PrintParticipants(groupChat.Participants);
                        List<Guid> usersToRemove = GetAllParticipants(groupChat.Participants);


                        _system.Write("enter names you want to add to group, when end enter o");
                        PrintParticipants(participants.AllParticipants);
                        List<Guid> usersToAdd = GetAllParticipants(participants.AllParticipants);
                        usersToAdd.RemoveAll(u => CheckIfParticipantsExist(u, groupChat.Participants) == true);

                        _system.Write("enter names you want to add as mangers, when end enter o");
                        List<Guid> usersToAddAsMangers = GetAllParticipants(groupChat.Participants);
                        usersToAddAsMangers.RemoveAll(u => CheckIfParticipantsExist(u, usersToRemove) == true);

                        ChangeGroup(groupChat, usersToRemove, usersToAdd, usersToAddAsMangers);

                        _requests.SendModelMessage(_client, groupChat);
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
                    _system.Write("you dont have any group that you managment");
                }
            }
            catch (Exception)
            {
            }
        }

        private void PrintAllGroups(AllGroupChat allGroupChat)
        {
            GruopUtils.PrintString(allGroupChat.GroupsName, _system);
        }
        private void PrintParticipants(List<Guid> participants)
        {
            GruopUtils.PrintString(participants.Select(p=>p.ToString()).ToList(), _system);
        }
        private bool CheckGroupName(string userResponse, AllGroupChat allGroupChat)
        {
            return GruopUtils.CheckGroupName(userResponse, allGroupChat);
        }
        private bool CheckIfParticipantsExist(Guid userGuid, List<Guid> participants)
        {
            return participants.Contains(userGuid);
        }
        private List<Guid> GetAllParticipants(List<Guid> allParticipants)
        {
            List<Guid> selectedParticipants = new List<Guid>();
            string userResponse = "";

            while (userResponse != "0")
            {
                userResponse = _system.ReadString();

                Guid userGuid;

                if (Guid.TryParse(userResponse, out userGuid))
                {
                    if (CheckIfParticipantsExist(userGuid, allParticipants))
                    {
                        selectedParticipants.Add(userGuid);
                    }
                    else
                    {
                        _system.Write($"the user guid not exist");
                    }

                }
                else if (userResponse != "0")
                {
                    _system.Write($"the user guid not exist");
                }
            }

            return selectedParticipants;
        }
        private void ChangeGroup(GroupChat groupChat,List<Guid> usersToRemove, List<Guid> usersToAdd, List<Guid> usersToAddAsMangers)
        {
            foreach (var user in usersToRemove)
            {
                groupChat.Participants.Remove(user);
            }

            groupChat.Participants.AddRange(usersToAdd);
            groupChat.Managers.AddRange(usersToAddAsMangers);
        }
    }
}
