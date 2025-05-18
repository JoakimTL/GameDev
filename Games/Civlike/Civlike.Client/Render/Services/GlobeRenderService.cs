using Civlike.World;
using Engine;

namespace Civlike.Client.Render.Services;
internal class GlobeRenderService( ActiveGlobeTrackingService activeGlobeTrackingService ) : Engine.IServiceProvider {
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;

	public GlobeModel Globe => _activeGlobeTrackingService.CurrentGlobe ?? throw new InvalidOperationException( "No globe is currently active." );
}
