using Civs.World;
using Engine.Module.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civs.Render.Services;
internal class GlobeRenderService( ActiveGlobeTrackingService activeGlobeTrackingService ) : IRenderServiceProvider {
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;

	public GlobeBlueprintModel Blueprint => _activeGlobeTrackingService.CurrentGlobe?.Blueprint ?? throw new InvalidOperationException( "No globe is currently active." );
	public GlobeStateModel State => _activeGlobeTrackingService.CurrentGlobe?.State ?? throw new InvalidOperationException( "No globe is currently active." );
}
