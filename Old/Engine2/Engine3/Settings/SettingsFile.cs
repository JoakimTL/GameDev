using Engine.MemLib;
using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Xml.Linq;

namespace Engine.Settings {
	public class SettingsFile {

		private bool changed;
		private string fileSource;
		private Dictionary<Type, Dictionary<string, object>> settings;
		private Thread saveThread;
		private AutoResetEvent saveEvent;

		public event Action SettingsChanged;

		public SettingsFile( string fileSource, params Setting[] settingsList ) {
			this.fileSource = fileSource;

			settings = new Dictionary<Type, Dictionary<string, object>>();
			for( int i = 0; i < settingsList.Length; i++ ) {
				Setting s = settingsList[ i ];
				if( !settings.TryGetValue( s.StoredType, out Dictionary<string, object> dict ) ) {
					settings.Add( s.StoredType, dict = new Dictionary<string, object>() );
				}
				dict[ s.SettingName ] = s.Value;
			}

			saveEvent = new AutoResetEvent( false );
			saveThread = Mem.Threads.StartNew( SaveThreadInternal, $"Save Thread [{fileSource}]" );

			if( !File.Exists( fileSource ) ) {
				SaveInternal();
			} else {
				Setting[] fileSettings = ReadFromFile( fileSource );
				for( int i = 0; i < fileSettings.Length; i++ ) {
					Setting s = fileSettings[ i ];
					if( !settings.TryGetValue( s.StoredType, out Dictionary<string, object> dict ) )
						settings.Add( s.StoredType, dict = new Dictionary<string, object>() );
					dict[ s.SettingName ] = s.Value;
				}
				SaveInternal();
			}
		}

		private void SaveThreadInternal() {
			while( Mem.Threads.Running ) {
				saveEvent.WaitOne();
				changed = false;
				SaveInternal();
			}
		}

		/// <summary>
		/// Marks the settings file as changed and in need of saving
		/// </summary>
		public void SaveSettings() {
			saveEvent.Set();
		}

		/// <summary>
		/// Utilized the current thread to save the settings, not recommended.
		/// </summary>
		public void ForceSave() {
			if( changed )
				SaveInternal();
		}

		private void SaveInternal() {
			string dir = Path.GetDirectoryName( fileSource );
			if( dir.Length > 0 )
				Directory.CreateDirectory( dir );
			try {
				List<byte[]> serializedData = new List<byte[]>();
				foreach( KeyValuePair<Type, Dictionary<string, object>> data in settings ) {
					foreach( KeyValuePair<string, object> setting in data.Value ) {
						using MemoryStream stream = new MemoryStream();
						BinaryFormatter formatter = new BinaryFormatter();
						formatter.Serialize( stream, setting.Value );
						serializedData.Add( DataTransform.GetBytes( setting.Key ) );
						serializedData.Add( stream.ToArray() );
					}
				}
				File.WriteAllBytes( fileSource, Segmentation.Segment( serializedData.ToArray() ) );
			} catch( Exception e ) {
				if( File.Exists( fileSource ) && File.ReadAllBytes( fileSource ).Length == 0 ) {
					File.Delete( fileSource );
					Logging.Warning( "Unable to save data, deleting empty file.\n" + e.ToString() );
				} else {
					Logging.Warning( $"Unable to save data, file is not empty and is not deleted. Consider resetting your settings. Go to [{fileSource}] and delete the file.\n" + e.ToString() );
				}
			}
		}

		private static Setting[] ReadFromFile( string filePath ) {
			if( !File.Exists( filePath ) )
				return new Setting[ 0 ];
			byte[] data = File.ReadAllBytes( filePath );
			Segmentation.Parse( data, out byte[][] dataSegments, out int[] segmentLengths );
			if( dataSegments.Length % 2 == 1 ) {
				Logging.Warning( $"Setting file [{filePath}] is corrupt. Unable to read data from file, will default the settings." );
				return new Setting[ 0 ];
			}
			Setting[] returns = new Setting[ dataSegments.Length / 2 ];
			for( int i = 0; i < dataSegments.Length; i += 2 ) {
				string name = DataTransform.ToString( dataSegments[ i ] );
				object obj;
				using MemoryStream stream = new MemoryStream( dataSegments[ i + 1 ] );
				BinaryFormatter formatter = new BinaryFormatter();
				obj = formatter.Deserialize( stream );
				returns[ i / 2 ] = new Setting( name, obj );
			}
			return returns;
		}

		public bool TryGet<T>( string setting, out T value ) {
			value = default;
			if( settings.TryGetValue( typeof( T ), out Dictionary<string, object> dict ) ) {
				if( dict.TryGetValue( setting, out object s ) ) {
					if( s is T ts ) {
						value = ts;
						return true;
					}
				}
			}
			return false;
		}

		public void Set( string setting, object value ) {
			Type vType = value.GetType();
			lock( settings ) {
				if( !settings.TryGetValue( vType, out Dictionary<string, object> dict ) ) {
					settings.Add( vType, dict = new Dictionary<string, object>() );
				}
				dict[ setting ] = value;
			}
			changed = true;
			saveEvent.Set();
			SettingsChanged?.Invoke();
		}

	}

	public class Setting {

		public string SettingName { get; private set; }
		public Type StoredType { get; private set; }
		public object Value { get; private set; }

		public Setting( string settingName, object value ) {
			SettingName = settingName;
			Value = value;
			StoredType = value.GetType();
		}

	}
}
