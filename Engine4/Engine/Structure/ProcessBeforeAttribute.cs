namespace Engine.Structure;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class ProcessBeforeAttribute : Attribute {
	public Type FollowingType { get; }
	public Type? ProcessType { get; }

	public ProcessBeforeAttribute( Type followingType, Type? processType ) {
		this.FollowingType = followingType ?? throw new ArgumentNullException( nameof( followingType ) );
		this.ProcessType = processType;
	}
}