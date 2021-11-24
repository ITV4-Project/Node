using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetworking.Messages
{
    public class TestMessage : IMessage<TestMessage>
    {
        public string Message { get; set; }

        public MessageTypes GetMessageType()
        {
            return MessageTypes.Test;
        }
    }
}
