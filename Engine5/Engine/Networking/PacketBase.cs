using System.Net;
using System.Net.Sockets;

namespace Engine.Networking;
public abstract class PacketBase : Identifiable {

	private byte[]? _data;

	/// <summary>
	/// The target of this packet, if sent. A null here results in the packet being sent to the server if this is a client, and all connected clients if this is a server.
	/// </summary>
	public IPEndPoint? RemoteTarget { get; set; } = null;
    /// <summary>
    /// The sender of this packet, if received.
    /// </summary>
    public IPEndPoint? RemoteSender { get; set; } = null;

    public abstract ProtocolType Protocol { get; }

	public PacketBase() {
		_data = null;
	}

	protected void SetBytes( byte[] data ) {
		_data = data;
	}

	public byte[]? GetPacketTransferData(int id) {
		var data = GetData();
		if ( data is null )
			return null;
		byte[] transferData = new byte[ data.Length + sizeof( int ) * 2 ];
		transferData.Length.CopyInto( transferData );
		id.CopyInto( transferData, sizeof( int ) );
		data.CopyInto( transferData, sizeof( int ) * 2 );
		return transferData;
	}

	private byte[]? GetData() {
		_data ??= GetRequestedData();
		return _data;
	}

	protected abstract byte[]? GetRequestedData();

}
