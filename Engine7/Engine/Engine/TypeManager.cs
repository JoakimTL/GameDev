using System.Collections.Concurrent;
using System.Reflection;

namespace Engine;

public static class TypeManager {
	private static readonly ConcurrentDictionary<Type, ResolvedType> _resolvedTypes;
	public static readonly TypeRegistry Registry;
	public static readonly IdentityRegistry IdentityRegistry;

	static TypeManager() {
		_resolvedTypes = [];
		Registry = new();
		IdentityRegistry = new( Registry );
	}

	/// <summary>
	/// Throws an InvalidOperationException if the type does not have exactly one constructor and that constructor is not parameterless.
	/// </summary>
	/// <exception cref="InvalidOperationException"></exception>
	public static void AssertHasOnlyParameterlessConstructor( Type type ) {
		ConstructorInfo[] constructors = type.GetConstructors();
		if (constructors.Length != 1)
			throw new InvalidOperationException( $"{type.Name} must have exactly one constructor." );
		ConstructorInfo constructor = constructors[ 0 ];
		ParameterInfo[] parameters = constructor.GetParameters();
		if (parameters.Length != 0)
			throw new InvalidOperationException( $"{type.Name} must have a parameterless constructor." );
	}

	public static bool HasParameterlessConstructor( this Type type, bool onlyConstructor ) => ResolveType( type ).HasParameterlessConstructor && (!onlyConstructor || ResolveType( type ).ConstructorCount == 1);

	public static ResolvedType ResolveType( Type type ) {
		if (_resolvedTypes.TryGetValue( type, out ResolvedType? resolvedType ))
			return resolvedType;
		resolvedType = new ResolvedType( type );
		_resolvedTypes.TryAdd( type, resolvedType );
		return resolvedType;
	}

	public static IEnumerable<Type> WithAttribute<T>( this IEnumerable<Type> types ) where T : Attribute => types.Where( p => Attribute.IsDefined( p, typeof( T ) ) );
}
