using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetworking.Messages
{
    public class NewServer : IMessage<NewServer>
    {
        public byte[] Address { get; set; }
        public int Port { get; set; }
        public string ServerKey { get; set; }
        public MessageTypes GetMessageType()
        {
            return MessageTypes.NewServer;
        }
    }
}
