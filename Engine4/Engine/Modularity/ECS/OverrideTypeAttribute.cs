namespace Engine.Modularity.ECS;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
public class OverrideTypeAttribute : Attribute {

	public readonly Type Type;

	public OverrideTypeAttribute( Type type ) {
		this.Type = type;
	}

}


