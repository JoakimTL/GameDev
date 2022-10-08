//using Engine.Modularity.Modules.Submodules;

//namespace Engine.Modularity.Modules;
//public static class ModuleGenerator {

//	public static Module CreateModule( string name, uint moduleId, bool essential, params IEnumerable<Type>[] submoduleTypes ) {
//		Module module = new( name, moduleId, essential );
//		foreach ( Type? submoduleType in submoduleTypes.SelectMany( p => p ) )
//			module.AddSubmodule( submoduleType );
//		return module;
//	}

//	public static Module CreateModule( string name, uint moduleId, bool essential, int delay, params IEnumerable<Type>[] submoduleTypes ) {
//		Module module = new( name, moduleId, essential, delay );
//		foreach ( Type? submoduleType in submoduleTypes.SelectMany( p => p ) )
//			module.AddSubmodule( submoduleType );
//		return module;
//	}

//	public static IEnumerable<Type> GetRenderSubmodules() {
//		yield return typeof( GlfwSubmodule );
//		yield return typeof( ContextUpdateSubmodule );
//		yield return typeof( WindowCreationSubmodule );
//		yield return typeof( WindowSwapSubmodule );
//	}

//	public static IEnumerable<Type> GetClientSubmodules() {
//		yield return typeof( ClientSubmodule );
//	}

//	public static IEnumerable<Type> GetServerSubmodules() {
//		yield return typeof( ServerSubmodule );
//	}
//}
