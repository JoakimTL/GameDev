using System.Collections;
using System.Collections.Concurrent;

namespace Engine;

//TESTS


public static class TypeManager {
	private static readonly ConcurrentDictionary<Type, ResolvedType> _resolvedTypes = [];
	public static IReadOnlyList<Type> AllTypes { get; } = AppDomain.CurrentDomain.GetAssemblies().SelectMany( x => x.GetTypes() ).ToArray().AsReadOnly();

	public static ResolvedType ResolveType( Type type ) {
		if (_resolvedTypes.TryGetValue( type, out ResolvedType? resolvedType ))
			return resolvedType;
		resolvedType = new ResolvedType( type );
		_resolvedTypes.TryAdd( type, resolvedType );
		return resolvedType;
	}
}
