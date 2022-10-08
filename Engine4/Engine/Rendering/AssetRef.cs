namespace Engine.Rendering;
public abstract class AssetRef : DisposableIdentifiable {

	public string Path { get; }

	protected AssetRef( string path ) : base( System.IO.Path.GetFileNameWithoutExtension( path ) ?? path ) {
		this.Path = path;
	}
}

public abstract class AssetRef<T> : AssetRef where T : Asset {

	private WeakReference<T>? _assetWeakReference;
	private IDisposable? _assetDisposable;

	protected AssetRef( string path ) : base( path ) { }

	internal T Resolve( Window window ) {
		if ( Thread.CurrentThread != window.ContextThread )
			this.LogError( "Tried resolving asset outside context thread." );
		if ( this._assetWeakReference is not null && this._assetWeakReference.TryGetTarget( out T? asset ) )
			return asset;
		if ( this._assetDisposable is not null )
			this._assetDisposable.Dispose();

		T newAsset = ResolveAsset();
		this._assetWeakReference = new( newAsset );
		this._assetDisposable = newAsset.GetDisposer();
		return newAsset;
	}

	protected override bool OnDispose() {
		if ( this._assetDisposable is not null )
			this._assetDisposable.Dispose();
		return true;
	}

	protected abstract T ResolveAsset();
}
