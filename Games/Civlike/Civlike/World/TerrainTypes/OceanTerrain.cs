using Civlike.World;

namespace Civlike.World.TerrainTypes;

public sealed class OceanTerrain() : TerrainTypeBase( 0, "Ocean", (0.07F, 0.19F, 0.64F, 1), false, isLand: false, claimable: false ) { }

public sealed class ShorelineTerrain() : TerrainTypeBase( 3, "Shore", (0.12F, 0.46F, 0.94F, 1), false, isLand: false ) { }