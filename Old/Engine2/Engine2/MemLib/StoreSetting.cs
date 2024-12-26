using Engine.Utilities.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.MemLib {
	public class StoreSetting {

		private Dictionary<string, SettingsFile> filesByName;
		private Dictionary<string, SettingsFile> filesByPath;

		public StoreSetting() {
			filesByName = new Dictionary<string, SettingsFile>();
			filesByPath = new Dictionary<string, SettingsFile>();

			if( !Directory.Exists( SettingsFile.PREFIX ) )
				Directory.CreateDirectory( SettingsFile.PREFIX );
		}

		public SettingsFile this[ string filename ] {
			get {
				if( filesByName.TryGetValue( filename, out SettingsFile o ) )
					return o;
				return null;
			}
		}

		public void Add( SettingsFile settings ) {
			if( filesByName.ContainsKey( settings.Name ) || settings is null )
				return;
			filesByName.Add( settings.Name, settings );
		}

		public bool FileExists( string filename ) {
			return filesByName.ContainsKey( filename );
		}

		internal void SaveAll() {
			foreach( SettingsFile s in filesByName.Values )
				s.Save();
		}
	}
}
