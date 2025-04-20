using Civs.Messages;
using Engine;
using Engine.Modularity;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civs.Render.Ui;

public sealed class NewGameMenu() : UserInterfaceElementWithMessageNodeBase( "ui_newgamemenu" ) {

	private Button _btnCreateWorld = null!;

	protected override void Initialize() {
		AddComponent( _btnCreateWorld = new Button( this, "Create World", "calibrib",
			( btn ) => btn.Background.Color = 1,
			( btn ) => btn.Background.Color = (.9, .9, .9, 1),
			( btn ) => btn.Background.Color = (.75, .75, .75, 1) ) );
		_btnCreateWorld.Placement.Set( new( (.3, -.15), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnCreateWorld.ButtonClicked += OnNewGameButtonClicked;
	}

	private void OnNewGameButtonClicked() => Publish( new CreateWorldMessage( 509600000, 9 ), "gamelogic" );

	protected override bool ShouldDisplay() {
		return GameStateProvider.Get<bool>( "showNewGameMenu" ); //TODO: Create a more complex state machine for ui?
	}

	protected override void OnMessageReceived( Message message ) {
	}
}
