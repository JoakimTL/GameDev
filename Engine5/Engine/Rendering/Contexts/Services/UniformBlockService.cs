using Engine.Rendering.Contexts.Objects;
using OpenGL;

namespace Engine.Rendering.Contexts.Services;

public sealed class UniformBlockService : Identifiable, IContextService, IDisposable
{
	private readonly Dictionary<string, UniformBlock> _ubs;
	private readonly VertexBufferObjectService _vertexBufferObjectService;

	public UniformBlockService(VertexBufferObjectService vertexBufferObjectService)
	{
		_ubs = new();
		_vertexBufferObjectService = vertexBufferObjectService;
	}

	public UniformBlock Create(string name, uint sizeBytes, params ShaderType[] shaderTypes)
	{
		if (!_ubs.TryGetValue(name, out var value))
		{
			_ubs.Add(name, value = new(_vertexBufferObjectService.ShaderStorage, name, sizeBytes, shaderTypes));
			value.Disposed += OnDisposed;
		}
		return value;
	}

	private void OnDisposed(DataBlock ssbo) => _ubs.Remove(ssbo.BlockName);

	public void Dispose()
	{
		foreach (var ssbo in _ubs.Values)
			ssbo.Dispose();
		_ubs.Clear();
	}
}