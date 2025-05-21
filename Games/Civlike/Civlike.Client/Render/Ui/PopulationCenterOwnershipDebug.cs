using Civlike.Client.Render.Ui.Components;
using Civlike.Logic.Nations;
using Civlike.Messages;
using Civlike.World;
using Engine;
using Engine.Modularity;
using Engine.Module.Render.Entities.Providers;
using Engine.Module.Render.Input;
using Engine.Standard.Render.Entities.Services;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civlike.Client.Render.Ui;

public sealed class PopulationCenterOwnershipDebug() : UserInterfaceElementWithMessageNodeBase( "ui_debug_popcenter" ) {

	private InteractableButton[] _setNeighbourOwner = null!;
	private InteractableButton _createNewOwner = null!;
	private InteractableButton _removeOwner = null!;
	private InteractableButton _createPlayer = null!;
	private DropdownMenu<PlayerChoiceButton, PlayerChoice> _playerSelection = null!;
	private bool _updatePlayerSelection = false;

	protected override void Initialize() {
		_createNewOwner = new InteractableButton( this, "Create New Owner" );
		_createNewOwner.Placement.Set( new( (.35, -.325), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
		_createNewOwner.OnClicked += OnNewOwnerClicked;
		_removeOwner = new InteractableButton( this, "Remove popc" );
		_removeOwner.Placement.Set( new( (.35, -.55), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
		_removeOwner.OnClicked += OnRemoveOwner;
		_setNeighbourOwner = new InteractableButton[ 3 ];
		for (int i = 0; i < _setNeighbourOwner.Length; i++) {
			_setNeighbourOwner[ i ] = new InteractableButton( this, $"Set Owner #{i + 1}" );
			_setNeighbourOwner[ i ].Placement.Set( new( (.35, -.775 - i * 0.225), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
			int localI = i;
			_setNeighbourOwner[ i ].OnClicked += ( button, @event ) => OnSetNeighbourToOwner( localI );
		}
		_createPlayer = new InteractableButton( this, "Create Player" );
		_createPlayer.Placement.Set( new( (-.35, -.1), 0, (.35, .1) ), Alignment.Positive, Alignment.Positive );
		_createPlayer.OnClicked += CreateNewPlayer;
		_playerSelection = new DropdownMenu<PlayerChoiceButton, PlayerChoice>( this );
		_playerSelection.Placement.Set( new( (-.35, -.325), 0, (.35, .1) ), Alignment.Positive, Alignment.Positive );

	}

	private void CreateNewPlayer( InteractableButton button, MouseButtonEvent @event ) {
		this.MessageBusNode.Publish( new CreateNewPlayerMessage(), "gamelogic", true );
	}

	private void OnSetNeighbourToOwner( int i ) {
		Face? selectedTile = GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null)
			throw new InvalidOperationException( "No tile selected" );
		this.MessageBusNode.Publish( new SetNeighbourOwnerMessage( selectedTile, i ), "gamelogic", true );
	}

	private void OnRemoveOwner( InteractableButton button, MouseButtonEvent @event ) {
		Face? selectedTile = GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null)
			throw new InvalidOperationException( "No tile selected" );
		this.MessageBusNode.Publish( new RemoveOwnerMessage( selectedTile ), "gamelogic", true );
	}

	private void OnNewOwnerClicked( InteractableButton button, MouseButtonEvent @event ) {
		Face? selectedTile = GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null)
			throw new InvalidOperationException( "No tile selected" );
		if (_playerSelection.SelectedValue is null)
			return;
		this.MessageBusNode.Publish( new CreateNewPopulationCenterMessage( selectedTile, _playerSelection.SelectedValue.PlayerId ), "gamelogic", true );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		base.OnUpdate( time, deltaTime );
		NeighbourButtonsUpdate();
		PlayerListUpdate();
	}

	private void PlayerListUpdate() {
		if (!_updatePlayerSelection)
			return;
		_updatePlayerSelection = false;

		Engine.Module.Entities.Container.SynchronizedEntityContainer? container = UserInterfaceServiceAccess.Get<SynchronizedEntityContainerProvider>().SynchronizedContainers
			.FirstOrDefault();
		if (container is null)
			return;

		List<PlayerComponent> players = container.SynchronizedEntities.Select( p => p.EntityCopy?.GetComponentOrDefault<PlayerComponent>() ).OfType<PlayerComponent>().ToList();
		List<Guid> playerIds = players.Select( p => p.Entity.EntityId ).ToList();
		List<Guid> currentDisplayedPlayers = _playerSelection.DataSource.Select( p => p.PlayerId ).ToList();

		IEnumerable<Guid> newPlayers = playerIds.Except( currentDisplayedPlayers );
		IEnumerable<Guid> removedPlayers = currentDisplayedPlayers.Except( playerIds );

		foreach (Guid playerId in removedPlayers) {
			PlayerChoice? playerChoice = _playerSelection.DataSource.FirstOrDefault( p => p.PlayerId == playerId );
			if (playerChoice is not null)
				_playerSelection.DataSource.Remove( playerChoice );
		}
		foreach (Guid playerId in newPlayers) {
			PlayerComponent player = players.FirstOrDefault( p => p.Entity.EntityId == playerId ) ?? throw new InvalidOperationException( $"Player with id {playerId} not found" );
			_playerSelection.DataSource.Add( new PlayerChoice( playerId, player.Name, player.MapColor ) );
		}
	}

	private void NeighbourButtonsUpdate() {
		Face? selectedTile = GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null) {
			return;
		}
		Engine.Module.Entities.Container.SynchronizedEntityContainer? container = UserInterfaceServiceAccess.Get<SynchronizedEntityContainerProvider>().SynchronizedContainers
			.FirstOrDefault();
		if (container is null) {
			return;
		}
		List<FaceOwnershipComponent> focs = container.SynchronizedEntities.Select( p => p.EntityCopy?.GetComponentOrDefault<FaceOwnershipComponent>() ).OfType<FaceOwnershipComponent>().ToList();
		List<Face> neighbours = selectedTile.Blueprint.Connections.Select( p => p.GetOther( selectedTile ) ).ToList();
		for (int i = 0; i < neighbours.Count; i++) {
			Face neighbourFace = neighbours[ i ];
			FaceOwnershipComponent? neighbourOwner = focs.FirstOrDefault( p => p.OwnedFaces.Contains( neighbourFace ) );
			InteractableButton button = _setNeighbourOwner[ i ];
			if (neighbourOwner is not null) {
				button.Label.Text = $"Give";
				button.Background.Color = neighbourOwner.Entity.Parent?.GetComponentOrDefault<PlayerComponent>()?.MapColor.CastSaturating<float, double>() ?? Vector4<double>.One;
				continue;
			}
			button.Label.Text = $"No neighbour";
			button.Background.Color = Vector4<double>.One;
		}
	}

	private bool InternalShouldDisplay() {
		Face? selectedTile = GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null)
			return false;
		if (!selectedTile.State.TerrainType.Claimable)
			return false;
		Engine.Module.Entities.Container.SynchronizedEntityContainer? container = UserInterfaceServiceAccess.Get<SynchronizedEntityContainerProvider>().SynchronizedContainers
			.FirstOrDefault();
		if (container is null)
			return false;
		List<FaceOwnershipComponent> focs = container.SynchronizedEntities.Select( p => p.EntityCopy?.GetComponentOrDefault<FaceOwnershipComponent>() ).OfType<FaceOwnershipComponent>().ToList();
		if (focs.Any( p => p.OwnedFaces.Contains( selectedTile ) ))
			return false;
		return true;
	}

	protected override bool ShouldDisplay() {
		return false;// InternalShouldDisplay();
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is CreateNewPlayerMessageResponse createNewPlayerResponse) {
			_updatePlayerSelection = true;
			return;
		}
	}
}
