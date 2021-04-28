using NATS.Client;

using System.Text;
using System.Text.Json;

namespace NatsConsumer.Nats
{
    public static class MsgExtensions
    {
        public static string GetDataString(this Msg msg)
        {
            return Encoding.UTF8.GetString(msg.Data);
        }

        public static TValue GetDataObject<TValue>(this Msg msg)
        where TValue : class
        {
            return JsonSerializer.Deserialize<TValue>(msg.GetDataString());
        }
    }
}