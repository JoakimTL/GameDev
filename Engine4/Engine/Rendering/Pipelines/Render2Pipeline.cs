using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Data.Datatypes.Composite;
using Engine.Data.Datatypes.Projections;
using Engine.Data.Datatypes.Views;
using Engine.Rendering.Data;
using Engine.Rendering.Framebuffers;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Utilities;
using Engine.Structure;
using OpenGL;
using static Engine.Rendering.Pipelines.Render3Pipeline;

namespace Engine.Rendering.Pipelines;

[ProcessAfter( typeof( Render3Pipeline ), typeof( IRenderPipeline ) )]
public class Render2Pipeline : DisposableIdentifiable, IRenderPipeline {

	public readonly MultiSceneRenderer Scenes;
	public readonly View2 View;
	private readonly Orthographic.Dynamic _projection;
	private readonly Camera _camera;
	private readonly DataBlockCollection _renderData;
	public readonly UniformBlock SceneCameraBlock;
	public readonly ScreenFramebuffer ScreenBuffer;
	private readonly DataBlockCollection _pfxUniforms;
	private readonly UniformBlock _pfxBlock;

	public Render2Pipeline( Window window ) {
		this.View = new();
		this._projection = new Orthographic.Dynamic( window, new Vector2( 2 ), -1, 1 );
		this._camera = new Camera( this.View, this._projection );
		this._renderData = new DataBlockCollection( this.SceneCameraBlock = new UniformBlock( "SceneCameraBlock", (uint) Marshal.SizeOf<SceneCameraBlock>(), ShaderType.VertexShader ) );
		this._pfxUniforms = new DataBlockCollection( this._pfxBlock = new UniformBlock( "PFXBlock", 24, ShaderType.FragmentShader ) );
		this.Scenes = new MultiSceneRenderer();
		this.ScreenBuffer = new( window, 2 );
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
		Vector2 cameraRotationRight = new( MathF.Cos( this.View.Rotation ), MathF.Sin( this.View.Rotation ) );
		Vector2 cameraRotationUp = new( -cameraRotationRight.Y, cameraRotationRight.X );
		this.SceneCameraBlock.DirectWrite( new SceneCameraBlock( this._camera.Matrix, new Vector3( cameraRotationUp, 0 ), new Vector3( cameraRotationRight, 0 ) ) );
		this.ScreenBuffer.Clear();
		this.ScreenBuffer.Bind();
		Gl.Disable( EnableCap.DepthTest );
		this.Scenes.Render( this._renderData, BlendFunc, prim: Resources.Render.Window.KeyboardEvents[ GLFW.Keys.X ] ? PrimitiveType.Lines : PrimitiveType.Triangles );
	}

	public void DrawToScreen() {
		this._pfxBlock.DirectWrite(
			new PFXBlock(
				this.ScreenBuffer.DiffuseTexture?.GetHandleDirect() ?? 0,
				0,
				0
			)
		);
		RenderUtils.RenderPFX( Resources.Render.Shader.Pipelines.GetOrAdd<TestPFX2Shader>(), Resources.Render.Mesh2.SquarePFX, this._pfxUniforms );
	}

	protected override bool OnDispose() {
		this.ScreenBuffer.Dispose();
		return true;
	}
}
