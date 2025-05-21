using Civlike.Logic.Nations;
using Civlike.Logic.World;
using Civlike.Messages;
using Civlike.World;
using Engine;
using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render.Components;
using Engine.Module.Entities.Services;
using Engine.Standard;

namespace Civlike.Logic;

public sealed class CivsGameLogicModule : ModuleBase {

	private EntityContainer _entities = null!;
	private Entity? _worldEntity = null;
	private List<Entity> _clusters = null!;
	private List<Entity> _populationCenters = null!;
	private List<Entity> _players = null!;

	private WorldGenerationParameters? _worldGenParameters;

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
			TectonicWorldTerrainGenerator procGen = new TectonicWorldTerrainGenerator( createNewWorldRequest.Parameters );
			GlobeModel globe = new GlobeModel( Guid.NewGuid(), createNewWorldRequest.Parameters, procGen );
			_worldGenParameters = createNewWorldRequest.Parameters;
			MessageBusNode.Publish( new CreateNewWorldRequestResponseMessage( globe ), "globe-tracking", true );
			return;
		}

		if (message.Content is CreateNewPopulationCenterMessage createNewOwnerMessage) {
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
				Entity? player = _entities.GetEntity( createNewOwnerMessage.PlayerGuid );
				if (player is null)
					return;
				Entity newOwner = _entities.CreateEntity();
				newOwner.SetParent( createNewOwnerMessage.PlayerGuid );
				newOwner.AddComponent<PopulationCenterComponent>();
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

		//if (message.Content is CreateNewPlayerMessage) {
		//	Entity player = _entities.CreateEntity();
		//	player.AddComponent<PlayerComponent>( p => p.SetColor( (Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle(), 1) ) );
		//	_players.Add( player );
		//	MessageBusNode.Publish( new CreateNewPlayerMessageResponse( player.EntityId ), null, true );
		//}
	}

	private void Init() {
		_entities = this.InstanceProvider.Get<EntityContainerService>().CreateContainer();
		_clusters = [];
		_populationCenters = [];
		_players = [];
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

		foreach (Entity populationCenter in _populationCenters)
			_entities.RemoveEntity( populationCenter );
		_populationCenters.Clear();

		foreach (Entity player in _players)
			_entities.RemoveEntity( player );
		_players.Clear();

		InstanceProvider.Get<GameStateService>().SetNewState( "selectedTile", null );

		if (model is null)
			return;

		_worldEntity = _entities.CreateEntity();
		_worldEntity.AddComponent<GlobeComponent>( p => p.SetGlobe( model ) );
		_worldEntity.AddComponent<RenderComponent>();

		foreach (BoundedRenderCluster cluster in model.Clusters) {
			Entity entity = _entities.CreateEntity();
			entity.SetParent( _worldEntity.EntityId );
			entity.AddComponent<BoundedRenderClusterComponent>( p => p.Set( model, (int) cluster.Id ) );
			_clusters.Add( entity );
			entity.AddComponent<RenderComponent>();
		}

		if (_worldGenParameters is null)
			return;
		Face[] facesToDistribute = new Face[ _worldGenParameters.PlayerCount ];
		for (int i = 0; i < _worldGenParameters.PlayerCount; i++) {
			Face face = null!;
			do
				face = model.Faces[ Random.Shared.Next( 0, model.Faces.Count ) ];
			while (!face.State.TerrainType.IsLand || facesToDistribute.Contains( face ));
			facesToDistribute[ i ] = face;
		}

		for (int i = 0; i < _worldGenParameters.PlayerCount; i++) {
			Entity player = _entities.CreateEntity();
			player.AddComponent<PlayerComponent>( p => p.SetColor( (Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle(), 1) ) );
			_players.Add( player );
			MessageBusNode.Publish( new CreateNewPlayerMessageResponse( player.EntityId ), null, true );
			if (i == 0)
				InstanceProvider.Get<GameStateProvider>().SetNewState( "localPlayerId", player.EntityId );
			//Create a population center for tribe
			Entity popCenter = _entities.CreateEntity();
			_populationCenters.Add( popCenter );
			popCenter.SetParent( player.EntityId );
			popCenter.AddComponent<PopulationCenterComponent>();
			popCenter.AddComponent<RenderComponent>();
			FaceOwnershipComponent foc = popCenter.AddComponent<FaceOwnershipComponent>();
			foc.AddFace( facesToDistribute[ i ] );
		}
	}

	private void Update( double time, double deltaTime ) {
	}
}
