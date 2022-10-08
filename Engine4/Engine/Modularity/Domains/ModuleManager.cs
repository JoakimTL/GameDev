//using System.Collections.Concurrent;

//namespace Engine.Modularity.Modules;
//public class ModuleManager : DisposableIdentifiable {

//	private readonly ConcurrentDictionary<uint, Module> _modules;

//	public ModuleManager() {
//		this._modules = new ConcurrentDictionary<uint, Module>();
//	}

//	public void AddModule( Module module ) {
//		if ( !this._modules.TryAdd( module.ModuleId, module ) ) {
//			this.LogWarning( $"Module id already taken, unable to add {module} to modules!" );
//			return;
//		}
//		module.OnDisposal += ModuleDisposed;
//		module.Initialize();
//		this.LogLine( $"Initialized {module}!", Log.Level.NORMAL, color: ConsoleColor.Blue );
//	}

//	private void ModuleDisposed( Module module ) {
//		if ( this._modules.TryRemove( module.ModuleId, out _ ) )
//			if ( !this._modules.Values.Any( p => p.Essential ) )
//				Dispose();
//		//Client disposes server?
//	}

//	protected override bool OnDispose() {
//		foreach ( Module? module in this._modules.Values )
//			module.Dispose();
//		this._modules.Clear();
//		return true;
//	}
//}
