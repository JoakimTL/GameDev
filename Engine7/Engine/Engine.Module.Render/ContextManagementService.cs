using Engine.Module.Render.Domain;
using Engine.Module.Render.Glfw;
using Engine.Module.Render.Ogl;

namespace Engine.Module.Render;

public sealed class ContextManagementService : DisposableIdentifiable, IUpdateable {

	private readonly Queue<Context> _initializationQueue = new();
	private readonly List<Context> _contexts = [];

	public IReadOnlyList<Context> Contexts => _contexts.AsReadOnly();
	public event Action<Context>? OnContextAdded;

	public bool ShouldStop { get; private set; }

	public Context CreateContext( WindowSettings settings ) {
		Context context = new( settings );
		_contexts.Add( context );
		_initializationQueue.Enqueue( context );
		OnContextAdded?.Invoke( context );
		return context;
	}

	public void Update( double time, double deltaTime ) {
		if (_contexts.Count == 0)
			return;

		while (_initializationQueue.TryDequeue( out Context? context ))
			context.Initialize();

		GLFW.PollEvents();

		foreach (Context context in _contexts)
			context.Update( time, deltaTime );

		for (int ir = _contexts.Count - 1; ir >= 0; ir--)
			if (_contexts[ ir ].Disposed)
				_contexts.RemoveAt( ir );

		if (_contexts.Count == 0)
			ShouldStop = true;
	}

	protected override bool InternalDispose() {
		foreach (Context context in _contexts)
			context.Dispose();
		return true;
	}
}