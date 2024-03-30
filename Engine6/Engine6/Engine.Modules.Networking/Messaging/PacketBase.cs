using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Engine.Modules.Networking.Messaging;
public abstract class PacketBase {

	private byte[]? _data;

	/// <summary>
	/// The target of this packet, if sent. A null here results in the packet being sent to the server if this is a client, and all connected clients if this is a server.
	/// </summary>
	public IPEndPoint? RemoteTarget { get; set; } = null;
	/// <summary>
	/// The sender of this packet, if received.
	/// </summary>
	public IPEndPoint? RemoteSender { get; set; } = null;

	/// <summary>
	/// The protocol is chosen by the packet type, but can be set manually if another protocol is desired. This is only used when sending the packet.
	/// </summary>
	public ProtocolType SendProtocol { get; set; }

	/// <summary>
	/// The protocol this packet was received on. This is set when the packet is received. It is not used for anything, but can be used to determine the protocol of the packet.
	/// </summary>
	public ProtocolType? ReceiveProtocol { get; init; }

	protected PacketBase( ProtocolType sendProtocol ) {
		_data = null;
		SendProtocol = sendProtocol;
	}

	protected void SetBytes( byte[] data ) => _data = data;

	public byte[]? GetPacketTransferData( int id ) {
		var data = GetData();
		if (data is null)
			return null;
		byte[] transferData = new byte[ data.Length + (sizeof( int ) * 2) ];
		transferData.Length.CopyInto( transferData );
		id.CopyInto( transferData, sizeof( int ) );
		data.CopyInto( transferData, sizeof( int ) * 2 );
		return transferData;
	}

	public byte[]? GetPacketTransferDataFast( int id ) {
		byte[]? data = GetData();
		if (data is null)
			return null;
		byte[] transferData = new byte[ (uint) data.Length + 8 ];
		new PacketTransferData( (uint) transferData.Length, id ).CopyInto( transferData );
		data.CopyInto( transferData, 8 );
		return transferData;
	}

	private byte[]? GetData() => _data ?? GetRequestedData();

	protected abstract byte[]? GetRequestedData();
}

[StructLayout( LayoutKind.Explicit )]
internal readonly struct PacketTransferData {
	[FieldOffset( 0 )]
	public readonly uint Length;
	[FieldOffset( 4 )]
	public readonly int Id;

	public PacketTransferData( uint length, int id ) {
		Length = length;
		Id = id;
	}
}