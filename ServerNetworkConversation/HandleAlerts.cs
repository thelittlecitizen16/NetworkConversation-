using Common.HandleRequests;
using Common.Models;
using ServerNetworkConversation.HandleData;
using System;
using System.Net.Sockets;

namespace ServerNetworkConversation
{
    public class HandleAlerts
    {
        private Data _data;
        private IRequests _requests;
        public HandleAlerts(Data data, IRequests requests)
        {
            _data = data;
            _requests = requests;
        }
        public void Send()
        {
            while (true)
            {
                foreach (var client in _data.ClientsAlerts.Alerts.Keys)
                {
                    Guid clientGuid = _data.ClientsConnectedInServer.GetGuid(client);
                    if (client.Connected)
                    {
                        if (CheckIfClientCanGetAlert(client, clientGuid))
                        {
                            if (_data.ClientsAlerts.HaveAlert(client))
                            {
                                SendAlert(client);
                            }
                        }
                    }
                }
            }          
        }

        private bool CheckIfClientCanGetAlert(TcpClient client, Guid clientGuid)
        {
            return (_data.ClientsConnectedInChat.IsClientInChat(clientGuid)
                || _data.ClientsInGlobalChat.IsClientOnChat(clientGuid)
                || _data.AllGroupsChat.IsClientInAnyGroupChat(client));
        }

        private void SendAlert(TcpClient client)
        {
            MessageRequest alert = _data.ClientsAlerts.GetAlert(client);
            _requests.SendModelMessage(client, alert);
            _data.ClientsAlerts.RemoveAlert(client, alert);
        }

    }
}
