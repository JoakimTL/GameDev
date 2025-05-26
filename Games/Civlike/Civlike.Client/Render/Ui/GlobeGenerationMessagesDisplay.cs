using Civlike.Messages;
using Engine;
using Engine.Modularity;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civlike.Client.Render.Ui;

public sealed class GlobeGenerationMessagesDisplay() : UserInterfaceElementWithMessageNodeBase( "ui_globegen" ) {

	private readonly List<string> _worldGenerationMessages = [];
	private readonly List<double> _worldGenerationTimes = [];
	private string? _subProgressMessage = null;
	private double? _currentStepProgress = null;
	private double _lastStepStarted = 0;
	private double _lastUpdateTime = 0;
	private Label _progressLabel = null!;

	protected override void Initialize() {
		this._progressLabel = new Label( this ) {
			Text = "No world generation in progress",
			FontName = "calibrib",
			TextScale = 0.06f,
			Color = (1, 1, 1, 1),
			HorizontalAlignment = Alignment.Positive,
			VerticalAlignment = Alignment.Negative
		};
		this._progressLabel.Placement.Set( new( (-.4, .3), 0, (.4, .3) ), Alignment.Positive, Alignment.Negative );
	}

	protected override bool ShouldDisplay() {
		return true; //TODO: Create a more complex state machine for ui?
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		_lastUpdateTime = time;
		base.OnUpdate( time, deltaTime );
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is WorldGenerationProgressMessage worldGenerationProgress) {
			this._worldGenerationMessages.Add( worldGenerationProgress.ProgressMessage );
			if (_lastStepStarted != 0) {
				double timeTaken = _lastUpdateTime - _lastStepStarted;
				this._worldGenerationTimes.Add( timeTaken );
			}
			this._currentStepProgress = null;
			_subProgressMessage = null;
			_lastStepStarted = _lastUpdateTime;
			UpdateGenerationDisplay();
		}
		if (message.Content is WorldGenerationCompleteMessage worldGenerationComplete) {
			this._worldGenerationMessages.Add( worldGenerationComplete.Message );
			double timeTaken = _lastUpdateTime - _lastStepStarted;
			this._worldGenerationTimes.Add( timeTaken );
			this._currentStepProgress = null;
			_subProgressMessage = null;
			_lastStepStarted = _lastUpdateTime;
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
				labelText += $"{message}...Done! ({_worldGenerationTimes[ i ]:N2}s){Environment.NewLine}";
				continue;
			}
			labelText += $"{message}...{(this._currentStepProgress.HasValue ? $"{this._currentStepProgress.Value:N1}%" : "")}{Environment.NewLine}";
			if (_subProgressMessage is not null)
				labelText += $"{_subProgressMessage}{Environment.NewLine}";
		}
		this._progressLabel.Text = labelText;
	}
}
