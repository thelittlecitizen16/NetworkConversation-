using Common.Enums;
using System;

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
