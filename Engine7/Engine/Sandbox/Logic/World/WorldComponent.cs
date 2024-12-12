using Engine;
using Engine.Module.Entities.Container;
using Engine.Standard.Render.Meshing;

namespace Sandbox.Logic.World;
public sealed class WorldComponent : ComponentBase {
	public double SimulatedSurfaceArea { get; set; } = 0;
}

public sealed class WorldTilingComponent : ComponentBase {
	public WorldTiling Tiling { get; } = new();
}

public sealed class WorldTiling {

	private readonly IReadOnlyList<Tile>[] _tilesPerSubdivision;
	public IReadOnlyList<Vector3<double>> TileVectors { get; }
	public IReadOnlyList<IReadOnlyList<Tile>> Tiles => _tilesPerSubdivision;

	public WorldTiling() {
		IcosphereGenerator.GenerateIcosphereVectors( 4, out List<Vector3<double>>? vectors, out List<List<uint>>? indicesPerSubdivision );
		TileVectors = vectors;
		_tilesPerSubdivision = new IReadOnlyList<Tile>[ indicesPerSubdivision.Count ];
		for (int subdivisionIndex = 0; subdivisionIndex < indicesPerSubdivision.Count; subdivisionIndex++) {
			List<uint> subdivision = indicesPerSubdivision[ subdivisionIndex ];
			List<Tile> tiles = [];
			for (int i = 0; i < subdivision.Count; i += 3) {
				tiles.Add( new( subdivision[ i ], subdivision[ i + 1 ], subdivision[ i + 2 ] ) );
			}
			_tilesPerSubdivision[ subdivisionIndex ] = tiles;
		}
	}

	public IReadOnlyList<Tile> GetTilesForSubdivision(int subdivision) {
		return _tilesPerSubdivision[ subdivision ];
	}

}

public sealed class Tile( uint indexA, uint indexB, uint indexC ) {
	public uint IndexA { get; } = indexA;
	public uint IndexB { get; } = indexB;
	public uint IndexC { get; } = indexC;
}

public sealed class WorldArchetype : ArchetypeBase {
	public WorldComponent WorldComponent { get; set; } = null!;
	public WorldTilingComponent WorldTilingComponent { get; set; } = null!;
}

public sealed class WorldSystem : SystemBase<WorldArchetype> {
	protected override void ProcessEntity( WorldArchetype archetype, double time, double deltaTime ) {

	}
}