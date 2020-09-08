using Common.Enums;
using System;

namespace Common
{
    public class Class1
    { 
        public ClientOptions ClientOptions { get; private set; }

        public Class1(ClientOptions clientOptions)
        {
            ClientOptions = clientOptions;
        }

    }
}
