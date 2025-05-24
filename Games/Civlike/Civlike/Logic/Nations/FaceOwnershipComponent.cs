using Civlike.World.GameplayState;
using Engine.Module.Entities.Container;

namespace Civlike.Logic.Nations;

public sealed class FaceOwnershipComponent : ComponentBase {

	private readonly HashSet<Face> _ownedTiles;

	public IReadOnlyCollection<Face> OwnedFaces => this._ownedTiles;

	public FaceOwnershipComponent() {
		this._ownedTiles = [];
	}

	internal void ClearOwnership() {
		this._ownedTiles.Clear();
	}

	public void AddFace( Face face ) {
		ArgumentNullException.ThrowIfNull( face );
		if (this._ownedTiles.Contains( face ))
			throw new InvalidOperationException( "Tile already owned." );
		this._ownedTiles.Add( face );
		InvokeComponentChanged();
	}

	public void RemoveFace( Face face ) {
		ArgumentNullException.ThrowIfNull( face );
		if (!this._ownedTiles.Contains( face ))
			throw new InvalidOperationException( "Tile not owned." );
		this._ownedTiles.Remove( face );
		InvokeComponentChanged();
	}
}
