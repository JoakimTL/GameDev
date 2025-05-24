using Engine.Module.Render.Ogl.OOP.Textures;
using Engine.Standard.Render.Meshing.Services;
using Engine.Standard.Render.Shaders;

namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class TexturedBackground : UserInterfaceComponentBase {
	private readonly SceneInstance<Entity2TexturedSceneData> _sceneInstance;

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

	private TextureReference? _reference;

	private bool _changed;

	public TexturedBackground( UserInterfaceElementBase element, OglTexture texture ) : base( element ) {
		this._sceneInstance = CreateSceneInstance();
		this._texture = texture;
	}

	private SceneInstance<Entity2TexturedSceneData> CreateSceneInstance() {
		SceneInstance<Entity2TexturedSceneData> sceneInstance = this.Element.UserInterfaceServiceAccess.RequestSceneInstance<SceneInstance<Entity2TexturedSceneData>>( this.RenderLayer );
		sceneInstance.SetVertexArrayObject( this.Element.UserInterfaceServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex2, Entity2TexturedSceneData>() );
		sceneInstance.SetShaderBundle( this.Element.UserInterfaceServiceAccess.ShaderBundleProvider.GetShaderBundle<TexturedShade2ShaderBundle>() );
		sceneInstance.SetMesh( this.Element.UserInterfaceServiceAccess.Get<PrimitiveMesh2Provider>().Get( Meshing.Primitive2.Rectangle ) );
		return sceneInstance;
	}


	protected override void OnPlacementChanged() => UpdateInstance();

	protected override void OnUpdate( double time, double deltaTime ) {
		if (!this._changed)
			return;
		this._changed = false;
		UpdateInstance();
	}

	private void UpdateInstance() {
		this._reference ??= this._texture.GetTextureReference();
		Vector4<ushort> color = (this.Color * ushort.MaxValue).Clamp<Vector4<double>, double>( 0, ushort.MaxValue ).CastSaturating<double, ushort>();
		this._sceneInstance.Write( new Entity2TexturedSceneData( this.TransformInterface.Matrix.CastSaturating<double, float>(), color, this._reference.GetHandle() ) );
	}

	protected override void InternalRemove() {
		this._sceneInstance.Remove();
	}

	protected internal override void DoHide() {
		this._sceneInstance.SetAllocated( false );
	}

	protected internal override void DoShow() {
		this._sceneInstance.SetAllocated( true );
		UpdateInstance();
	}
}
