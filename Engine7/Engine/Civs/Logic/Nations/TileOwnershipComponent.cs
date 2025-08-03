using Civs.World;
using Engine.Module.Entities.Container;

namespace Civs.Logic.Nations;

public sealed class TileOwnershipComponent : ComponentBase {

	private readonly HashSet<Face> _ownedTiles;

	public IReadOnlyCollection<Face> OwnedFaces => _ownedTiles;

	public TileOwnershipComponent() {
		_ownedTiles = [];
	}

	internal void ClearOwnership() {
		_ownedTiles.Clear();
	}

	public void AddFace( Face face ) {
		ArgumentNullException.ThrowIfNull( face );
		if (_ownedTiles.Contains( face ))
			throw new InvalidOperationException( "Tile already owned." );
		_ownedTiles.Add( face );
		this.InvokeComponentChanged();
	}

	public void RemoveFace( Face face ) {
		ArgumentNullException.ThrowIfNull( face );
		if (!_ownedTiles.Contains( face ))
			throw new InvalidOperationException( "Tile not owned." );
		_ownedTiles.Remove( face );
		this.InvokeComponentChanged();
	}
}
