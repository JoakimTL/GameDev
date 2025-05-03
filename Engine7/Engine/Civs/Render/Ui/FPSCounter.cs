using Engine;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civs.Render.Ui;

public sealed class FPSCounter : UserInterfaceElementBase {

	private Label _fpsLabel = null!;

	private int _sampleIndex = 0;
	private readonly double[] _samples = new double[ 30 ];

	protected override void Initialize() {
		_fpsLabel = new Label( this ) {
			Text = "noupdate",
			FontName = "COURBD",
			TextScale = 0.15f,
			Color = (1, 1, 1, 1),
			HorizontalAlignment = Alignment.Negative,
			VerticalAlignment = Alignment.Negative
		};
		_fpsLabel.Placement.Set( new( (1, .1), 0, (1, .1) ), Alignment.Negative, Alignment.Negative );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		_samples[ _sampleIndex++ ] = deltaTime;
		if (_sampleIndex >= _samples.Length)
			_sampleIndex = 0;
		double sampleSum = 0;
		for (int i = 0; i < _samples.Length; i++)
			sampleSum += _samples[ i ];
		double average = sampleSum / _samples.Length;
		_fpsLabel.Text = $"Frametime: {average * 1000:#,##0.00} ms FPS: {(1 / average):#,##0.0} f/s";
	}

	protected override bool ShouldDisplay() {
		return GameStateProvider.Get<bool>( "showFpsCounter" );
	}
}
