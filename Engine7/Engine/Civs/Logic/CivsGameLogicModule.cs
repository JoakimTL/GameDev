using Civs.Logic.Nations;
using Civs.Logic.World;
using Civs.Messages;
using Civs.World.NewWorld;
using Engine;
using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Services;
using Engine.Module.Render.Entities.Components;
using Engine.Standard.Entities.Components;

namespace Civs.Logic;

public sealed class CivsGameLogicModule : ModuleBase {

	private EntityContainer _entities = null!;
	private Entity? _worldEntity = null;
	private List<Entity> _clusters = null!;
	private List<Entity> _populationCenters = null!;

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

		if (message.Content is CreateNewWorldRequestMessage createNewWorldRequest) {
			var procGen = new ProceduralWorldTerrainGenerator( createNewWorldRequest.Parameters );
			var globe = new GlobeModel( Guid.NewGuid(), createNewWorldRequest.Parameters.Subdivisions, procGen );
			MessageBusNode.Publish( new CreateNewWorldRequestResponseMessage( globe ), "globe-tracking" );
			return;
		}

		if (message.Content is CreateNewOwnerMessage createNewOwnerMessage) {
			Entity? currentOwner = _populationCenters.FirstOrDefault( p => p.GetComponentOrThrow<FaceOwnershipComponent>().OwnedFaces.Contains( createNewOwnerMessage.Face ) );
			if (currentOwner is not null) {
				FaceOwnershipComponent toc = currentOwner.GetComponentOrThrow<FaceOwnershipComponent>();
				toc.RemoveFace( createNewOwnerMessage.Face );
				if (toc.OwnedFaces.Count == 0) {
					_entities.RemoveEntity( currentOwner );
					_populationCenters.Remove( currentOwner );
				}
			}
			{
				Entity newOwner = _entities.CreateEntity();
				newOwner.AddComponent<PopulationCenterComponent>();
				newOwner.AddComponent<FaceOwnershipRenderComponent>( p => p.SetColor( (Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle(), 1) ) );
				FaceOwnershipComponent toc = newOwner.AddComponent<FaceOwnershipComponent>();
				toc.AddFace( createNewOwnerMessage.Face );
				newOwner.AddComponent<RenderComponent>();
				_populationCenters.Add( newOwner );
			}
		}
		if (message.Content is RemoveOwnerMessage removeOwnerMessage) {
			Entity? currentOwner = _populationCenters.FirstOrDefault( p => p.GetComponentOrThrow<FaceOwnershipComponent>().OwnedFaces.Contains( removeOwnerMessage.Face ) );
			if (currentOwner is not null) {
				FaceOwnershipComponent toc = currentOwner.GetComponentOrThrow<FaceOwnershipComponent>();
				toc.RemoveFace( removeOwnerMessage.Face );
				if (toc.OwnedFaces.Count == 0) {
					_entities.RemoveEntity( currentOwner );
					_populationCenters.Remove( currentOwner );
				}
			}
		}
		if (message.Content is SetNeighbourOwnerMessage setNeighbourOwnerMessage) {
			Face otherTile = setNeighbourOwnerMessage.Face.Blueprint.Connections[ setNeighbourOwnerMessage.Index ].GetOther( setNeighbourOwnerMessage.Face );
			Entity? currentOwner = _populationCenters.FirstOrDefault( p => p.GetComponentOrThrow<FaceOwnershipComponent>().OwnedFaces.Contains( setNeighbourOwnerMessage.Face ) );
			Entity? newOwner = _populationCenters.FirstOrDefault( p => p.GetComponentOrThrow<FaceOwnershipComponent>().OwnedFaces.Contains( otherTile ) );
			if (newOwner is null)
				return;
			if (currentOwner is not null) {
				FaceOwnershipComponent toc = currentOwner.GetComponentOrThrow<FaceOwnershipComponent>();
				toc.RemoveFace( setNeighbourOwnerMessage.Face );
				if (toc.OwnedFaces.Count == 0) {
					_entities.RemoveEntity( currentOwner );
					_populationCenters.Remove( currentOwner );
				}
			}
			{
				FaceOwnershipComponent toc = newOwner.GetComponentOrThrow<FaceOwnershipComponent>();
				toc.AddFace( setNeighbourOwnerMessage.Face );
			}
		}
	}

	private void Init() {
		_entities = this.InstanceProvider.Get<EntityContainerService>().CreateContainer();
		_clusters = [];
		_populationCenters = [];
		this.InstanceProvider.Get<ActiveGlobeTrackingService>().AfterGlobeChange += OnGlobeChanged;
	}

	private void OnGlobeChanged( GlobeModel? model ) {
		if (_worldEntity is not null) {
			_entities.RemoveEntity( _worldEntity );
			_worldEntity = null;
		}
		foreach (Entity cluster in _clusters)
			_entities.RemoveEntity( cluster );
		_clusters.Clear();

		if (model is null)
			return;

		_worldEntity = _entities.CreateEntity();
		_worldEntity.AddComponent<Transform3Component>();
		_worldEntity.AddComponent<GlobeComponent>( p => p.SetGlobe( model ) );
		_worldEntity.AddComponent<RenderComponent>();

		foreach (BoundedRenderCluster cluster in model.Clusters) {
			Entity entity = _entities.CreateEntity();
			entity.SetParent( _worldEntity.EntityId );
			entity.AddComponent<Transform3Component>();
			entity.AddComponent<BoundedRenderClusterComponent>( p => p.Set( model, (int) cluster.Id ) );
			_clusters.Add( entity );
			entity.AddComponent<RenderComponent>();
		}
	}

	private void Update( double time, double deltaTime ) {

	}
}
