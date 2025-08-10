using Civlike.World;
using Civlike.World.State;

namespace Civlike.Client.Render.Services;
internal class GlobeRenderService( GlobeStoreService globeStore ) : Engine.IServiceProvider {
	private readonly GlobeStoreService _globeStore = globeStore;

	public Globe? CurrentGlobe => _globeStore.CurrentGlobe;
}
