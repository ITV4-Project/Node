using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetworking.Messages
{
    public class BroadcastMessage
    {
        public Message Message { get; set; }
        public int TimedBroadcasted { get; set; } = 0;
    }
}
