using System.Runtime.InteropServices;
using Engine.Data.Datatypes.Composite;
using Engine.Data.Datatypes.Projections;
using Engine.Data.Datatypes.Views;
using Engine.Rendering.Data;
using Engine.Rendering.Framebuffers;
using Engine.Rendering.Lighting;
using Engine.Rendering.Shaders;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Utilities;
using OpenGL;

namespace Engine.Rendering.Pipelines;
public class Render3Pipeline : DisposableIdentifiable, IRenderPipeline {

	public readonly MultiSceneRenderer Scenes;
	public readonly Perspective Perspective;
	public readonly View3 View;
	public readonly Camera Camera;

	public readonly GeometryBuffer GeometryBuffer;
	public readonly LightManager Lights;

	public readonly UniformBlock SceneCameraBlock;
	private readonly DataBlockCollection _sceneUniforms;
	private readonly DataBlockCollection _pfxUniforms;
	private readonly UniformBlock _pfxBlock;
	private bool _normal;

	public Render3Pipeline() : base() {
		//Redo rendering. This should be a singleton in renderresources
		//Maybe in it's own rendermanager singleton? The rendermanager contains the window, which the renderpipelines can access
		this.Scenes = new MultiSceneRenderer();
		this._sceneUniforms = new DataBlockCollection( this.SceneCameraBlock = new UniformBlock( "SceneCameraBlock", (uint) Marshal.SizeOf<SceneCameraBlock>(), ShaderType.VertexShader ) );
		this._pfxUniforms = new DataBlockCollection( this._pfxBlock = new UniformBlock( "PFXBlock", 24, ShaderType.FragmentShader ) );
		this.Camera = new Camera( this.View = new View3(), this.Perspective = new Perspective.Dynamic( 90 ) );
		this.GeometryBuffer = new GeometryBuffer();
		this.Lights = new LightManager( this.SceneCameraBlock );
	}

