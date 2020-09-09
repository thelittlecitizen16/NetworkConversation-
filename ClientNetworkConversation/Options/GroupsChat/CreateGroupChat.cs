using Common;
using Common.Enums;
using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ClientNetworkConversation.Options.GroupsChat
{
    public class CreateGroupChat : IOption
    {
        public string OptionMessage => "Create Group Chat";
        private static TcpClient _client;
        private HandleServer _handleServer;

        public CreateGroupChat(TcpClient client, HandleServer handleServer)
        {
            _handleServer = handleServer;
            _client = client;
        }

        public void Run()
        {
            try
            {
                _handleServer.SendMessageToServer(_client, ClientOptions.CREATE_GROUP_CHAT.ToString());
                Participants participants = (Participants)_handleServer.GetFromServer(_client);

                Console.WriteLine("enter group name");
                string gropuName = Console.ReadLine();

                PrintAllParticipants(participants.AllParticipants);

                Console.WriteLine("add all participants, when end send 0");
                List<Guid> usersToAdd = new List<Guid>();
                string userResponse = "";
                usersToAdd = GetAllParticipants(userResponse, participants.AllParticipants);

                SendGroupToServer(gropuName, usersToAdd);
            }
            catch (Exception)
            { 
            }
        }
        private void PrintAllParticipants(List<Guid> participants)
        {
            foreach (var participant in participants)
            {
                Console.WriteLine(participant);
            }
        }
        private List<Guid> GetAllParticipants(string userResponse, List<Guid> participants)
        {
            List<Guid> usersToAdd = new List<Guid>();

            while (userResponse != "0")
            {
                userResponse = Console.ReadLine();

                Guid userGuid;

                if (Guid.TryParse(userResponse, out userGuid))
                {
                    if (CheckIfParticipantsExist(userGuid, participants))
                    {
                        usersToAdd.Add(userGuid);
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
            return usersToAdd;
        }
        private bool CheckIfParticipantsExists(Guid userGuid, List<Guid> participants)
        {
            return participants.Contains(userGuid);   
        }
        private void SendGroupToServer(string gropuName, List<Guid> usersToAdd)
        {
             GroupChat groupChat = new GroupChat(gropuName, usersToAdd, new List<Guid>());
            _handleServer.SendToServer(_client, groupChat);
        }
    }
}
