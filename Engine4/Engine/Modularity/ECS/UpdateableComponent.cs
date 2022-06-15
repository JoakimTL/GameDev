using Engine.Structure;

namespace Engine.Modularity.ECS;
public abstract class UpdateableComponent : Component, IUpdateable {

	public bool Active { get; protected set; }

	//Component requests?
	//Component requirements?
	//How to handle components in systems, continually check or add entity event listeners?
	//Event listeners would mean a system to check if components are available or not.

	public abstract void Update( float time, float deltaTime );

}
