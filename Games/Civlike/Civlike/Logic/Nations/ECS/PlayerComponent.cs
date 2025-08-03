using Engine;
using Engine.Module.Entities.Container;
using Engine.Structures;

namespace Civlike.Logic.Nations.ECS;

public sealed class PlayerComponent : ComponentBase {

	private readonly BitSet _discoveredFaces;
	private readonly BitSet _revealedFaces;

	public PlayerComponent() {
		this.Name = $"Player {Random.Shared.Next():X8}";
		this.MapColor = (1, 1, 1, 1);
		this._discoveredFaces = new( 0 );
		this._revealedFaces = new( 0 );
	}

	public string Name { get; private set; } = string.Empty;
	public Vector4<float> MapColor { get; private set; }
	public ReadOnlyBitSet DiscoveredFaces => this._discoveredFaces.ReadOnly;
	public ReadOnlyBitSet RevealedFaces => this._revealedFaces.ReadOnly;

	public void SetName( string name ) {
		if (this.Name == name)
			return;
		this.Name = name;
		InvokeComponentChanged();
	}

	public void SetColor( Vector4<float> color ) {
		if (this.MapColor == color)
			return;
		this.MapColor = color;
		InvokeComponentChanged();
	}

	public void DiscoverFace( uint faceIndex ) {
		if (this._discoveredFaces[ faceIndex ])
			return;
		this._discoveredFaces[ faceIndex ] = true;
		InvokeComponentChanged();
	}

	public void RevealFace( uint faceIndex ) {
		if (this._revealedFaces[ faceIndex ])
			return;
		this._revealedFaces[ faceIndex ] = true;
		InvokeComponentChanged();
	}

	public void HideFace( uint faceIndex ) {
		if (!this._revealedFaces[ faceIndex ])
			return;
		this._revealedFaces[ faceIndex ] = false;
		InvokeComponentChanged();
	}

	internal void SetDiscoveredFaces( ReadOnlySpan<byte> discoveredFacesBits ) {
		this._discoveredFaces.SetRange( discoveredFacesBits, 0 );
		InvokeComponentChanged();
	}

	internal void SetRevealedFaces( ReadOnlySpan<byte> revealedFacesBits ) {
		this._revealedFaces.SetRange( revealedFacesBits, 0 );
		InvokeComponentChanged();
	}
}