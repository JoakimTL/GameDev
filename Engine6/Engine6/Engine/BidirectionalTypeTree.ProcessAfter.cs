namespace Engine;

public partial class Direction {
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
    public class AfterAttribute : Attribute {
        public Type PrecedingType { get; }
        public Type? ProcessType { get; }

        public AfterAttribute( Type precedingType, Type? processType ) {
            PrecedingType = precedingType;
            ProcessType = processType;
        }

        public AfterAttribute( Type precedingType ) {
            PrecedingType = precedingType;
            ProcessType = null;
        }
    }

    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
    public class AfterAttribute<T> : AfterAttribute {
        public AfterAttribute( Type? processType ) : base( typeof( T ), processType ) { }
        public AfterAttribute() : base( typeof( T ) ) { }
    }

    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
    public class AfterAttribute<T, PT> : AfterAttribute {
        public AfterAttribute() : base( typeof( T ), typeof( PT ) ) { }
    }
}
