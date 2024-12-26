using Engine.Settings;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.MemLib {
	public class StoreSetting {

		public const string PREFIX = "settings\\";
		public const string POSTFIX = ".cfg";

		private Dictionary<string, SettingsFile> filesByName;

		public StoreSetting() {
			filesByName = new Dictionary<string, SettingsFile>();

			if( !Directory.Exists( PREFIX ) )
				Directory.CreateDirectory( PREFIX );
		}

		public SettingsFile this[ string filename ] {
			get {
				if( filesByName.TryGetValue( filename, out SettingsFile o ) )
					return o;
				return null;
			}
		}

		public SettingsFile Add( string name, params Setting[] defaultSettings ) {
			if( filesByName.TryGetValue( name, out SettingsFile settings ) )
				return settings;
			settings = Create(name, defaultSettings);
			filesByName.Add( name, settings );
			return settings;
		}

		private SettingsFile Create( string name, params Setting[] defaultSettings ) {
			return new SettingsFile( PREFIX + name + POSTFIX, defaultSettings );
		}

		public bool FileExists( string name ) {
			return filesByName.ContainsKey( name );
		}

		internal void SaveAll() {
			foreach( SettingsFile s in filesByName.Values )
				s.ForceSave();
		}
	}
}
