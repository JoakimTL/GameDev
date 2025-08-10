using Engine.Standard.Render.Meshing.Services;
using Engine.Standard.Render.Shaders;

namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class FlatBackground : UserInterfaceComponentBase {

	private readonly SceneInstance<Entity2SceneData> _sceneInstance;

	private Vector4<double> _color = (1, 1, 1, 1);
	private bool _colorChanged;
	public Vector4<double> Color {
		get => this._color;
		set {
			if (this._color == value)
				return;
			this._color = value;
			this._colorChanged = true;
		}
	}

	public FlatBackground( UserInterfaceElementBase element ) : base( element ) {
		this._sceneInstance = CreateSceneInstance();
	}

	private SceneInstance<Entity2SceneData> CreateSceneInstance() {
		SceneInstance<Entity2SceneData> sceneInstance = this.Element.UserInterfaceServiceAccess.RequestSceneInstance<SceneInstance<Entity2SceneData>>( this.RenderLayer );
		sceneInstance.SetVertexArrayObject( this.Element.UserInterfaceServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex2, Entity2SceneData>() );
		sceneInstance.SetShaderBundle( this.Element.UserInterfaceServiceAccess.ShaderBundleProvider.GetShaderBundle<FlatShade2ShaderBundle>() );
		sceneInstance.SetMesh( this.Element.UserInterfaceServiceAccess.Get<PrimitiveMesh2Provider>().Get( Meshing.Primitive2.Rectangle ) );
		return sceneInstance;
	}


	protected override void OnPlacementChanged() => UpdateInstance();
	protected override void RenderLayerChanged() => this._sceneInstance.SetLayer( this.RenderLayer );

	protected override void OnUpdate( double time, double deltaTime ) {
		if (!this._colorChanged)
			return;
		this._colorChanged = false;
		UpdateInstance();
	}

	private void UpdateInstance() {
		Vector4<ushort> color = (this.Color * ushort.MaxValue).Clamp<Vector4<double>, double>( 0, ushort.MaxValue ).CastSaturating<double, ushort>();
		this._sceneInstance.Write( new Entity2SceneData( this.TransformInterface.Matrix.CastSaturating<double, float>(), color ) );
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
