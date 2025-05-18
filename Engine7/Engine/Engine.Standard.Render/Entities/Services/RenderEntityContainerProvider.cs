using Engine.Module.Render.Entities;

namespace Engine.Standard.Render.Entities.Services;

public sealed class RenderEntityContainerProvider( RenderEntityContainerService renderEntityContainerService ) : Identifiable, IServiceProvider {
	private readonly RenderEntityContainerService _renderEntityContainerService = renderEntityContainerService;

	public IReadOnlyCollection<RenderEntityContainer> Containers => _renderEntityContainerService.RenderEntityContainers;
}