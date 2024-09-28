using System.Reflection;

namespace Engine;

public abstract class DependencyInjectorBase( IServiceRegistry? serviceRegistry ) : Identifiable {

	protected readonly IServiceRegistry? _serviceRegistry = serviceRegistry;

	protected abstract object? GetInternal( Type t );

	protected object Create( Type implementingType, bool loadRequiredTypes ) {
		ConstructorInfo[]? ctors = implementingType.GetConstructors( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
		if ( ctors.Length == 0 )
			throw new InvalidOperationException( "Type must have a valid constructor" );

		ConstructorInfo? ctor = ctors[ 0 ];
		if ( ctors.Length > 1 )
			this.LogLine( $"Found multiple constructors for type {implementingType.FullName}. Using {implementingType.Name}({string.Join( ", ", ctor.GetParameters().Select( p => $"{p.ParameterType.Name} {p.Name}" ) )})!", Log.Level.CRITICAL );

		Type[] parameterTypes = ctor.GetParameters().Select( p => p.ParameterType ).ToArray();

		if ( parameterTypes.Any( p => p == implementingType ) )
			throw new Exception( $"{this.FullName}: Object {implementingType.FullName} can't depend on itself!" );

		Type selfType = GetType();
		object?[]? parameters = parameterTypes.Select( p => p == selfType ? this : GetInternal( p ) ).ToArray();

		if ( parameters.Any( p => p is null ) )
			throw new Exception( $"{this.FullName}: Unable to load all dependencies for {implementingType.FullName}! Missing {string.Join( ", ", parameterTypes.Except( parameters.Where( p => p is not null ).Select( p => p!.GetType() ) ) )}" );
		object obj = ctor.Invoke( parameters );

		//if ( loadRequiredTypes )
		//	foreach ( Type requiredServiceType in t.GetCustomAttributes<RequireAttribute>().Select( p => p.RequiredType ) )
		//		GetInternal( requiredServiceType );
		return obj;
	}

	protected Type GetImplementingType( Type t ) {
		Type originalType = t;
		if ( t.IsValueType )
			throw new ServiceProviderException( t, "Value types can't be services." );
		if ( t.IsAbstract )
			t = this._serviceRegistry?.GetImplementation( t ) ?? throw new ServiceProviderException( originalType, "No registered type for abstract class found." );
		if ( t.IsInterface )
			t = this._serviceRegistry?.GetImplementation( t ) ?? throw new ServiceProviderException( originalType, "No registered type for interface found." );
		return t;
	}

}


