using Engine.Rendering.Contexts.Objects.VAOs;
using Engine.Structure;
using Engine.Structure.Interfaces;
using OpenGL;

namespace Engine.Rendering.Contexts.Services;

public sealed class RenderPipelineService : IContextService, IUpdateable
{
    private readonly Window _window;
    private readonly Context _context;
    private readonly Dictionary<Type, IRenderPipeline> _pipelines;
    private readonly BidirectionalTypeTree _renderPipelineTree;
    private readonly List<IRenderPipeline> _sortedRenderPipelines;
    private readonly List<IInitializable> _initalizationList;

    public RenderPipelineService(Window window, Context context)
    {
        _pipelines = new();
        _window = window;
        _context = context;
        _renderPipelineTree = new(typeof(IRenderPipeline));
        _initalizationList = new();
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
        if (pipeline is IInitializable initializable)
            _initalizationList.Add(initializable);
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
        foreach (var initializable in _initalizationList)
            initializable.Initialize();
        _initalizationList.Clear();
        for (int i = 0; i < _sortedRenderPipelines.Count; i++)
            _sortedRenderPipelines[i].Update(time, deltaTime);

        _window.Bind();
        Gl.ClearColor(0f, 0f, 0f, 0f);
        Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Gl.Enable(EnableCap.Blend);
        Gl.BlendFunc(BlendingFactor.One, BlendingFactor.One);
        Gl.Disable(EnableCap.DepthTest);
        Gl.Disable(EnableCap.CullFace);
        for (int i = 0; i < _sortedRenderPipelines.Count; i++)
            _sortedRenderPipelines[i].DrawToScreen();
        Gl.Disable(EnableCap.Blend);
    }

}
