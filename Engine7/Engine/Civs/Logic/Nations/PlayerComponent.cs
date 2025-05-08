using Civs.World;
using Engine;
using Engine.Module.Entities.Container;

namespace Civs.Logic.Nations;

public sealed class PlayerComponent : ComponentBase {

	private readonly HashSet<uint> _discoveredFaces;
	private readonly HashSet<uint> _revealedFaces;

	public PlayerComponent() {
		Name = $"Player {Random.Shared.Next():X8}";
		MapColor = (1, 1, 1, 1);
		_discoveredFaces = [];
		_revealedFaces = [];
	}

	public string Name { get; private set; } = string.Empty;
	public Vector4<float> MapColor { get; private set; }
	public IReadOnlySet<uint> DiscoveredFaces => _discoveredFaces;
	public IReadOnlySet<uint> RevealedFaces => _revealedFaces;

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
		if (!_revealedFaces.Add( faceIndex ))
			return;
		this.InvokeComponentChanged();
	}

	public void RevealFace( uint faceIndex ) {
		if (!_discoveredFaces.Add( faceIndex ))
			return;
		this.InvokeComponentChanged();
	}

	public void HideFace( uint faceIndex ) {
		if (!_revealedFaces.Remove( faceIndex ))
			return;
		this.InvokeComponentChanged();
	}

	internal void SetDiscoveredFaces( ReadOnlySpan<uint> discoveredFaces ) {
		_discoveredFaces.Clear();
		foreach (uint face in discoveredFaces) {
			if (!_discoveredFaces.Add( face ))
				continue;
		}
		this.InvokeComponentChanged();
	}

	internal void SetRevealedFaces( ReadOnlySpan<uint> revealedFaces ) {
		_revealedFaces.Clear();
		foreach (uint face in revealedFaces) {
			if (!_revealedFaces.Add( face ))
				continue;
		}
		this.InvokeComponentChanged();
	}
}