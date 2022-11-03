using Engine.GlobalServices;
using Engine.Rendering.Objects;
using Engine.Rendering.Services;
using Engine.Structure.Interfaces;
using System.Collections.Concurrent;

namespace Engine.Rendering;
/// <summary>
/// Assets are files that are loaded by an OGL context
/// </summary>
public abstract class Asset : IDisposable {
	public string Path { get; }
	/// <summary>
	/// The time at which this asset will unload if no more references points to it.
	/// </summary>
	public float UnloadingTime { get; internal set; }

	public delegate void AssetChangeHandler( Asset asset );
	public event AssetChangeHandler? AssetChanged;

	protected Asset( string path ) {
		this.Path = path;
		Global.Get<FileWatchingService>().Track( path, OnFileChanged );
	}

	internal void ContextLoadAsset( Context context ) {
		OnContextLoad( context );
	}

	protected abstract void OnContextLoad( Context context );
	private void OnFileChanged( string path ) => AssetChanged?.Invoke( this );
	public abstract void Dispose();
}

public sealed class MaterialAsset : Asset, IBaseDirectoryProvider, IAssetFactory<MaterialAsset> {
	public static string BaseDirectory => "assets/materials";
	public ShaderBundleBase? ShaderBundle { get; private set; }
	public IReadOnlyDictionary<string, Texture>? Textures { get; private set; }

	private MaterialAsset( string path ) : base( path ) {
	}

	protected override void OnContextLoad( Context context ) {
		//File load
		//Parse usecase and shader pipeline name
		//Load shaderbundle
		string data = File.ReadAllText( Path );
		var bundleShaderData = data.Split( '\n' );
		Dictionary<string, Texture> textures = new();
		for ( int i = 0; i < bundleShaderData.Length; i++ ) {
			string line = bundleShaderData[ i ];
			if ( line.StartsWith( "shader" ) )
				ShaderBundle = context.Service<ShaderBundleService>().Get( line[ "shader:".Length.. ].Trim() );
			if ( line.StartsWith( "tex" ) ) {
				var lineSplit = line[ "tex:".Length.. ].Split( ':' );
				if ( lineSplit.Length != 2 )
					continue;
				textures.Add( lineSplit[ 0 ].Trim(), context.Service<TextureService>().Get( lineSplit[ 1 ].Trim() ) );
			}
		}
		Textures = textures;
	}

	public override void Dispose() { }

	public static MaterialAsset? CreateAsset( string relativePath ) {
		string path = $"{BaseDirectory}/{relativePath}";
		if ( !File.Exists( path ) )
			return null;
		return new MaterialAsset( path );
	}
}

public interface IBaseDirectoryProvider {
	static abstract string BaseDirectory { get; }
}
public interface IAssetFactory<TSelf> where TSelf : Asset {
	static abstract TSelf? CreateAsset( string path );
}

public static class AssetProviderExtensions {
	public static T? GetAsset<T>( this object caller, string assetName ) where T : Asset, IAssetFactory<T> {
		return Global.Get<AssetProviderService>().Get<T>( caller, assetName );
	}
}

public sealed class ContextAssetLoadingService : IContextService, IUpdateable {
	private readonly Context _context;
	private readonly ConcurrentQueue<Asset> _loadedAssets;
	private readonly ConcurrentQueue<Asset> _unloadedAssets;

	public ContextAssetLoadingService( Context context ) {
		this._context = context;
		_loadedAssets = new();
		_unloadedAssets = new();
		Global.Get<AssetProviderService>().AssetLoaded += OnAssetLoaded;
		Global.Get<AssetProviderService>().AssetUnloaded += OnAssetUnloaded;
	}

	public void Update( float time, float deltaTime ) {
		while ( _loadedAssets.TryDequeue( out Asset? loadedAsset ) )
			loadedAsset.ContextLoadAsset( _context );
		while ( _unloadedAssets.TryDequeue( out Asset? unloadedAsset ) )
			unloadedAsset.Dispose();
	}

	private void OnAssetLoaded( Asset asset ) => _loadedAssets.Enqueue( asset );

	private void OnAssetUnloaded( Asset asset ) => _unloadedAssets.Enqueue( asset );
}