using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Data.Datatypes;
using Engine.Data.Datatypes.Composite;
using Engine.Data.Datatypes.Projections;
using Engine.Data.Datatypes.Views;
using Engine.Rendering.Data;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Scenes;
using OpenGL;

namespace Engine.Rendering.Lighting.Directional;

public class Cascade : DisposableIdentifiable {

	public readonly CascadeDepthBuffer _depth;
	public readonly CascadeDepthBlurBuffer _depthBlurred;
	public readonly CascadeDepthBlurBuffer _depthBlurHalfstep;
	private readonly View3 _cascadeView;
	private readonly Orthographic _cascadeProjection;
	private readonly Camera _cascadeCamera;
	private readonly DataBlockCollection _sceneUniforms;
	private readonly DataBlockCollection _blurringUniforms;
	private readonly UniformBlock _sceneUniformBlock;
	private readonly UniformBlock _blurringUniformBlock;
	public readonly Vector3[] _frustumPoints;
	private readonly Vector2 _textureSize;
	private readonly float _startDepth;
	private readonly float _endDepth;

	public Vector3 Translation;
	public Quaternion Rotation;
	public Vector3 Scale;

	public ulong UnfilteredDepthTexture => this._depth.DepthTexture?.GetHandleDirect() ?? 0;
	public ulong FilteredDepthTexture => this._depthBlurred.RedTexture?.GetHandleDirect() ?? 0;
	public ulong TransparencyColorTexture => this._depth.TransparencyColorTexture?.GetHandleDirect() ?? 0;
	public ulong TransparencyRevealTexture => this._depth.TransparencyRevealTexture?.GetHandleDirect() ?? 0;
	public Matrix4x4 ViewProjectionMatrix => this._cascadeCamera.Matrix;
	public Vector2 TextureSize => this._textureSize;
	public float MaxDepth { get; private set; }

	public Cascade( Vector2i size, float startDepth, float endDepth ) {
		this._depth = new CascadeDepthBuffer( size );
		this._depthBlurred = new CascadeDepthBlurBuffer( size );
		this._depthBlurHalfstep = new CascadeDepthBlurBuffer( size );
		this._sceneUniformBlock = new UniformBlock( "SceneCameraBlock", (uint) Marshal.SizeOf<SceneCameraBlock>(), ShaderType.VertexShader );
		this._sceneUniforms = new DataBlockCollection( this._sceneUniformBlock );
		this._blurringUniformBlock = new UniformBlock( "BlurBlock", 8, ShaderType.FragmentShader );
		this._blurringUniforms = new DataBlockCollection( this._blurringUniformBlock );
		this._cascadeCamera = new Camera( this._cascadeView = new View3(), this._cascadeProjection = new Orthographic( new Vector2( 1, 1 ), -10, 10 ) );
		this._frustumPoints = new Vector3[ 8 ];
		this._textureSize = size.AsFloat;
		this._startDepth = startDepth;
		this._endDepth = endDepth;
	}

	public void SetFrustumPoints( Vector3[] frustumPoints, Vector3 cameraTranslation ) {
		Vector3 avg = new( 0 );
		for ( int i = 0; i < 4; i++ ) {
			this._frustumPoints[ i ] = Vector3.Lerp( frustumPoints[ i ], frustumPoints[ i + 4 ], this._startDepth );
			this._frustumPoints[ i + 4 ] = Vector3.Lerp( frustumPoints[ i ], frustumPoints[ i + 4 ], this._endDepth );
			avg += this._frustumPoints[ i + 4 ];
		}
		this.MaxDepth = ( ( avg / 4 ) - cameraTranslation ).Length();
	}

	public void SetBoundaries( Quaternion lightRotation, Vector3 lightUp, Vector3 lightRight, Vector3 lightForward, float maxDepth ) {
		Vector3 max = new( float.MinValue ), min = new( float.MaxValue );
		foreach ( Vector3 frustumPoint in this._frustumPoints ) {
			Vector3 s = new( Vector3.Dot( lightRight, frustumPoint ), Vector3.Dot( lightUp, frustumPoint ), Vector3.Dot( lightForward, frustumPoint ) );
			max = Vector3.Max( max, s );
			min = Vector3.Min( min, s );
		}
		Set( Vector3.Transform( max + min, lightRotation ) * 0.5f, lightRotation, max - min, maxDepth );

	}

	private void Set( Vector3 translation, Quaternion rotation, Vector3 scale, float maxDepth ) {
		this._cascadeView.Translation = translation;
		this.Translation = translation;
		this._cascadeView.Rotation = rotation;
		this.Rotation = rotation;
		this._cascadeProjection.Size = new Vector2( scale.X, scale.Y );
		float depth = MathF.Max( maxDepth, scale.Z * 0.5f );
		this.Scale = new Vector3( scale.X * 0.5f, scale.Y * 0.5f, depth );
		this._cascadeProjection.ZNear = -depth;
		this._cascadeProjection.ZFar = depth;
	}

	public void Render( MultiSceneRenderer scenes ) {
		this._depth.Clear();
		this._depthBlurHalfstep.Clear();
		this._depthBlurred.Clear();

		this._depth.Bind();
		this._sceneUniformBlock.DirectWrite( new SceneCameraBlock( this._cascadeCamera.Matrix, this._cascadeView.Rotation.Up(), this._cascadeView.Rotation.Right() ) );
		scenes.Render( this._sceneUniforms, Transparency, 1 );

		/*if ( this._depth.DepthTexture is null ) {
			this.LogError( "Depth texture null." );
			return;
		}

		this._depthBlurHalfstep.Bind();
		this._blurringUniformBlock.DirectWrite( new BlurRedShaderUniformBlock() {
			TextureHandle = this._depth.DepthTexture.GetHandleDirect(),
			TextureSize = this._depth.Size.AsFloat,
			BlurVector = new Vector2( 1, 0 )
		} );
		Utilities.RenderUtils.RenderPFX( Resources.Context.Shader.Pipelines.Get<BlurRedShader>(), Resources.Context.Mesh2.SquarePFX, this._blurringUniforms );

		if ( this._depthBlurHalfstep.RedTexture is null ) {
			this.LogError( "Depth Halfstep texture null." );
			return;
		}

		this._depthBlurred.Bind();
		this._blurringUniformBlock.DirectWrite( new BlurRedShaderUniformBlock() {
			TextureHandle = this._depthBlurHalfstep.RedTexture.GetHandleDirect(),
			TextureSize = this._depthBlurHalfstep.Size.AsFloat,
			BlurVector = new Vector2( 0, 1 )
		} );
		Utilities.RenderUtils.RenderPFX( Resources.Context.Shader.Pipelines.Get<BlurRedShader>(), Resources.Context.Mesh2.SquarePFX, this._blurringUniforms );*/
	}

	private void Transparency( bool transparent ) {
		if ( transparent ) {
			Gl.Enable( EnableCap.Blend );
			Gl.BlendFunc( 0, BlendingFactor.One, BlendingFactor.One );
			Gl.BlendFunc( 1, BlendingFactor.Zero, BlendingFactor.OneMinusSrcColor );
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
		this._depth.Dispose();
		this._depthBlurred.Dispose();
		return true;
	}
}
