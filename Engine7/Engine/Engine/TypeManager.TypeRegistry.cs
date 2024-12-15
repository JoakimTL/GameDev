using Engine.Logging;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace Engine;
public sealed class TypeRegistry {

	public readonly IReadOnlyList<Type> AllTypes;
	public readonly IReadOnlyList<Type> AbstractTypes;
	public readonly IReadOnlyList<Type> ValueTypes;
	public readonly IReadOnlyList<Type> SealedTypes;
	public readonly IReadOnlyList<Type> DerivedTypes;
	public readonly IReadOnlyList<Type> InterfaceTypes;
	/// <summary>
	/// These are non-abstract classes implementing an interface and/or has a base class
	/// </summary>
	public readonly IReadOnlyList<Type> ImplementationTypes;

	public TypeRegistry() {
		List<string> directoriesToSearch = [ ".", ".\\mods" ];
		if (Directory.Exists( ".\\mods" ))
			directoriesToSearch.AddRange( Directory.GetDirectories( ".\\mods", "*", SearchOption.AllDirectories ) );

		List<string> allDlls = [];
		foreach (string directory in directoriesToSearch)
			if (Directory.Exists( directory ))
				allDlls.AddRange( Directory.GetFiles( directory, "*.dll", SearchOption.TopDirectoryOnly ) );

		List<ComposablePartCatalog> catalogs = [];
		foreach (string dllPath in allDlls) {
			try {
				catalogs.Add( new AssemblyCatalog( dllPath ) );
			} catch (Exception ex) {
				Log.Warning( $"Error loading assembly {dllPath}: {ex.Message}" );
			}
		}

		AggregateCatalog aggregateCatalog = new( catalogs );

		List<Assembly> assemblies = aggregateCatalog.Catalogs.OfType<AssemblyCatalog>().Select( p => p.Assembly ).ToList();
		this.LogLine( $"Found {assemblies.Count} assembl{(assemblies.Count == 1 ? "y" : "ies")}!", Log.Level.NORMAL );
		foreach (Assembly assembly in assemblies)
			this.LogLine( $"- {assembly.GetName().Name}", Log.Level.VERBOSE );

		List<Type> allTypes = [];

		foreach (Assembly assembly in assemblies)
			try {
				allTypes.AddRange( assembly.GetTypes() );
			} catch (ReflectionTypeLoadException ex) {
				this.LogWarning( $"Error loading types from assembly {assembly.FullName}: {ex.Message}" );
				allTypes.AddRange( ex.Types.OfType<Type>() );
			}

		List<Type> abstractTypes = [];
		List<Type> valueTypes = [];
		List<Type> sealedTypes = [];
		List<Type> interfaceTypes = [];
		List<Type> derivedTypes = [];
		List<Type> implementationTypes = [];

		foreach (Type type in allTypes) {
			if (type.IsAbstract)
				abstractTypes.Add( type );
			if (type.IsValueType)
				valueTypes.Add( type );
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

		this.AllTypes = allTypes.AsReadOnly();
		this.ValueTypes = valueTypes.AsReadOnly();
		this.AbstractTypes = abstractTypes.AsReadOnly();
		this.SealedTypes = sealedTypes.AsReadOnly();
		this.DerivedTypes = derivedTypes.AsReadOnly();
		this.InterfaceTypes = interfaceTypes.AsReadOnly();
		this.ImplementationTypes = implementationTypes.AsReadOnly();
	}

	/// <summary>
	/// Assumes the types you want are not abstract and are subclasses of the generic type directly.
	/// </summary>
	public IEnumerable<Type> GetAllSubclassesOfGenericType( Type genericType )
		=> this.DerivedTypes.Where( p => HasSpecificBaseType( p, genericType ) && !p.IsAbstract );

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