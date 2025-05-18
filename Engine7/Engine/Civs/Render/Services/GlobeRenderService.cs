using Civs.World;
using Engine;

namespace Civs.Render.Services;
internal class GlobeRenderService( ActiveGlobeTrackingService activeGlobeTrackingService ) : Engine.IServiceProvider {
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;

	public GlobeModel Globe => _activeGlobeTrackingService.CurrentGlobe ?? throw new InvalidOperationException( "No globe is currently active." );
}
