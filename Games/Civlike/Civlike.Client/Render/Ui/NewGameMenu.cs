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

	private readonly List<string> _worldGenerationMessages = [];
	private string? _subProgressMessage = null;
	private double? _currentStepProgress = null;
	private Label _progressLabel = null!;
	private InteractableButton _btnCreateWorld = null!;

	protected override void Initialize() {
		this._progressLabel = new Label( this ) {
			Text = "No world generation in progress",
			FontName = "calibrib",
			TextScale = 0.06f,
			Color = (1, 1, 1, 1),
			HorizontalAlignment = Alignment.Positive,
			VerticalAlignment = Alignment.Negative
		};
		this._progressLabel.Placement.Set( new( (-.4, .3 ), 0, (.4, .3 ) ), Alignment.Positive, Alignment.Negative );
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

	//=> Publish( new CreateNewGlobeRequestMessage( new( 8, 6378000, 43, 1400, -1100, 1400, 14000, 7000, double.Pi * 2 / (24 * 60 * 60), double.Pi * 2 / (365.2422 * 24 * 60 * 60), 23.5f, 128, 6 ) ), "gamelogic", true );

	protected override bool ShouldDisplay() {
		return this.GameStateProvider.Get<bool>( UiElementConstants.ShowNewGameMenu ); //TODO: Create a more complex state machine for ui?
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is CreateNewWorldRequestResponseMessage)
			this.GameStateProvider.SetNewState( UiElementConstants.ShowNewGameMenu, false );
		if (message.Content is WorldGenerationProgressMessage worldGenerationProgress) {
			this._worldGenerationMessages.Add( worldGenerationProgress.ProgressMessage );
			this._currentStepProgress = null;
			_subProgressMessage = null;
			UpdateGenerationDisplay();
		}
		if (message.Content is WorldGenerationStepProgressPercentMessage worldGenerationStepProgressPercent) {
			this._currentStepProgress = worldGenerationStepProgressPercent.ProgressPercent;
			UpdateGenerationDisplay();
		}
		if (message.Content is WorldGenerationSubProgressMessage worldGenerationSubProgress) {
			this._subProgressMessage = worldGenerationSubProgress.SubProgressMessage;
			UpdateGenerationDisplay();
		}
	}

	private void UpdateGenerationDisplay() {
		string labelText = "";
		for (int i = 0; i < this._worldGenerationMessages.Count; i++) {
			string message = this._worldGenerationMessages[ i ];
			bool isLastMessage = i == this._worldGenerationMessages.Count - 1;
			if (!isLastMessage) {
				labelText += $"{message}...Done!{Environment.NewLine}";
				continue;
			}
			labelText += $"{message}...{(this._currentStepProgress.HasValue ? $"{this._currentStepProgress.Value:N1}%" : "")}{Environment.NewLine}";
			if (_subProgressMessage is not null)
				labelText += $"{_subProgressMessage}{Environment.NewLine}";
		}
		this._progressLabel.Text = labelText;
	}
}
