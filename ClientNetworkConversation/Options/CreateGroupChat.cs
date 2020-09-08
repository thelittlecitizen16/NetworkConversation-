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

            Console.WriteLine("enter group name");
            string gropuName = Console.ReadLine();

            Participants participants = (Participants)_handleServer.GetFromServer(_client);

            foreach (var participant in participants.AllParticipants)
            {
                Console.WriteLine(participant);
            }

            Console.WriteLine("add all participants, when end send 0");
            List<Guid> usersToAdd = new List<Guid>();
            string exist = "";

            while (exist != "0")
            {
                string user = Console.ReadLine();
                Guid userGuid;

                if (Guid.TryParse(user, out userGuid))
                {
                    usersToAdd.Add(userGuid);
                }
                else
                {
                    Console.WriteLine($"thr user guid not exist");
                }
            }

              GroupChat groupChat = new GroupChat(gropuName, usersToAdd);
             string messageToSend = Console.ReadLine();
            _handleServer.SendToServer(_client, groupChat);
        }
    }
}
