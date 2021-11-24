using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetworking.Messages
{
    public class RelayMessage
    {
        public Message Message { get; set; }
        public int TimesRelayed { get; set; } = 0;
    }
}
