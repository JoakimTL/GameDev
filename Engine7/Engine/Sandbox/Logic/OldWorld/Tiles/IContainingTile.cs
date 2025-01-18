namespace Sandbox.Logic.OldWorld.Tiles;

public interface IContainingTile : ITile {
	IReadOnlyList<ITile> SubTiles { get; }
}
