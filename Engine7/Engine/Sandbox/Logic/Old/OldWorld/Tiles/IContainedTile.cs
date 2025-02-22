namespace Sandbox.Logic.Old.OldWorld.Tiles;

public interface IContainedTile : ITile {
	ITile? ContainingTile { get; }
}
