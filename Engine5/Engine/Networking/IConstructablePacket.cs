using System.Net;

namespace Engine.Networking;

public interface IConstructablePacket {
	abstract static PacketBase? Construct( byte[] data, IPEndPoint? remote );
}
