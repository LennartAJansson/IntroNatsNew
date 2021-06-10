using CloudNative.CloudEvents;

using System.Threading.Tasks;

namespace NatsConsumer.Services
{
    public interface INatsService
    {
        Task SendAsync(string subject, CloudEvent cloudEvent);
    }
}