namespace Sandbox.Logic.World.Tiles;

public interface IContainedTile : ITile {
	ITile? ContainingTile { get; }
}
