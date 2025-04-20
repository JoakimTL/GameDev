using Engine.Modularity;
using Engine.Standard.Render.Entities.Services;

namespace Engine.Standard.Render.Entities;

public sealed class EntityRenderModification : IModuleModification {
	public void ModifyModule( ModuleBase module ) {
		if (module is RenderModuleBase renderModule)
			renderModule.InstanceProvider.Catalog.Host<RenderBehaviourService>();
	}
}
