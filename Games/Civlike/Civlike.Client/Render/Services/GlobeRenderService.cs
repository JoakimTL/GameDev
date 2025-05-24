using Civlike.World.GameplayState;

namespace Civlike.Client.Render.Services;
internal class GlobeRenderService( ActiveGlobeTrackingService activeGlobeTrackingService ) : Engine.IServiceProvider {
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;

	public Globe Globe => this._activeGlobeTrackingService.CurrentGlobe ?? throw new InvalidOperationException( "No globe is currently active." );
}
