using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model
{
    public class MessageModel
    {
        public string MessageId { get; set; }
        public string ThreadId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string MsgDate { get; set; }
        public string Msg { get; set; }
        public string Type { get; set; }
    }
}
