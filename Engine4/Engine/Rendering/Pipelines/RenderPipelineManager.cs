using Engine.Structure;
using OpenGL;

namespace Engine.Rendering.Pipelines;
public class RenderPipelineManager : ModuleService {

	//TODO Fix window parameter
	private readonly ServiceProvider<IRenderPipeline> _pipelines;
	private readonly BidirectionalTreeStructureProvider _pipelineTypeTree;
	private readonly List<IRenderPipeline> _sortedPipelines;

	public RenderPipelineManager() {
		this._pipelines = new();
		this._pipelines.ServiceAdded += PipelineAdded;
		this._pipelineTypeTree = new( typeof( IRenderPipeline ) );
		this._sortedPipelines = new();
		this._pipelineTypeTree.TreeUpdated += TreeUpdated;
	}

	private void TreeUpdated() {
		this._sortedPipelines.Clear();
		this._sortedPipelines.AddRange( this._pipelineTypeTree.GetNodesSorted().Select( this._pipelines.Peek ).OfType<IRenderPipeline>() );
	}

	private void PipelineAdded( object obj ) {
		if ( obj is IRenderPipeline )
			this._pipelineTypeTree.Add( obj.GetType() );
	}

	public void AddPipeline<T>() where T : IRenderPipeline => this._pipelines.GetOrAdd<T>();

	public void Render() {
		this._pipelineTypeTree.Update();
		for ( int i = 0; i < this._sortedPipelines.Count; i++ )
			this._sortedPipelines[ i ].RenderFrame();

		Resources.Render.Window.Bind();
		Gl.ClearColor( 0, 0, 0, 0 );
		Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		Gl.Enable( EnableCap.Blend );
		Gl.BlendFunc( BlendingFactor.One, BlendingFactor.One );
		Gl.Disable( EnableCap.DepthTest );
		Gl.Disable( EnableCap.CullFace );

		for ( int i = 0; i < this._sortedPipelines.Count; i++ )
			this._sortedPipelines[ i ].DrawToScreen();

		Gl.Disable( EnableCap.Blend );
	}

	protected override bool OnDispose() {
		this._pipelines.Dispose();
		return true;
	}
}
