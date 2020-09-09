﻿using Common;
using Common.Enums;
using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ClientNetworkConversation.Options.GroupsChat
{
    public class ManagerSettings : IOption
    {
        public string OptionMessage => "Manager Settings";
        private static TcpClient _client;
        private HandleServer _handleServer;

        public ManagerSettings(TcpClient client, HandleServer handleServer)
        {
            _handleServer = handleServer;
            _client = client;
        }

        public void Run()
        {
            try
            {
                _handleServer.SendMessageToServer(_client, ClientOptions.MANAGER_SETTINGS.ToString());

                AllGroupChat allGroupChat = (AllGroupChat)_handleServer.GetFromServer(_client);
                PrintAllGroups(allGroupChat);

                Console.WriteLine("enter group name");
                string userResponse = Console.ReadLine();

                if (CheckGroupName(userResponse, allGroupChat))
                {
                    _handleServer.SendMessageToServer(_client, userResponse);
                    GroupChat groupChat = (GroupChat)_handleServer.GetFromServer(_client);
                    Participants participants = (Participants)_handleServer.GetFromServer(_client);

                   
                    Console.WriteLine("enter names you want to remove from group, when end enter o");
                    PrintParticipants(groupChat.Participants);
                    List<Guid> usersToRemove = GetAllParticipants(groupChat.Participants);

                    
                    Console.WriteLine("enter names you want to add to group, when end enter o");
                    PrintParticipants(participants.AllParticipants);
                    List<Guid> usersToAdd = GetAllParticipants(participants.AllParticipants);
                    usersToAdd.RemoveAll(u => CheckIfParticipantsExist(u, groupChat.Participants) == true);
                    
                    Console.WriteLine("enter names you want to add as mangers, when end enter o");
                    List<Guid> usersToAddAsMangers =  GetAllParticipants(groupChat.Participants);
                    usersToAddAsMangers.RemoveAll(u => CheckIfParticipantsExist(u, usersToRemove) == true);

                    ChangeGroup(groupChat, usersToRemove, usersToAdd, usersToAddAsMangers);

                    _handleServer.SendToServer(_client, groupChat);
                }
                else
                {
                    _handleServer.SendMessageToServer(_client, "0");
                    Console.WriteLine("the group  not exist");
                }
            }
            catch (Exception)
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
        private void PrintParticipants(List<Guid> participants)
        {
            foreach (var participant in participants)
            {
                Console.WriteLine(participant);
            }
        }
        private bool CheckGroupName(string userResponse, AllGroupChat allGroupChat)
        {
            return allGroupChat.GroupsName.Contains(userResponse);
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
                userResponse = Console.ReadLine();

                Guid userGuid;

                if (Guid.TryParse(userResponse, out userGuid))
                {
                    if (CheckIfParticipantsExist(userGuid, allParticipants))
                    {
                        selectedParticipants.Add(userGuid);
                    }
                    else
                    {
                        Console.WriteLine($"the user guid not exist");
                    }

                }
                else if (userResponse != "0")
                {
                    Console.WriteLine($"the user guid not exist");
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