using System.Reflection;

namespace Engine;

public sealed class ResolvedType {

	public Type Type { get; }
	public string? Identity { get; }
	public bool HasParameterlessConstructor { get; }
	public int ConstructorCount { get; }
	public IReadOnlyList<Attribute> Attributes { get; }
	private readonly Dictionary<Type, object> _attributesByType = [];
	private readonly Dictionary<BindingFlags, List<PropertyInfo>> _propertiesByBindingFlags = [];
	private readonly TypeInstanceFactory _instanceFactory;

	public ResolvedType( Type type ) {
		this.Type = type;
		this.HasParameterlessConstructor = type.IsValueType || type.GetConstructor( Type.EmptyTypes ) != null;
		this.ConstructorCount = type.GetConstructors().Length;
		this.Attributes = type.GetCustomAttributes( true ).OfType<Attribute>().ToArray();
		this._instanceFactory = new TypeInstanceFactory( type );
		this.Identity = Attributes.OfType<IdentityAttribute>().FirstOrDefault()?.Identity;
	}

	public IReadOnlyList<T> GetAttributes<T>() {
		lock (this._attributesByType) {
			if (this._attributesByType.TryGetValue( typeof( T ), out object? list ))
				return (IReadOnlyList<T>) list;
			list = new List<T>( this.Attributes.OfType<T>() );
			this._attributesByType.Add( typeof( T ), list );
			return (IReadOnlyList<T>) list;
		}
	}

	public IReadOnlyList<PropertyInfo> GetProperties( BindingFlags flags ) {
		if (this._propertiesByBindingFlags.TryGetValue( flags, out List<PropertyInfo>? list ))
			return list;
		list = new List<PropertyInfo>( this.Type.GetProperties( flags ) );
		this._propertiesByBindingFlags.Add( flags, list );
		return list;
	}

	public object? CreateInstance( object[]? parameters ) => this._instanceFactory.CreateInstance( parameters );
}
