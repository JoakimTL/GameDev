using Engine.Modularity.ECS.Components;
using Engine.Rendering.Standard.Scenes;

namespace Engine.Modularity.ECS.Organization;
public abstract class EntitySceneManager<R> : Identifiable where R : Component {

	protected readonly EntityManager _manager;
	protected readonly Scene _scene;
	private readonly ComponentOrganizer<R> _renderComponentOrganizer;

	public EntitySceneManager( EntityManager manager, Scene scene ) {
		this._manager = manager;
		this._scene = scene;
		this._renderComponentOrganizer = new ComponentOrganizer<R>( manager );
		this._renderComponentOrganizer.OnComponentAdded += ComponentAdded;
		this._renderComponentOrganizer.OnComponentRemoved += ComponentRemoved;
	}

	private void ComponentAdded( Entity e, R c ) => AddSceneObjectComponent( e );
	private void ComponentRemoved( Entity e, R c ) => RemoveSceneObjectComponent( e );
	protected abstract void AddSceneObjectComponent( Entity e );
	protected abstract void RemoveSceneObjectComponent( Entity e );
}

public class EntityScene3Manager : EntitySceneManager<Render3Component> {
	public EntityScene3Manager( EntityManager manager, Scene scene ) : base( manager, scene ) { }

	protected override void AddSceneObjectComponent( Entity e ) {
		SceneObject3Component sceneObjectComponent = new();
		e.AddComponent( sceneObjectComponent );
		this._scene.AddSceneObject( sceneObjectComponent.SceneObject );
	}

	protected override void RemoveSceneObjectComponent( Entity e ) {
		SceneObject3Component? sceneObjectComponent = e.RemoveComponent<SceneObject3Component>();
		if ( sceneObjectComponent is not null )
			this._scene.RemoveSceneObject( sceneObjectComponent.SceneObject );
	}
}