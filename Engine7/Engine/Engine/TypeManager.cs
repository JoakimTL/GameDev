using System.Collections.Concurrent;

namespace Engine;

//TESTS


public static class TypeManager {
	private static readonly ConcurrentDictionary<Type, ResolvedType> _resolvedTypes = [];
	public static IReadOnlyList<Type> AllTypes { get; } = AppDomain.CurrentDomain.GetAssemblies().SelectMany( x => x.GetTypes() ).ToArray().AsReadOnly();

	/// <summary>
	/// Assumes the types you want are not abstract and are subclasses of the generic type directly.
	/// </summary>
	public static IEnumerable<Type> GetAllSubclassesOfGenericType( Type genericType ) => AllTypes.Where( p => p.BaseType != null && p.BaseType.IsGenericType && p.BaseType.GetGenericTypeDefinition() == genericType && !p.IsAbstract );

	public static ResolvedType ResolveType( Type type ) {
		if (_resolvedTypes.TryGetValue( type, out ResolvedType? resolvedType ))
			return resolvedType;
		resolvedType = new ResolvedType( type );
		_resolvedTypes.TryAdd( type, resolvedType );
		return resolvedType;
	}
}
