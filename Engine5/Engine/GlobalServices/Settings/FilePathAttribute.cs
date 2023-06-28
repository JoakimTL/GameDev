namespace Engine.GlobalServices.Settings;

[AttributeUsage( AttributeTargets.Class )]
public sealed class FilePathAttribute : Attribute {
	public string FilePath { get; }
	public FilePathAttribute( string filePath ) {
		FilePath = filePath ?? throw new ArgumentNullException( nameof( filePath ) );
	}
}
