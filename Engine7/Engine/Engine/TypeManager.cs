using System.Collections.Concurrent;
using System.Reflection;

namespace Engine;

//TESTS


public static class TypeManager {
	private static readonly ConcurrentDictionary<Type, ResolvedType> _resolvedTypes = [];
	public static IReadOnlyList<Type> AllTypes { get; } = AppDomain.CurrentDomain.GetAssemblies().SelectMany( x => x.GetTypes() ).ToArray().AsReadOnly();

	/// <summary>
	/// Assumes the types you want are not abstract and are subclasses of the generic type directly.
	/// </summary>
	public static IEnumerable<Type> GetAllSubclassesOfGenericType( Type genericType )
		=> AllTypes.Where( p => HasSpecificBaseType( p, genericType ) && !p.IsAbstract );

	private static bool HasSpecificBaseType( Type inquiringType, Type baseType ) {
		if (inquiringType.BaseType == null)
			return false;
		if (inquiringType.BaseType.IsGenericType) {
			if (inquiringType.BaseType.GetGenericTypeDefinition() == baseType)
				return true;
		} else if (inquiringType.BaseType == baseType)
			return true;
		return HasSpecificBaseType( inquiringType.BaseType, baseType );
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
}
