using Civlike.Messages;
using Civlike.World;
using Engine;
using Engine.Modularity;
using Engine.Module.Render.Input;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civlike.Client.Render.Ui;

public sealed class NewGameMenu() : UserInterfaceElementWithMessageNodeBase( "\b(?:globe-tracking|ui_newgame)\b" ) {

	private Label _progressLabel = null!;
	private InteractableButton _btnCreateWorld = null!;

	protected override void Initialize() {
		_progressLabel = new Label( this ) {
			Text = "No world generation in progress",
			FontName = "calibrib",
			TextScale = 0.04f,
			Color = (1, 1, 1, 1),
			HorizontalAlignment = Alignment.Positive,
			VerticalAlignment = Alignment.Negative
		};
		_progressLabel.Placement.Set( new( (-.3, .3 ), 0, (.3, .3 ) ), Alignment.Positive, Alignment.Negative );
		_btnCreateWorld = new InteractableButton( this, "Create World" );
		_btnCreateWorld.Placement.Set( new( (.3, -.15), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnCreateWorld.OnClicked += OnNewGameButtonClicked;
	}

	private void OnNewGameButtonClicked( InteractableButton btn, MouseButtonEvent @event )
		=> Publish( new CreateNewWorldRequestMessage( new( 8, 6378000, 43, 400, -400, 700, 9000, 4000, double.Pi * 2 / (24 * 60 * 60), double.Pi * 2 / (365.2422 * 24 * 60 * 60), 23.5f, 128, 6 ) ), "gamelogic", true );

	protected override bool ShouldDisplay() {
		return GameStateProvider.Get<bool>( UiElementConstants.ShowNewGameMenu ); //TODO: Create a more complex state machine for ui?
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is CreateNewWorldRequestResponseMessage)
			GameStateProvider.SetNewState( UiElementConstants.ShowNewGameMenu, false );
		if (message.Content is WorldGenerationProgressMessage worldGenerationProgress) {
			if (!string.IsNullOrEmpty( _progressLabel.Text ))
				_progressLabel.Text += Environment.NewLine;
			_progressLabel.Text += worldGenerationProgress.ProgressMessage;
		}
	}
}
