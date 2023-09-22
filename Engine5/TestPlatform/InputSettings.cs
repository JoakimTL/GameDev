using Engine.GlobalServices.Settings;

namespace TestPlatformClient;

[FilePath( "InputBindings.json" )]
public sealed class InputSettings : SettingsFileBase {
	[DefaultValue( "Key:W" )]
	public string Forward { get => Get<string>(); set => Set( value ); }
	[DefaultValue( "Key:S" )]
	public string Backward { get => Get<string>(); set => Set( value ); }
	[DefaultValue( "Key:A" )]
	public string Left { get => Get<string>(); set => Set( value ); }
	[DefaultValue( "Key:D" )]
	public string Right { get => Get<string>(); set => Set( value ); }
}
