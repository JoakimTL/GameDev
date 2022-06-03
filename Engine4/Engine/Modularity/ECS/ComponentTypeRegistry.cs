namespace Engine.Modularity.ECS;

public class ComponentTypeRegistry : Identifiable {

	private readonly Dictionary<Type, Type> _typesRegisteredAs = new();

	public ComponentTypeRegistry() {
		this._typesRegisteredAs = new Dictionary<Type, Type>();
		ScanTypes();
	}

	private void ScanTypes() {
		Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes() ).ToArray();
		foreach ( Type? type in types )
			if ( type.GetCustomAttributes( typeof( OverrideTypeAttribute ), true ).FirstOrDefault() is OverrideTypeAttribute overriding )
				if ( !this._typesRegisteredAs.ContainsKey( overriding.Type ) ) {
					this._typesRegisteredAs.Add( type, overriding.Type );
					this.LogLine( $"Registered {type} as {overriding.Type}!", Log.Level.LOW );
				}
	}

	/// <returns>If the input type has a <see cref="OverrideTypeAttribute"/> associated with it, it will return the type in the attribute, otherwise the input type will be returned.</returns>
	public Type GetRegisteredAs( Type type ) {
		if ( this._typesRegisteredAs.TryGetValue( type, out Type? t ) )
			return t;
		return type;
	}
}