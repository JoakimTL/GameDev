namespace Engine;

public partial class Direction {
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
    public class BeforeAttribute : Attribute {
        public Type FollowingType { get; }
        public Type? ProcessType { get; }

        public BeforeAttribute( Type followingType, Type? processType ) {
            FollowingType = followingType;
            ProcessType = processType;
        }

        public BeforeAttribute( Type followingType ) {
            FollowingType = followingType;
            ProcessType = null;
        }
    }

    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
    public class BeforeAttribute<T> : BeforeAttribute {
        public BeforeAttribute( Type? processType ) : base( typeof( T ), processType ) { }
        public BeforeAttribute() : base( typeof( T ) ) { }
    }

    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
    public class BeforeAttribute<T, PT> : BeforeAttribute {
        public BeforeAttribute() : base( typeof( T ), typeof( PT ) ) { }
    }
}