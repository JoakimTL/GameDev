using Civs.Messages;
using Civs.World;
using Engine;
using Engine.Modularity;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civs.Render.Ui;

public sealed class PopulationCenterOwnershipDebug() : UserInterfaceElementWithMessageNodeBase( "ui_debug_popcenter" ) {

	private Label _ownerLabel = null!;
	private Button[] _setNeighbourOwner = null!;
	private Button _createNewOwner = null!;
	private Button _removeOwner = null!;

	protected override void Initialize() {
		//AddComponent( _ownerLabel = new Label( this ) {
		//	Text = "noupdate",
		//	FontName = "COURBD",
		//	TextScale = 0.15f,
		//	Color = (1, 1, 1, 1),
		//	HorizontalAlignment = Alignment.Negative,
		//	VerticalAlignment = Alignment.Center
		//} );
		//_ownerLabel.Placement.Set( new( (1, -.1), 0, (1, .1) ), Alignment.Negative, Alignment.Positive );
		//AddComponent( _createNewOwner = new Button( this, "Create New Owner", "COURBD",
		//	( btn ) => btn.Background.Color = 1,
		//	( btn ) => btn.Background.Color = (.9, .9, .9, 1),
		//	( btn ) => btn.Background.Color = (.75, .75, .75, 1) )
		//);
		//_createNewOwner.Placement.Set( new( (.35, -.325), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
		//_createNewOwner.ButtonClicked += OnNewOwnerClicked;
		//AddComponent( _removeOwner = new Button( this, "Remove popc", "COURBD",
		//	( btn ) => btn.Background.Color = 1,
		//	( btn ) => btn.Background.Color = (.9, .9, .9, 1),
		//	( btn ) => btn.Background.Color = (.75, .75, .75, 1) )
		//);
		//_removeOwner.Placement.Set( new( (.35, -.55), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
		//_removeOwner.ButtonClicked += OnRemoveOwner;
		//_setNeighbourOwner = new Button[ 3 ];
		//for (int i = 0; i < _setNeighbourOwner.Length; i++) {
		//	AddComponent( _setNeighbourOwner[ i ] = new Button( this, $"Set Owner #{i + 1}", "COURBD",
		//		( btn ) => btn.Background.Color = 1,
		//		( btn ) => btn.Background.Color = (.9, .9, .9, 1),
		//		( btn ) => btn.Background.Color = (.75, .75, .75, 1) )
		//	);
		//	_setNeighbourOwner[ i ].Placement.Set( new( (.35, -.775 - i * 0.225), 0, (.35, .1) ), Alignment.Negative, Alignment.Positive );
		//	int localI = i;
		//	_setNeighbourOwner[ i ].ButtonClicked += () => OnSetNeighbourToOwner( localI );
		//}

	}

	//private void OnSetNeighbourToOwner( int i ) {
	//	Tile? selectedTile = GameStateProvider.Get<Tile>( "selectedTile" );
	//	if (selectedTile is null)
	//		throw new InvalidOperationException( "No tile selected" );
	//	this.MessageBusNode.Publish( new SetNeighbourOwnerMessage( selectedTile, i ), "gamelogic" );
	//}

	//private void OnRemoveOwner() {
	//	Tile? selectedTile = GameStateProvider.Get<Tile>( "selectedTile" );
	//	if (selectedTile is null)
	//		throw new InvalidOperationException( "No tile selected" );
	//	this.MessageBusNode.Publish( new RemoveOwnerMessage( selectedTile ), "gamelogic" );
	//}

	//private void OnNewOwnerClicked() {
	//	Tile? selectedTile = GameStateProvider.Get<Tile>( "selectedTile" );
	//	if (selectedTile is null)
	//		throw new InvalidOperationException( "No tile selected" );
	//	this.MessageBusNode.Publish( new CreateNewOwnerMessage( selectedTile ), "gamelogic" );
	//}

	protected override void OnUpdate( double time, double deltaTime ) {
		base.OnUpdate( time, deltaTime );
	}

	protected override bool ShouldDisplay() {
		return false;//		GameStateProvider.Get<Tile>( "selectedTile" ) is not null;
	}

	protected override void OnMessageReceived( Message message ) {

	}
}
