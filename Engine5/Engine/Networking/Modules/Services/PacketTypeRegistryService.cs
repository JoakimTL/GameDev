using Engine.GlobalServices;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;

namespace Engine.Networking.Modules.Services;

public class PacketTypeRegistryService : Identifiable, INetworkClientService, INetworkServerService {

    private readonly List<PacketTypeData> _packetTypes;
    private readonly Dictionary<Type, int> _packetTypeIds;

    public string PacketCollectionString { get; }
    public string PacketCollectionHash { get; }

    public PacketTypeRegistryService( TypeRegistryService typeService ) {
        foreach ( var missingConstructablePacket in typeService.ImplementationTypes.Where( p => p.IsAssignableTo( typeof( PacketBase ) ) && !p.IsAssignableTo( typeof( IConstructablePacket ) ) ) )
            this.LogWarning( $"Packet type {missingConstructablePacket} does not implement {nameof( IConstructablePacket )}" );
        var packetTypeList = typeService.ImplementationTypes.Where( p => p.GetInterfaces().Contains( typeof( IConstructablePacket ) ) ).OrderBy( p => p.AssemblyQualifiedName ).ToList();
        _packetTypes = new();
        foreach ( var packetType in packetTypeList ) {
            var packetTypeData = new PacketTypeData( _packetTypes.Count, packetType );
			_packetTypes.Add( packetTypeData );
            this.LogLine( $"Packet type {packetTypeData.PacketType.Namespace}.{packetTypeData.PacketType.Name} identifying as {packetTypeData.Id}", Log.Level.VERBOSE );
        }
        _packetTypeIds = _packetTypes.ToDictionary( p => p.PacketType, p => p.Id );

        PacketCollectionString = string.Join( '|', _packetTypes.Select( p => $"{p.Id}-{p.PacketType.AssemblyQualifiedName}" ) );
        PacketCollectionHash = SHA256.HashData( PacketCollectionString.ToBytes() ).CreateString().NotNull();
    }

    public PacketBase? Construct( byte[] data, IPEndPoint? remoteSender ) {
        if ( data.Length < sizeof( int ) )
            return this.LogWarningThenReturnDefault<PacketBase>( $"Not enough packet data!" );
        int id;
        unsafe {
            fixed ( byte* ptr = data )
                id = *(int*) ptr;
        }
        if ( id < 0 || id >= _packetTypes.Count )
            return this.LogWarningThenReturnDefault<PacketBase>( $"{id} is not a packet id!" );
        return _packetTypes[ id ].Constructor.Invoke( data[ sizeof( int ).. ], remoteSender );
    }

    public int GetPacketTypeId( Type type ) => _packetTypeIds.TryGetValue( type, out int id ) ? id : -1;

    private class PacketTypeData {
        public readonly int Id;
        public readonly Type PacketType;
        public readonly Func<byte[], IPEndPoint?, PacketBase?> Constructor;

        public PacketTypeData( int id, Type packetType ) {
            Id = id;
            PacketType = packetType;
            Constructor = packetType
                .GetMethod( nameof( IConstructablePacket.Construct ), BindingFlags.Public | BindingFlags.Static ).NotNull()
                .CompileStaticMethod<Func<byte[], IPEndPoint?, PacketBase?>>( typeof( byte[] ), typeof( IPEndPoint ) ).NotNull();
        }
    }
}
