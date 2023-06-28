using Engine.Structure.Interfaces;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Engine.GlobalServices;

public class SettingsService : Identifiable, IGlobalService {

	private readonly ConcurrentDictionary<Type, object> _settingsFiles;
	private readonly JsonSerializerOptions _jsonSerializerOptions;

	public SettingsService() {
		_settingsFiles = new();
		_jsonSerializerOptions = new( JsonSerializerOptions.Default ) {
			WriteIndented = true
		};
	}

	//public T? Get<T>() where T : class, SettingsFileBase, new() {
	//	if ( _settingsFiles.TryGetValue( typeof( T ), out var settingsFile ) )
	//		return settingsFile as T;
	//	if ( !File.Exists( T.SettingsFilePath ) )
	//		return this.LogWarningThenReturnDefault<T>( $"Settings file {T.SettingsFilePath} does not exist." );
	//	try {
	//		T? settings;
	//		if ( !File.Exists( T.SettingsFilePath ) ) {
	//			string fileData = File.ReadAllText( T.SettingsFilePath );
	//			settings = JsonSerializer.Deserialize<T>( fileData );
	//			if ( settings is null )
	//				return this.LogWarningThenReturnDefault<T>( $"Settings file {T.SettingsFilePath} could not be deserialized." );
	//		} else {
	//			settings = new();
	//		}
	//		_settingsFiles.TryAdd( typeof( T ), settings );
	//		settings.SettingsChanged += SettingsChanged;
	//		return settings;
	//	} catch ( Exception e ) {
	//		this.LogError( e );
	//	}
	//	return default;
	//}

	//private void SettingsChanged() {
	//	foreach ( var settingsFile in _settingsFiles.Values )
	//		try {
	//			var settingsPath = settingsFile.GetType().GetProperty( nameof( ISettingsFile.SettingsFilePath ) )?.GetValue( null ) as string;
	//			if ( settingsPath is null ) {
	//				this.LogWarning( $"Settings file {settingsFile.GetType().Name} does not have a SettingsFilePath property." );
	//				continue;
	//			}
	//			File.WriteAllText( settingsPath, JsonSerializer.Serialize( settingsFile, _jsonSerializerOptions ) );
	//		} catch ( Exception e ) {
	//			this.LogError( e );
	//		}
	//}
}