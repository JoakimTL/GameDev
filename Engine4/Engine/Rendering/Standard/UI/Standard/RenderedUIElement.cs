using Engine.Rendering.Standard.Scenes;

namespace Engine.Rendering.Standard.UI.Standard;

public abstract class RenderedUIElement<DATA, SHADER, V, SD> : UIElement where DATA : class, new() where SHADER : ShaderBundle where V : unmanaged where SD : unmanaged {

	private IUIConstraint<DATA>? _renderConstraint;
	protected readonly DATA _renderData;
	protected OpenSceneObject<V, SD> SceneObject { get; private set; }
	protected SceneInstanceData<SD> SceneInstanceData { get; private set; }

	public event Action<UIElement>? RenderingUpdated;

	public RenderedUIElement() {
		this.SceneObject = new OpenSceneObject<V, SD>();
		this.SceneObject.SetShaders<SHADER>();
		this.SceneObject.SetSceneData( this.SceneInstanceData = new SceneInstanceData<SD>( 1, 1 ) );
		SetSceneObject( this.SceneObject );
		TransformsUpdated += TransformUpdated;
		this._renderData = new DATA();
		Updated += RenderConstraintUpdate;
	}

	public void SetConstraint( IUIConstraint<DATA> constriant ) => this._renderConstraint = constriant;

	private void RenderConstraintUpdate( UIElement e, float time, float deltaTime ) {
		this._renderConstraint?.Apply( time, deltaTime, this._renderData );
		RenderingUpdated?.Invoke( this );
	}

	private void TransformUpdated( UIElement @this ) => this.SceneInstanceData.SetInstance( 0, GetSceneData() );

	protected abstract SD GetSceneData();
}
