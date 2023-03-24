using Engine.Rendering.Contexts.Objects;
using OpenGL;

namespace Engine.Rendering.Contexts.Services;

public sealed class SegmentedVertexBufferObjectService : Identifiable, IContextService, IDisposable
{
	private readonly Dictionary<Type, SegmentedVertexBufferObject> _vbos;

	public SegmentedVertexBufferObjectService()
	{
		_vbos = new();
	}

	public SegmentedVertexBufferObject Get(Type t)
	{
		if (_vbos.TryGetValue(t, out SegmentedVertexBufferObject? vbo))
			return vbo;
		vbo = new SegmentedVertexBufferObject(t.Name, 65536, BufferUsage.DynamicDraw);
		_vbos[t] = vbo;
		return vbo;
	}

	public void Dispose()
	{
		foreach (var vbo in _vbos.Values)
			vbo.Dispose();
	}
}