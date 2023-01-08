namespace Engine.ECS;
public abstract class ComponentBase : Identifiable {

	public Entity? Owner { get; internal set; }

	

}
