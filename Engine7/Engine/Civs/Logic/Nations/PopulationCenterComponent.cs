using Civs.World;
using Civs.World.NewWorld;
using Engine;
using Engine.Module.Entities.Container;

namespace Civs.Logic.Nations;
public sealed class PopulationCenterComponent : ComponentBase {

}

public sealed class FaceOwnershipComponent : ComponentBase {

	private readonly HashSet<Face> _ownedTiles;

	public IReadOnlyCollection<Face> OwnedFaces => _ownedTiles;

	public FaceOwnershipComponent() {
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

public sealed class FaceOwnershipRenderComponent : ComponentBase {

	public Vector4<float> Color { get; private set; }

	public FaceOwnershipRenderComponent() {
		Color = (1, 1, 1, 1);
	}

	public void SetColor( Vector4<float> color ) {
		if (Color == color)
			return;
		Color = color;
		this.InvokeComponentChanged();
	}
}
