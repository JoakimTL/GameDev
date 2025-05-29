using Civlike.Messages;
using Engine;
using Engine.Modularity;
using Engine.Module.Render.Input;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;
using Civlike.World.TectonicGeneration;
using Civlike.World.TectonicGeneration.Steps;

namespace Civlike.Client.Render.Ui;

public sealed class NewGameMenu() : UserInterfaceElementWithMessageNodeBase( "\b(?:globe-tracking|ui_newgame)\b" ) {

	private InteractableButton _btnCreateWorld = null!;

	protected override void Initialize() {
		this._btnCreateWorld = new InteractableButton( this, "Create World" );
		this._btnCreateWorld.Placement.Set( new( (.3, -.15), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		this._btnCreateWorld.OnClicked += OnNewGameButtonClicked;
	}

	private void OnNewGameButtonClicked( InteractableButton btn, MouseButtonEvent @event ) {
		TectonicParameters tectonicParameters = new() {
			BaseHeightVariance = 150,
			PlateCountBase = 40,
			PlateCountVariance = 10,
			PlateHeight = -200,
			PlateHeightVariance = 400,
			FaultMaxHeight = 8000,
			MountainHeight = 5000,
			OceanSeeds = 8
		};
		Publish( new CreateNewGlobeRequestMessage<TectonicGeneratingGlobe, TectonicGlobeParameters>( new( 7, 6378000, 43, 128, tectonicParameters ) ), "gamelogic", true );
	}
	protected override bool ShouldDisplay() {
		return this.GameStateProvider.Get<bool>( UiElementConstants.ShowNewGameMenu ); //TODO: Create a more complex state machine for ui?
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is CreateNewWorldRequestResponseMessage)
			this.GameStateProvider.SetNewState( UiElementConstants.ShowNewGameMenu, false );
	}
}
