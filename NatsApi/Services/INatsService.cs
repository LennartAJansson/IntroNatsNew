using CloudNative.CloudEvents;

using System.Threading.Tasks;

namespace NatsApi.Services
{
    public interface INatsService
    {
        Task SendAsync(CloudEvent cloudEvent);
    }
}