using Civs.World;
using Engine;
using Engine.Module.Entities.Container;

namespace Civs.Logic.Nations;
public sealed class PopulationCenterComponent : ComponentBase {

	private readonly List<Tile> _ownedTiles;

	public IReadOnlyList<Tile> OwnedTiles => _ownedTiles;

	public Vector4<float> Color { get; set; }

	public event Action? TileOwnershipChanged;

	public PopulationCenterComponent() {
		_ownedTiles = [];
		Color = (1, 1, 1, 1);
	}

	public void AddTile( Tile tile ) {
		if (tile == null)
			throw new ArgumentNullException( nameof( tile ) );
		if (_ownedTiles.Contains( tile ))
			throw new InvalidOperationException( "Tile already owned." );
		_ownedTiles.Add( tile );
		TileOwnershipChanged?.Invoke();
	}

	public void RemoveTile( Tile tile ) {
		if (tile == null)
			throw new ArgumentNullException( nameof( tile ) );
		if (!_ownedTiles.Contains( tile ))
			throw new InvalidOperationException( "Tile not owned." );
		_ownedTiles.Remove( tile );
		TileOwnershipChanged?.Invoke();
	}

}
