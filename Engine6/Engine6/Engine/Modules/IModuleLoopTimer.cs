namespace Engine.Modules;

public interface IModuleLoopTimer {
	bool Block();
	void Cancel();
}
