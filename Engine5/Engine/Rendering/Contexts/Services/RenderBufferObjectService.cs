using Engine.Rendering.Contexts.Objects;
using Engine.Structure.Interfaces;

namespace Engine.Rendering.Contexts.Services;

public class RenderBufferObjectService : IContextService, IUpdateable
{

	private readonly Dictionary<Type, RenderBufferObject> _rbos;
	private readonly VertexBufferObjectService _vertexBufferObjectService;

	public RenderBufferObject ElementBuffer { get; private set; } = null!;
	public RenderBufferObject UniformBuffer { get; private set; } = null!;
	public RenderBufferObject ShaderStorage { get; private set; } = null!;

	public RenderBufferObjectService(VertexBufferObjectService vertexBufferObjectService)
	{
		_rbos = new();
		_vertexBufferObjectService = vertexBufferObjectService;
		ElementBuffer = new RenderBufferObject(nameof(ElementBuffer), 65_536u);
		UniformBuffer = new RenderBufferObject(nameof(UniformBuffer), 65_536u);
		ShaderStorage = new RenderBufferObject(nameof(ShaderStorage), 65_536u);

	}

	public RenderBufferObject Get(Type t)
	{
		if (_rbos.TryGetValue(t, out RenderBufferObject? rbo))
			return rbo;
		rbo = new RenderBufferObject(t.Name, 65536);
		_rbos[t] = rbo;
		return rbo;
	}

	public void Update(float time, float deltaTime)
	{
		ElementBuffer.SyncChanges(_vertexBufferObjectService.ElementBuffer);
		UniformBuffer.SyncChanges(_vertexBufferObjectService.UniformBuffer);
		ShaderStorage.SyncChanges(_vertexBufferObjectService.ShaderStorage);
		foreach (var rboKvp in _rbos)
			rboKvp.Value.SyncChanges(_vertexBufferObjectService.Get(rboKvp.Key));
	}
}
