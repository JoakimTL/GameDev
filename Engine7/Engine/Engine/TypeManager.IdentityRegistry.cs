using System.Collections.Frozen;

namespace Engine;

public sealed class IdentityRegistry {
	private readonly FrozenSet<Type> _typesWithIdentity;
	private readonly FrozenDictionary<string, ResolvedType> _typesByIdentity;
	public IdentityRegistry( TypeRegistry registry ) {
		Dictionary<string, ResolvedType> typesByIdentity = [];
		foreach (ResolvedType resolvedType in registry.AllTypes.WithAttribute<IdentityAttribute>().Select( TypeManager.ResolveType )) {
			IdentityAttribute identityAttribute = resolvedType.GetAttribute<IdentityAttribute>();
			if (typesByIdentity.TryGetValue( identityAttribute.Identity, out ResolvedType? occupyingType ))
				throw new InvalidOperationException( $"Identity collision for \"{identityAttribute.Identity}\". Collision between {occupyingType.Type.FullName} and {resolvedType.Type.FullName}" );
			typesByIdentity.Add( resolvedType.GetAttribute<IdentityAttribute>().Identity, resolvedType );
		}
		this._typesByIdentity = FrozenDictionary.ToFrozenDictionary( typesByIdentity );
		this._typesWithIdentity = this._typesByIdentity.Values.Select( p => p.Type ).ToFrozenSet();
	}

	public IReadOnlySet<Type> TypesWithIdentity => this._typesWithIdentity;
	public IReadOnlyDictionary<string, ResolvedType> TypesByIdentity => this._typesByIdentity;
}
