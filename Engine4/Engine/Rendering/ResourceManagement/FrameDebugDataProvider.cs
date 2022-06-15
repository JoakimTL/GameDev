using Engine.Rendering.Utilities;
using Engine.Structure;

namespace Engine.Rendering.ResourceManagement;

public class FrameDebugDataProvider : Identifiable, IUpdateable {

	public float FrameTime => this._fpsTally.AverageDeltaTime * 1000;
	public float FramesPerSecond => this._fpsTally.AverageFramesPerSecond;
	public string FrameRateInformation => $"{  this._fpsTally.AverageDeltaTime * 1000:N4}ms/{this._fpsTally.AverageFramesPerSecond:N2}FPS::{this.TitleInfo ?? "No info"}";
	public string? TitleInfo { get; set; }

	public bool Active => true;
	private readonly FramerateTallyer _fpsTally;

	public FrameDebugDataProvider() {
		this._fpsTally = new FramerateTallyer( 120 );
	}

	public void Update( float time, float deltaTime ) => this._fpsTally.Tally();
}

