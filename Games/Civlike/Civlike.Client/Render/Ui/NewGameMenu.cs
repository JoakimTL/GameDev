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
			BaseHeightVariance = 1400,
			PlateCountBase = 60,
			PlateCountVariance = 20,
			PlateHeight = -1100,
			PlateHeightVariance = 1400,
			FaultMaxHeight = 14000,
			MountainHeight = 7000,
			OceanSeeds = 8
		};
		Publish( new CreateNewGlobeRequestMessage<TectonicGeneratingGlobe, TectonicGlobeParameters>( new( 8, 6378000, 43, 128, tectonicParameters ) ), "gamelogic", true );
	}
	protected override bool ShouldDisplay() {
		return this.GameStateProvider.Get<bool>( UiElementConstants.ShowNewGameMenu ); //TODO: Create a more complex state machine for ui?
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is CreateNewWorldRequestResponseMessage)
			this.GameStateProvider.SetNewState( UiElementConstants.ShowNewGameMenu, false );
	}
}
