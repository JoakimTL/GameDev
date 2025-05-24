using Civlike.Messages;
using Engine;
using Engine.Modularity;
using Engine.Module.Entities.Services;
using Engine.Processing;

namespace Civlike.World.GameplayState;

[Do<IUpdateable>.Before<SynchronizedEntityContainerService>]
public sealed class ActiveGlobeTrackingService : IUpdateable {

	private readonly MessageBusNode _messageBusNode;
	public Globe? CurrentGlobe { get; private set; }

	public event Action<Globe?>? BeforeGlobeChange;
	public event Action<Globe?>? AfterGlobeChange;

	public ActiveGlobeTrackingService() {
		this._messageBusNode = MessageBus.CreateNode( "globe-tracking" );
		this._messageBusNode.OnMessageProcessed += MessageProcessed;
	}

	private void MessageProcessed( Message message ) {
		if (message.Content is CreateNewWorldRequestResponseMessage createNewWorldRequestResponse) {
			BeforeGlobeChange?.Invoke( this.CurrentGlobe );
			this.CurrentGlobe = createNewWorldRequestResponse.Globe;
			AfterGlobeChange?.Invoke( this.CurrentGlobe );
		}
	}

	public void Update( double time, double deltaTime ) {
		this._messageBusNode.ProcessQueue();
	}
}