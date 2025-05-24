using Engine.Logging;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Security.AccessControl;

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

		List<Assembly> assemblies = [ .. aggregateCatalog.Catalogs.OfType<AssemblyCatalog>().Select( p => p.Assembly ) ];
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

	///// <summary>
	///// Assumes the types you want are not abstract and are subclasses of the generic type directly.
	///// </summary>
	//public IEnumerable<Type> GetAllNonAbstractSubclassesOf( Type genericType )
	//	=> this.DerivedTypes.Where( p => HasSpecificBaseType( p, genericType ) && !p.IsAbstract );

	/// <summary>
	/// Used when trying to find the specific generic argument types the implementation class is derived from.
	/// </summary>
	//public Type[] GetGenericArgumentsOf( Type implementationType, Type genericBaseType ) {
	//	if (implementationType.BaseType == null)
	//		return [];
	//	if (!implementationType.BaseType.IsGenericType || implementationType.BaseType.GetGenericTypeDefinition() != genericBaseType)
	//		return GetGenericArgumentsOf( implementationType.BaseType, genericBaseType );
	//	return implementationType.BaseType.GetGenericArguments();
	//}

	//private bool HasSpecificBaseType( Type implementationType, Type baseType ) {
	//	if (implementationType.BaseType == null)
	//		return false;
	//	if (implementationType.BaseType.IsGenericType) {
	//		if (implementationType.BaseType.GetGenericTypeDefinition() == baseType)
	//			return true;
	//	} else if (implementationType.BaseType == baseType)
	//		return true;
	//	return HasSpecificBaseType( implementationType.BaseType, baseType );
	//}

	/// <summary>
	/// Finds every non-abstract subtype of the open generic <paramref name="openGenericTypeDefinition"/> in the provided <paramref name="allTypes"/> set.
	/// </summary>
	public IEnumerable<Type> GetAllNonAbstractSubclassesOf( Type openGenericTypeDefinition ) {
		if (!openGenericTypeDefinition.IsGenericTypeDefinition)
			throw new ArgumentException( "Must be an open generic", nameof( openGenericTypeDefinition ) );

		return this.DerivedTypes
			.Where( t => !t.IsAbstract )
			.Where( t => FindClosedGenericBase( t, openGenericTypeDefinition ) != null );
	}

	/// <summary>
	/// Walks up the inheritance chain (and checks interfaces) for a closed‐generic of <paramref name="openGenericTypeDefinition"/>, returning that closed type	(e.g. SomeBase&lt;Foo,Bar&gt;), or null if none is found.
	/// </summary>
	public Type? FindClosedGenericBase( Type concreteType, Type openGenericTypeDefinition ) {
		ArgumentNullException.ThrowIfNull( concreteType );
		ArgumentNullException.ThrowIfNull( openGenericTypeDefinition );
		if (!openGenericTypeDefinition.IsGenericTypeDefinition)
			throw new ArgumentException( "Must be an open generic", nameof( openGenericTypeDefinition ) );

		for (Type? t = concreteType; t != null; t = t.BaseType)
			if (t.IsGenericType && t.GetGenericTypeDefinition() == openGenericTypeDefinition)
				return t;

		foreach (Type iface in concreteType.GetInterfaces())
			if (iface.IsGenericType && iface.GetGenericTypeDefinition() == openGenericTypeDefinition)
				return iface;

		return null;
	}

	/// <summary>
	/// Returns the generic arguments of the closed‐generic base, or an empty array if none.
	/// </summary>
	public Type[] GetGenericArgumentsOf( Type concreteType, Type openGenericTypeDefinition ) {
		Type? closed = FindClosedGenericBase( concreteType, openGenericTypeDefinition );
		return closed?.GetGenericArguments() ?? Type.EmptyTypes;
	}

	/// <summary>
	/// True if <paramref name="concreteType"/> has a closed‐generic base of <paramref name="openGenericTypeDefinition"/> whose arguments exactly match <paramref name="desiredArgs"/>.
	/// </summary>
	public bool HasExactGenericArguments( Type concreteType, Type openGenericTypeDefinition, params Type[] desiredArgs ) {
		Type[] args = GetGenericArgumentsOf( concreteType, openGenericTypeDefinition );
		if (args.Length != desiredArgs.Length)
			return false;
		return args.SequenceEqual( desiredArgs );
	}

	/// <summary>
	/// True if <paramref name="concreteType"/> has a closed‐generic base of <paramref name="openGenericTypeDefinition"/> whose arguments are assignable to <paramref name="requiredBaseArgs"/> respectively.
	/// </summary>
	public bool HasAssignableGenericArguments( Type concreteType, Type openGenericTypeDefinition, params Type[] requiredBaseArgs ) {
		Type[] args = GetGenericArgumentsOf( concreteType, openGenericTypeDefinition );
		if (args.Length != requiredBaseArgs.Length)
			return false;

		for (int i = 0; i < args.Length; i++)
			if (!args[ i ].IsAssignableTo( requiredBaseArgs[ i ] ))
				return false;

		return true;
	}

}