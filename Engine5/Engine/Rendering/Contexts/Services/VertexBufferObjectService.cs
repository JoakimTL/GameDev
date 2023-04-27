using Engine.Rendering.Contexts.Objects;
using Engine.Structure.Interfaces;
using OpenGL;

namespace Engine.Rendering.Contexts.Services;

public sealed class VertexBufferObjectService : Identifiable, IContextService, IInitializable, IDisposable
{
	private readonly Dictionary<Type, VertexBufferObject> _vbos;

	public VertexBufferObject ElementBuffer { get; private set; } = null!;
	public SegmentedVertexBufferObject UniformBuffer { get; private set; } = null!;
	public SegmentedVertexBufferObject ShaderStorage { get; private set; } = null!;

	public VertexBufferObjectService()
	{
		_vbos = new();
	}

	public void Initialize()
	{
		ElementBuffer = new(nameof(ElementBuffer), 65_536u, BufferUsage.DynamicDraw);
		UniformBuffer = new(nameof(UniformBuffer), 65_536u, BufferUsage.DynamicDraw, 256);
		ShaderStorage = new(nameof(ShaderStorage), 65_536u, BufferUsage.DynamicDraw);
	}

	public VertexBufferObject Get(Type t)
	{
		if (_vbos.TryGetValue(t, out VertexBufferObject? vbo))
			return vbo;
		vbo = new VertexBufferObject(t.Name, 65536, BufferUsage.DynamicDraw);
		_vbos[t] = vbo;
		return vbo;
	}

	public void Dispose()
	{
		ElementBuffer.Dispose();
		UniformBuffer.Dispose();
		ShaderStorage.Dispose();
		foreach (var vbo in _vbos.Values)
			vbo.Dispose();
	}
}
