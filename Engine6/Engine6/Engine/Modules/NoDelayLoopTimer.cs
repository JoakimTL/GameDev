namespace Engine.Modules;

public sealed class NoDelayLoopTimer : IModuleLoopTimer {
	private bool _alive = true;
	public bool Block() => _alive;
	public void Cancel() => _alive = false;
}
