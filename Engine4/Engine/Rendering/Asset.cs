namespace Engine.Rendering;

public abstract class Asset : DisposableIdentifiable {
	protected Asset( string name ) : base( name ) { }

	public abstract IDisposable? GetDisposer();
}
