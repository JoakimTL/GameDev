using Engine.Structure;
using Engine.Structure.Interfaces;

namespace Engine.Rendering.Contexts.Services;

public sealed class RenderPipelineService : IContextService, IUpdateable
{
	private readonly Window _window;
	private readonly Context _context;
	private readonly Dictionary<Type, IRenderPipeline> _pipelines;
	private readonly BidirectionalTypeTree _renderPipelineTree;
	private readonly List<IRenderPipeline> _sortedRenderPipelines;

	public RenderPipelineService(Window window, Context context)
	{
		_pipelines = new();
		_window = window;
		_context = context;
		_renderPipelineTree = new(typeof(IRenderPipeline));
		_sortedRenderPipelines = new();
	}

	public void Add<T>() where T : IRenderPipeline
	{
		Type t = typeof(T);
		if (_pipelines.ContainsKey(t))
			return;
		var pipelineObject = t.GetInjectedInstance(_context.Injector);
		if (pipelineObject is not T pipeline)
			return;
		_pipelines.Add(t, pipeline);
		_renderPipelineTree.Add(t);
		_sortedRenderPipelines.Clear();
		_sortedRenderPipelines.AddRange(_renderPipelineTree.GetNodesSorted().Select(p => _pipelines[p]));
	}

	public void Remove<T>() where T : IRenderPipeline
	{
		Type t = typeof(T);
		if (_pipelines.Remove(t, out var pipeline))
		{
			_renderPipelineTree.Remove(t);
			_sortedRenderPipelines.Remove(pipeline);
		}
	}

	public void Update(float time, float deltaTime)
	{
		for (int i = 0; i < _sortedRenderPipelines.Count; i++)
			_sortedRenderPipelines[i].Update(time, deltaTime);
		_window.Bind();
		for (int i = 0; i < _sortedRenderPipelines.Count; i++)
			_sortedRenderPipelines[i].DrawToScreen();
	}

}
