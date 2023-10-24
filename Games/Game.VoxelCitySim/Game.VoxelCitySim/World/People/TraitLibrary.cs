using Engine;
using System.Collections.Frozen;
using System.Reflection;

namespace Game.VoxelCitySim.World.People;

public static class TraitLibrary {

	private static readonly FrozenDictionary<Guid, Type> _traitTypeByTraitCode;
	private static readonly FrozenDictionary<Type, Guid> _traitCodeByTraitType;

	static TraitLibrary() {
		var traitTypes = TypeHelper.AllTypes
			.Where( p => p.IsAssignableTo( typeof( TraitBase ) ) && !p.IsAbstract )
			.OrderBy( p => p.FullName )
			.Select( p => (p, p.GetCustomAttribute<IdentifierAttribute>()) )
			.Where( p => p.Item2 is not null )
			.Select( p => (Type: p.p, Identifier: p.Item2?.Identifier ?? Guid.Empty) )
			.ToList();
		_traitTypeByTraitCode = traitTypes.ToFrozenDictionary( p => p.Identifier, p => p.Type );
		_traitCodeByTraitType = traitTypes.ToFrozenDictionary( p => p.Type, p => p.Identifier );
	}

	public static Guid? GetTraitGuid<T>() where T : TraitBase => GetTraitGuid( typeof( T ) );
	public static Guid? GetTraitGuid( Type traitType ) => _traitCodeByTraitType.TryGetValue( traitType, out Guid identifier ) ? identifier : null;
	public static Type GetTraitType( Guid traitGuid ) => _traitTypeByTraitCode[ traitGuid ];

}
