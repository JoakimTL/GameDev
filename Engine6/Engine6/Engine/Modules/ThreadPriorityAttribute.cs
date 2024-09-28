namespace Engine.Modules;

[AttributeUsage( AttributeTargets.Class )]
public sealed class ThreadPriorityAttribute( ThreadPriority priority ) : Attribute {
	public ThreadPriority Priority { get; } = priority;
}
