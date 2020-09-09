using Common;
using Common.Enums;
using Common.HandleRequests;
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
        private ISystem _system;

        public CreateGroupChat(TcpClient client, HandleServer handleServer, ISystem system)
        {
            _handleServer = handleServer;
            _client = client;
            _system = system;
        }

        public void Run()
        {
            try
            {
                _handleServer.SendMessageToServer(_client, ClientOptions.CREATE_GROUP_CHAT.ToString());
             
                Participants participants = (Participants)_handleServer.GetFromServer(_client);

                _system.Write("enter group name");
                string gropuName = _system.ReadString();

                PrintAllParticipants(participants.AllParticipants);

                _system.Write("add all participants, when end send 0");
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
                _system.Write(participant.ToString());
            }
        }
        private List<Guid> GetAllParticipants(string userResponse, List<Guid> participants)
        {
            List<Guid> usersToAdd = new List<Guid>();

            while (userResponse != "0")
            {
                userResponse = _system.ReadString();

                Guid userGuid;

                if (Guid.TryParse(userResponse, out userGuid))
                {
                    if (CheckIfParticipantsExists(userGuid, participants))
                    {
                        usersToAdd.Add(userGuid);
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