	public void RenderFrame() {
		this.SceneCameraBlock.DirectWrite( new SceneCameraBlock( this.Camera.Matrix, this.View.Rotation.Up(), this.View.Rotation.Right() ) );
		this.GeometryBuffer.Clear();
		this.GeometryBuffer.Bind();
		Gl.Enable( EnableCap.CullFace );
		Gl.Enable( EnableCap.DepthTest );
		this.Scenes.Render( this._sceneUniforms, prim: Resources.Render.Window.KeyboardEvents[ GLFW.Keys.X ] ? PrimitiveType.Lines : PrimitiveType.Triangles );

		this.Lights.Update( this.Camera, this.Scenes, this.View );
		this.Lights.Render( this.Camera, this.View, this.Perspective, this.GeometryBuffer );

		Resources.Render.FrameDebugData.TitleInfo = $"{this.View.Translation} .. {( System.Numerics.Vector3.Dot( -System.Numerics.Vector3.Normalize( this.View.Translation ), this.View.Rotation.Forward() ) + 1 ) * 50:N3}%";

		Resources.Render.Window.Bind();
		Gl.ClearColor( 0, 0, 0, 0 );
		Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		Gl.Enable( EnableCap.Blend );
		Gl.Disable( EnableCap.DepthTest );
		Gl.Disable( EnableCap.CullFace );
		this._pfxBlock.DirectWrite( new PFXBlock(
			this.Lights.LightBufferTextureHandle,
			0,
			0
		) );
		RenderUtils.RenderPFX( Resources.Render.Shader.Pipelines.Get<TestPFX2Shader>(), Resources.Render.Mesh2.SquarePFX, this._pfxUniforms );
		if ( Resources.Render.Window.KeyboardEvents[ GLFW.Keys.R ] ) {
			this._pfxBlock.DirectWrite( new PFXBlock(
				this.GeometryBuffer.DiffuseTexture?.GetHandleDirect() ?? 0,
				this.GeometryBuffer.TransparencyColorTexture?.GetHandleDirect() ?? 0,
				this.GeometryBuffer.TransparencyRevealTexture?.GetHandleDirect() ?? 0
			) );
			RenderUtils.RenderPFX( Resources.Render.Shader.Pipelines.Get<TestPFXShader>(), Resources.Render.Mesh2.SquarePFX, this._pfxUniforms );
		}
		if ( Resources.Render.Window.KeyboardEvents[ GLFW.Keys.Y ] )
			this._normal = true;
		if ( Resources.Render.Window.KeyboardEvents[ GLFW.Keys.Home ] )
			this._normal = false;
		if ( this._normal ) {
			this._pfxBlock.DirectWrite( new PFXBlock(
				this.GeometryBuffer.NormalTexture?.GetHandleDirect() ?? 0,
				0,
				0
			) );
			RenderUtils.RenderPFX( Resources.Render.Shader.Pipelines.Get<TestPFX2Shader>(), Resources.Render.Mesh2.SquarePFX, this._pfxUniforms );
		}
		/*if ( RenderResourceManager.Window.KeyboardEvents[ GLFW.Keys.U ] ) {
			this._pfxBlock.DirectWrite( new PFXBlock(
				this.GeometryBuffer.GlowTexture?.GetHandleDirect() ?? 0,
				0,
				0
			) );
			RenderUtils.RenderPFX( RenderResourceManager.Context.Shader.Pipelines.Get<TestPFX2Shader>(), RenderResourceManager.Context.Mesh2.SquarePFX, this._pfxUniforms );
		}
		if ( RenderResourceManager.Window.KeyboardEvents[ GLFW.Keys.I ] ) {
			this._pfxBlock.DirectWrite( new PFXBlock(
				this.GeometryBuffer.DepthTexture?.GetHandleDirect() ?? 0,
				0,
				0
			) );
			RenderUtils.RenderPFX( RenderResourceManager.Context.Shader.Pipelines.Get<TestPFX2Shader>(), RenderResourceManager.Context.Mesh2.SquarePFX, this._pfxUniforms );
		}
		if ( RenderResourceManager.Window.KeyboardEvents[ GLFW.Keys.O ] ) {
			this._pfxBlock.DirectWrite( new PFXBlock(
				this.GeometryBuffer.LightPropertiesTexture?.GetHandleDirect() ?? 0,
				0,
				0
			) );
			RenderUtils.RenderPFX( RenderResourceManager.Context.Shader.Pipelines.Get<TestPFX2Shader>(), RenderResourceManager.Context.Mesh2.SquarePFX, this._pfxUniforms );
		}
		if ( RenderResourceManager.Window.KeyboardEvents[ GLFW.Keys.H ] ) {
			this._pfxBlock.DirectWrite( new PFXBlock(
				this.Lights.DirLight?._cascades[ 0 ].UnfilteredDepthTexture ?? 0,
				0,
				0
			) );
			RenderUtils.RenderPFX( RenderResourceManager.Context.Shader.Pipelines.Get<TestPFX2Shader>(), RenderResourceManager.Context.Mesh2.SquarePFX, this._pfxUniforms );
		}
		if ( RenderResourceManager.Window.KeyboardEvents[ GLFW.Keys.J ] ) {
			this._pfxBlock.DirectWrite( new PFXBlock(
				this.Lights.DirLight?._cascades[ 1 ].UnfilteredDepthTexture ?? 0,
				0,
				0
			) );
			RenderUtils.RenderPFX( RenderResourceManager.Context.Shader.Pipelines.Get<TestPFX2Shader>(), RenderResourceManager.Context.Mesh2.SquarePFX, this._pfxUniforms );
		}
		if ( RenderResourceManager.Window.KeyboardEvents[ GLFW.Keys.K ] ) {
			this._pfxBlock.DirectWrite( new PFXBlock(
				this.Lights.DirLight?._cascades[ 2 ].UnfilteredDepthTexture ?? 0,
				0,
				0
			) );
			RenderUtils.RenderPFX( RenderResourceManager.Context.Shader.Pipelines.Get<TestPFX2Shader>(), RenderResourceManager.Context.Mesh2.SquarePFX, this._pfxUniforms );
		}
		if ( RenderResourceManager.Window.KeyboardEvents[ GLFW.Keys.L ] ) {
			this._pfxBlock.DirectWrite( new PFXBlock(
				this.Lights.DirLight?._cascades[ 3 ].UnfilteredDepthTexture ?? 0,
				0,
				0
			) );
			RenderUtils.RenderPFX( RenderResourceManager.Context.Shader.Pipelines.Get<TestPFX2Shader>(), RenderResourceManager.Context.Mesh2.SquarePFX, this._pfxUniforms );
		}*/
		Gl.Disable( EnableCap.Blend );
	}

