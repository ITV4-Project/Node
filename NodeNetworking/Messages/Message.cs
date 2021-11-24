using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NodeNetworking.Messages
{
    [Serializable]
    public class Message
    {
        public Guid guid { get; set; } = Guid.NewGuid();
        public MessageTypes Type { get; set; }
        public HashSet<string> ServerHistory { get; set; } = new HashSet<string>();
        public string Data { get; set; }
        public string Origin { get; set; }

        public T Deserialize<T>()
        {
            return JsonSerializer.Deserialize<T>(Data);
        }

        public override bool Equals(object obj)
        {
            return obj is Message message &&
                   guid.Equals(message.guid) &&
                   Type == message.Type &&
                   EqualityComparer<HashSet<string>>.Default.Equals(ServerHistory, message.ServerHistory) &&
                   Data == message.Data &&
                   Origin == message.Origin;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(guid, Type, ServerHistory, Data, Origin);
        }
    }

    public static class MessageExtensions
    {
        public static Message ToMessage<T>(this IMessage<T> msg)
        {
            return new Message
            {
                Type = msg.GetMessageType(),
                Data = msg.Serialize(),
            };
        }
    }
}
