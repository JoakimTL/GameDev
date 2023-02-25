using Engine.Structure.Interfaces;
using System.Reflection;

namespace Engine.GlobalServices;

public sealed class TypeService : IGlobalService {

	public readonly IReadOnlyList<Type> AllTypes;
	public readonly IReadOnlyList<Type> AbstractTypes;
	public readonly IReadOnlyList<Type> SealedTypes;
	public readonly IReadOnlyList<Type> DerivedTypes;
	public readonly IReadOnlyList<Type> InterfaceTypes;
	/// <summary>
	/// These are non-abstract classes implementing an interface and/or has a base class
	/// </summary>
	public readonly IReadOnlyList<Type> ImplementationTypes;

	public TypeService() {
		LoadAllAssemblies();
		List<Type> allTypes = new();
		AllTypes = allTypes;
		List<Type> abstractTypes = new();
		AbstractTypes = abstractTypes;
		List<Type> sealedTypes = new();
		SealedTypes = sealedTypes;
		List<Type> interfaceTypes = new();
		InterfaceTypes = interfaceTypes;
		List<Type> derivedTypes = new();
		DerivedTypes = derivedTypes;
		List<Type> implementationTypes = new();
		ImplementationTypes = implementationTypes;

		foreach ( var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes() ) ) {
			allTypes.Add( type );
			if ( type.IsAbstract )
				abstractTypes.Add( type );
			if ( type.IsInterface )
				interfaceTypes.Add( type );
			if ( type.IsSealed )
				sealedTypes.Add( type );
			if ( type.BaseType is not null || type.GetInterfaces().Length > 0 ) {
				derivedTypes.Add( type );
				if ( type.IsClass && !type.IsAbstract )
					implementationTypes.Add( type );
			}
		}
	}

	private void LoadAllAssemblies() {
		foreach ( Assembly assembly in AppDomain.CurrentDomain.GetAssemblies() )
			LoadReferencedAssembly( assembly );
		Log.Line( $"Loaded {AppDomain.CurrentDomain.GetAssemblies().Count()} assemblies!", Log.Level.NORMAL );
	}

	private void LoadReferencedAssembly( Assembly assembly ) {
		foreach ( AssemblyName name in assembly.GetReferencedAssemblies() )
			if ( !AppDomain.CurrentDomain.GetAssemblies().Any( a => a.FullName == name.FullName ) )
				try {
					LoadReferencedAssembly( Assembly.Load( name ) );
				} catch ( Exception e ) {
					Log.Warning( e.Message );
				}
	}

}
