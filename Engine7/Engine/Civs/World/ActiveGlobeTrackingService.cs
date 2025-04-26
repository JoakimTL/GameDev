using Civs.World.Messages;
using Engine;
using Engine.Modularity;

namespace Civs.World;

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
		if (message.Content is NewWorldCreatedMessage newGlobeCreated) {
			BeforeGlobeChange?.Invoke( CurrentGlobe );
			CurrentGlobe = newGlobeCreated.Globe;
			AfterGlobeChange?.Invoke( CurrentGlobe );
		}
	}

	public void Update( double time, double deltaTime ) {
		_messageBusNode.ProcessQueue();
	}
}