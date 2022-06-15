using Engine.Structure;

namespace Engine.Rendering.ResourceManagement;
public class ContextUpdateManager : Identifiable, IUpdateable {

	private readonly Queue<WeakReference<IContextUpdateable>> _dead;
	private readonly HashSet<WeakReference<IContextUpdateable>> _alive;

	public bool Active => true;

	public ContextUpdateManager() {
		this._dead = new Queue<WeakReference<IContextUpdateable>>();
		this._alive = new HashSet<WeakReference<IContextUpdateable>>();
	}

	public void Update( float time, float deltaTime ) {
		foreach ( WeakReference<IContextUpdateable>? alive in this._alive )
			if ( alive.TryGetTarget( out IContextUpdateable? updateable ) )
				updateable?.ContextUpdate();
			else
				this._dead.Enqueue( alive );

		while ( this._dead.TryDequeue( out WeakReference<IContextUpdateable>? dead ) )
			this._alive.Remove( dead );
		this._dead.Clear();
	}

	public void Add( IContextUpdateable updateable ) {
		if ( updateable is null )
			return;
		this._alive.Add( new WeakReference<IContextUpdateable>( updateable ) );
	}
}
