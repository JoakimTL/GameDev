using Engine.Settings;
using Engine.Utilities.Graphics.Disposables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
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
		public static StoreShader Shaders { get; private set; }
		public static UNFINISHEDCacheGLStates EnablesGL { get; private set; }
		public static CacheTexture Textures { get; private set; }
		public static CacheMesh3 Mesh3 { get; private set; }
		public static CacheMesh2 Mesh2 { get; private set; }
		public static SampleCollisionData CollisionMolds { get; private set; }
		public static SampleShaderBundles ShaderBundles { get; private set; }
		public static StoreFont Font { get; private set; }
		/*
		public static TextLogCache TextLog { get; private set; } = new TextLogCache();
		public static CacheDisposable Disposables { get; private set; } = new DisposablesCache();
		*/

		public static void Initialize( bool cacheCurrentThread = false ) {
			Threads = new StoreThread( cacheCurrentThread );
			ReferenceVault = new VaultReferences();
			ReferenceVault.Initialize();
			Logs = new CacheLog( ReferenceVault );
			Mesh2 = new CacheMesh2( ReferenceVault, "res\\meshes", "obj" );
			Mesh3 = new CacheMesh3( ReferenceVault, "res\\meshes", "obj" );
			Textures = new CacheTexture( ReferenceVault, "res\\textures", "png" );
			Font = new StoreFont();
			Settings = new StoreSetting();
			ThreadPool = new StoreThreadpool();
			Windows = new StoreWindow();
			Shaders = new StoreShader();
			EnablesGL = new UNFINISHEDCacheGLStates();
			CollisionMolds = new SampleCollisionData();
			AppDomain.CurrentDomain.FirstChanceException += FirstChangeExceptionLogging;
			Initialized = true;
			InitializationDone.Set();
		}

		private static void FirstChangeExceptionLogging( object sender, FirstChanceExceptionEventArgs e ) {
			Logs.SilentException.WriteLine( e.Exception );
		}

		public static void PostGLInit() {
			EnablesGL.Initialize();
			Textures.Initialize();
			Mesh3.Initialize();
			Mesh2.Initialize();
			ShaderBundles = new SampleShaderBundles();
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
