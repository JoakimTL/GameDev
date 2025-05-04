using Engine.Module.Render.Ogl;
using Engine.Module.Render;

namespace Engine.Standard.Render.Entities.Services;

public class RenderBehaviourService : IInitializable {
	private readonly ContextManagementService _contextManagementService;

	public RenderBehaviourService( ContextManagementService contextManagementService ) {
		this._contextManagementService = contextManagementService;
		this._contextManagementService.OnContextAdded += OnContextAdded;
	}

	private void OnContextAdded( Context context )
		=> context.InstanceProvider.Catalog.Host<RenderEntityContainerService>();

	public void Initialize() {
		foreach (Context context in this._contextManagementService.Contexts)
			OnContextAdded( context );
	}
}
