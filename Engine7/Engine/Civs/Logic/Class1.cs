using Civs.Logic.World;
using Civs.Messages;
using Civs.World;
using Engine;
using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Services;
using Engine.Standard.Entities.Components;

namespace Civs.Logic;
internal class Class1 {
}

public sealed class CivsGameLogicModule : ModuleBase {

	private EntityContainer? _entities;
	private List<Entity> _clusters;

	public CivsGameLogicModule() : base( false, 40, "gamelogic" ) {
		OnInitialize += Init;
		OnUpdate += Update;
		OnMessageReceived += MessageReceived;
	}

	private void MessageReceived( Message message ) {
		if (message.Content is NewGameMessage newGameMessage) {
			message.SendResponseFrom( MessageBusNode, new NewGameMessageResponse() );
			return;
		}
		if (message.Content is CreateWorldMessage createWorldMessage) {
			this.InstanceProvider.Get<WorldProviderService>().GenerateWorld( createWorldMessage.SubdivisionCount, createWorldMessage.SurfaceArea );
			_entities = this.InstanceProvider.Get<EntityContainerService>().CreateContainer();
			_clusters = [];
			foreach (var cluster in this.InstanceProvider.Get<WorldProviderService>().CurrentGlobe!.Clusters) {
				var entity = _entities.CreateEntity();
				entity.AddComponent<Transform3Component>( p => p.Transform.Translation = cluster.Bounds.GetCenter().CastSaturating<float, double>() ); //TODO: might need to scale!
				entity.AddComponent<TileEdgeClusterComponent>( p => p.SetCluster( cluster ) );
				_clusters.Add( entity );
			}
			message.SendResponseFrom( MessageBusNode, new CreateWorldMessageResponse( this.InstanceProvider.Get<WorldProviderService>().CurrentGlobe ) );
			return;
		}
	}

	private void Init() {
		var worldProviderService = InstanceProvider.Get<WorldProviderService>();
	}

	private void Update( double time, double deltaTime ) {
	}
}

public sealed class WorldProviderService {

	public Globe? CurrentGlobe { get; private set; }

	public void GenerateWorld( uint subdivisionCount, double surfaceArea ) {
		this.CurrentGlobe = new( subdivisionCount, surfaceArea );
	}

	public void DeserializeWorld() {
		//TODO
	}
}