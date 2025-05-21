using Civlike.World;

namespace Civlike.World.TerrainTypes;

public sealed class OceanTerrain() : TerrainTypeBase( 0, "Ocean", (0.07F, 0.19F, 0.64F, 1), false, isLand: false, claimable: false ) { }

public sealed class GrasslandTerrain() : TerrainTypeBase( 1, "Grasslands", (0.47F, 0.99F, 0.17F, 1), true ) { }
public sealed class MountainTerrain() : TerrainTypeBase( 2, "Mountains", (0.5F, 0.5F, 0.5F, 1), true ) { }
public sealed class ShorelineTerrain() : TerrainTypeBase( 3, "Shore", (0.12F, 0.46F, 0.94F, 1), false, isLand: false ) { }
//Ice

public sealed class FrozenDesertTerrain() : TerrainTypeBase( 4, "Frozen Desert", (0.73f, 0.73F, 1, 1), true, isLand: true ) { }
public sealed class WarmDesertTerrain() : TerrainTypeBase( 5, "Warm Desert", (0.83f, 0.63F, 0.12f, 1), true, isLand: true ) { }

public sealed class RainforestTerrain() : TerrainTypeBase( 6, "Rainforest", (0.12f, 0.73F, 0.02f, 1), true, isLand: true ) { }
public sealed class ForestTerrain() : TerrainTypeBase( 7, "Forest", (0.12f, 0.94F, 0.12f, 1), true, isLand: true ) { }

public sealed class BoglandTerrain() : TerrainTypeBase( 8, "Bogland", (0.36f, 0.25f, 0.2f, 1), true, isLand: true ) { }