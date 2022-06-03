namespace Engine.Structure;

public static class TypeManager {

	private static readonly List<Type> _types;

	static TypeManager() {
		_types = AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes() ).ToList();
	}

	public static IEnumerable<Type> GetAllTypes( Type type, bool allowAbstract, bool allowInterfaces ) => _types.Where( q => q.IsAssignableTo( type ) && ( allowAbstract || !q.IsAbstract ) && ( allowInterfaces || !q.IsInterface ) );

	public static IEnumerable<Type> GetAllTypes<T>( bool allowAbstract, bool allowInterfaces ) => GetAllTypes( typeof( T ), allowAbstract, allowInterfaces );
}
