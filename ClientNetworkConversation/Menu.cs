using ClientNetworkConversation.IO;
using ClientNetworkConversation.Options;
using ClientNetworkConversation.Options.GroupsChat;
using MenuBuilder;
using MenuBuilder.Options;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ClientNetworkConversation
{
    public class Menu
    {
        private MenuBuilder<int> _menuBuilderInt;
        private ConsoleSystem _consoleSystem;
        private Validation _validation;
        private GlobalChatOption _globalChatOption;
        private PrivateChat _privateChat;
        private CreateGroupChat _createGroupChat;
        private EnterGroupChat _enterGroupChat;
        private ManagerSettings _managerSettings;
        private LeaveGroupChat _leaveGroupChat;


        public Menu(TcpClient client, HandleServer handleServer)
        {
            _consoleSystem = new ConsoleSystem();
            _validation = new Validation();
            _globalChatOption = new GlobalChatOption(client, handleServer, _consoleSystem);
            _privateChat = new PrivateChat(client, handleServer, _consoleSystem);
            _createGroupChat = new CreateGroupChat(client, handleServer, _consoleSystem);
            _enterGroupChat=new EnterGroupChat(client, handleServer, _consoleSystem);
            _managerSettings = new ManagerSettings(client, handleServer, _consoleSystem);
            _leaveGroupChat = new LeaveGroupChat(client, handleServer, _consoleSystem);
            _menuBuilderInt = new MenuBuilder<int>(_consoleSystem, _validation);
  
        }
        public void RunMenu()
        {
            var mainMenu = _menuBuilderInt
               .AddOption(1, _globalChatOption)
               .AddOption(2, _privateChat)
               .AddOption(3, _createGroupChat)
               .AddOption(4, _enterGroupChat)
               .AddOption(5, _managerSettings)
               .AddOption(6, _leaveGroupChat)
               .Build();

            mainMenu.RunMenu();
        }
    }
}
