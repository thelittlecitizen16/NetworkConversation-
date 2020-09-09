using Common;
using Common.Enums;
using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ClientNetworkConversation.Options
{
    public class CreateGroupChat : IOption
    {
        public string OptionMessage => "Create Group Chat";
        private static TcpClient _client;
        private HandleServer _handleServer;
        private string message;
        public CreateGroupChat(TcpClient client, HandleServer handleServer)
        {
            _handleServer = handleServer;
            _client = client;
        }
        public void Run()
        {
            _handleServer.SendMessageToServer(_client, ClientOptions.CREATE_GROUP_CHAT.ToString());
            Participants participants = (Participants)_handleServer.GetFromServer(_client);

            Console.WriteLine("enter group name");
            string gropuName = Console.ReadLine();

            foreach (var participant in participants.AllParticipants)
            {
                Console.WriteLine(participant);
            }

            Console.WriteLine("add all participants, when end send 0");
            List<Guid> usersToAdd = new List<Guid>();
            string userResponse = "";

            while (userResponse != "0")
            {
                userResponse = Console.ReadLine();
                Guid userGuid;
                if (Guid.TryParse(userResponse, out userGuid))
                {
                    if (participants.AllParticipants.Contains(userGuid))
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

            GroupChat groupChat = new GroupChat(gropuName, usersToAdd, new List<Guid>());
            _handleServer.SendToServer(_client, groupChat);
        }
    }
}
