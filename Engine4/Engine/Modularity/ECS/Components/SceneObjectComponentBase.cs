using Engine.Rendering;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Scenes;

namespace Engine.Modularity.ECS.Components;

[OverrideType( typeof( SceneObjectComponentBase ) )]
public abstract class SceneObjectComponentBase : Component {
	public abstract ISceneObject SceneObject { get; }
	protected override sealed byte[]? GetSerializedData() => null;
	public override sealed void SetFromSerializedData( byte[] data ) { }
}

public abstract class SceneObjectComponentBase<V, SD> : SceneObjectComponentBase where V : unmanaged where SD : unmanaged {

	protected readonly OpenSceneObject<V, SD> _sceneObject;
	public override ISceneObject SceneObject => this._sceneObject;

	public SceneObjectComponentBase() {
		this._sceneObject = new OpenSceneObject<V, SD>();
		this._sceneObject.SetSceneData( new SceneInstanceData<SD>( 1, 1 ) );
	}
}
