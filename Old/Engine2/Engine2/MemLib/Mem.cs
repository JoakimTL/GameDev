using Engine.Utilities.Graphics.Disposables;
using Engine.Utilities.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Engine.MemLib {
	public static class Mem {

		public static ManualResetEvent InitializationDone { get; private set; } = new ManualResetEvent( false );
		public static VaultReferences ReferenceVault { get; private set; }

		public static bool Initialized { get; private set; } = false;
		public static CacheLog Logs { get; private set; }
		public static StoreThread Threads { get; private set; }
		public static StoreThreadpool ThreadPool { get; private set; }
		public static StoreSetting Settings { get; private set; }
		public static StoreWindow Windows { get; private set; }
		public static StoreMesh Meshes { get; private set; }
		public static StoreShader Shaders { get; private set; }
		public static UNFINISHEDCacheGLStates EnablesGL { get; private set; }
		public static CacheTexture Textures { get; private set; }
		public static CacheMesh3 Mesh3 { get; private set; }
		public static CacheMesh2 Mesh2 { get; private set; }
		/*public static Store_CollisionData CollisionData { get; private set; }
		public static CacheFont Font { get; private set; }
		/*
		public static TextLogCache TextLog { get; private set; } = new TextLogCache();
		public static CacheDisposable Disposables { get; private set; } = new DisposablesCache();
		*/

		public static void Initialize( bool cacheCurrentThread = false ) {
#if DEBUG
			Process.Start( "EngineDebugTools.exe", $"{Process.GetCurrentProcess().Id}" );
#endif
			Threads = new StoreThread( cacheCurrentThread );
			ReferenceVault = new VaultReferences();
			ReferenceVault.Initialize();
			//	Font = new CacheFont( ReferenceVault );
			Logs = new CacheLog( ReferenceVault );
			Meshes = new StoreMesh( 16777216 ); //2^24
			Mesh2 = new CacheMesh2( ReferenceVault, "res\\meshes", "obj" );
			Mesh3 = new CacheMesh3( ReferenceVault, "res\\meshes", "obj" );
			Textures = new CacheTexture( ReferenceVault, "res\\textures", "png" );
			Settings = new StoreSetting();
			ThreadPool = new StoreThreadpool();
			Windows = new StoreWindow();
			Shaders = new StoreShader();
			EnablesGL = new UNFINISHEDCacheGLStates();
			//	CollisionData = new Store_CollisionData();

			Settings.Add( new SettingsFile( "engine",
					new KeyValuePair<string, object>( $"{SettingsFile.PREFIX_FLOAT}:LightDirShadowWidth", 64.0f ),
					new KeyValuePair<string, object>( $"{SettingsFile.PREFIX_FLOAT}:LightDirShadowDepth", 400.0f ),
					new KeyValuePair<string, object>( $"{SettingsFile.PREFIX_INT}:DirectionalLightShadowResolution", 2048 ),
					new KeyValuePair<string, object>( $"{SettingsFile.PREFIX_INT}:PointLightShadowResolution", 512 ),
					new KeyValuePair<string, object>( $"{SettingsFile.PREFIX_INT}:SpotLightShadowResolution", 512 )
				) );
			Initialized = true;
			InitializationDone.Set();
		}

		public static void PostGLInit() {
			EnablesGL.Initialize();
			Textures.Initialize();
			Mesh3.Initialize();
			Mesh2.Initialize();
		}

		public static void Dispose() {
			Logs.Routine.WriteLine( "Disposing caches!" );
			Windows.Dispose();
			Settings.SaveAll();
			ReferenceVault.Dispose();
			OGLDisposable.DisposeAmount( -1 );
			Thread.Sleep( 250 );
			Logs.Routine.WriteLine( "Done disposing caches!" );
		}
	}
}
