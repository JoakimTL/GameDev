using Engine.Module.Render.Ogl;
using Engine.Module.Render;

namespace Engine.Standard.Entities.Render.Services;

public class RenderBehaviourService : IInitializable {
	private readonly ContextManagementService _contextManagementService;

	public RenderBehaviourService( ContextManagementService contextManagementService ) {
		this._contextManagementService = contextManagementService;
		this._contextManagementService.OnContextAdded += OnContextAdded;
	}

	private void OnContextAdded( Context context )
		=> context.InstanceProvider.Catalog.Host<RenderBehaviourEntityContainerEventReceiverService>();

	public void Initialize() {
		foreach (Context context in this._contextManagementService.Contexts)
			OnContextAdded( context );
	}
}
