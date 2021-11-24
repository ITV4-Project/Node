using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetworking.Messages
{
    public class IPAddressSerializible
    {
        public IPAddressSerializible()
        {}
        public IPAddressSerializible(IPEndPoint ipEndpoint)
        {
            Address = ipEndpoint.Address.GetAddressBytes();
            Port = ipEndpoint.Port;
            ServerKey = ipEndpoint.ToString();
        }
        public byte[] Address { get; set; }
        public int Port { get; set; }
        public string ServerKey { get; set; }
    }
}
