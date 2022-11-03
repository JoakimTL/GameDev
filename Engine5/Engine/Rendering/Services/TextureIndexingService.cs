using Engine.Rendering.Objects;
using Engine.Rendering.Objects.Assets;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using OpenGL;

namespace Engine.Rendering.Services;

[ProcessAfter<TextureAssetService>( typeof( IUpdateable ) )]
public sealed class TextureIndexingService : Identifiable, IContextService, IUpdateable {

	private ushort _lowestOpenIndex;
	private readonly TextureAsset?[] _assets;
	private bool _updated;
	private ShaderStorageBufferObject _ssbo;

	public TextureIndexingService() {
		_lowestOpenIndex = 0;
		_assets = new TextureAsset?[ ushort.MaxValue + 1 ];
		_ssbo = new( "TextureAddresses", sizeof( ulong ) * ( ushort.MaxValue + 1 ), ShaderType.VertexShader, ShaderType.FragmentShader );
	}

	public void Update( float time, float deltaTime ) {
		if ( !_updated )
			return;
		ulong[] handles = new ulong[ _assets.Length ];
		for ( int i = 0; i < _assets.Length; i++ ) {
			ulong handle = _assets[ i ]?.GetHandle() ?? 0;
			if ( handle == 0 )
				handle = defaultMissingTexture;
			handles[ i ] = handle;
		}
		Memory<ulong> handleMemory = handles;
		unsafe {
			using var pin = handleMemory.Pin();
			_ssbo.Write( pin.Pointer, (uint) ( handles.Length * sizeof( ulong ) ) );
		}
	}

	internal void SetIndex( TextureAsset asset ) {
		if ( _assets[ _lowestOpenIndex ] is not null ) {
			this.LogWarning( "No more free indexes for textures!" );
			return;
		}
		asset.SetIndex( _lowestOpenIndex );
		_assets[ _lowestOpenIndex ] = asset;
		_updated = true;
		asset.OnDispose += AssetDisposed;
		_lowestOpenIndex = GetFirstOpenIndexFrom( (ushort) ( _lowestOpenIndex + 1 ) );
	}

	private void AssetDisposed( TextureAsset asset ) {
		_assets[ asset.Index ] = null;
		_updated = true;
		if ( asset.Index < _lowestOpenIndex )
			_lowestOpenIndex = asset.Index;
	}

	private ushort GetFirstOpenIndexFrom( ushort start ) {
		for ( ushort i = start; i < _assets.Length; i++ )
			if ( _assets[ i ] is null )
				return i;
		return ushort.MaxValue;
	}
}
