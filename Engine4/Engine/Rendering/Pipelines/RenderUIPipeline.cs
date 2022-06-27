using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Data.Datatypes.Composite;
using Engine.Data.Datatypes.Projections;
using Engine.Data.Datatypes.Views;
using Engine.Rendering.Data;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.UI;
using Engine.Structure;
using OpenGL;

namespace Engine.Rendering.Pipelines;

[ProcessAfter( typeof( Render2Pipeline ), typeof( IRenderPipeline ) )]
public class RenderUIPipeline : DisposableIdentifiable, IRenderPipeline {

	private readonly SceneRenderer _uiSceneRenderer;
	private readonly UIManager _uiManager;
	private readonly View2 _uiView;
	private readonly Orthographic.Dynamic _projection;
	private readonly Camera _camera;
	private DataBlockCollection _uiRenderData;
	public readonly UniformBlock SceneCameraBlock;

	public RenderUIPipeline() {
		this._uiManager = Resources.Render.Get<UIManager>();
		this._uiView = new();
		this._projection = new Orthographic.Dynamic( new Vector2( 2 ), -1, 1 );
		this._camera = new Camera( this._uiView, this._projection );
		this._uiRenderData = new DataBlockCollection( this.SceneCameraBlock = new UniformBlock( "SceneCameraBlock", (uint) Marshal.SizeOf<SceneCameraBlock>(), ShaderType.VertexShader ) );
		this._uiSceneRenderer = new SceneRenderer( this._uiManager.Scene, this._uiRenderData, BlendFunc );
	}

	private void BlendFunc( bool transparent ) {
		if ( transparent ) {
			Gl.Enable( EnableCap.Blend );
			Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
			Gl.BlendEquation( BlendEquationMode.FuncAdd );
			Gl.DepthFunc( DepthFunction.Less );
			Gl.DepthMask( false );
		} else {
			Gl.Disable( EnableCap.Blend );
			Gl.DepthFunc( DepthFunction.Less );
			Gl.DepthMask( true );
		}
	}

	public void RenderFrame() {
		Vector2 cameraRotationRight = new( MathF.Cos( this._uiView.Rotation ), MathF.Sin( this._uiView.Rotation ) );
		Vector2 cameraRotationUp = new( -cameraRotationRight.Y, cameraRotationRight.X );
		Gl.Enable( EnableCap.Multisample );
		this.SceneCameraBlock.DirectWrite( new SceneCameraBlock( this._camera.Matrix, new Vector3( cameraRotationUp, 0 ), new Vector3( cameraRotationRight, 0 ) ) );
		Gl.Disable( EnableCap.Multisample );
	}

	public void DrawToScreen() {
		this._uiSceneRenderer.Render( this._uiRenderData, prim: Resources.Render.Window.KeyboardEvents[ GLFW.Keys.X ] ? PrimitiveType.Lines : PrimitiveType.Triangles );
	}

	protected override bool OnDispose() {
		return true;
	}
}