namespace OldGen.World.TerrainTypes;

public sealed class OceanTerrain() : TerrainTypeBase( 0, "Ocean", (0.07F, 0.19F, 0.64F, 1), false, isLand: false, claimable: false ) { }
public sealed class GrasslandTerrain() : TerrainTypeBase( 1, "Grasslands", (0.47F, 0.99F, 0.17F, 1), true ) { }
public sealed class MountainTerrain() : TerrainTypeBase( 2, "Mountains", (0.5F, 0.5F, 0.5F, 1), true ) { }
public sealed class ShorelineTerrain() : TerrainTypeBase( 3, "Shore", (0.12F, 0.46F, 0.94F, 1), false, isLand: false ) { }
public sealed class FrozenDesertTerrain() : TerrainTypeBase( 4, "Frozen Desert", (0.73f, 0.73F, 1, 1), true, isLand: true ) { }
public sealed class WarmDesertTerrain() : TerrainTypeBase( 5, "Warm Desert", (0.83f, 0.63F, 0.12f, 1), true, isLand: true ) { }
public sealed class RainforestTerrain() : TerrainTypeBase( 6, "Rainforest", (0.12f, 0.73F, 0.02f, 1), true, isLand: true ) { }
public sealed class ForestTerrain() : TerrainTypeBase( 7, "Forest", (0.12f, 0.94F, 0.12f, 1), true, isLand: true ) { }
public sealed class BoglandTerrain() : TerrainTypeBase( 8, "Bogland", (0.36f, 0.25f, 0.2f, 1), true, isLand: true ) { }

/*
🌳 Land Biomes

    Plains
    − Flat, open grasslands. Low movement cost, moderate food yield, easy to build on.

    Grassland
    − Slightly wilder than plains, with tall grasses. Good food yield, moderate defense bonus.

    Forest
    − Dense trees. Slows movement, provides wood resources, offers defensive cover.

    Jungle
    − Thick tropical foliage. High resource (food/wood) yield, very high movement cost, chance of disease/ambush events.

    Savanna
    − Grassland with scattered trees. Moderate food and wood, occasional wildlife spawns.

    Steppe
    − Semi-arid grassland. Lower food than plains but faster movement. Good for cavalry.

    Taiga
    − Boreal forest. Provides wood, slows movement, reduced food yield in winter.

    Tundra
    − Cold, treeless plains. Minimal food, low movement penalty, can support grazing units.

    Desert
    − Sandy dunes or rocky badlands. Very low food, high heat (morale penalty), occasional oasis “resource” nodes.

    Snow/Ice
    − Frozen ground. Very low yields, high movement cost, affects unit visibility.

💧 Water & Coastal

    Coast
    − Shallow water adjacent to land. Allows fishing (food), beach landings, moderate ship movement.

    Ocean
    − Deep water. Supports naval units, high movement but requires ports to access.

    River
    − Freshwater channel. Creates natural chokepoints, reduces movement crossing, provides fresh water bonus to adjacent tiles.

    Lake
    − Inland water body. Fishing, naval skirmishes, potential for hydro-power structures.

    Marsh/Swamp
    − Waterlogged land. Very high movement penalty, chance of disease/debuffs, unique “herbal” resources.

⛰️ Elevation & Rugged

    Hills
    − Rolling terrain. Slight defense bonus, increased visibility, low extra movement cost.

    Mountains
    − Impassable or very high movement cost. Blocking terrain, potential mineral/resource outcrops (e.g., ore, gems), provides high-ground vision.

    Valley/Canyon
    − Narrow passes through mountains. Natural chokepoints, can host special “gorge” resources or hidden camps.

🔥 Special / Event-Driven

    Volcanic
    − Active volcano tiles. Periodically erupt (area damage), yields lava fields (impassable) but rich minerals nearby.

    Oasis
    − Desert-only fresh-water spot. High food/water yield, strategic control point.

    Ruins/Ancient Site
    − Scattered across multiple biomes. Provide one-time bonuses (relics, research boost) when explored.*/