using System.Threading.Tasks;

namespace Readarr.SignalR
{
    public interface IBroadcastSignalRMessage
    {
        bool IsConnected { get; }
        Task BroadcastMessage(SignalRMessage message);
    }
}
