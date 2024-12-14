using Engine;
using Engine.Module.Entities.Container;
using Engine.Standard.Render.Meshing;
using System.Resources;

namespace Sandbox.Logic.World;
public sealed class WorldComponent : ComponentBase {
	public double SimulatedSurfaceArea { get; set; } = 0;
}

public sealed class WorldTilingComponent : ComponentBase {
	public WorldTiling Tiling { get; } = new();
}

public sealed class WorldTiling {

	private readonly Icosphere _worldIcosphere;
	public Icosphere WorldIcosphere => _worldIcosphere;

	public WorldTiling() {
		_worldIcosphere = new Icosphere( 9, normalizeUpTo: 5 );
	}



	//public IReadOnlyList<Tile> GetTilesForSubdivision(int subdivision) {
	//	return _tilesPerSubdivision[ subdivision ];
	//}

}

public sealed class BaseTile( uint baseVectorIndexA, uint baseVectorIndexB, uint baseVectorIndexC ) {
	public uint BaseVectorIndexA { get; } = baseVectorIndexA;
	public uint BaseVectorIndexB { get; } = baseVectorIndexB;
	public uint BaseVectorIndexC { get; } = baseVectorIndexC;

	//A basetile can be subdivided 4 times. The border always stays on the same edge.

}

//public sealed class TileSubdivision {

//	public IReadOnlyList<TileSubdivision>? Subdivisions { get; }
//	public IReadOnlyList<Tile>? Tiles { get; }

//	public TileSubdivision(uint level, Span<uint> indices) {
//		if (level > 1) {
//			Subdivisions = CreateSubdivision( level - 1 );
//		} else {
//			Tiles = CreateTiles();
//		}
//	}

//	private IReadOnlyList<TileSubdivision> CreateSubdivision( uint level ) {
//		var subdivisions = new List<TileSubdivision>();
//		for (uint i = 0; i < 4; i++) {
//			subdivisions.Add( new( level ) );
//		}
//		return subdivisions;
//	}

//	private IReadOnlyList<Tile> CreateTiles() {
//		 var tiles = new List<Tile>();
//	}
//}

public sealed class Tile() {
}

public sealed class WorldArchetype : ArchetypeBase {
	public WorldComponent WorldComponent { get; set; } = null!;
	public WorldTilingComponent WorldTilingComponent { get; set; } = null!;
}

public sealed class WorldSystem : SystemBase<WorldArchetype> {
	protected override void ProcessEntity( WorldArchetype archetype, double time, double deltaTime ) {

	}
}