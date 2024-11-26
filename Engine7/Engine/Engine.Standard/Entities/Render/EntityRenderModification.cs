using Engine.Modularity;
using Engine.Module.Render;
using Engine.Standard.Entities.Render.Services;

namespace Engine.Standard.Entities.Render;

public sealed class EntityRenderModification : IModuleModification {
	public void ModifyModule( ModuleBase module ) {
		if (module is RenderModuleBase renderModule)
			renderModule.InstanceProvider.Catalog.Host<RenderBehaviourService>();
	}
}
