namespace Engine.Structure;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class ProcessAfterAttribute : Attribute {
	public Type PrecedingType { get; }
	public Type? ProcessType { get; }

	public ProcessAfterAttribute( Type precedingType, Type? processType ) {
		this.PrecedingType = precedingType ?? throw new ArgumentNullException( nameof( precedingType ) );
		this.ProcessType = processType;
	}
}
