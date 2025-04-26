using Engine;
using System.Collections.Frozen;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

public static class TerrainTypeList {

	private static readonly FrozenDictionary<Type, TerrainTypeBase> _terrainTypesByType;
	private static readonly FrozenDictionary<uint, TerrainTypeBase> _terrainTypesById;

	static TerrainTypeList() {
		IEnumerable<ResolvedType> resolvedTypes = TypeManager.Registry.ImplementationTypes.Where( p => p.IsAssignableTo( typeof( TerrainTypeBase ) ) ).Select( p => p.Resolve() );
		Dictionary<Type, TerrainTypeBase> terrainTypesByType = [];
		Dictionary<uint, TerrainTypeBase> terrainTypesById = [];

		foreach (ResolvedType? type in resolvedTypes) {
			if (!type.HasParameterlessConstructor)
				throw new InvalidOperationException( $"Terrain type {type} does not have a parameterless constructor." );
			TerrainTypeBase terrainType = type.CreateInstance( null ) as TerrainTypeBase ?? throw new InvalidOperationException( $"Terrain type {type} is not a valid TerrainTypeBase." );
			if (terrainTypesById.ContainsKey( terrainType.Id ))
				throw new InvalidOperationException( $"Terrain type {terrainType.Id} is already registered." );
			terrainTypesById[ terrainType.Id ] = terrainType;
			terrainTypesByType[ type.Type ] = terrainType;
		}

		_terrainTypesById = terrainTypesById.ToFrozenDictionary();
		_terrainTypesByType = terrainTypesByType.ToFrozenDictionary();
	}

	public static TerrainTypeBase GetTerrainType( uint id ) {
		if (_terrainTypesById.TryGetValue( id, out TerrainTypeBase? terrainType ))
			return terrainType;
		throw new KeyNotFoundException( $"Terrain type with ID {id} not found." );
	}

	public static T GetTerrainType<T>() where T : TerrainTypeBase, new() => (T) _terrainTypesByType[ typeof( T ) ];
}
