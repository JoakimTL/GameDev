namespace Engine.Structure;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class ProcessBeforeAttribute : Attribute {
	public Type FollowingType { get; }
	public Type? ProcessType { get; }

	public ProcessBeforeAttribute( Type followingType, Type? processType ) {
		FollowingType = followingType;
		ProcessType = processType;
	}
}

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class ProcessBeforeAttribute<T> : ProcessBeforeAttribute {
	public ProcessBeforeAttribute( Type? processType ) : base( typeof( T ), processType ) { }
}
