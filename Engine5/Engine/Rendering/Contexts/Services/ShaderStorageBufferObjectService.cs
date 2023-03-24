using Engine.Rendering.Contexts.Objects;
using OpenGL;

namespace Engine.Rendering.Contexts.Services;

public sealed class ShaderStorageBufferObjectService : Identifiable, IContextService, IDisposable
{
	private readonly Dictionary<string, ShaderStorageBufferObject> _ssbos;
	private readonly RenderBufferObjectService _renderBufferObjectService;
	private readonly VertexBufferObjectService _vertexBufferObjectService;

	public ShaderStorageBufferObjectService(RenderBufferObjectService renderBufferObjectService, VertexBufferObjectService vertexBufferObjectService)
	{
		_ssbos = new();
		_renderBufferObjectService = renderBufferObjectService;
		_vertexBufferObjectService = vertexBufferObjectService;
	}

	public ShaderStorageBufferObject Create(string name, uint sizeBytes, params ShaderType[] shaderTypes)
	{
		if (!_ssbos.TryGetValue(name, out var value))
		{
			_ssbos.Add(name, value = new(_renderBufferObjectService.ShaderStorage, _vertexBufferObjectService.ShaderStorage, name, sizeBytes, shaderTypes));
			value.Disposed += OnDisposed;
		}
		return value;
	}

	private void OnDisposed(DataBlock ssbo) => _ssbos.Remove(ssbo.BlockName);

	public void Dispose()
	{
		foreach (var ssbo in _ssbos.Values)
			ssbo.Dispose();
		_ssbos.Clear();
	}
}
