using Engine.Time;

namespace Engine.Rendering.Objects.Assets;
public class TextureAsset {

	public string Path { get; }
	public ushort Index { get; private set; }
	private Texture? _texture;

	//Needs a path, the path will also be considered the asset name
	//This path is the local path, in the asset/textures directory.

	public float DeadlineTillDisposal { get; internal set; }
	public bool ShouldDispose => Clock32.StartupTime > DeadlineTillDisposal;

	public event Action<TextureAsset>? OnDispose;

	internal TextureAsset( string path ) {
		this.Index = 0;
		this.Path = path;
		this._texture = null;
	}

	internal void SetIndex( ushort index ) {
		Index = index;
	}

	internal void SetTexture( Texture newTexture ) {
		_texture = newTexture;
	}

	internal void Dispose() {
		OnDispose?.Invoke( this );
		_texture?.Dispose();
		_texture = null;
	}

	public ulong GetHandle() {
		if ( _texture is null )
			return 0;
		return _texture.GetHandle();
	}
}
