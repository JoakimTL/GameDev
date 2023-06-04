using Engine.Networking;
using Engine.Structure.Interfaces;

namespace Engine.GlobalServices.Network;
public class NetworkMessagingService : IGlobalService
{

    //TODO: Entities need to be able to update based on component changes. Adding/changing and removing components.

    public event Action<PacketBase>? PacketReceived;
    internal event Action<PacketBase>? PacketSent;

    internal void ReceivedPacket(PacketBase packet) => PacketReceived?.Invoke(packet);
    public void SendPacket(PacketBase packet) => PacketSent?.Invoke(packet);

}
