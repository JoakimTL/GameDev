using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using Engine.Data;

namespace Engine.Networking;
public abstract class Packet : Identifiable {

	public int Id { get; }
	public int PostId { get; }
	protected byte[] _rawdata;
	public IReadOnlyList<byte> Data => this._rawdata;

	private byte[][]? _separated;

	/// <summary>
	/// The remote that might be the target for this packet
	/// </summary>
	public IPEndPoint? RemoteTarget { get; set; }
	/// <summary>
	/// The remote that sent this packet
	/// </summary>
	public IPEndPoint? RemoteSender { get; private set; }

	public Packet( byte[] packetData ) {
		this._rawdata = packetData;
		unsafe {
			fixed ( byte* ptr = packetData )
				this.Id = ( (int*) ptr )[ 0 ];
		}
		SetIdInData( this._rawdata );
		unsafe {
			fixed ( byte* ptr = packetData )
				this.PostId = ( (int*) ptr )[ 0 ];
		}
	}

	private unsafe void SetIdInData( byte[] packetData ) {
		if ( packetData.Length < sizeof( int ) )
			throw new ArgumentOutOfRangeException( nameof( packetData ), "Size of array can't fit header." );
		int id = Resources.GlobalService<PacketTypeManager>().GetPacketId( GetType() );
		if ( id < 0 )
			throw new InvalidOperationException( "Type not supported!" );
		fixed ( byte* ptr = packetData )
			( (int*) ptr )[ 0 ] = id;
	}

	private void SetSender( IPEndPoint sender ) => this.RemoteSender = sender;

	protected static byte[] GeneratePacketData( params byte[][] data ) => Segmentation.SegmentWithPadding( 4, 0, data );

	protected byte[][] GetSeparatedData() {
		if ( this._separated is not null )
			return this._separated;
		this._separated = Segmentation.Parse( this._rawdata, 4 );
		if ( this._separated is null )
			throw new Exception( "Unable to separate packet data." );
		return this._separated;
	}

	internal unsafe void CopyDataTo( byte* dstPtr, int dstOffsetBytes ) {
		fixed ( byte* srcPtr = this._rawdata )
			Unsafe.CopyBlock( dstPtr + dstOffsetBytes, srcPtr, (uint) this._rawdata.Length );
	}
}

[Protocol( ProtocolType.Udp )]
public class UdpPing : Packet {
	public string Username { get; }
	public double Time { get; }

	public UdpPing( string username, double time, IPEndPoint? remoteTarget ) : base( GeneratePacketData( DataUtils.ToBytes( username ), DataUtils.ToBytes( time ) ) ) {
		this.Username = username;
		this.Time = time;
		this.RemoteTarget = remoteTarget;
	}

	public UdpPing( byte[] data ) : base( data ) {
		byte[][] separatedData = GetSeparatedData();
		this.Username = DataUtils.ToStringUTF8( separatedData[ 0 ] ) ?? "_Unknown_";
		this.Time = DataUtils.ToUnmanaged<long>( separatedData[ 1 ] ) ?? long.MinValue;
	}
}

[Protocol( ProtocolType.Tcp )]
public class TcpLogin : Packet {
	public string Username { get; }

	public TcpLogin( string name ) : base( GeneratePacketData( DataUtils.ToBytes( name ) ) ) {
		this.Username = name;
	}

	public TcpLogin( byte[] data ) : base( data ) {
		byte[][] separatedData = GetSeparatedData();
		this.Username = DataUtils.ToStringUTF8( separatedData[ 0 ] ) ?? "_Unknown_";
	}
}

[Protocol( ProtocolType.Tcp )]
public class TcpLoginAck : Packet {
	public string Username { get; }
	public ulong NetworkId { get; }

	public TcpLoginAck( string name, ulong id ) : base( GeneratePacketData( DataUtils.ToBytes( name ), DataUtils.ToBytes( id ) ) ) {
		this.Username = name;
		this.NetworkId = id;
	}

	public TcpLoginAck( byte[] data ) : base( data ) {
		byte[][] separatedData = GetSeparatedData();
		this.Username = DataUtils.ToStringUTF8( separatedData[ 0 ] ) ?? "_Unknown_";
		this.NetworkId = DataUtils.ToUnmanaged<ulong>( separatedData[ 1 ] ) ?? throw new ArgumentException( "Failed reading network id" );
	}
}

[Protocol( ProtocolType.Tcp )]
public class ConnectionFailed : Packet {
	public string Reason { get; }

	public ConnectionFailed( string reason ) : base( GeneratePacketData( DataUtils.ToBytes( reason ) ) ) {
		this.Reason = reason;
	}

	public ConnectionFailed( byte[] data ) : base( data ) {
		byte[][] separatedData = GetSeparatedData();
		this.Reason = DataUtils.ToStringUTF8( separatedData[ 0 ] ) ?? "Error message was corrupt";
	}
}

