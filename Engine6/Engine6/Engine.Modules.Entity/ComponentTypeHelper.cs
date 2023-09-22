using System.Collections.Immutable;
using System.Reflection;

namespace Engine.Modules.Entity;

internal static class ComponentTypeHelper {

	private static readonly ImmutableDictionary<Type, Guid> _componentTypeIdsByType;
	private static readonly ImmutableDictionary<Guid, Type> _componentTypeById;

	static ComponentTypeHelper() {
		(Type Type, IdentifierAttribute? Identifier)[] componentBaseTypes = TypeHelper.AllTypes.Where( x => x.IsSubclassOf( typeof( SerializableComponentBase ) ) ).OrderBy( p => p.FullName ).Select( p => (Type: p, Identifier: p.GetCustomAttribute<IdentifierAttribute>()) ).ToArray();
		(Type Type, IdentifierAttribute? Identifier)[] validComponentBaseTypes = componentBaseTypes.Where( p => p.Identifier is not null ).ToArray();
		_componentTypeIdsByType = validComponentBaseTypes.ToImmutableDictionary( p => p.Type, p => p.Identifier!.Identifier );
		_componentTypeById = validComponentBaseTypes.ToImmutableDictionary( p => p.Identifier!.Identifier, p => p.Type );

		foreach ( (Type Type, IdentifierAttribute? Identifier) componentMissingGuid in componentBaseTypes.Where( p => p.Identifier is null ) )
			Log.Warning( $"Component {componentMissingGuid.Type.Name} in {componentMissingGuid.Type.Namespace} is missing an IdentifierAttribute. This will cause issues with serialization." );
	}

	public static Type? GetComponentType( Guid id ) => _componentTypeById.TryGetValue( id, out Type? type ) ? type : null;

	public static Guid? GetComponentTypeId( Type type ) => _componentTypeIdsByType.TryGetValue( type, out Guid id ) ? id : null;
}
