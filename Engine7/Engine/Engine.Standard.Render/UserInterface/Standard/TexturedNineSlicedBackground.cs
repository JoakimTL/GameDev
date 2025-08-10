using Engine.Module.Render.Ogl.OOP.Textures;
using Engine.Standard.Render.Meshing.Services;
using Engine.Standard.Render.Shaders;

namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class TexturedNineSlicedBackground : UserInterfaceComponentBase {
	private readonly SceneInstance<Entity2SlicedTexturedSceneData>[] _sceneInstances;

	private Vector4<double> _color = (1, 1, 1, 1);
	public Vector4<double> Color {
		get => this._color;
		set {
			if (this._color == value)
				return;
			this._color = value;
			this._changed = true;
		}
	}

	private OglTexture _texture;
	public OglTexture Texture {
		get => this._texture;
		set {
			if (this._texture == value)
				return;
			this._texture = value;
			this._reference = null;
			this._changed = true;
		}
	}

	private float _edgeWidth = .33f;
	public float EdgeWidth {
		get => this._edgeWidth;
		set {
			if (this._edgeWidth == value)
				return;
			if (value < 0)
				throw new ArgumentException( "Edge width must be greater than or equal to 0." );
			if (value > 0.5f)
				throw new ArgumentException( "Edge width must be less than or equal to 0.5." );
			this._edgeWidth = value;
			this._changed = true;
		}
	}

	private float _edgeScale = 2;
	public float EdgeScale {
		get => this._edgeScale;
		set {
			if (this._edgeScale == value)
				return;
			this._edgeScale = value;
			this._changed = true;
		}
	}

	private TextureReference? _reference;

	private bool _changed;

	public TexturedNineSlicedBackground( UserInterfaceElementBase element, OglTexture texture ) : base( element ) {
		this._sceneInstances = CreateSceneInstances();
		this._texture = texture;
	}

	private SceneInstance<Entity2SlicedTexturedSceneData>[] CreateSceneInstances() {
		SceneInstance<Entity2SlicedTexturedSceneData>[] result = new SceneInstance<Entity2SlicedTexturedSceneData>[ 9 ];
		for (int i = 0; i < result.Length; i++) {
			SceneInstance<Entity2SlicedTexturedSceneData> sceneInstance = this.Element.UserInterfaceServiceAccess.RequestSceneInstance<SceneInstance<Entity2SlicedTexturedSceneData>>( this.RenderLayer );
			sceneInstance.SetVertexArrayObject( this.Element.UserInterfaceServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex2, Entity2SlicedTexturedSceneData>() );
			sceneInstance.SetShaderBundle( this.Element.UserInterfaceServiceAccess.ShaderBundleProvider.GetShaderBundle<SlicedTexturedShade2ShaderBundle>() );
			sceneInstance.SetMesh( this.Element.UserInterfaceServiceAccess.Get<PrimitiveMesh2Provider>().Get( Meshing.Primitive2.Rectangle ) );
			result[ i ] = sceneInstance;
		}
		return result;
	}


	protected override void OnPlacementChanged() => UpdateInstance();
	protected override void RenderLayerChanged() {
		for (int i = 0; i < this._sceneInstances.Length; i++)
			this._sceneInstances[ i ].SetLayer( this.RenderLayer );
		this._changed = true;
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (!this._changed)
			return;
		this._changed = false;
		UpdateInstance();
	}

	private void UpdateInstance() {
		this._reference ??= this._texture.GetTextureReference();
		Vector4<ushort> color = (this.Color * ushort.MaxValue).Clamp<Vector4<double>, double>( 0, ushort.MaxValue ).CastSaturating<double, ushort>();

		Span<Vector2<float>> sliceUvOffsets = stackalloc Vector2<float>[ this._sceneInstances.Length ];
		Span<Vector2<float>> sliceUvDimensions = stackalloc Vector2<float>[ this._sceneInstances.Length ];

		Vector2<float> textureSize = this.Texture.Level0.CastSaturating<int, float>();
		Vector2<float> textureAspectRatioVector = textureSize.Y > textureSize.X ? (1, textureSize.X / textureSize.Y) : (textureSize.Y / textureSize.X, 1);

		Vector2<float> edgeSize = textureAspectRatioVector * this.EdgeWidth;
		Vector2<float> centerUvLength = 1 - (edgeSize * 2);

		sliceUvOffsets[ 0 ] = (0, 0);
		sliceUvDimensions[ 0 ] = (edgeSize.X, edgeSize.Y);
		sliceUvOffsets[ 1 ] = (edgeSize.X, 0);
		sliceUvDimensions[ 1 ] = (centerUvLength.X, edgeSize.Y);
		sliceUvOffsets[ 2 ] = (1 - edgeSize.X, 0);
		sliceUvDimensions[ 2 ] = (edgeSize.X, edgeSize.Y);
		sliceUvOffsets[ 3 ] = (0, edgeSize.Y);
		sliceUvDimensions[ 3 ] = (edgeSize.X, centerUvLength.Y);
		sliceUvOffsets[ 4 ] = (edgeSize.X, edgeSize.Y);
		sliceUvDimensions[ 4 ] = (centerUvLength.X, centerUvLength.Y);
		sliceUvOffsets[ 5 ] = (1 - edgeSize.X, edgeSize.Y);
		sliceUvDimensions[ 5 ] = (edgeSize.X, centerUvLength.Y);
		sliceUvOffsets[ 6 ] = (0, 1 - edgeSize.Y);
		sliceUvDimensions[ 6 ] = (edgeSize.X, edgeSize.Y);
		sliceUvOffsets[ 7 ] = (edgeSize.X, 1 - edgeSize.Y);
		sliceUvDimensions[ 7 ] = (centerUvLength.X, edgeSize.Y);
		sliceUvOffsets[ 8 ] = (1 - edgeSize.X, 1 - edgeSize.Y);
		sliceUvDimensions[ 8 ] = (edgeSize.X, edgeSize.Y);

		Vector2<float> scale = this.TransformInterface.GlobalScale.CastSaturating<double, float>();
		float lowestScale = scale.X < scale.Y ? scale.X : scale.Y;

		Span<Vector2<float>> sliceOffsets = stackalloc Vector2<float>[ this._sceneInstances.Length ];
		Span<Vector2<float>> sliceDimensions = stackalloc Vector2<float>[ this._sceneInstances.Length ];

		float scaledEdge = lowestScale * this.EdgeWidth * this.EdgeScale;
		float scaledHalfEdge = scaledEdge * .5f;
		Vector2<float> scaledCenterUvLength = (scale - scaledEdge);

		sliceOffsets[ 0 ] = (-scale.X, -scale.Y);
		sliceDimensions[ 0 ] = (scaledHalfEdge, scaledHalfEdge);
		sliceOffsets[ 1 ] = (scaledEdge - scale.X, -scale.Y);
		sliceDimensions[ 1 ] = (scaledCenterUvLength.X, scaledHalfEdge);
		sliceOffsets[ 2 ] = (scale.X - scaledEdge, -scale.Y);
		sliceDimensions[ 2 ] = (scaledHalfEdge, scaledHalfEdge);
		sliceOffsets[ 3 ] = (-scale.X, scaledEdge - scale.Y);
		sliceDimensions[ 3 ] = (scaledHalfEdge, scaledCenterUvLength.Y);
		sliceOffsets[ 4 ] = (scaledEdge - scale.X, scaledEdge - scale.Y);
		sliceDimensions[ 4 ] = (scaledCenterUvLength.X, scaledCenterUvLength.Y);
		sliceOffsets[ 5 ] = (scale.X - scaledEdge, scaledEdge - scale.Y);
		sliceDimensions[ 5 ] = (scaledHalfEdge, scaledCenterUvLength.Y);
		sliceOffsets[ 6 ] = (-scale.X, scale.Y - scaledEdge);
		sliceDimensions[ 6 ] = (scaledHalfEdge, scaledHalfEdge);
		sliceOffsets[ 7 ] = (scaledEdge - scale.X, scale.Y - scaledEdge);
		sliceDimensions[ 7 ] = (scaledCenterUvLength.X, scaledHalfEdge);
		sliceOffsets[ 8 ] = (scale.X - scaledEdge, scale.Y - scaledEdge);
		sliceDimensions[ 8 ] = (scaledHalfEdge, scaledHalfEdge);

		Matrix4x4<float> transform = Matrix.Create4x4.RotationZ( (float) this.TransformInterface.GlobalRotation ) * Matrix.Create4x4.Translation( this.TransformInterface.GlobalTranslation.CastSaturating<double, float>() );

		for (int i = 0; i < this._sceneInstances.Length; i++) {
			//Fix scaling issue. Use transform but don't use the transform matrix?
			Matrix4x4<float> sliceMatrix = Matrix.Create4x4.Scaling( sliceDimensions[ i ] ) * Matrix.Create4x4.Translation( sliceOffsets[ i ] + sliceDimensions[ i ] );

			Matrix4x4<float> transformedSliceMatrix = sliceMatrix * transform;

			this._sceneInstances[ i ].Write( new Entity2SlicedTexturedSceneData( transformedSliceMatrix, color, this._reference.GetHandle(), sliceUvOffsets[ i ], sliceUvDimensions[ i ] ) );
		}
	}

	protected override void InternalRemove() {
		foreach (SceneInstance<Entity2SlicedTexturedSceneData> sceneInstance in this._sceneInstances)
			sceneInstance.Remove();
	}

	protected internal override void DoHide() {
		foreach (SceneInstance<Entity2SlicedTexturedSceneData> sceneInstance in this._sceneInstances)
			sceneInstance.SetAllocated( false );
	}

	protected internal override void DoShow() {
		foreach (SceneInstance<Entity2SlicedTexturedSceneData> sceneInstance in this._sceneInstances)
			sceneInstance.SetAllocated( true );
		UpdateInstance();
	}
}