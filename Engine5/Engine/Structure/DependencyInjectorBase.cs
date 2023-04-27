using Engine.Structure.Attributes;
using System.Reflection;

namespace Engine.Structure;
public abstract class DependencyInjectorBase : Identifiable {
	protected abstract object? GetInternal( Type t );

	protected object? Create( Type t, bool loadRequiredTypes ) {
		t = GetImplementingType( t );

		ConstructorInfo[]? ctors = t.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		if ( ctors.Length == 0 )
			throw new InvalidOperationException( "Type must have a valid constructor" );

		ConstructorInfo? ctor = ctors[ 0 ];
		if ( ctors.Length > 1 )
			this.LogLine( $"Found multiple constructors for type {t.FullName}. Using {t.Name}({string.Join( ", ", ctor.GetParameters().Select( p => $"{p.ParameterType.Name} {p.Name}" ) )})!", Log.Level.CRITICAL );

		Type[] parameterTypes = ctor.GetParameters().Select( p => p.ParameterType ).ToArray();

		if ( parameterTypes.Any( p => p == t ) )
			throw new Exception( $"{this.FullName}: Object {t.FullName} can't depend on itself!" );

		Type selfType = GetType();
		object?[]? parameters = parameterTypes.Select( p => p == selfType ? this : GetInternal( p ) ).ToArray();

		if ( parameters.Any( p => p is null ) )
			throw new Exception( $"{this.FullName}: Unable to load all dependencies for {t.FullName}! Missing {string.Join(", ", parameterTypes.Except(parameters.Where(p => p is not null).Select(p => p!.GetType())))}" );
		var obj = ctor.Invoke( parameters );

		if ( loadRequiredTypes )
			foreach ( Type requiredServiceType in t.GetCustomAttributes<RequireAttribute>().Select( p => p.RequiredType ) )
				GetInternal( requiredServiceType );
		return obj;
	}

	protected Type GetImplementingType( Type type ) {
		if ( !type.IsInterface && !type.IsAbstract )
			return type;
		var implementations = AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes().Where( q => q.IsAssignableTo( type ) && q.IsClass && !q.IsAbstract ) ).ToList();

		if ( implementations.Count == 0 )
			throw new NotImplementedException( type.FullName );

		var chosenImplementation = implementations.First();
		if ( implementations.Count > 1 )
			this.LogWarning( $"{type} has multiple implementations, chose {chosenImplementation}!" );
		return chosenImplementation;
	}

}
