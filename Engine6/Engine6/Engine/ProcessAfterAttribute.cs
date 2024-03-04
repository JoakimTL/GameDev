namespace Engine;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class ProcessAfterAttribute : Attribute {
    public Type PrecedingType { get; }
    public Type? ProcessType { get; }

    public ProcessAfterAttribute( Type precedingType, Type? processType ) {
        PrecedingType = precedingType;
        ProcessType = processType;
    }

    public ProcessAfterAttribute( Type precedingType ) {
        PrecedingType = precedingType;
        ProcessType = null;
    }
}

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class ProcessAfterAttribute<T> : ProcessAfterAttribute {
    public ProcessAfterAttribute( Type? processType ) : base( typeof( T ), processType ) { }
    public ProcessAfterAttribute() : base( typeof( T ) ) { }
}

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class ProcessAfterAttribute<T, PT> : ProcessAfterAttribute {
    public ProcessAfterAttribute() : base( typeof( T ), typeof( PT ) ) { }
}
