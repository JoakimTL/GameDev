namespace Engine.Structure;

/// <summary>
/// If a service is required but not used within the service.
/// </summary>
[AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
public class RequireServiceAttribute : Attribute {

	public Type Type { get; }
	public RequireServiceAttribute( Type type ) {
		this.Type = type ?? throw new ArgumentNullException( nameof( type ) );
	}
}