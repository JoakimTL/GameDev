using Engine.Rendering.Standard.VertexArrayObjects;
using Engine.Structure;

namespace Engine.Rendering.Services;

public class CompositeVertexArrayObjectDataLayoutManager : ModuleService {

	private readonly ServiceProvider<CompositeVertexArrayObjectDataLayout> _compositeVertexArrayObjectDataLayoutProvider;

	public CompositeVertexArrayObjectDataLayoutManager() {
		_compositeVertexArrayObjectDataLayoutProvider = new();
	}

	public CompositeVertexArrayObjectDataLayout GetOrAdd( Type t ) => _compositeVertexArrayObjectDataLayoutProvider.GetOrAdd( t );
	public T Get<T>() where T : CompositeVertexArrayObjectDataLayout => _compositeVertexArrayObjectDataLayoutProvider.GetOrAdd<T>();
	protected override bool OnDispose() => true;
}
