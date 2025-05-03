using Civs.Logic.Nations;
using Civs.Messages;
using Civs.World.NewWorld;
using Engine;
using Engine.Modularity;
using Engine.Module.Render.Input;
using Engine.Standard.Render.Entities.Services;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civs.Render.Ui;

public sealed class PopulationCenterOwnershipDebug() : UserInterfaceElementWithMessageNodeBase( "ui_debug_popcenter" ) {

	private Label _ownerLabel = null!;
	private Button[] _setNeighbourOwner = null!;
	private Button _createNewOwner = null!;
	private Button _removeOwner = null!;
	private Button _createPlayer = null!;
	private DropdownMenu<PlayerChoice> _playerSelection = null!;
	private bool _updatePlayerSelection = false;

	protected override void Initialize() {
		_ownerLabel = new Label( this ) {
			Text = "noupdate",
			FontName = "COURBD",
			TextScale = 0.15f,
			Color = (1, 1, 1, 1),
			HorizontalAlignment = Alignment.Negative,
			VerticalAlignment = Alignment.Center
		};
		_ownerLabel.Placement.Set( new( (1, -.1), 0, (1, .1) ), Alignment.Negative, Alignment.Positive );
		_createNewOwner = new Button( this, "Create New Owner" );
		_createNewOwner.Placement.Set( new( (.35, -.325), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
		_createNewOwner.OnClicked += OnNewOwnerClicked;
		_removeOwner = new Button( this, "Remove popc" );
		_removeOwner.Placement.Set( new( (.35, -.55), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
		_removeOwner.OnClicked += OnRemoveOwner;
		_setNeighbourOwner = new Button[ 3 ];
		for (int i = 0; i < _setNeighbourOwner.Length; i++) {
			_setNeighbourOwner[ i ] = new Button( this, $"Set Owner #{i + 1}" );
			_setNeighbourOwner[ i ].Placement.Set( new( (.35, -.775 - i * 0.225), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
			int localI = i;
			_setNeighbourOwner[ i ].OnClicked += ( Button button, MouseButtonEvent @event ) => OnSetNeighbourToOwner( localI );
		}
		_createPlayer = new Button( this, "Create Player" );
		_createPlayer.Placement.Set( new( (-.35, -.1), 0, (.35, .1) ), Alignment.Positive, Alignment.Positive );
		_createPlayer.OnClicked += CreateNewPlayer;
		_playerSelection = new DropdownMenu<PlayerChoice>( this );
		_playerSelection.Placement.Set( new( (-.35, -.325), 0, (.35, .1) ), Alignment.Positive, Alignment.Positive );

	}

	private void CreateNewPlayer( Button button, MouseButtonEvent @event ) {
		this.MessageBusNode.Publish( new CreateNewPlayerMessage(), "gamelogic", true );
	}

	private void OnSetNeighbourToOwner( int i ) {
		Face? selectedTile = GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null)
			throw new InvalidOperationException( "No tile selected" );
		this.MessageBusNode.Publish( new SetNeighbourOwnerMessage( selectedTile, i ), "gamelogic", true );
	}

	private void OnRemoveOwner( Button button, MouseButtonEvent @event ) {
		Face? selectedTile = GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null)
			throw new InvalidOperationException( "No tile selected" );
		this.MessageBusNode.Publish( new RemoveOwnerMessage( selectedTile ), "gamelogic", true );
	}

	private void OnNewOwnerClicked( Button button, MouseButtonEvent @event ) {
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

		var container = UserInterfaceServiceAccess.Get<RenderEntityContainerProvider>().Containers
			.FirstOrDefault();
		if (container is null)
			return;

		var players = container.RenderEntities.Select( p => p.Entity.GetComponentOrDefault<PlayerComponent>() ).OfType<PlayerComponent>().ToList();

		_playerSelection.DataSource.Clear();
		int i = 0;
		foreach (var player in players) {
			_playerSelection.DataSource.Add( new PlayerChoice( player.Entity.EntityId, $"test{i++}" ) );
		}
	}

	private void NeighbourButtonsUpdate() {
		Face? selectedTile = GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null) {
			_ownerLabel.Text = "No tile selected";
			return;
		}
		var container = UserInterfaceServiceAccess.Get<RenderEntityContainerProvider>().Containers
			.FirstOrDefault();
		if (container is null) {
			_ownerLabel.Text = "No owner";
			return;
		}
		var focs = container.RenderEntities.Select( p => p.Entity.GetComponentOrDefault<FaceOwnershipComponent>() ).OfType<FaceOwnershipComponent>().ToList();
		var neighbours = selectedTile.Blueprint.Connections.Select( p => p.GetOther( selectedTile ) ).ToList();
		for (int i = 0; i < neighbours.Count; i++) {
			var neighbourFace = neighbours[ i ];
			var neighbourOwner = focs.FirstOrDefault( p => p.OwnedFaces.Contains( neighbourFace ) );
			var button = _setNeighbourOwner[ i ];
			if (neighbourOwner is not null) {
				button.Label.Text = $"Give";
				button.Background.Color = neighbourOwner.Entity.GetComponentOrDefault<PlayerComponent>()?.MapColor.CastSaturating<float, double>() ?? Vector4<double>.One;
				continue;
			}
			button.Label.Text = $"No neighbour";
			button.Background.Color = Vector4<double>.One;
		}
	}

	protected override bool ShouldDisplay() {
		return GameStateProvider.Get<Face>( "selectedTile" ) is not null;
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is CreateNewPlayerMessageResponse createNewPlayerResponse) {
			_updatePlayerSelection = true;
			return;
		}
	}
}

public sealed class PlayerChoice {

	public Guid PlayerId { get; }
	public string PlayerName { get; }
	public PlayerChoice( Guid playerId, string playerName ) {
		PlayerId = playerId;
		PlayerName = playerName;
	}
	public override string ToString() => PlayerName;
}