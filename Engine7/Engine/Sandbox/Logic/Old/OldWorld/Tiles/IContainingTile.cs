namespace Sandbox.Logic.Old.OldWorld.Tiles;

public interface IContainingTile : ITile {
	IReadOnlyList<ITile> SubTiles { get; }
}