	public void DrawToScreen() {
		this._pfxBlock.DirectWrite( 
			new PFXBlock(
				this.Lights.LightBufferTextureHandle,
				0,
				0
			)
		);
		RenderUtils.RenderPFX( Resources.Render.Shader.Pipelines.Get<TestPFX2Shader>(), Resources.Render.Mesh2.SquarePFX, this._pfxUniforms );
		if ( Resources.Render.Window.KeyboardEvents[ GLFW.Keys.R ] ) {
			this._pfxBlock.DirectWrite( new PFXBlock(
				this.GeometryBuffer.DiffuseTexture?.GetHandleDirect() ?? 0,
				this.GeometryBuffer.TransparencyColorTexture?.GetHandleDirect() ?? 0,
				this.GeometryBuffer.TransparencyRevealTexture?.GetHandleDirect() ?? 0
			) );
			RenderUtils.RenderPFX( Resources.Render.Shader.Pipelines.Get<TestPFXShader>(), Resources.Render.Mesh2.SquarePFX, this._pfxUniforms );
		}
		if ( Resources.Render.Window.KeyboardEvents[ GLFW.Keys.Y ] )
			this._normal = true;
		if ( Resources.Render.Window.KeyboardEvents[ GLFW.Keys.Home ] )
			this._normal = false;
		if ( this._normal ) {
			this._pfxBlock.DirectWrite( new PFXBlock(
				this.GeometryBuffer.NormalTexture?.GetHandleDirect() ?? 0,
				0,
				0
			) );
			RenderUtils.RenderPFX( Resources.Render.Shader.Pipelines.Get<TestPFX2Shader>(), Resources.Render.Mesh2.SquarePFX, this._pfxUniforms );
		}
	}

	public struct PFXBlock {
		public ulong handleDiffuse;
		public ulong handleAccumulation;
		public ulong handleReveal;

		public PFXBlock( ulong handleDiffuse, ulong handleAccumulation, ulong handleReveal ) {
			this.handleDiffuse = handleDiffuse;
			this.handleAccumulation = handleAccumulation;
			this.handleReveal = handleReveal;
		}
	}

	public class TestPFX2Fragment : ShaderProgram {
		protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "pfx.frag" ] );
	}

	public class TestPFXFragment : ShaderProgram {
		protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "gBuffer.frag" ] );
	}

	internal class TestPFX2Shader : ShaderPipeline {

		public TestPFX2Shader() : base( typeof( PfxShaderProgramVertex ), typeof( TestPFX2Fragment ) ) { }

	}
	internal class TestPFXShader : ShaderPipeline {

		public TestPFXShader() : base( typeof( PfxShaderProgramVertex ), typeof( TestPFXFragment ) ) { }

	}

	private void Transparency( bool transparent ) {
		if ( transparent ) {
			Gl.Enable( EnableCap.Blend );
			Gl.BlendFunc( GeometryBuffer.TransparencyColorTextureTarget, BlendingFactor.One, BlendingFactor.One );
			Gl.BlendFunc( GeometryBuffer.TransparencyRevealTextureTarget, BlendingFactor.Zero, BlendingFactor.OneMinusSrcColor );
			Gl.BlendEquation( BlendEquationMode.FuncAdd );
			Gl.DepthFunc( DepthFunction.Less );
			Gl.DepthMask( false );
		} else {
			Gl.Disable( EnableCap.Blend );
			Gl.DepthFunc( DepthFunction.Less );
			Gl.DepthMask( true );
		}
	}
	protected override bool OnDispose() {
		this.GeometryBuffer.Dispose();
		return true;
	}
}
