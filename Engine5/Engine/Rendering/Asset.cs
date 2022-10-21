using Engine.GlobalServices;
using Engine.Rendering.Objects;
using Engine.Structure.Interfaces;
using Engine.Time;
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
	private void OnFileChanged() => AssetChanged?.Invoke( this );
	public abstract void Dispose();
}

public sealed class ShaderAsset : Asset, IBaseDirectoryProvider {
	public static string BaseDirectory => "assets/shaders";
	public ShaderBundle? ShaderBundle { get; private set; }

	public ShaderAsset( string relativePath ) : base( $"{BaseDirectory}/{relativePath}" ) {
	}

	protected override void OnContextLoad( Context context ) {
		//File load
		//Parse usecase and shader pipeline name
		//Load shaderbundle
		context.Service<ShaderBundleService>().Get()
	}

	public override void Dispose() { }
}

public interface IBaseDirectoryProvider {
	static abstract string BaseDirectory { get; }
}
public interface IAssetFactory<T> where T : Asset {
	static abstract T? CreateAsset( string path );
}

public sealed class AssetProviderService : Identifiable, IGlobalService {

	public delegate void AssetEvent( Asset asset );

	public event AssetEvent? AssetLoaded;
	public event AssetEvent? AssetUnloaded;

	private readonly ConcurrentDictionary<string, Asset> _assets;
	private readonly ConcurrentDictionary<Asset, ConcurrentBag<WeakReference>> _references;
	private readonly Queue<Asset> _unloadingQueue;
	private readonly TickingTimer _unloadingTimer;

	public AssetProviderService() {
		_assets = new();
		_references = new();
		_unloadingQueue = new();
		_unloadingTimer = new();
		_unloadingTimer.Elapsed += CheckUnloaded;
		_unloadingTimer.Start();
	}

	private T? LoadAsset<T>( string assetPath ) where T : Asset, IAssetFactory<T> {
		T? asset = T.CreateAsset( assetPath );
		if ( asset is null )
			return null;
		if ( !_assets.TryAdd( assetPath, asset ) ) {
			if ( !_assets.TryGetValue( assetPath, out Asset? assetBase ) )
				throw new InvalidOperationException( "Asset loading error." );
			if ( assetBase is T assetT )
				return assetT;
			this.LogWarning( $"{assetPath} could not be loaded as {typeof( T ).Name}!" );
			return null;
		}
		AssetLoaded?.Invoke( asset );
		return asset;
	}

	public T? Get<T>( object caller, string assetPath ) where T : Asset, IAssetFactory<T> {
		{
			if ( _assets.TryGetValue( assetPath, out var asset ) ) {
				if ( asset is T t ) {
					RegisterReference( asset, caller );
					return t;
				}
				this.LogWarning( $"{assetPath} could not be loaded as {typeof( T ).Name}!" );
				return null;
			}
		}

		{
			var asset = LoadAsset<T>( assetPath );
			if ( asset is null )
				return null;
			RegisterReference( asset, caller );
			return asset;
		}

	}

	private void RegisterReference( Asset asset, object caller ) {
		if ( !_references.TryGetValue( asset, out var refs ) )
			_references.TryAdd( asset, refs = new() );
		refs.Add( new( caller ) );
	}

	private void CheckUnloaded( double time, double deltaTime ) {
		foreach ( var kvp in _references )
			if ( !kvp.Value.Any( p => p.IsAlive ) ) {
				if ( kvp.Key.UnloadingTime == 0 ) {
					kvp.Key.UnloadingTime = (float) time + 60;
				} else {
					if ( kvp.Key.UnloadingTime < time )
						_unloadingQueue.Enqueue( kvp.Key );
				}
			} else
				kvp.Key.UnloadingTime = 0;

		while ( _unloadingQueue.TryDequeue( out Asset? unloadedAsset ) ) {
			if ( _assets.TryRemove( unloadedAsset.Path, out _ ) ) {
				AssetUnloaded?.Invoke( unloadedAsset );
			}

		}
	}
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