using Civlike.Logic.Nations;
using Civlike.Logic.World;
using Civlike.Messages;
using Civlike.World;
using Civlike.World.GameplayState;
using Engine;
using Engine.Logging;
using Engine.Modularity;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render.Components;
using Engine.Module.Entities.Services;
using Engine.Standard;
using System.Reflection;

namespace Civlike.Logic;

public sealed class CivsGameLogicModule : ModuleBase {

	private EntityContainer _entities = null!;
	private Entity? _globeEntity = null;
	private List<Entity> _clusters = null!;
	private List<Entity> _populationCenters = null!;
	private List<Entity> _players = null!;

	private Civlike.World.GenerationState.GlobeGeneratorParameterBase? _globeGenParameters;

	public CivsGameLogicModule() : base( false, 40, "gamelogic" ) {
		OnInitialize += Init;
		OnUpdate += Update;
		OnMessageReceived += MessageReceived;
	}

	private void MessageReceived( Message message ) {
		if (message.Content is NewGameMessage newGameMessage) {
			message.SendResponseFrom( this.MessageBusNode, new NewGameMessageResponse() );
			return;
		}

		if (message.Content.GetType().IsGenericType && message.Content.GetType().GetGenericTypeDefinition().IsAssignableTo( typeof( CreateNewGlobeRequestMessage<,> ) )) {
			Type contentType = message.Content.GetType();
			Type[] genericArgs = contentType.GetGenericArguments();
			MethodInfo openGen = typeof( GlobeGenerator ).GetMethod( nameof( GlobeGenerator.Generate ), BindingFlags.Public | BindingFlags.Static ) ?? throw new InvalidOperationException( "Couldn’t find Generate method" );
			MethodInfo genMethod = openGen.MakeGenericMethod( genericArgs );
			PropertyInfo propParams = contentType.GetProperty( "Parameters", BindingFlags.Public | BindingFlags.Instance ) ?? throw new InvalidOperationException( "No Parameters property" );
			PropertyInfo propSteps = contentType.GetProperty( "StepsToIgnore", BindingFlags.Public | BindingFlags.Instance ) ?? throw new InvalidOperationException( "No StepsToIgnore property" );
			object? paramValue = propParams.GetValue( message.Content );
			object? stepsValue = propSteps.GetValue( message.Content );
			Globe globe = (Globe) (genMethod.Invoke( null, [ paramValue, stepsValue ] ) ?? throw new InvalidOperationException( "Generate returned null" ));
			this._globeGenParameters = paramValue as Civlike.World.GenerationState.GlobeGeneratorParameterBase ?? throw new InvalidOperationException( "Parameters property is not a GlobeGeneratorParameterBase" );

			this.MessageBusNode.Publish( new CreateNewWorldRequestResponseMessage( globe ), "globe-tracking", true );
			return;
		}

		//if (message.Content is CreateNewPopulationCenterMessage createNewOwnerMessage) {
		//	Entity? currentOwner = _populationCenters.FirstOrDefault( p => p.GetComponentOrThrow<FaceOwnershipComponent>().OwnedFaces.Contains( createNewOwnerMessage.Face ) );
		//	if (currentOwner is not null) {
		//		FaceOwnershipComponent toc = currentOwner.GetComponentOrThrow<FaceOwnershipComponent>();
		//		toc.RemoveFace( createNewOwnerMessage.Face );
		//		if (toc.OwnedFaces.Count == 0) {
		//			_entities.RemoveEntity( currentOwner );
		//			_populationCenters.Remove( currentOwner );
		//		}
		//	}
		//	{
		//		Entity? player = _entities.GetEntity( createNewOwnerMessage.PlayerGuid );
		//		if (player is null)
		//			return;
		//		Entity newOwner = _entities.CreateEntity();
		//		newOwner.SetParent( createNewOwnerMessage.PlayerGuid );
		//		newOwner.AddComponent<PopulationCenterComponent>();
		//		FaceOwnershipComponent toc = newOwner.AddComponent<FaceOwnershipComponent>();
		//		toc.AddFace( createNewOwnerMessage.Face );
		//		newOwner.AddComponent<RenderComponent>();
		//		_populationCenters.Add( newOwner );
		//	}
		//}
		//if (message.Content is RemoveOwnerMessage removeOwnerMessage) {
		//	Entity? currentOwner = _populationCenters.FirstOrDefault( p => p.GetComponentOrThrow<FaceOwnershipComponent>().OwnedFaces.Contains( removeOwnerMessage.Face ) );
		//	if (currentOwner is not null) {
		//		FaceOwnershipComponent toc = currentOwner.GetComponentOrThrow<FaceOwnershipComponent>();
		//		toc.RemoveFace( removeOwnerMessage.Face );
		//		if (toc.OwnedFaces.Count == 0) {
		//			_entities.RemoveEntity( currentOwner );
		//			_populationCenters.Remove( currentOwner );
		//		}
		//	}
		//}
		//if (message.Content is SetNeighbourOwnerMessage setNeighbourOwnerMessage) {
		//	Civlike.World.GameplayState.Face otherTile = setNeighbourOwnerMessage.Face.Blueprint.Neighbours[ setNeighbourOwnerMessage.Index ];
		//	Entity? currentOwner = _populationCenters.FirstOrDefault( p => p.GetComponentOrThrow<FaceOwnershipComponent>().OwnedFaces.Contains( setNeighbourOwnerMessage.Face ) );
		//	Entity? newOwner = _populationCenters.FirstOrDefault( p => p.GetComponentOrThrow<FaceOwnershipComponent>().OwnedFaces.Contains( otherTile ) );
		//	if (newOwner is null)
		//		return;
		//	if (currentOwner is not null) {
		//		FaceOwnershipComponent toc = currentOwner.GetComponentOrThrow<FaceOwnershipComponent>();
		//		toc.RemoveFace( setNeighbourOwnerMessage.Face );
		//		if (toc.OwnedFaces.Count == 0) {
		//			_entities.RemoveEntity( currentOwner );
		//			_populationCenters.Remove( currentOwner );
		//		}
		//	}
		//	{
		//		FaceOwnershipComponent toc = newOwner.GetComponentOrThrow<FaceOwnershipComponent>();
		//		toc.AddFace( setNeighbourOwnerMessage.Face );
		//	}
		//}

		//if (message.Content is CreateNewPlayerMessage) {
		//	Entity player = _entities.CreateEntity();
		//	player.AddComponent<PlayerComponent>( p => p.SetColor( (Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle(), 1) ) );
		//	_players.Add( player );
		//	MessageBusNode.Publish( new CreateNewPlayerMessageResponse( player.EntityId ), null, true );
		//}
	}

