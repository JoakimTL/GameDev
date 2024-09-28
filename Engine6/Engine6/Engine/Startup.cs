using Engine.Modules;

namespace Engine;

public static class Startup {

	private static readonly ModuleManager _moduleManager = new();

	public static void StartModule<T>( params Type[] serviceTypes ) where T : ModuleBase, new() {
		T t = new();
		t.AddServices( serviceTypes );
		_moduleManager.Start( t );
	}
}