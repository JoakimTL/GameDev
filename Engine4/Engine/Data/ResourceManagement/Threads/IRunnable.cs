namespace Engine.Data.ResourceManagement.Threads;

public interface IRunnable {

	/// <summary>
	/// The entry point for threads newly started by <see cref="ThreadManager"/>.
	/// </summary>
	void Run();

}
