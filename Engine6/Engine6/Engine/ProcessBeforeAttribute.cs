namespace Engine;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class ProcessBeforeAttribute : Attribute {
    public Type FollowingType { get; }
    public Type? ProcessType { get; }

    public ProcessBeforeAttribute( Type followingType, Type? processType ) {
        FollowingType = followingType;
        ProcessType = processType;
    }

    public ProcessBeforeAttribute( Type followingType ) {
        FollowingType = followingType;
        ProcessType = null;
    }
}

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class ProcessBeforeAttribute<T> : ProcessBeforeAttribute {
    public ProcessBeforeAttribute( Type? processType ) : base( typeof( T ), processType ) { }
    public ProcessBeforeAttribute() : base( typeof( T ) ) { }
}

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class ProcessBeforeAttribute<T, PT> : ProcessBeforeAttribute {
    public ProcessBeforeAttribute() : base( typeof( T ), typeof( PT ) ) { }
}
