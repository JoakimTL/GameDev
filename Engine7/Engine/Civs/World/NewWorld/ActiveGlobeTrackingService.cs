using Civs.Messages;
using Engine;
using Engine.Modularity;
using Engine.Processing;
using Engine.Standard.Render.Entities.Services;
using System.Diagnostics.CodeAnalysis;

namespace Civs.World.NewWorld;

//public sealed class WorldProviderService : DisposableIdentifiable {

//	public Globe? CurrentGlobe { get; private set; }

//	public void GenerateWorld( uint subdivisionCount, double surfaceArea ) {
//		this.CurrentGlobe = new( subdivisionCount, surfaceArea, 25000, 15000, 6_378_000 );
//	}

//	public void DeserializeWorld() {
//		//TODO
//	}

//	protected override bool InternalDispose() {
//		throw new NotImplementedException();
//	}
//}

[Do<IUpdateable>.Before<RenderEntityContainerService>]
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