using NodeNetworking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace NodeNetworking
{
    public class Serializer
    {
        public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.General)
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public static byte[] Serialize<T>(IMessage<T> message)
        {
            var data = new Message()
            {
                Data = message.Serialize(),
                Type = message.GetMessageType(),
            };
            return Serialize(data);
        }

        public static byte[] Serialize(Message message)
        {
            return JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);
        }

        public static async Task SerializeToStream(Stream stream, Message message)
        {
            await JsonSerializer.SerializeAsync(stream, message);
        }

        public static async Task<Message> DeserializeFromStream(Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<Message>(stream);
        }

        public static object Deserialize(ReadOnlySpan<byte> data, Type t)
        {
            return JsonSerializer.Deserialize(data, t);
        }

        public static Message Deserialize(ReadOnlySpan<byte> data)
        {
            return JsonSerializer.Deserialize<Message>(data);
        }

        public static Message Deserialize(string str)
        {
            return JsonSerializer.Deserialize<Message>(str);
        }
    }
}
