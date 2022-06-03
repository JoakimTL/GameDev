namespace Engine.Structure;

public static class DependencyTypeProvider {

	private static readonly Dictionary<Type, List<Type>> _injectionTypes;

	static DependencyTypeProvider() {
		_injectionTypes = new Dictionary<Type, List<Type>>();

		IEnumerable<Type>? types = TypeManager.GetAllTypes<object>( false, true );

		foreach ( Type? t in types ) {
			if ( t.IsInterface || t.IsAbstract )
				continue;

			if ( !t.IsInterface )
				foreach ( Type? iT in t.GetInterfaces() )
					AddInjectionType( iT, t );
			if ( t.BaseType is not null )
				AddInjectionType( t.BaseType, t );
		}
	}

	private static void AddInjectionType( Type interfaceType, Type implementingType ) {
		if ( !_injectionTypes.TryGetValue( interfaceType, out List<Type>? injectionTypes ) )
			_injectionTypes.Add( interfaceType, injectionTypes = new List<Type>() );
		injectionTypes.Add( implementingType );
	}

	public static Type GetImplementation( Type t ) {
		IReadOnlyList<Type>? implementations = GetImplementations( t );
		if ( implementations.Count > 1 )
			Log.Line( $"Multiple implementations found for {t}. Consider using GetImplementations to get all instead of one.", Log.Level.HIGH );
		return implementations.First();
	}

	public static IReadOnlyList<Type> GetImplementations( Type t ) {
		if ( !t.IsInterface && !t.IsAbstract )
			throw new InvalidOperationException( "Queried type must be an interface or abstract type!" );

		if ( !_injectionTypes.TryGetValue( t, out List<Type>? list ) )
			throw new InvalidOperationException( $"There are no registered implementation for {t}!" );

		return list;
	}
}
