using Engine.Structure;
using OpenGL;

namespace Engine.Rendering.Pipelines;
public class RenderPipelineManager : SingletonProvider<IRenderPipeline> {

	private readonly BidirectionalTreeStructureProvider _pipelineTypeTree;
	private readonly List<IRenderPipeline> _sortedPipelines;

	public RenderPipelineManager() {
		this._pipelineTypeTree = new( typeof( IRenderPipeline ) );
		this._sortedPipelines = new();
		this._pipelineTypeTree.TreeUpdated += TreeUpdated;
		SingletonAdded += CheckPipeline;
	}

	private void TreeUpdated() {
		this._sortedPipelines.Clear();
		this._sortedPipelines.AddRange( this._pipelineTypeTree.WalkTreeForward().Select( GetOrDefault ).OfType<IRenderPipeline>() );
	}

	private void CheckPipeline( object obj ) {
		if ( obj is IRenderPipeline )
			this._pipelineTypeTree.Add( obj.GetType() );
	}

	public void AddPipeline<T>() where T : IRenderPipeline => Add<T>();

	public void Render() {
		this._pipelineTypeTree.Update();
		for ( int i = 0; i < this._sortedPipelines.Count; i++ )
			this._sortedPipelines[ i ].RenderFrame();

		Resources.Render.Window.Bind();
		Gl.ClearColor( 0, 0, 0, 0 );
		Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		Gl.Enable( EnableCap.Blend );
		Gl.Disable( EnableCap.DepthTest );
		Gl.Disable( EnableCap.CullFace );

		for ( int i = 0; i < this._sortedPipelines.Count; i++ )
			this._sortedPipelines[ i ].DrawToScreen();

		Gl.Disable( EnableCap.Blend );
	}
}
