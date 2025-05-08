using Civs.World;
using Engine;
using Engine.Module.Entities.Container;
using Engine.Structures;
using System.Collections;

namespace Civs.Logic.Nations;

public sealed class PlayerComponent : ComponentBase {

	private readonly BitSet _discoveredFaces;
	private readonly BitSet _revealedFaces;

	public PlayerComponent() {
		Name = $"Player {Random.Shared.Next():X8}";
		MapColor = (1, 1, 1, 1);
		_discoveredFaces = new( 0 );
		_revealedFaces = new( 0 );
	}

	public string Name { get; private set; } = string.Empty;
	public Vector4<float> MapColor { get; private set; }
	public ReadOnlyBitSet DiscoveredFaces => _discoveredFaces.ReadOnly;
	public ReadOnlyBitSet RevealedFaces => _revealedFaces.ReadOnly;

	public void SetName( string name ) {
		if (Name == name)
			return;
		Name = name;
		this.InvokeComponentChanged();
	}

	public void SetColor( Vector4<float> color ) {
		if (MapColor == color)
			return;
		MapColor = color;
		this.InvokeComponentChanged();
	}

	public void DiscoverFace( uint faceIndex ) {
		if (_discoveredFaces[ faceIndex ])
			return;
		_discoveredFaces[ faceIndex ] = true;
		this.InvokeComponentChanged();
	}

	public void RevealFace( uint faceIndex ) {
		if (_revealedFaces[ faceIndex ])
			return;
		_revealedFaces[ faceIndex ] = true;
		this.InvokeComponentChanged();
	}

	public void HideFace( uint faceIndex ) {
		if (!_revealedFaces[ faceIndex ])
			return;
		_revealedFaces[ faceIndex ] = false;
		this.InvokeComponentChanged();
	}

	internal void SetDiscoveredFaces( ReadOnlySpan<byte> discoveredFacesBits ) {
		_discoveredFaces.SetRange( discoveredFacesBits, 0 );
		this.InvokeComponentChanged();
	}

	internal void SetRevealedFaces( ReadOnlySpan<byte> revealedFacesBits ) {
		_revealedFaces.SetRange( revealedFacesBits, 0 );
		this.InvokeComponentChanged();
	}
}