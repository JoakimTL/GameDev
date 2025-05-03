using Civs.World.NewWorld;
using Engine;
using Engine.Module.Entities.Container;
using System.Drawing;

namespace Civs.Logic.Nations;
public sealed class PopulationCenterComponent : ComponentBase {

	public string Name { get; private set; }

	public PopulationCenterComponent() {
		Name = $"Pop {Random.Shared.Next():X4}";
	}

	public void SetName( string name ) {
		if (string.IsNullOrWhiteSpace( name ))
			throw new ArgumentException( "Name cannot be null or whitespace.", nameof( name ) );
		if (name == Name)
			return;
		Name = name;
		this.InvokeComponentChanged();
	}

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

public sealed class PlayerComponent : ComponentBase {

	public Vector4<float> MapColor { get; private set; }

	public PlayerComponent() {
		MapColor = (1, 1, 1, 1);
	}

	public void SetColor( Vector4<float> color ) {
		if (MapColor == color)
			return;
		MapColor = color;
		this.InvokeComponentChanged();
	}
}