namespace Sandbox.Logic.World.Tiles;

public interface IContainingTile : ITile {
	IReadOnlyList<ITile> SubTiles { get; }
}
