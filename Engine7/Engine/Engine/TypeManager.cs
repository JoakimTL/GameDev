using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Engine;

public static class TypeManager {
	private static readonly ConcurrentDictionary<Type, ResolvedType> _resolvedTypes;
	public static readonly TypeRegistry Registry;
	public static readonly IdentityRegistry Identities;
	public static readonly SerializerRegistry Serializers;
	private static readonly Func<Type, int> _sizeOfFunc;

	static TypeManager() {
		_resolvedTypes = [];
		Registry = new();
		Identities = new( Registry );
		Serializers = new( Registry );
		_sizeOfFunc = CreateSizeOfResolver();
	}

	private static Func<Type, int> CreateSizeOfResolver() {
		MethodInfo genericMethodInfo = typeof( TypeManager ).GetMethod( nameof( GetSizeOfGeneric ), BindingFlags.NonPublic | BindingFlags.Static ) ?? throw new InvalidOperationException( "GetSizeOfGeneric method not found." );
		MethodInfo methodBaseInvoke = typeof( MethodBase ).GetMethod( nameof( MethodBase.Invoke ), [ typeof( object ), typeof( object[] ) ] ) ?? throw new InvalidOperationException( "MethodBase.Invoke method not found." );

		// Define the input parameter for the delegate: Type
		ParameterExpression typeParameter = Expression.Parameter( typeof( Type ), "type" );

		// Create the call to MethodInfo.MakeGenericMethod(type)
		MethodCallExpression makeGenericMethodCall = Expression.Call(
			Expression.Constant( genericMethodInfo ),
			nameof( MethodInfo.MakeGenericMethod ),
			null,
			Expression.NewArrayInit( typeof( Type ), typeParameter )
		);

		// Create the call to Invoke(null, null) to invoke the generic method
		MethodCallExpression invokeMethodCall = Expression.Call(
			makeGenericMethodCall,
			methodBaseInvoke,
			Expression.Constant( null ),         // No instance for static methods
			Expression.Constant( null, typeof( object[] ) ) // No parameters
		);

		// Convert the result of Invoke (object) to int
		UnaryExpression castResult = Expression.Convert( invokeMethodCall, typeof( int ) );

		// Compile the expression into a delegate
		Expression<Func<Type, int>> lambda = Expression.Lambda<Func<Type, int>>( castResult, typeParameter );
		return lambda.Compile();
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

	public static ResolvedType Resolve( this Type type ) => ResolveType( type );

	public static int? SizeOf( Type t ) {
		if (!t.IsValueType)
			return null;
		int size = _sizeOfFunc( t );
		return size;
	}

	private static int GetSizeOfGeneric<T>() where T : unmanaged => System.Runtime.CompilerServices.Unsafe.SizeOf<T>();

	public static IEnumerable<Type> WithAttribute<T>( this IEnumerable<Type> types ) where T : Attribute => types.Where( p => Attribute.IsDefined( p, typeof( T ) ) );

}
