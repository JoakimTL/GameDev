using Civlike.World;
using Engine.Module.Entities.Container;

namespace Civlike.Logic.Nations.ECS;

public sealed class FaceOwnershipComponent : ComponentBase {

	private readonly HashSet<Tile> _ownedTiles;

	public Guid GlobeId { get; set; }
	public IReadOnlyCollection<Tile> OwnedTiles => this._ownedTiles;

	public FaceOwnershipComponent() {
		this._ownedTiles = [];
	}

	internal void ClearOwnership() {
		this._ownedTiles.Clear();
	}

	public void AddFace( Tile tile ) {
		ArgumentNullException.ThrowIfNull( tile );
		if (this._ownedTiles.Contains( tile ))
			throw new InvalidOperationException( "Tile already owned." );
		this._ownedTiles.Add( tile );
		InvokeComponentChanged();
	}

	public void RemoveFace( Tile tile ) {
		ArgumentNullException.ThrowIfNull( tile );
		if (!this._ownedTiles.Contains( tile ))
			throw new InvalidOperationException( "Tile not owned." );
		this._ownedTiles.Remove( tile );
		InvokeComponentChanged();
	}
}
