using Civlike.Messages;
using Engine;
using Engine.Modularity;
using Engine.Module.Render.Input;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civlike.Client.Render.Ui;

public sealed class NewGameMenu() : UserInterfaceElementWithMessageNodeBase( "\b(?:globe-tracking|ui_newgame)\b" ) {

	private InteractableButton _btnCreateWorld = null!;

	protected override void Initialize() {
		_btnCreateWorld = new InteractableButton( this, "Create World" );
		_btnCreateWorld.Placement.Set( new( (.3, -.15), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnCreateWorld.OnClicked += OnNewGameButtonClicked;
	}

	private void OnNewGameButtonClicked( InteractableButton btn, MouseButtonEvent @event )
		=> Publish( new CreateNewWorldRequestMessage( new( 7, 6378000, 43, 400, -400, 700, 9000, 4000, double.Pi * 2 / (24 * 60 * 60), double.Pi * 2 / (365.2422 * 24 * 60 * 60), 23.5f, 128, 8 ) ), "gamelogic", true );

	protected override bool ShouldDisplay() {
		return GameStateProvider.Get<bool>( UiElementConstants.ShowNewGameMenu ); //TODO: Create a more complex state machine for ui?
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is CreateNewWorldRequestResponseMessage)
			GameStateProvider.SetNewState( UiElementConstants.ShowNewGameMenu, false );
	}
}
