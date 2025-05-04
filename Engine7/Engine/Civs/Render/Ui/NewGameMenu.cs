using Civs.Messages;
using Engine;
using Engine.Modularity;
using Engine.Module.Render.Input;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civs.Render.Ui;

public sealed class NewGameMenu() : UserInterfaceElementWithMessageNodeBase( "ui_newgamemenu" ) {

	private InteractableButton _btnCreateWorld = null!;

	protected override void Initialize() {
		_btnCreateWorld = new InteractableButton( this, "Create World" );
		_btnCreateWorld.Placement.Set( new( (.3, -.15), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnCreateWorld.OnClicked += OnNewGameButtonClicked;
	}

	private void OnNewGameButtonClicked( InteractableButton btn, MouseButtonEvent @event ) => Publish( new CreateNewWorldRequestMessage( new( 6, 6378000, 42, 25000, 17500 ) ), "gamelogic", true );

	protected override bool ShouldDisplay() {
		return GameStateProvider.Get<bool>( "showNewGameMenu" ); //TODO: Create a more complex state machine for ui?
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is CreateNewWorldRequestResponseMessage) {
			GameStateProvider.SetNewState( "showNewGameMenu", false );

		}
	}
}
