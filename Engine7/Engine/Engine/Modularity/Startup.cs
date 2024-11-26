namespace Engine.Modularity;

public class Startup {
	public static Startup BeginInit() {
		return new();
	}

	public Startup WithModule<T>() where T : ModuleBase, new() {
		ModuleManager.StartModule<T>();
		return this;
	}

	/// <summary>
	/// Called after all modules have been added.
	/// </summary>
	public void Start() {
		ModuleManager.Initialize();
	}
}
