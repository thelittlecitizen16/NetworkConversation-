using Common.Enums;
using Common.Models;
using MenuBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientNetworkConversation.HandleRequest.MessageOption
{
    public class AlertMessage : IMessageOption
    {
        public void HandleMessage(ISystem system, object message)
        {
            Alert alert = (Alert)message;
            switch (alert.Option)
            {
                case AlertOptions.NEW_MESSAGE:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    system.Write(alert.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case AlertOptions.NEW_GROUP:
                    Console.ForegroundColor = ConsoleColor.Green;
                    system.Write(alert.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case AlertOptions.EXIT_GROUP:
                    Console.ForegroundColor = ConsoleColor.Red;
                    system.Write(alert.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    break;
            }
        }
    }
}
