using Engine.Modularity;

namespace Civlike.World;

public sealed class GlobeStoreService : MessageBusServiceBase {

	private Globe? _currentGlobe;

	public event Action<Globe?>? OnGlobeChanged;

	public GlobeStoreService() : base( "globe-store" ) {
		this._currentGlobe = null;
	}

	public Globe? CurrentGlobe => this._currentGlobe;

	protected override void MessageProcessed( Message message ) {
		if (message.Content is not GlobeCreatedMessage globeCreated)
			return;
		_currentGlobe = globeCreated.Globe;
		OnGlobeChanged?.Invoke( _currentGlobe );
	}
}
