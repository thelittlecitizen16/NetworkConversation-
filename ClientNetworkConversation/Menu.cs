﻿using ClientNetworkConversation.Options;
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


        public Menu(TcpClient client, HandleServer handleServer)
        {
            _consoleSystem = new ConsoleSystem();
            _validation = new MenuBuilder.Validation();
            _globalChatOption = new GlobalChatOption(client, handleServer);
            _privateChat = new PrivateChat(client, handleServer);
            _createGroupChat = new CreateGroupChat(client, handleServer);
            _enterGroupChat=new EnterGroupChat(client, handleServer);
            _managerSettings = new ManagerSettings(client, handleServer);
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
               .Build();

            mainMenu.RunMenu();
        }
    }
}