	private void Init() {
		this._entities = this.InstanceProvider.Get<EntityContainerService>().CreateContainer();
		this._clusters = [];
		this._populationCenters = [];
		this._players = [];
		this.InstanceProvider.Get<ActiveGlobeTrackingService>().AfterGlobeChange += OnGlobeChanged;
	}

	private void OnGlobeChanged( Globe? model ) {
		foreach (Entity cluster in this._clusters)
			this._entities.RemoveEntity( cluster );
		this._clusters.Clear();

		if (this._globeEntity is not null) {
			this._entities.RemoveEntity( this._globeEntity );
			this._globeEntity = null;
		}

		foreach (Entity populationCenter in this._populationCenters)
			this._entities.RemoveEntity( populationCenter );
		this._populationCenters.Clear();

		foreach (Entity player in this._players)
			this._entities.RemoveEntity( player );
		this._players.Clear();

		this.InstanceProvider.Get<GameStateService>().SetNewState( "selectedTile", null );

		if (model is null)
			return;

		this._globeEntity = this._entities.CreateEntity();
		this._globeEntity.AddComponent<GlobeComponent>( p => p.SetGlobe( model ) );
		this._globeEntity.AddComponent<RenderComponent>();

		foreach (BoundedRenderCluster cluster in model.Clusters) {
			Entity entity = this._entities.CreateEntity();
			entity.SetParent( this._globeEntity.EntityId );
			entity.AddComponent<BoundedRenderClusterComponent>( p => p.Set( model, (int) cluster.Id ) );
			this._clusters.Add( entity );
			entity.AddComponent<RenderComponent>();
		}

		if (this._globeGenParameters is null)
			return;

		if (model.Faces.Count(p => p.State.TerrainType.IsLand) < this._globeGenParameters.PlayerCount) {
			this.LogWarning( "Not enough land tiles for the number of players. Globe is unusable for gameplay and no spawns will be given." );
			this.InstanceProvider.Get<GameStateProvider>().SetNewState( "startLocation", Vector3<float>.UnitZ * 2 );
			return;
		}

		Face[] facesToDistribute = new Face[ this._globeGenParameters.PlayerCount ];
		for (int i = 0; i < this._globeGenParameters.PlayerCount; i++) {
			Face face = null!;
			do
				face = model.Faces[ Random.Shared.Next( 0, model.Faces.Count ) ];
			while (!face.State.TerrainType.IsLand || facesToDistribute.Contains( face ));
			facesToDistribute[ i ] = face;
		}

		for (int i = 0; i < this._globeGenParameters.PlayerCount; i++) {
			Entity player = this._entities.CreateEntity();
			player.AddComponent<PlayerComponent>( p => p.SetColor( (Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle(), 1) ) );
			this._players.Add( player );
			this.MessageBusNode.Publish( new CreateNewPlayerMessageResponse( player.EntityId ), null, true );
			if (i == 0)
				this.InstanceProvider.Get<GameStateProvider>().SetNewState( "localPlayerId", player.EntityId );
			//Create a population center for tribe
			Entity popCenter = this._entities.CreateEntity();
			this._populationCenters.Add( popCenter );
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
