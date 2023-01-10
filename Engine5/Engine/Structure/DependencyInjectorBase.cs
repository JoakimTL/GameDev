using Engine.Structure.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Structure;
public abstract class DependencyInjectorBase : Identifiable {
	protected abstract object? GetInternal( Type t );

	protected object? Create( Type t, bool getRequiredTypes ) {
		if ( t.IsAbstract || t.IsInterface )
			throw new Exception( $"{t.FullName} is not implemented and cannot be loaded." );

		ConstructorInfo[]? ctors = t.GetConstructors();
		if ( ctors.Length == 0 )
			throw new InvalidOperationException( "Type must have a valid constructor" );

		ConstructorInfo? ctor = ctors[ 0 ];
		if ( ctors.Length > 1 )
			this.LogLine( $"Found multiple constructors for type {t.FullName}. Using {t.Name}({string.Join( ", ", ctor.GetParameters().Select( p => $"{p.ParameterType.Name} {p.Name}" ) )})!", Log.Level.CRITICAL );

		Type[] parameterTypes = ctor.GetParameters().Select( p => p.ParameterType ).ToArray();

		if ( parameterTypes.Any( p => p == t ) )
			throw new Exception( $"Object {t.FullName} can't depend on itself!" );

		Type selfType = GetType();
		object?[]? parameters = parameterTypes.Select( p => p == selfType ? this : GetInternal( p ) ).ToArray();

		if ( parameters.Any( p => p is null ) )
			throw new Exception( $"Unable to load all dependencies for {t.FullName}!" );
		var obj = ctor.Invoke( parameters );

		if ( getRequiredTypes )
			foreach ( Type requiredServiceType in t.GetCustomAttributes<RequireAttribute>().Select( p => p.RequiredType ) )
				GetInternal( requiredServiceType );
		return obj;
	}

}
