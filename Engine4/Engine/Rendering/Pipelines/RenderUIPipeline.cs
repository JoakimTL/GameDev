using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Data.Datatypes.Composite;
using Engine.Data.Datatypes.Projections;
using Engine.Data.Datatypes.Views;
using Engine.Rendering.Data;
using Engine.Rendering.Framebuffers;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.UI;
using Engine.Rendering.Utilities;
using Engine.Structure;
using OpenGL;
using static Engine.Rendering.Pipelines.Render3Pipeline;

namespace Engine.Rendering.Pipelines;

[ProcessAfter( typeof( Render3Pipeline ), typeof( IRenderPipeline ) )]
[ProcessAfter( typeof( Render2Pipeline ), typeof( IRenderPipeline ) )]
public class RenderUIPipeline : DisposableIdentifiable, IRenderPipeline {

	private readonly SceneRenderer _uiSceneRenderer;
	private readonly UIManager _uiManager;
	private readonly View2 _uiView;
	private readonly Orthographic.Dynamic _projection;
	private readonly Camera _camera;
	private DataBlockCollection _uiRenderData;
	public readonly UniformBlock SceneCameraBlock;
	public readonly UIFramebuffer UIBuffer;
	public readonly ScreenFramebuffer UIToScreenBuffer;
	private readonly DataBlockCollection _pfxUniforms;
	private readonly UniformBlock _pfxBlock;

	public RenderUIPipeline( Window window ) {
		this._uiManager = new UIManager( window );
		this._uiView = new();
		this._projection = new Orthographic.Dynamic( window, new Vector2( 2 ), -1, 1 );
		this._camera = new Camera( this._uiView, this._projection );
		this._uiRenderData = new DataBlockCollection( this.SceneCameraBlock = new UniformBlock( "SceneCameraBlock", (uint) Marshal.SizeOf<SceneCameraBlock>(), ShaderType.VertexShader ) );
		this._pfxUniforms = new DataBlockCollection( this._pfxBlock = new UniformBlock( "PFXBlock", 24, ShaderType.FragmentShader ) );
		this._uiSceneRenderer = new SceneRenderer( this._uiManager.Scene, this._uiRenderData, BlendFunc );
		this.UIBuffer = new( window, 2 );
		this.UIToScreenBuffer = new( window, 2 );
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
		this.SceneCameraBlock.DirectWrite( new SceneCameraBlock( this._camera.Matrix, new Vector3( cameraRotationUp, 0 ), new Vector3( cameraRotationRight, 0 ) ) );
		Gl.Enable( EnableCap.Multisample );
		this.UIBuffer.Clear();
		this.UIToScreenBuffer.Clear();
		this.UIBuffer.Bind();
		this._uiSceneRenderer.Render( this._uiRenderData, prim: Resources.Render.Window.KeyboardEvents[ GLFW.Keys.X ] ? PrimitiveType.Lines : PrimitiveType.Triangles );
		Gl.Disable( EnableCap.Multisample );
		this.UIBuffer.BlitToFbo( this.UIToScreenBuffer, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear );
	}

	public void DrawToScreen() {
		this._pfxBlock.DirectWrite(
			new PFXBlock(
				this.UIToScreenBuffer.DiffuseTexture?.GetHandleDirect() ?? 0,
				0,
				0
			)
		);
		RenderUtils.RenderPFX( Resources.Render.Shader.Pipelines.GetOrAdd<TestPFX2Shader>(), Resources.Render.Mesh2.SquarePFX, this._pfxUniforms );
	}

	protected override bool OnDispose() {
		this.UIBuffer.Dispose();
		this.UIToScreenBuffer.Dispose();
		return true;
	}
}