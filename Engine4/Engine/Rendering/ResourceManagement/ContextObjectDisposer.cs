using System.Collections.Concurrent;
using Engine.Structure;

namespace Engine.Rendering.ResourceManagement;

[Structure.ProcessBefore( typeof( Window ), typeof( IDisposable ) )]
public class ContextObjectDisposer : DisposableIdentifiable, IUpdateable {

	private readonly ConcurrentQueue<IDisposable> _disposables;
	public bool Active => true;

	public ContextObjectDisposer() {
		this._disposables = new ConcurrentQueue<IDisposable>();
	}

	public void Update( float time, float deltaTime ) {
		while ( this._disposables.TryDequeue( out IDisposable? d ) )
			d.Dispose();
	}

	public void Add( IDisposable disposable ) {
		if ( disposable is null ) {
			this.LogWarning( "Cannot commit null as work for the OpenGL context!" );
			return;
		}
		if ( this.Disposed ) {
			this.LogWarning( "Cannot commit work to a disposed context queue!" );
			return;
		}
		this._disposables.Enqueue( disposable );
	}

	protected override bool OnDispose() {
		while ( this._disposables.TryDequeue( out IDisposable? d ) )
			d.Dispose();
		return true;
	}
}
