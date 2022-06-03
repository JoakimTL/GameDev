using System.Reflection;

namespace Engine;
public static class IdentificationRegistry {

	private static readonly Dictionary<Guid, Type> _typesByGuid = new();
	private static readonly Dictionary<Type, Guid> _guidByTypes = new();

	static IdentificationRegistry() {
		_typesByGuid = new Dictionary<Guid, Type>();
		ScanTypes();
	}

	private static void ScanTypes() {
		Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes() ).ToArray();
		foreach ( Type? type in types ) {
			IdentificationAttribute? identification = type.GetCustomAttribute<IdentificationAttribute>();
			if ( identification is not null )
				if ( !_typesByGuid.ContainsKey( identification.Guid ) ) {
					_typesByGuid.Add( identification.Guid, type );
					_guidByTypes.Add( type, identification.Guid );
				} else {
					_typesByGuid.TryGetValue( identification.Guid, out Type? t );
					Log.Warning( $"You did it. You actually did it. You managed to find a Guid matching another in this install. God damn, good job!{Environment.NewLine}Please change the guid if you want {type.Name} to work properly during save/load sequences. {( t is not null ? $"If you want to know {t?.Name} is currently occupying {identification.Guid}" : "" ) }" );
				}
		}
	}

	public static Type? Get( Guid guid ) {
		if ( _typesByGuid.TryGetValue( guid, out Type? t ) )
			return t;
		return null;
	}

	public static Guid? Get( Type type ) {
		if ( _guidByTypes.TryGetValue( type, out Guid g ) )
			return g;
		return null;
	}
}