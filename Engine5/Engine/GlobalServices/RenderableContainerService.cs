using Engine.Rendering;
using Engine.Structure.Interfaces;

namespace Engine.GlobalServices;

/// <summary>
/// The rendermodule uses this container to get the data to render.
/// </summary>
public sealed class RenderableContainerService : IGlobalService
{

	public event Action<IRenderable>? AddedRenderable;
	public event Action<IRenderable>? RemovedRenderable;

	private HashSet<IRenderable> _renderables;

	public RenderableContainerService()
	{
		_renderables = new();
	}

	public void Add(IRenderable renderable)
	{
		lock (_renderables)
		{
			if (_renderables.Add(renderable))
				AddedRenderable?.Invoke(renderable);
		}
	}

	public void Remove(IRenderable renderable)
    {
		lock (_renderables)
		{
			if (_renderables.Remove(renderable))
				RemovedRenderable?.Invoke(renderable);
		}
	}

	public IReadOnlySet<IRenderable> Renderables => _renderables;
}