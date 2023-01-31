using Engine.Rendering.Objects;

namespace Engine.Rendering.Services;

public class VertexBufferObjectService : IContextService {
	private readonly Dictionary<Type, VertexBufferObject> _vbos;

	public VertexBufferObjectService() {
		_vbos = new();
	}

	public VertexBufferObject Get( Type t ) {
		if ( _vbos.TryGetValue( t, out VertexBufferObject? vbo ) )
			return vbo;
		vbo = new VertexBufferObject( t.Name, 65536, OpenGL.BufferUsage.DynamicDraw );
		_vbos[ t ] = vbo;
		return vbo;
	}
}
