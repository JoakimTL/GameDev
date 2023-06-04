using Engine.GlobalServices;
using Engine.Networking;
using Engine.Networking.Modules.Services;
using Engine.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineTests.Networking.Packets;

[TestFixture]
public class IConstructablePacketTests {

	private T TransferToCopy<T>( T packet ) where T : PacketBase {
		var typeRegistry = new TypeRegistryService();
		var packetTypeRegistry = new PacketTypeRegistryService( typeRegistry );
		var transferdata = packet.GetPacketTransferData( packetTypeRegistry.GetPacketTypeId( packet.GetType() ) );

		Assert.That( transferdata, Is.Not.Null );
		var transferdataWithoutLength = transferdata[ sizeof( uint ).. ];

		var reconstructed = packetTypeRegistry.Construct( transferdataWithoutLength, null );

		Assert.That( reconstructed, Is.Not.Null );
		Assert.That( reconstructed, Is.InstanceOf<T>() );
		return (T) reconstructed;
	}

	//Test all packet types found in Engine.Networking.Packets here (alphabertical order)
	[Test]
	public void PacketTypeRegistryService_Construct_ClientChangedUsername() {
		ClientChangedUsername packet = new( new( 15 ), "Username too long" );
		ClientChangedUsername reconstructed = TransferToCopy( packet );
		Assert.Multiple( () => {
			Assert.That( reconstructed.NetworkId, Is.EqualTo( packet.NetworkId ) );
			Assert.That( reconstructed.NewUsername, Is.EqualTo( packet.NewUsername ) );
		} );
	}

	[Test]
	public void PacketTypeRegistryService_Construct_ClientDisconnected() {
		ClientDisconnected packet = new( new( 15 ), "Didn't like the hat look" );
		ClientDisconnected reconstructed = TransferToCopy( packet );
		Assert.Multiple( () => {
			Assert.That( reconstructed.NetworkId, Is.EqualTo( packet.NetworkId ) );
			Assert.That( reconstructed.Reason, Is.EqualTo( packet.Reason ) );
		} );
	}

	[Test]
	public void PacketTypeRegistryService_Construct_ConnectionFailed() {
		ConnectionFailed packet = new( "Couldn't connect to server" );
		ConnectionFailed reconstructed = TransferToCopy( packet );
		Assert.That( reconstructed.Reason, Is.EqualTo( packet.Reason ) );
	}

	[Test]
	public void PacketTypeRegistryService_Construct_TcpLogin() {
		TcpLogin packet = new( "Username", 50043 );
		TcpLogin reconstructed = TransferToCopy( packet );
		Assert.Multiple( () => {
			Assert.That( reconstructed.Username, Is.EqualTo( packet.Username ) );
			Assert.That( reconstructed.UdpPort, Is.EqualTo( packet.UdpPort ) );
		} );
	}

	[Test]
	public void PacketTypeRegistryService_Construct_TcpLoginAck() {
		TcpLoginAck packet = new( new( 15 ), "Username" );
		TcpLoginAck reconstructed = TransferToCopy( packet );
		Assert.Multiple( () => {
			Assert.That( reconstructed.NetworkId, Is.EqualTo( packet.NetworkId ) );
			Assert.That( reconstructed.Username, Is.EqualTo( packet.Username ) );
		} );
	}

	[Test]
	public void PacketTypeRegistryService_Construct_UdpPing() {
		UdpPing packet = new( new( 15 ), 123456789 );
		UdpPing reconstructed = TransferToCopy( packet );
		Assert.Multiple( () => {
			Assert.That( reconstructed.NetworkId, Is.EqualTo( packet.NetworkId ) );
			Assert.That( reconstructed.Time, Is.EqualTo( packet.Time ) );
		} );
	}

	[Test]
	public void PacketTypeRegistryService_Construct_UsernameChange() {
		UsernameChange packet = new( "NewUsername" );
		UsernameChange reconstructed = TransferToCopy( packet );
		Assert.That( reconstructed.NewUsername, Is.EqualTo( packet.NewUsername ) );
	}

	[Test]
	public void PacketTypeRegistryService_Construct_UsernameChangeFailed() {
		UsernameChangeFailed packet = new( "Username taken!" );
		UsernameChangeFailed reconstructed = TransferToCopy( packet );
		Assert.That( reconstructed.Reason, Is.EqualTo( packet.Reason ) );
	}
}
