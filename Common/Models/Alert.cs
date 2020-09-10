using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    [Serializable()]
    public class Alert
    {
        public AlertOptions Option { get; set; }
        public string Message { get; set; }
        public Alert(AlertOptions option, string message)
        {
            Option = option;
            Message = message;
        }
    }
}
