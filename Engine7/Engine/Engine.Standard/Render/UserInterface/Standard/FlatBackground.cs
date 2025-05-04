using Engine.Standard.Render.Meshing.Services;
using Engine.Standard.Render.Shaders;
using Engine.Standard.Render.Text.Typesetting;

namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class FlatBackground : UserInterfaceComponentBase {

	private readonly SceneInstance<Entity2SceneData> _sceneInstance;

	private Vector4<double> _color = (1, 1, 1, 1);
	private bool _colorChanged;
	public Vector4<double> Color {
		get => _color;
		set {
			if (_color == value)
				return;
			_color = value;
			_colorChanged = true;
		}
	}

	public FlatBackground( UserInterfaceElementBase element ) : base( element ) {
		_sceneInstance = CreateSceneInstance();
	}

	private SceneInstance<Entity2SceneData> CreateSceneInstance() {
		SceneInstance<Entity2SceneData> sceneInstance = Element.UserInterfaceServiceAccess.RequestSceneInstance<SceneInstance<Entity2SceneData>>( RenderLayer );
		sceneInstance.SetVertexArrayObject( Element.UserInterfaceServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex2, Entity2SceneData>() );
		sceneInstance.SetShaderBundle( Element.UserInterfaceServiceAccess.ShaderBundleProvider.GetShaderBundle<FlatShade2ShaderBundle>() );
		sceneInstance.SetMesh( Element.UserInterfaceServiceAccess.Get<PrimitiveMesh2Provider>().Get( Meshing.Primitive2.Rectangle ) );
		return sceneInstance;
	}


	protected override void OnPlacementChanged() => UpdateInstance();

	protected override void OnUpdate( double time, double deltaTime ) {
		if (!_colorChanged)
			return;
		_colorChanged = false;
		UpdateInstance();
	}

	private void UpdateInstance() {
		Vector4<ushort> color = (Color * ushort.MaxValue).Clamp<Vector4<double>, double>( 0, ushort.MaxValue ).CastSaturating<double, ushort>();
		_sceneInstance.Write( new Entity2SceneData( TransformInterface.Matrix.CastSaturating<double, float>(), color ) );
	}

	protected override void InternalRemove() {
		_sceneInstance.Remove();
	}

	protected internal override void DoHide() {
		_sceneInstance.SetAllocated( false );
	}

	protected internal override void DoShow() {
		_sceneInstance.SetAllocated( true );
		UpdateInstance();
	}
}
