using Engine.Structure.Interfaces;

namespace Engine.GlobalServices;

public sealed class TypeService : IGlobalService {

	public readonly IReadOnlyList<Type> AllTypes;
	public readonly IReadOnlyList<Type> AbstractTypes;
	public readonly IReadOnlyList<Type> SealedTypes;
	public readonly IReadOnlyList<Type> DerivedTypes;
	public readonly IReadOnlyList<Type> InterfaceTypes;

	public TypeService() {
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

		foreach ( var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes() ) ) {
			allTypes.Add( type );
			if ( type.IsAbstract )
				abstractTypes.Add( type );
			if ( type.IsInterface )
				interfaceTypes.Add( type );
			if ( type.IsSealed )
				sealedTypes.Add( type );
			if ( type.BaseType is not null || type.GetInterfaces().Length > 0 )
				derivedTypes.Add( type );
		}
	}

}
