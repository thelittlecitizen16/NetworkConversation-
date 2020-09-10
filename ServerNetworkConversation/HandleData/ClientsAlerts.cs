using Common.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerNetworkConversation.HandleData
{
    public class ClientsAlerts
    {
        public ConcurrentDictionary<TcpClient, List<MessageRequest>> Alerts { get; private set; }

        public ClientsAlerts()
        {
            Alerts = new ConcurrentDictionary<TcpClient, List<MessageRequest>>();
        }

        public void AddNewAlert(TcpClient client, MessageRequest messageRequest)
        {
            if (Alerts.ContainsKey(client))
            {
                Alerts[client].Add(messageRequest);
            }
            else
            {
                Alerts.TryAdd(client, new List<MessageRequest>() { messageRequest });
            }
        }
        public void RemoveAlert(TcpClient client, MessageRequest messageRequest)
        {
            Alerts[client].Remove(messageRequest);
        }
        public MessageRequest GetAlert(TcpClient client)
        {
            return Alerts[client][0];
        }
        public bool HaveAlert(TcpClient client)
        {
            if (Alerts.ContainsKey(client))
            {
                return Alerts[client].Count > 0;
  
            }

            return false;
        }
    }
}
