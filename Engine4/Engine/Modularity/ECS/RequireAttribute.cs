namespace Engine.Modularity.ECS;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = false )]
public class RequireAttribute : Attribute {

	public Type ComponentType { get; }

	public RequireAttribute(Type componentType) {
		this.ComponentType = componentType ?? throw new ArgumentNullException( nameof( componentType ) );
	}
}