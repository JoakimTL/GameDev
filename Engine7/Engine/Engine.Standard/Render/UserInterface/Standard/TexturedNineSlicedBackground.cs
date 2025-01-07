using Engine.Module.Render.Ogl.OOP.Textures;
using Engine.Standard.Render.Meshing.Services;
using Engine.Standard.Render.Shaders;

namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class TexturedNineSlicedBackground : UserInterfaceComponentBase {
	private readonly SceneInstance<Entity2SlicedTexturedSceneData>[] _sceneInstances;

	private Vector4<double> _color = (1, 1, 1, 1);
	public Vector4<double> Color {
		get => _color;
		set {
			if (_color == value)
				return;
			_color = value;
			_changed = true;
		}
	}

	private OglTexture _texture;
	public OglTexture Texture {
		get => _texture;
		set {
			if (_texture == value)
				return;
			_texture = value;
			_reference = null;
			_changed = true;
		}
	}

	private float _edgeWidth = 0.1f;
	public float EdgeWidth {
		get => _edgeWidth;
		set {
			if (_edgeWidth == value)
				return;
			if (value < 0)
				throw new ArgumentException( "Edge width must be greater than or equal to 0." );
			if (value > 0.5f)
				throw new ArgumentException( "Edge width must be less than or equal to 0.5." );
			_edgeWidth = value;
			_changed = true;
		}
	}

	private float _edgeScale = 1;
	public float EdgeScale {
		get => _edgeScale;
		set {
			if (_edgeScale == value)
				return;
			_edgeScale = value;
			_changed = true;
		}
	}

	private OglTextureBase<OglMipmappedTextureMetadata>.TextureReference? _reference;

	private bool _changed;

	public TexturedNineSlicedBackground( UserInterfaceElementBase element, OglTexture texture ) : base( element ) {
		_sceneInstances = CreateSceneInstances();
		_texture = texture;
	}

	public TexturedNineSlicedBackground( UserInterfaceComponentBase parent, OglTexture texture ) : base( parent, 0 ) {
		_sceneInstances = CreateSceneInstances();
		_texture = texture;
	}

	private SceneInstance<Entity2SlicedTexturedSceneData>[] CreateSceneInstances() {
		SceneInstance<Entity2SlicedTexturedSceneData>[] result = new SceneInstance<Entity2SlicedTexturedSceneData>[ 9 ];
		for (int i = 0; i < result.Length; i++) {
			SceneInstance<Entity2SlicedTexturedSceneData> sceneInstance = Element.UserInterfaceServiceAccess.RequestSceneInstance<SceneInstance<Entity2SlicedTexturedSceneData>>( RenderLayer );
			sceneInstance.SetVertexArrayObject( Element.UserInterfaceServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex2, Entity2SlicedTexturedSceneData>() );
			sceneInstance.SetShaderBundle( Element.UserInterfaceServiceAccess.ShaderBundleProvider.GetShaderBundle<TexturedShade2ShaderBundle>() );
			sceneInstance.SetMesh( Element.UserInterfaceServiceAccess.Get<PrimitiveMesh2Provider>().Get( Meshing.Primitive2.Rectangle ) );
			result[ i ] = sceneInstance;
		}
		return result;
	}


	protected override void OnPlacementChanged() => UpdateInstance();

	protected override void OnUpdate( double time, double deltaTime ) {
		if (!_changed)
			return;
		_changed = false;
		UpdateInstance();
	}

	private void UpdateInstance() {
		_reference ??= _texture.GetTextureReference();
		Vector4<ushort> color = (Color * ushort.MaxValue).Clamp<Vector4<double>, double>( 0, ushort.MaxValue ).CastSaturating<double, ushort>();

		Span<Vector2<float>> sliceUvOffsets = stackalloc Vector2<float>[ _sceneInstances.Length ];
		Span<Vector2<float>> sliceUvDimensions = stackalloc Vector2<float>[ _sceneInstances.Length ];

		float centerDimensions = 1 - EdgeWidth * 2;

		sliceUvOffsets[0] = (0, 0);
		sliceUvDimensions[ 0 ] = (EdgeWidth, EdgeWidth);
		sliceUvOffsets[ 1 ] = (EdgeWidth, 0);
		sliceUvDimensions[ 1 ] = (centerDimensions, EdgeWidth);
		sliceUvOffsets[ 2 ] = (1 - EdgeWidth, 0);
		sliceUvDimensions[ 2 ] = (EdgeWidth, EdgeWidth);
		sliceUvOffsets[ 3 ] = (0, EdgeWidth);
		sliceUvDimensions[ 3 ] = (EdgeWidth, centerDimensions);
		sliceUvOffsets[ 4 ] = (EdgeWidth, EdgeWidth);
		sliceUvDimensions[ 4 ] = (centerDimensions, centerDimensions);
		sliceUvOffsets[ 5 ] = (1 - EdgeWidth, EdgeWidth);
		sliceUvDimensions[ 5 ] = (EdgeWidth, centerDimensions);
		sliceUvOffsets[ 6 ] = (0, 1 - EdgeWidth);
		sliceUvDimensions[ 6 ] = (EdgeWidth, EdgeWidth);
		sliceUvOffsets[ 7 ] = (EdgeWidth, 1 - EdgeWidth);
		sliceUvDimensions[ 7 ] = (centerDimensions, EdgeWidth);
		sliceUvOffsets[ 8 ] = (1 - EdgeWidth, 1 - EdgeWidth);
		sliceUvDimensions[ 8 ] = (EdgeWidth, EdgeWidth);

		for (int i = 0; i < _sceneInstances.Length; i++) {
			//Fix scaling issue. Use transform but don't use the transform matrix?
			Matrix4x4<float> sliceMatrix = Matrix.Create4x4.Scaling( sliceUvDimensions[ i ] ) * Matrix.Create4x4.Translation( (sliceUvOffsets[ i ] + sliceUvDimensions[ i ] * 0.5f) * 2 - 1 );

			_sceneInstances[ i ].Write( new Entity2SlicedTexturedSceneData( sliceMatrix * TransformInterface.Matrix.CastSaturating<double, float>(), color, _reference.GetHandle(), sliceUvOffsets[ i ], sliceUvDimensions[ i ] ) );
		}
	}

	protected override bool InternalDispose() => true;
}