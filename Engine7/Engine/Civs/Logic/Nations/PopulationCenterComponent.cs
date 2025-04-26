using Civs.World;
using Engine;
using Engine.Module.Entities.Container;

namespace Civs.Logic.Nations;
public sealed class PopulationCenterComponent : ComponentBase {

}

//public sealed class TileOwnershipComponent : ComponentBase {

//	private readonly HashSet<Tile> _ownedTiles;

//	public IReadOnlyCollection<Tile> OwnedTiles => _ownedTiles;

//	public Vector4<float> Color { get; set; }

//	public event Action? TileOwnershipChanged;

//	public TileOwnershipComponent() {
//		_ownedTiles = [];
//		Color = (1, 1, 1, 1);
//	}

//	public void AddTile( Tile tile ) {
//		ArgumentNullException.ThrowIfNull( tile );
//		if (_ownedTiles.Contains( tile ))
//			throw new InvalidOperationException( "Tile already owned." );
//		_ownedTiles.Add( tile );
//		TileOwnershipChanged?.Invoke();
//	}

//	public void RemoveTile( Tile tile ) {
//		ArgumentNullException.ThrowIfNull( tile );
//		if (!_ownedTiles.Contains( tile ))
//			throw new InvalidOperationException( "Tile not owned." );
//		_ownedTiles.Remove( tile );
//		TileOwnershipChanged?.Invoke();
//	}
//}

public sealed class TileOwnershipRenderComponent : ComponentBase {

	public Vector4<float> Color { get; private set; }

	public event Action? ColorChanged;

	public TileOwnershipRenderComponent() {
		Color = (1, 1, 1, 1);
	}

	public void SetColor( Vector4<float> color ) {
		if (Color == color)
			return;
		Color = color;
		ColorChanged?.Invoke();
	}
}
