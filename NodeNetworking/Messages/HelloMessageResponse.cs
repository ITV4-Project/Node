using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetworking.Messages
{
    public class HelloMessageResponse : IMessage<HelloMessageResponse>
    {
        public byte[] Address { get; set; }
        public int Port { get; set; }
        public string ServerKey { get; set; }
        public string Type { get; set; }
        public List<IPAddressSerializible> ServerIps { get; set; } = new List<IPAddressSerializible>();

        public MessageTypes GetMessageType() {
            return MessageTypes.HelloResponse;
        }
    }
}
