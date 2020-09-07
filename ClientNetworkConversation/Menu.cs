using ClientNetworkConversation.Options;
using MenuBuilder;
using MenuBuilder.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientNetworkConversation
{
    public class Menu 
    {
        private MenuBuilder<int> _menuBuilderInt;
        private ConsoleSystem _consoleSystem;
        private Validation _validation;
        private GlobalChatOption _globalChatOption;
        public Menu()
        {
            _consoleSystem = new ConsoleSystem();
            _validation = new MenuBuilder.Validation();
            _globalChatOption = new GlobalChatOption();
            _menuBuilderInt = new MenuBuilder<int>(_consoleSystem, _validation);
        }
        public void RunMenu()
        {
            var mainMenu = _menuBuilderInt
               .AddOption(1, _globalChatOption)
                .Build();

            mainMenu.RunMenu();
        }
    }
}
