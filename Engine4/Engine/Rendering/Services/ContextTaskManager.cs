using System.Collections.Concurrent;

namespace Engine.Rendering.Services;
public class ContextTaskManager : ModuleService, IUpdateable {

	private readonly ConcurrentQueue<Action> _work;
	public bool Active => true;

	public ContextTaskManager() {
		this._work = new ConcurrentQueue<Action>();
	}

	public void Update( float time, float deltaTime ) {
		uint executed = 0;
		while ( this._work.TryDequeue( out Action? work ) ) {
			work?.Invoke();
			executed++;
		}
		if ( executed > 0 )
			this.LogLine( $"Executed {executed} work{( executed > 1 ? "s" : "" )}!", Log.Level.LOW );
	}

	public void Enqueue( Action work ) {
		if ( work is null ) {
			this.LogWarning( "Cannot commit null as work for the OpenGL context!" );
			return;
		}
		if ( this.Disposed ) {
			this.LogWarning( "Cannot commit work to a disposed context queue!" );
			return;
		}
		this._work.Enqueue( work );
	}

	protected override bool OnDispose() {
		this._work.Clear();
		return true;
	}
}
