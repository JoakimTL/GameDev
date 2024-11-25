using Engine.Logging;
using System.Reflection;

namespace Engine;
public sealed class TypeRegistry {

	public readonly IReadOnlyList<Type> AllTypes;
	public readonly IReadOnlyList<Type> AbstractTypes;
	public readonly IReadOnlyList<Type> SealedTypes;
	public readonly IReadOnlyList<Type> DerivedTypes;
	public readonly IReadOnlyList<Type> InterfaceTypes;
	/// <summary>
	/// These are non-abstract classes implementing an interface and/or has a base class
	/// </summary>
	public readonly IReadOnlyList<Type> ImplementationTypes;

	public TypeRegistry() {
		LoadAllAssemblies();
		List<Type> allTypes = [];
		List<Type> abstractTypes = [];
		List<Type> sealedTypes = [];
		List<Type> interfaceTypes = [];
		List<Type> derivedTypes = [];
		List<Type> implementationTypes = [];

		foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes() )) {
			allTypes.Add( type );
			if (type.IsAbstract)
				abstractTypes.Add( type );
			if (type.IsInterface)
				interfaceTypes.Add( type );
			if (type.IsSealed)
				sealedTypes.Add( type );
			if (type.BaseType is not null || type.GetInterfaces().Length > 0) {
				derivedTypes.Add( type );
				if (type.IsClass && !type.IsAbstract)
					implementationTypes.Add( type );
			}
		}

		AllTypes = allTypes.AsReadOnly();
		AbstractTypes = abstractTypes.AsReadOnly();
		SealedTypes = sealedTypes.AsReadOnly();
		DerivedTypes = derivedTypes.AsReadOnly();
		InterfaceTypes = interfaceTypes.AsReadOnly();
		ImplementationTypes = implementationTypes.AsReadOnly();
	}

	private void LoadAllAssemblies() {
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			LoadReferencedAssembly( assembly );
		foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
			this.LogLine( $"Assembly {ass.GetName().Name} loaded", Log.Level.VERBOSE );
		this.LogLine( $"Loaded {AppDomain.CurrentDomain.GetAssemblies().Count()} assemblies!", Log.Level.NORMAL );
	}

	private void LoadReferencedAssembly( Assembly assembly ) {
		foreach (AssemblyName name in assembly.GetReferencedAssemblies())
			if (!AppDomain.CurrentDomain.GetAssemblies().Any( a => a.FullName == name.FullName ))
				try {
					LoadReferencedAssembly( Assembly.Load( name ) );
				} catch (Exception e) {
					Log.Warning( e.Message );
				}
	}

	/// <summary>
	/// Assumes the types you want are not abstract and are subclasses of the generic type directly.
	/// </summary>
	public IEnumerable<Type> GetAllSubclassesOfGenericType( Type genericType )
		=> DerivedTypes.Where( p => HasSpecificBaseType( p, genericType ) && !p.IsAbstract );

	private bool HasSpecificBaseType( Type inquiringType, Type baseType ) {
		if (inquiringType.BaseType == null)
			return false;
		if (inquiringType.BaseType.IsGenericType) {
			if (inquiringType.BaseType.GetGenericTypeDefinition() == baseType)
				return true;
		} else if (inquiringType.BaseType == baseType)
			return true;
		return HasSpecificBaseType( inquiringType.BaseType, baseType );
	}

}
