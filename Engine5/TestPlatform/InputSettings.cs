namespace TestPlatformClient;
public class InputSettings {
	public static string SettingsFilePath => "settings/InputBindings.json";

	public event Action? SettingsChanged;
}
