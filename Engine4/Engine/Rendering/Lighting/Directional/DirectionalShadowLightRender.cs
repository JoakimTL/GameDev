using System.Numerics;
using Engine.Data.Datatypes;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Lighting.Directional;

// TWO TEXTURES
// One for unfiltered shadow depth
// Another for blurred depth
// Check if shadow exists in nearby pixels, if they do, use shadow depth in blurred image.
public class DirectionalShadowLightRender : DirectionalLightRenderBase<DirectionalLightData>, IShadowBufferedLight {

	private static readonly float[] _cascadeRanges = GetCascadeRanges();
	private static float[] GetCascadeRanges() {
		float[] ranges = new float[ CascadeData.NUMCASCADES + 1 ];
		ranges[ 0 ] = 0;
		for ( int i = 1; i < CascadeData.NUMCASCADES + 1; i++ ) {
			float x = (float) i / CascadeData.NUMCASCADES;
			ranges[ i ] = x * x * x * x;
		}
		Log.Line( $"Cascade range distribution: {string.Join( ", ", ranges.Select( p => p.ToString( "N2" ) ) )}", Log.Level.NORMAL );
		return ranges;
	}
	private static readonly Vector3[] _cube = new Vector3[] {
		new Vector3(-1, -1, -1),
		new Vector3(1, -1, -1),
		new Vector3(-1, 1, -1),
		new Vector3(1, 1, -1),
		new Vector3(-1, -1, 1),
		new Vector3(1, -1, 1),
		new Vector3(-1, 1, 1),
		new Vector3(1, 1, 1)
	};

	private bool _changed;
	private Matrix4x4 _currentMatrix;
	private readonly Vector3[] _frustumPoints;
	public readonly Cascade[] _cascades;
	private readonly CascadeData[] _cascadeData;
	private readonly float _maxDepth;
	public IReadOnlyList<CascadeData> LightCascadeData => this._cascadeData;

	public DirectionalShadowLightRender( DirectionalLight light, Vector2i size, float maxDepth ) : base( light ) {
		SetShaders( Resources.Render.Shader.Bundles.Get<DirectionalShadowedLightShaderBundle>() );
		this._maxDepth = maxDepth;
		this._cascades = new Cascade[ CascadeData.NUMCASCADES ];
		this._cascadeData = new CascadeData[ CascadeData.NUMCASCADES ];
		for ( int i = 0; i < CascadeData.NUMCASCADES; i++ )
			this._cascades[ i ] = new Cascade( GetCascadeSize( size, i ), _cascadeRanges[ i ], _cascadeRanges[ i + 1 ] );
		this._frustumPoints = new Vector3[ 8 ];
		this.Light.Changed += LightChanged;
		SetSceneData();
	}

	private void LightChanged() => this._changed = true;

	public DirectionalShadowLightRender( DirectionalLight light, float maxDepth ) : this( light, new Vector2i( 2048, 2048 ), maxDepth ) { }

	public void UpdateShadowBuffers( IMatrixProvider camera, MultiSceneRenderer scenes, Vector3 cameraTranslation ) {
		UpdateCascades( camera, cameraTranslation );
		for ( int i = 0; i < CascadeData.NUMCASCADES; i++ )
			this._cascades[ i ].Render( scenes );
		SetCascadeData( this._cascadeData );
	}

	public void UpdateCascades( IMatrixProvider camera, Vector3 cameraTranslation ) {
		if ( this._currentMatrix == camera.InverseMatrix && !this._changed )
			return;
		this._currentMatrix = camera.InverseMatrix;
		this._changed = false;

		for ( int i = 0; i < this._frustumPoints.Length; i++ ) {
			Vector4 a = Vector4.Transform( new Vector4( _cube[ i ], 1 ), this._currentMatrix );
			this._frustumPoints[ i ] = new Vector3( a.X, a.Y, a.Z ) / a.W;
		}

		Vector3 lightUp = Vector3.Normalize( this.Light.DirectionQuaternion.Up() );
		Vector3 lightRight = Vector3.Normalize( this.Light.DirectionQuaternion.Right() );
		Vector3 lightForward = Vector3.Normalize( -this.Light.DirectionQuaternion.Forward() );
		for ( int i = 0; i < CascadeData.NUMCASCADES; i++ ) {
			this._cascades[ i ].SetFrustumPoints( this._frustumPoints, cameraTranslation );
			this._cascades[ i ].SetBoundaries( this.Light.DirectionQuaternion, lightUp, lightRight, lightForward, this._maxDepth );
		}
	}

	private static Vector2i GetCascadeSize( Vector2i size, int cascadeIndex ) {
		float s = (float) cascadeIndex / CascadeData.NUMCASCADES;
		Vector2i newSize = Vector2i.Round( size.AsFloat * ( 1 - ( s * s * s * s ) ) );
		return Vector2i.NegativeOrZero( newSize ) ? 1 : newSize;
	}

	protected override DirectionalLightData GetSceneData() => new() {
		Color = this.Light.Color,
		Intensity = this.Light.Intensity,
		Direction = this.Light.Direction
	};

	private void SetCascadeData( CascadeData[] data ) {
		for ( int i = 0; i < CascadeData.NUMCASCADES; i++ ) {
			data[ i ].UnfilteredTextureHandle = this._cascades[ i ].UnfilteredDepthTexture;
			data[ i ].FilteredTextureHandle = this._cascades[ i ].FilteredDepthTexture;
			data[ i ].TransparencyColorTextureHandle = this._cascades[ i ].TransparencyColorTexture;
			data[ i ].TransparencyRevealTextureHandle = this._cascades[ i ].TransparencyRevealTexture;
			data[ i ].ViewProjectionMatrix = this._cascades[ i ].ViewProjectionMatrix;
			data[ i ].TextureSize = this._cascades[ i ].TextureSize;
			data[ i ].Depth = this._cascades[ i ].MaxDepth;
		}
	}

	protected override bool OnDispose() {
		base.OnDispose();
		for ( int i = 0; i < CascadeData.NUMCASCADES; i++ )
			this._cascades[ i ].Dispose();
		return true;
	}
}
