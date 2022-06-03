using Engine.Modularity.Modules;

namespace Engine;
public static class Startup {

	private static readonly ModuleManager _moduleManager = new();

	public static void Start( params Module[] modules ) {
		foreach ( Module? module in modules ) 
			_moduleManager.AddModule( module );
	}

}
