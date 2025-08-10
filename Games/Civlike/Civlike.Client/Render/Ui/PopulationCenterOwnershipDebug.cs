using Civlike.Client.Render.Ui.Components;
using Civlike.Logic.Nations.ECS;
using Civlike.Messages;
using Civlike.World.State;
using Engine;
using Engine.Modularity;
using Engine.Module.Render.Entities.Providers;
using Engine.Module.Render.Input;
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
		this._createNewOwner = new InteractableButton( this, "Create New Owner" );
		this._createNewOwner.Placement.Set( new( (.35, -.325), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
		this._createNewOwner.OnClicked += OnNewOwnerClicked;
		this._removeOwner = new InteractableButton( this, "Remove popc" );
		this._removeOwner.Placement.Set( new( (.35, -.55), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
		this._removeOwner.OnClicked += OnRemoveOwner;
		this._setNeighbourOwner = new InteractableButton[ 3 ];
		for (int i = 0; i < this._setNeighbourOwner.Length; i++) {
			this._setNeighbourOwner[ i ] = new InteractableButton( this, $"Set Owner #{i + 1}" );
			this._setNeighbourOwner[ i ].Placement.Set( new( (.35, -.775 - (i * 0.225)), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
			int localI = i;
			this._setNeighbourOwner[ i ].OnClicked += ( button, @event ) => OnSetNeighbourToOwner( localI );
		}
		this._createPlayer = new InteractableButton( this, "Create Player" );
		this._createPlayer.Placement.Set( new( (-.35, -.1), 0, (.35, .1) ), Alignment.Positive, Alignment.Positive );
		this._createPlayer.OnClicked += CreateNewPlayer;
		this._playerSelection = new DropdownMenu<PlayerChoiceButton, PlayerChoice>( this );
		this._playerSelection.Placement.Set( new( (-.35, -.325), 0, (.35, .1) ), Alignment.Positive, Alignment.Positive );

	}

	private void CreateNewPlayer( InteractableButton button, MouseButtonEvent @event ) {
		this.MessageBusNode.Publish( new CreateNewPlayerMessage(), "gamelogic", true );
	}

	private void OnSetNeighbourToOwner( int i ) {
		Tile? selectedTile = this.GameStateProvider.Get<Tile>( "selectedTile" ) ?? throw new InvalidOperationException( "No tile selected" );
		this.MessageBusNode.Publish( new SetNeighbourOwnerMessage( selectedTile, i ), "gamelogic", true );
	}

	private void OnRemoveOwner( InteractableButton button, MouseButtonEvent @event ) {
		Tile? selectedTile = this.GameStateProvider.Get<Tile>( "selectedTile" ) ?? throw new InvalidOperationException( "No tile selected" );
		this.MessageBusNode.Publish( new RemoveOwnerMessage( selectedTile ), "gamelogic", true );
	}

	private void OnNewOwnerClicked( InteractableButton button, MouseButtonEvent @event ) {
		Tile? selectedTile = this.GameStateProvider.Get<Tile>( "selectedTile" ) ?? throw new InvalidOperationException( "No tile selected" );
		if (this._playerSelection.SelectedValue is null)
			return;
		this.MessageBusNode.Publish( new CreateNewPopulationCenterMessage( selectedTile, this._playerSelection.SelectedValue.PlayerId ), "gamelogic", true );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		base.OnUpdate( time, deltaTime );
		NeighbourButtonsUpdate();
		PlayerListUpdate();
	}

	private void PlayerListUpdate() {
		if (!this._updatePlayerSelection)
			return;
		this._updatePlayerSelection = false;

		Engine.Module.Entities.Container.SynchronizedEntityContainer? container = this.UserInterfaceServiceAccess.Get<SynchronizedEntityContainerProvider>().SynchronizedContainers
			.FirstOrDefault();
		if (container is null)
			return;

		List<PlayerComponent> players = [ .. container.SynchronizedEntities.Select( p => p.EntityCopy?.GetComponentOrDefault<PlayerComponent>() ).OfType<PlayerComponent>() ];
		List<Guid> playerIds = [ .. players.Select( p => p.Entity.EntityId ) ];
		List<Guid> currentDisplayedPlayers = [ .. this._playerSelection.DataSource.Select( p => p.PlayerId ) ];

		IEnumerable<Guid> newPlayers = playerIds.Except( currentDisplayedPlayers );
		IEnumerable<Guid> removedPlayers = currentDisplayedPlayers.Except( playerIds );

		foreach (Guid playerId in removedPlayers) {
			PlayerChoice? playerChoice = this._playerSelection.DataSource.FirstOrDefault( p => p.PlayerId == playerId );
			if (playerChoice is not null)
				this._playerSelection.DataSource.Remove( playerChoice );
		}
		foreach (Guid playerId in newPlayers) {
			PlayerComponent player = players.FirstOrDefault( p => p.Entity.EntityId == playerId ) ?? throw new InvalidOperationException( $"Player with id {playerId} not found" );
			this._playerSelection.DataSource.Add( new PlayerChoice( playerId, player.Name, player.MapColor ) );
		}
	}

	private void NeighbourButtonsUpdate() {
		Tile? selectedTile = this.GameStateProvider.Get<Tile>( "selectedTile" );
		if (selectedTile is null) {
			return;
		}
		Engine.Module.Entities.Container.SynchronizedEntityContainer? container = this.UserInterfaceServiceAccess.Get<SynchronizedEntityContainerProvider>().SynchronizedContainers
			.FirstOrDefault();
		if (container is null)
			return;
		List<FaceOwnershipComponent> focs = [ .. container.SynchronizedEntities.Select( p => p.EntityCopy?.GetComponentOrDefault<FaceOwnershipComponent>() ).OfType<FaceOwnershipComponent>() ];
		IReadOnlyList<Tile> neighbours = [ .. selectedTile.Neighbours ];
		for (int i = 0; i < neighbours.Count; i++) {
			Tile neighbourFace = neighbours[ i ];
			FaceOwnershipComponent? neighbourOwner = focs.FirstOrDefault( p => p.OwnedTiles.Contains( neighbourFace ) );
			InteractableButton button = this._setNeighbourOwner[ i ];
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
		Tile? selectedTile = this.GameStateProvider.Get<Tile>( "selectedTile" );
		if (selectedTile is null)
			return false;
		//if (!selectedTile.GetState<TerrainState>().TerrainType.Claimable)
		//	return false;
		Engine.Module.Entities.Container.SynchronizedEntityContainer? container = this.UserInterfaceServiceAccess.Get<SynchronizedEntityContainerProvider>().SynchronizedContainers
			.FirstOrDefault();
		if (container is null)
			return false;
		List<FaceOwnershipComponent> focs = [ .. container.SynchronizedEntities.Select( p => p.EntityCopy?.GetComponentOrDefault<FaceOwnershipComponent>() ).OfType<FaceOwnershipComponent>() ];
		if (focs.Any( p => p.OwnedTiles.Contains( selectedTile ) ))
			return false;
		return true;
	}

	protected override bool ShouldDisplay() {
		return false;// InternalShouldDisplay();
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is CreateNewPlayerMessageResponse createNewPlayerResponse) {
			this._updatePlayerSelection = true;
			return;
		}
	}
}
