using Engine.Modules.Rendering.Ogl.OOP;

namespace Engine.Modules.Rendering;

public class Class1 {

}

public abstract class RenderModule : DisposableIdentifiable, IUpdateable {
    public void Update( in double time, in double deltaTime ) {
        throw new NotImplementedException();
    }
}

public sealed class ContextManagementService : DisposableIdentifiable, IUpdateable {

	private readonly List<Context > _contexts = new();

	public Context CreateContext(WindowSettings settings) {
		Context context = new( settings );
		_contexts.Add( context );
		return context;
	}

	public void Update( in double time, in double deltaTime ) {
		foreach (Context context in _contexts)
			context.Update( time, deltaTime );
	}

	protected override bool InternalDispose() {
		foreach (Context context in _contexts)
			context.Dispose();
		return true;
	}
}