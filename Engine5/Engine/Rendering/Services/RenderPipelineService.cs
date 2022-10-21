using Engine.Rendering.Objects;

namespace Engine.Rendering.Services;

public sealed class RenderPipelineService : IContextService {
	private readonly Window _window;
	private readonly List<IRenderPipeline> _pipelines;

	public RenderPipelineService( Window window ) {
		_pipelines = new();
		this._window = window;
	}

	public void Add( IRenderPipeline pipeline ) => _pipelines.Add( pipeline );
	public void Remove( IRenderPipeline pipeline ) => _pipelines.Remove( pipeline );

	public void Render() {
		for ( int i = 0; i < _pipelines.Count; i++ )
			_pipelines[ i ].Render();

		_window.Bind();
		for ( int i = 0; i < _pipelines.Count; i++ )
			_pipelines[ i ].DrawToScreen();
	}

}
