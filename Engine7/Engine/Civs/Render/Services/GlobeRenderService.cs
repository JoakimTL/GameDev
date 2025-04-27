using Civs.World.NewWorld;
using Engine.Module.Render;

namespace Civs.Render.Services;
internal class GlobeRenderService( ActiveGlobeTrackingService activeGlobeTrackingService ) : IRenderServiceProvider {
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;

	public GlobeModel Globe => _activeGlobeTrackingService.CurrentGlobe ?? throw new InvalidOperationException( "No globe is currently active." );
}
