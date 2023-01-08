namespace Engine.ECS;

public abstract class SystemBase : Identifiable {
	public SystemBase() {

	}

	public abstract void Update( IEnumerable<Entity> entities, float time, float deltaTime );
}
