using Engine.Module.Render;
using Engine.Module.Render.Entities;

namespace Engine.Standard.Render.Entities.Services;

public sealed class RenderEntityContainerProvider( RenderEntityContainerService renderEntityContainerService ) : Identifiable, IRenderServiceProvider {
	private readonly RenderEntityContainerService _renderEntityContainerService = renderEntityContainerService;

	public IReadOnlyCollection<RenderEntityContainer> Containers => _renderEntityContainerService.RenderEntityContainers;
}