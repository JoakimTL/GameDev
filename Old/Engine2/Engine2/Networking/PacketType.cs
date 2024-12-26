using Engine.MemLib;
using Engine.Networking.Packets;
using Engine.Utilities.Data;
using Engine.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking {
	public class PacketType {

		public delegate Packet PacketCreationHandler( IPEndPoint endpoint, IReadOnlyList<byte> bytes );
		private static Dictionary<Type, PacketType> typesByType = new Dictionary<Type, PacketType>();
		private static Dictionary<uint, PacketType> typesById = new Dictionary<uint, PacketType>();
		private static event Action PacketSetupEvent;

		public static readonly PacketType INVALID = new PacketType( 0, default, ProtocolType.IP, delegate ( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) { return null; } );
		public static readonly PacketType CONNECT = new PacketType( 1, typeof( PacketConnection ), ProtocolType.Tcp, delegate ( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) { return new PacketConnection( endpoint, bytes ); } );
		public static readonly PacketType DISCONNECT = new PacketType( 2, typeof( PacketDisconnection ), ProtocolType.Tcp, delegate ( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) { return new PacketDisconnection( endpoint, bytes ); } );
		public static readonly PacketType PROTOPORTCOR = new PacketType( 3, typeof( PacketProtocolPortCorrection ), ProtocolType.Tcp, delegate ( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) { return new PacketProtocolPortCorrection( endpoint, bytes ); } );
		public static readonly PacketType PINGTCP = new PacketType( 4, typeof( PacketPingTCP ), ProtocolType.Tcp, delegate ( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) { return new PacketPingTCP( endpoint, bytes ); } );
		public static readonly PacketType PINGUDP = new PacketType( 5, typeof( PacketPingUDP ), ProtocolType.Udp, delegate ( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) { return new PacketPingUDP( endpoint, bytes ); } );
		public static readonly PacketType REJECTION = new PacketType( 6, typeof( PacketRejection ), ProtocolType.Tcp, delegate ( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) { return new PacketRejection( endpoint, bytes ); } );
		public static readonly PacketType PINGTIMES = new PacketType( 7, typeof( PacketPingTimes ), ProtocolType.Tcp, delegate ( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) { return new PacketPingTimes( endpoint, bytes ); } );
		public static readonly PacketType CHATMESSAGE = new PacketType( 8, typeof( PacketMessage ), ProtocolType.Tcp, delegate ( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) { return new PacketMessage( endpoint, bytes ); } );
		public static readonly PacketType UDPFORCE = new PacketType( 9, typeof( PacketUDPForce ), ProtocolType.Udp, delegate ( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) { return new PacketUDPForce( endpoint, bytes ); } );
		public static readonly PacketType PLAYERLIST = new PacketType( 10, typeof( PacketPlayerList ), ProtocolType.Tcp, delegate ( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) { return new PacketPlayerList( endpoint, bytes ); } );

		public static Dictionary<uint, PacketType>.KeyCollection ListIDs {
			get {
				return typesById.Keys;
			}
		}

		public static PacketType GetPacketType( uint iD ) {
			if( typesById.TryGetValue( iD, out PacketType t ) )
				return t;
			return INVALID;
		}

		public static PacketType GetPacketType<T>() where T : Packet {
			if( typesByType.TryGetValue( typeof( T ), out PacketType t ) )
				return t;
			return INVALID;
		}

		public static PacketType GetPacketType( Type t ) {
			if( typesByType.TryGetValue( t, out PacketType pt ) )
				return pt;
			return INVALID;
		}

		public static void ClearAndSetup() {
			typesById.Clear();
			typesByType.Clear();
			AddPacketType( CONNECT );
			AddPacketType( DISCONNECT );
			AddPacketType( PROTOPORTCOR );
			AddPacketType( PINGTCP );
			AddPacketType( PINGUDP );
			AddPacketType( REJECTION );
			AddPacketType( PINGTIMES );
			AddPacketType( CHATMESSAGE );
			AddPacketType( UDPFORCE );
			AddPacketType( PLAYERLIST );
			PacketSetupEvent?.Invoke();
		}

		public static uint GetNextAvailable( uint start ) {
			if( start == 0 )
				start++;
			while( typesById.ContainsKey( start ) )
				start++;
			return start;
		}

		public static bool AddPacketType( PacketType t ) {
			t.ID = GetNextAvailable( t.ID );
			if( t.ID == 0 ) {
				MemLib.Mem.Logs.Routine.WriteLine( $"Unable to find available packet factory ID for {t.ID}. How many packet factories exists???" );
				return false;
			}
			typesById.Add( t.ID, t );
			typesByType.Add( t.Type, t );
			MemLib.Mem.Logs.Routine.WriteLine( $"Added packet type {t.Type.Name} with id {t.ID}." );
			return true;
		}

		private static byte[] arr = new byte[ 4 ];
		public static Packet CreatePacket( IPEndPoint endpoint, IReadOnlyList<byte> bytes ) {
			if( bytes.Count >= Packet.HEADER_SIZE ) {
				lock( arr ) {
					arr[ 0 ] = bytes[ Packet.HEADER_ID_LOC + 0 ];
					arr[ 1 ] = bytes[ Packet.HEADER_ID_LOC + 1 ];
					arr[ 2 ] = bytes[ Packet.HEADER_ID_LOC + 2 ];
					arr[ 3 ] = bytes[ Packet.HEADER_ID_LOC + 3 ];
				}
				uint id = DataTransform.ToUInt32( arr );

				if( typesById.TryGetValue( id, out PacketType p ) )
					return p.Factory.Invoke( endpoint, bytes );
				MemLib.Mem.Logs.Error.WriteLine( $"Incapable of interpreting the packets ID[{id}]. The ID cannot be found in the list of accepted IDs!" );
				return null;
			}
			MemLib.Mem.Logs.Error.WriteLine( "Packet size is not compatible with system. Packet length is " + bytes.Count + " while the requirement is " + Packet.HEADER_SIZE + " or above!" );
			return null;
		}

		public uint ID { get; private set; }
		public Type Type { get; private set; }
		public ProtocolType Protocol { get; private set; }
		public PacketCreationHandler Factory { get; private set; }

		public PacketType( uint ID, Type t, ProtocolType protocol, PacketCreationHandler factory ) {
			this.ID = ID;
			this.Type = t;
			this.Protocol = protocol;
			this.Factory = factory;
		}
	}
}