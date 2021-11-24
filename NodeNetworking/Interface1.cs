using NodeNetworking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetworking.Messages
{
    public interface IMessage<T>
    {
        MessageTypes GetMessageType();
        string Serialize()
        {
            return System.Text.Json.JsonSerializer.Serialize((T)this, Serializer.JsonOptions);
        }
    }
}
