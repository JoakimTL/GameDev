namespace Engine.Structure.Attributes;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class RequireAttribute : Attribute {
	public Type RequiredType { get; }

	public RequireAttribute( Type requiredType ) {
		RequiredType = requiredType;
	}
}

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class RequireAttribute<T> : RequireAttribute {
	public RequireAttribute() : base( typeof( T ) ) { }
}