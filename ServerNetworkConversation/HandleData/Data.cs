namespace ServerNetworkConversation.HandleData
{
    public class Data
    {
        public AllGroupsChat AllGroupsChat { get; private set; }
        public ClientsConnectedInChat ClientsConnectedInChat { get; private set; }
        public ClientsInGlobalChat ClientsInGlobalChat { get; private set; }
        public ClientsConnectedInServer ClientsConnectedInServer { get; private set; }
        public ClientsAlerts ClientsAlerts { get; private set; }

        public Data()
        {
            AllGroupsChat = new AllGroupsChat();
            ClientsConnectedInChat = new ClientsConnectedInChat();
            ClientsInGlobalChat = new ClientsInGlobalChat();
            ClientsConnectedInServer = new ClientsConnectedInServer();
            ClientsAlerts = new ClientsAlerts();
        }
    }
}
