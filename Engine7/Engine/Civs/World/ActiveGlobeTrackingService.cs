using Civs.Messages;
using Engine;
using Engine.Modularity;
using Engine.Module.Entities.Services;
using Engine.Processing;

namespace Civs.World;

[Do<IUpdateable>.Before<SynchronizedEntityContainerService>]
public sealed class ActiveGlobeTrackingService : IUpdateable {

	private readonly MessageBusNode _messageBusNode;
	public GlobeModel? CurrentGlobe { get; private set; }

	public event Action<GlobeModel?>? BeforeGlobeChange;
	public event Action<GlobeModel?>? AfterGlobeChange;

	public ActiveGlobeTrackingService() {
		_messageBusNode = MessageBus.CreateNode( "globe-tracking" );
		_messageBusNode.OnMessageProcessed += MessageProcessed;
	}

	private void MessageProcessed( Message message ) {
		if (message.Content is CreateNewWorldRequestResponseMessage createNewWorldRequestResponse) {
			BeforeGlobeChange?.Invoke( CurrentGlobe );
			CurrentGlobe = createNewWorldRequestResponse.Globe;
			AfterGlobeChange?.Invoke( CurrentGlobe );
		}
	}

	public void Update( double time, double deltaTime ) {
		_messageBusNode.ProcessQueue();
	}
}