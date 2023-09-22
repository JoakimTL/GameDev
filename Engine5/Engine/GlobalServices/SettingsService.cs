using Engine.GlobalServices.Settings;
using Engine.Structure.Interfaces;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace Engine.GlobalServices;

public class SettingsService : Identifiable, IGlobalService {

	private readonly ConcurrentDictionary<Type, SettingsFileBase> _settingsFiles;
	private readonly JsonSerializerOptions _serializeOptions;

	public SettingsService() {
		_settingsFiles = new();
		_serializeOptions = new JsonSerializerOptions( JsonSerializerDefaults.General ) {
			WriteIndented = true,
		};
	}

	private static string GetFilePath( FilePathAttribute filePathAttribute ) => Path.Combine( "settings", filePathAttribute.FilePath );

	public T? Get<T>() where T : SettingsFileBase, new() {
		if ( _settingsFiles.TryGetValue( typeof( T ), out var settingsFile ) )
			return settingsFile as T;
		var t = typeof( T );
		var filePathAttribute = t.GetCustomAttribute<FilePathAttribute>();
		if ( filePathAttribute is null )
			return this.LogWarningThenReturnDefault<T>( $"Settings file {t.Name} does not have a FilePathAttribute." );
		var filePath = GetFilePath( filePathAttribute );
		try {
			T? settings;
			if ( File.Exists( filePath ) ) {
				string fileData = File.ReadAllText( filePath );
				settings = new();
				var data = JsonSerializer.Deserialize<Dictionary<string, object>>( fileData );
				if ( data is not null ) {
					settings.SetValues( data );
				} else
					this.LogWarning( $"Settings file {t.Name} could not be deserialized." );
				_settingsFiles.TryAdd( typeof( T ), settings );
			} else {
				settings = new();
				_settingsFiles.TryAdd( typeof( T ), settings );
				SettingsChanged( settings );
			}
			settings.SettingsChanged += SettingsChanged;
			return settings;
		} catch ( Exception e ) {
			this.LogError( e );
		}
		return default;
	}

	private void SettingsChanged( SettingsFileBase settingsFile ) {
		try {
			var filePath = GetFilePath( settingsFile.GetType().GetCustomAttribute<FilePathAttribute>().NotNull() );
			var directory = Path.GetDirectoryName( filePath ).NotNull();
			if ( !Directory.Exists( directory ) )
				Directory.CreateDirectory( directory );
			var serializedData = JsonSerializer.Serialize( settingsFile._settingValues, _serializeOptions );
			if ( serializedData is null ) {
				this.LogWarning( $"Settings file {filePath} could not be serialized." );
				return;
			}
			File.WriteAllText( filePath, serializedData );
		} catch ( Exception e ) {
			this.LogError( e );
		}
	}
}