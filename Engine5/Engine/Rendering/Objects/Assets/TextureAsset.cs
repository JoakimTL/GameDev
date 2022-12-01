namespace Engine.Rendering.Objects.Assets;
public sealed class TextureAsset : LoadedAssetBase {

	public ushort Index { get; private set; }
	private Texture? _texture;

	internal TextureAsset( string path ) : base( path ) {
		this.Index = 0;
		this._texture = null;
	}

	internal void SetIndex( ushort index ) {
		Index = index;
	}

	internal void SetTexture( Texture newTexture ) {
		_texture = newTexture;
	}

	protected override void OnDispose() {
		_texture?.Dispose();
		_texture = null;
	}

	public ulong GetHandle() {
		if ( _texture is null )
			return 0;
		return _texture.GetHandle();
	}
}
