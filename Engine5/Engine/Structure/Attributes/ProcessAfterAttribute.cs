namespace Engine.Structure.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class ProcessAfterAttribute : Attribute
{
    public Type PrecedingType { get; }
    public Type? ProcessType { get; }

    public ProcessAfterAttribute(Type precedingType, Type? processType)
    {
        PrecedingType = precedingType;
        ProcessType = processType;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class ProcessAfterAttribute<T> : ProcessAfterAttribute
{
    public ProcessAfterAttribute(Type? processType) : base(typeof(T), processType) { }
}
