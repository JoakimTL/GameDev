using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.IO {
	public class SettingsFile : IDisposable {

		public const string PREFIX = "settings\\";
		public const string POSTFIX = ".cfg";

		public static readonly string PREFIX_STRING = typeof( string ).Name.ToUpper();
		public static readonly string PREFIX_INT = typeof( int ).Name.ToUpper();
		public static readonly string PREFIX_FLOAT = typeof( float ).Name.ToUpper();

		private FileSystemWatcher fileWatcher;
		private string filepath;
		public string Filepath => filepath;
		private string name;
		public string Name => name;

		public delegate void FileContentChangedEvent( SettingsFile file );
		public event FileContentChangedEvent OnChanged;

		private Dictionary<string, string> list;
		private Dictionary<string, int> intList;
		private Dictionary<string, float> floatList;

		public SettingsFile( string name, params KeyValuePair<string, object>[] @default ) {
			this.name = name;
			this.filepath = PREFIX + name + POSTFIX;
			this.list = new Dictionary<string, string>();
			this.intList = new Dictionary<string, int>();
			this.floatList = new Dictionary<string, float>();

			string directory = Path.GetDirectoryName( this.filepath );
			string filename = Path.GetFileName( this.filepath );
			if( !Directory.Exists( directory ) )
				Directory.CreateDirectory( directory );

			LoadDefaults( @default );
			Load();

			fileWatcher = new FileSystemWatcher {
				Path = directory,
				Filter = filename,
				IncludeSubdirectories = true,
				EnableRaisingEvents = true
			};
			fileWatcher.Changed += FileChangeEvent;
		}

		private void FileChangeEvent( object sender, FileSystemEventArgs e ) {
			Reload();
		}

		public string Get( string settingString ) {
			if( list.TryGetValue( settingString, out string v ) )
				return v;
			return string.Empty;
		}

		public string GetString( string settingName ) {
			return Get( $"{PREFIX_STRING}:{settingName}" );
		}

		public int GetInt( string settingName ) {
			if( intList.TryGetValue( $"{PREFIX_INT}:{settingName}", out int v ) )
				return v;
			return 0;
		}

		public float GetFloat( string settingName ) {
			if( floatList.TryGetValue( $"{PREFIX_FLOAT}:{settingName}", out float v ) )
				return v;
			return 0;
		}

		public void Set( string settingString, string value ) {
			list[ settingString ] = value;
		}

		public void SetString( string settingName, string value ) {
			Set( $"{PREFIX_STRING}:{settingName}", value );
		}

		public void SetInt( string settingName, int value ) {
			string settingstring = $"{PREFIX_INT}:{settingName}";
			Set( settingstring, value.ToString() );
			intList[ settingstring ] = value;
		}

		public void SetFloat( string settingName, float value ) {
			string settingstring = $"{PREFIX_FLOAT}:{settingName}";
			Set( settingstring, value.ToString() );
			floatList[ settingstring ] = value;
		}

		private void Load() {
			if( !File.Exists( filepath ) )
				Save();

			string[] lines = File.ReadAllLines( filepath );
			for( int i = 0; i < lines.Length; i++ ) {
				string[] line = lines[ i ].Split( '=' );
				if( line.Length == 2 ) {
					string[] identifiers = line[ 0 ].Split( ':' );
					list[ line[ 0 ] ] = line[ 1 ];
					if( identifiers[ 0 ] == PREFIX_INT && int.TryParse( line[ 1 ], out int iV ) )
						intList[ line[ 0 ] ] = iV;
					if( identifiers[ 0 ] == PREFIX_FLOAT && float.TryParse( line[ 1 ], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out float fV ) )
						floatList[ line[ 0 ] ] = fV;
				}
			}

		}

		private void Reload() {
			bool readFile = false;
			while( !readFile ) {
				try {
					Load();
					readFile = true;
				} catch( IOException ) {
					MemLib.Mem.Logs.Error.WriteLine( "Updated file is busy... Attempting to read again!" );
				} catch {
					MemLib.Mem.Logs.Error.WriteLine( "Couldn't read updated file. Aborting!" );
					break;
				}
			}
			OnChanged?.Invoke( this );
		}

		public void Save() {
			if( !( fileWatcher is null ) )
				fileWatcher.EnableRaisingEvents = false;
			List<string> lines = new List<string>();

			foreach( string key in list.Keys )
				lines.Add( $"{key}={list[ key ]}" );

			string path = Path.GetDirectoryName( filepath );
			if( !Directory.Exists( path ) )
				Directory.CreateDirectory( path );

			File.WriteAllLines( filepath, lines.ToArray() );

			MemLib.Mem.Logs.Routine.WriteLine( $"Saved settings for [{Name}] at [{path}]!" );
			if( !( fileWatcher is null ) )
				fileWatcher.EnableRaisingEvents = true;
		}

		private void LoadDefaults( KeyValuePair<string, object>[] @default ) {
			for( int i = 0; i < @default.Length; i++ ) {
				list[ @default[ i ].Key ] = @default[ i ].Value.ToString();

				if( @default[ i ].Key.StartsWith( PREFIX_INT ) )
					intList[ @default[ i ].Key ] = (int) @default[ i ].Value;
				if( @default[ i ].Key.StartsWith( PREFIX_FLOAT ) )
					floatList[ @default[ i ].Key ] = (float) @default[ i ].Value;
			}
		}

		public void Dispose() {
			fileWatcher.Dispose();
		}
	}
}