[Protocol( ProtocolType.Tcp )]
public class ClientDisconnected : Packet {
	public string Username { get; }
	public string Reason { get; }

	public ClientDisconnected( string name, string reason ) : base( GeneratePacketData( DataUtils.ToBytes( name ), DataUtils.ToBytes( reason ) ) ) {
		this.Username = name;
		this.Reason = reason;
	}

	public ClientDisconnected( byte[] data ) : base( data ) {
		byte[][] separatedData = GetSeparatedData();
		this.Username = DataUtils.ToStringUTF8( separatedData[ 0 ] ) ?? "_Unknown_";
		this.Reason = DataUtils.ToStringUTF8( separatedData[ 1 ] ) ?? "Error message was corrupt";
	}
}

public sealed class PacketTypeManager : Identifiable {

	private readonly List<Type> _packetTypes;
	private readonly Dictionary<Type, int> _packetIdFromTypes;
	private readonly Dictionary<int, Func<byte[], Packet>> _packetConstructors;
	private readonly Dictionary<Type, ProtocolType> _packetProtocols;
	private readonly Action<Packet, IPEndPoint> _senderSetter;

	public PacketTypeManager() {
		this._packetTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany( p => p.GetTypes().Where( q => q.IsAssignableTo( typeof( Packet ) ) && !q.IsAbstract ) )
			.OrderBy( p => p.AssemblyQualifiedName )
			.ToList();
		this._packetIdFromTypes = new Dictionary<Type, int>();

		for ( int i = 0; i < this._packetTypes.Count; i++ ) {
			this._packetIdFromTypes[ this._packetTypes[ i ] ] = i;
			this.LogLine( $"Registered {this._packetTypes[ i ]} with id {i}!", Log.Level.NORMAL, ConsoleColor.Blue );
		}

		this._packetProtocols = new Dictionary<Type, ProtocolType>();
		foreach ( Type type in this._packetTypes ) {
			ProtocolAttribute? protocolAttribute = type.GetCustomAttribute<ProtocolAttribute>();
			if ( protocolAttribute is null )
				this.LogWarning( $"{type.FullName} does not have a Protocol attribute! Defaulting to TCP!" );
			this._packetProtocols[ type ] = protocolAttribute?.Protocol ?? ProtocolType.Tcp;
			this.LogLine( $"Registered {type} as {protocolAttribute?.Protocol ?? ProtocolType.Tcp}!", Log.Level.NORMAL, ConsoleColor.Blue );
		}

		this._packetConstructors = new Dictionary<int, Func<byte[], Packet>>();
		for ( int i = 0; i < this._packetTypes.Count; i++ ) {
			ConstructorInfo? ctor = this._packetTypes[ i ].GetConstructor( new[] { typeof( byte[] ) } );
			if ( ctor is null ) {
				this.LogWarning( $"{this._packetTypes[ i ].FullName} does not have a constructor compliant with Packet guidelines." );
				continue;
			}
			ParameterExpression param = Expression.Parameter( typeof( byte[] ) );
			Func<byte[], Packet> fastCtor = Expression.Lambda<Func<byte[], Packet>>( Expression.New( ctor, param ), param ).Compile();
			this._packetConstructors.Add( i, fastCtor );
		}

		MethodInfo? method = typeof( Packet ).GetMethod( "SetSender", BindingFlags.NonPublic | BindingFlags.Instance, new Type[] { typeof( IPEndPoint ) } );
		if ( method is null )
			throw new NullReferenceException( nameof( method ) );
		this._senderSetter = (Action<Packet, IPEndPoint>) Delegate.CreateDelegate( typeof( Action<Packet, IPEndPoint> ), method );
	}

	public int GetPacketId( Type type ) => this._packetIdFromTypes.TryGetValue( type, out int id ) ? id : -1;
	public ProtocolType GetPacketProtocol( Type type ) {
		if ( this._packetProtocols.TryGetValue( type, out ProtocolType id ) )
			return id;
		return ProtocolType.Tcp;
	}

	public Packet? GetPacketFromData( byte[] data, IPEndPoint sender ) {
		if ( data.Length < sizeof( int ) )
			return null;
		int id = BitConverter.ToInt32( data );
		if ( !this._packetConstructors.TryGetValue( id, out Func<byte[], Packet>? func ) )
			return null;
		Packet p = func.Invoke( data );
		this._senderSetter.Invoke( p, sender );
		return p;
	}

}

[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
public sealed class ProtocolAttribute : Attribute {
	private readonly ProtocolType _protocol;

	public ProtocolAttribute( ProtocolType protocol ) {
		this._protocol = protocol;
	}

	public ProtocolType Protocol => this._protocol;
}
