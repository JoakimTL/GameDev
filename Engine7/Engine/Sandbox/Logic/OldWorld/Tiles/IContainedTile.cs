namespace Sandbox.Logic.OldWorld.Tiles;

public interface IContainedTile : ITile {
	ITile? ContainingTile { get; }
}
