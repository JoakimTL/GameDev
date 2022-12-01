//using Engine.Rendering;
//using Engine.Structure.Interfaces;
//using Engine.Time;
//using System.Collections.Concurrent;

//namespace Engine.GlobalServices;

//public sealed class AssetProviderService : Identifiable, IGlobalService {

//	public delegate void AssetEvent( AssetBase asset );

//	public event AssetEvent? AssetLoaded;
//	public event AssetEvent? AssetUnloaded;

//	private readonly ReferenceCountingService _referenceCountingService;
//	private readonly ConcurrentDictionary<string, Asset> _assets;
//	private readonly ConcurrentQueue<Asset> _unreferencedQueue;
//	private readonly List<Asset> _unloadingList;
//	private readonly TickingTimer _unloadingTimer;

//	public AssetProviderService( ReferenceCountingService referenceCountingService ) {
//		_referenceCountingService = referenceCountingService;
//		_referenceCountingService.Unreferenced += UnreferencedAsset;
//		_assets = new();
//		_unreferencedQueue = new();
//		_unloadingList = new();
//		_unloadingTimer = new();
//		_unloadingTimer.Elapsed += CheckUnloaded;
//		_unloadingTimer.Start();
//	}

//	private void UnreferencedAsset( object obj ) {
//		if ( obj is Asset asset )
//			_unreferencedQueue.Enqueue( asset );
//	}

//	private T? LoadAsset<T>( string assetPath ) where T : Asset, IAssetFactory<T> {
//		T? asset = T.CreateAsset( assetPath );
//		if ( asset is null )
//			return null;
//		if ( !_assets.TryAdd( assetPath, asset ) ) {
//			if ( !_assets.TryGetValue( assetPath, out Asset? assetBase ) )
//				throw new InvalidOperationException( "Asset loading error." );
//			if ( assetBase is T assetT )
//				return assetT;
//			this.LogWarning( $"{assetPath} could not be loaded as {typeof( T ).Name}!" );
//			return null;
//		}
//		AssetLoaded?.Invoke( asset );
//		return asset;
//	}

//	public T? Get<T>( object caller, string assetPath ) where T : Asset, IAssetFactory<T> {
//		{
//			if ( _assets.TryGetValue( assetPath, out var asset ) ) {
//				if ( asset is T t ) {
//					_referenceCountingService.Reference( caller, asset );
//					asset.UnloadingTime = -1;
//					return t;
//				}
//				this.LogWarning( $"{assetPath} could not be loaded as {typeof( T ).Name}!" );
//				return null;
//			}
//		}

//		{
//			var asset = LoadAsset<T>( assetPath );
//			if ( asset is null )
//				return null;
//			_referenceCountingService.Reference( caller, asset );
//			asset.UnloadingTime = -1;
//			return asset;
//		}

//	}

//	private void CheckUnloaded( double time, double deltaTime ) {
//		while ( _unreferencedQueue.TryDequeue( out Asset? asset ) ) {
//			_unloadingList.Add( asset );
//			asset.UnloadingTime = (float) time + 60;
//		}

//		_unloadingList.RemoveAll( p => p.UnloadingTime < 0 );

//		for ( int i = 0; i < _unloadingList.Count; i++ ) {
//			var asset = _unloadingList[ i ];

//			if ( time > asset.UnloadingTime )
//				if ( _assets.TryRemove( asset.Path, out _ ) )
//					AssetUnloaded?.Invoke( asset );
//		}

//		_unloadingList.RemoveAll( p => time > p.UnloadingTime );
//	}
//}
