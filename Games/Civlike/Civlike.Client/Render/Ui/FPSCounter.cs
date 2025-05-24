using Engine;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civlike.Client.Render.Ui;

public sealed class FPSCounter : UserInterfaceElementBase {

	private Label _fpsLabel = null!;

	private int _sampleIndex = 0;
	private readonly double[] _samples = new double[ 30 ];

	protected override void Initialize() {
		this._fpsLabel = new Label( this ) {
			Text = "noupdate",
			FontName = "COURBD",
			TextScale = 0.15f,
			Color = (1, 1, 1, 1),
			HorizontalAlignment = Alignment.Negative,
			VerticalAlignment = Alignment.Negative
		};
		this._fpsLabel.Placement.Set( new( (1, .1), 0, (1, .1) ), Alignment.Negative, Alignment.Negative );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		this._samples[ this._sampleIndex++ ] = deltaTime;
		if (this._sampleIndex >= this._samples.Length)
			this._sampleIndex = 0;
		double sampleSum = 0;
		for (int i = 0; i < this._samples.Length; i++)
			sampleSum += this._samples[ i ];
		double average = sampleSum / this._samples.Length;
		this._fpsLabel.Text = $"Frametime: {average * 1000:#,##0.00} ms FPS: {1 / average:#,##0.0} f/s";
	}

	protected override bool ShouldDisplay() {
		return this.GameStateProvider.Get<bool>( UiElementConstants.ShowFPSCounter );
	}
}

public sealed class UiElementConstants {
	public const string ShowFPSCounter = "showFpsCounter";
	public const string ShowNewGameMenu = "showNewGameMenu";
	public const string ShowMainMenu = "showMainMenu";
	public const string ShowStartMenu = "showStartMenu";
}