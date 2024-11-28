using Engine.Module.Render.Domain;
using Engine.Module.Render.Glfw;
using Engine.Module.Render.Ogl;

namespace Engine.Module.Render;

public sealed class ContextManagementService : DisposableIdentifiable, IUpdateable {

	private readonly Queue<Context> _initializationQueue = new();
	private readonly List<Context> _contexts = [];

	public IReadOnlyList<Context> Contexts => this._contexts.AsReadOnly();
	public event Action<Context>? OnContextAdded;

	public bool ShouldStop { get; private set; }

	public Context CreateContext( WindowSettings settings ) {
		Context context = new( settings );
		this._contexts.Add( context );
		this._initializationQueue.Enqueue( context );
		OnContextAdded?.Invoke( context );
		return context;
	}

	public void Update( double time, double deltaTime ) {
		if (this._contexts.Count == 0)
			return;

		while (this._initializationQueue.TryDequeue( out Context? context ))
			context.Initialize();

		GLFW.PollEvents();

		foreach (Context context in this._contexts)
			context.Update( time, deltaTime );

		for (int ir = this._contexts.Count - 1; ir >= 0; ir--)
			if (this._contexts[ ir ].Disposed)
				this._contexts.RemoveAt( ir );

		if (this._contexts.Count == 0)
			this.ShouldStop = true;
	}

	protected override bool InternalDispose() {
		foreach (Context context in this._contexts)
			context.Dispose();
		return true;
	}
}