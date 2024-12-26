using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Engine.Networking {
	public static class PacketManager {

		private static readonly List<(Type, ProtocolType, Func<byte[], Packet>)> packets = new List<(Type, ProtocolType, Func<byte[], Packet>)>();
		private static readonly Dictionary<Type, uint> packetIDs = new Dictionary<Type, uint>();

		private static List<Type> GetPacketTypes() {
			HashSet<Assembly> assemblies = new HashSet<Assembly>();
			Queue<AssemblyName> queue = new Queue<AssemblyName>();
			{
				Assembly a = Assembly.GetEntryAssembly();
				if( assemblies.Add( a ) )
					foreach( AssemblyName r in a.GetReferencedAssemblies() ) {
						queue.Enqueue( r );
					}
			}

			while( queue.TryDequeue( out AssemblyName ar ) ) {
				try {
					Assembly a = Assembly.Load( ar );
					if( assemblies.Add( a ) )
						foreach( AssemblyName r in a.GetReferencedAssemblies() ) {
							queue.Enqueue( r );
						}
				} catch( Exception e ) {
					Logging.Error( e.Message );
				}
			}

			HashSet<Type> temp_PacketTypes = new HashSet<Type>();

			foreach( Assembly assembly in assemblies ) {
				Console.WriteLine( assembly );
				foreach( Type type in assembly.GetTypes() )
					if( type.IsSubclassOf( typeof( Packet ) ) && !type.IsAbstract ) {
						Console.WriteLine( type );
						temp_PacketTypes.Add( type );
					}
			}

			List<Type> packets = new List<Type>( temp_PacketTypes );

			packets.Sort( PacketTypeSorter );

			return packets;
		}

		/// <summary>
		/// Scans trhough all the assemblies in this program, and associates all classes deriving from Packet that are not abstract with an ID and determines the Packet's protocol type.
		/// Packet types can have different protocols, using the PacketProtocol attribute when creating a custom packet.
		/// </summary>
		public static void ScanPacketTypes() {
			List<Type> temp_PacketTypes = GetPacketTypes();

			for( int i = 0; i < temp_PacketTypes.Count; i++ ) {
				if( TryCreatePacketCreator( temp_PacketTypes[ i ], out Func<byte[], Packet> function ) ) {
					PacketProtocol attr = (PacketProtocol) temp_PacketTypes[ i ].GetCustomAttribute( typeof( PacketProtocol ) );
					ProtocolType protocol = ProtocolType.Tcp;
					if( !( attr is null ) )
						protocol = attr.Protocol;
					uint id = (uint) packets.Count;
					packetIDs.Add( temp_PacketTypes[ i ], id );
					packets.Add( (temp_PacketTypes[ i ], protocol, function) );
					Logging.Routine( $"Added packet type [{temp_PacketTypes[ i ].FullName}:{protocol}] to list of packets with ID [{id}]." );
				} else {
					Logging.Error( $"The packet manager failed to find a constructor taking a byte array as parameter for packet type [{temp_PacketTypes[ i ].FullName}]." );
				}
			}
		}

		private static bool TryCreatePacketCreator( Type packetType, out Func<byte[], Packet> function ) {
			function = null;
			ConstructorInfo constructor = packetType.GetConstructor( new Type[] { typeof( byte[] ) } );
			if( constructor is null )
				return false;
			ParameterExpression parameter = Expression.Parameter( typeof( byte[] ), "data" );
			Expression<Func<byte[], Packet>> creatorExpression = Expression.Lambda<Func<byte[], Packet>>( Expression.New( constructor, new Expression[] { parameter } ), parameter );
			function = creatorExpression.Compile();
			return true;
		}

		private static int PacketTypeSorter( Type x, Type y ) {
			return string.Compare( x.FullName, y.FullName, StringComparison.Ordinal );
		}

		public static Packet CreatePacket( byte[] data ) {
			if( data.Length < Packet.HEADER_SIZE )
				return null;

			uint id = Packet.GetID( data );
			if( id >= packets.Count )
				return null;

			return packets[ (int) id ].Item3.Invoke( data );
		}

		internal static uint GetID( Type type ) {
			if( packetIDs.TryGetValue( type, out uint id ) )
				return id;
			Logging.Warning( "Tried to get the id of packettype " + type + ", but that type has not been registered." );
			return 2147483648;
		}

		internal static (Type, ProtocolType) Get( uint packetID ) {
			if( packetID < packets.Count )
				return (packets[ (int) packetID ].Item1, packets[ (int) packetID ].Item2);
			Logging.Warning( "Tried to access a packettype with id " + packetID + ", but that ID has not been registered." );
			return (null, ProtocolType.IP);
		}

		internal static Type GetType( uint packetID ) {
			if( packetID < packets.Count )
				return packets[ (int) packetID ].Item1;
			Logging.Warning( "Tried to access a packettype with id " + packetID + ", but that ID has not been registered." );
			return null;
		}

		internal static ProtocolType GetProtocol( uint packetID ) {
			if( packetID < packets.Count )
				return packets[ (int) packetID ].Item2;
			Logging.Warning( "Tried to access a packettype with id " + packetID + ", but that ID has not been registered." );
			return ProtocolType.IP;
		}
	}
}
