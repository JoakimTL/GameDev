using Engine.Logging;
using Engine.Time;

namespace Engine.Modularity;

public abstract class ModuleBase : DisposableIdentifiable {

	//TODO Add message bus so that EntityContainer can be sent to the render module
	public IInstanceProvider InstanceProvider { get; }
	private readonly InstanceUpdaterExtension _instanceUpdaterExtension;
	private readonly InstanceInitializerExtension _instanceInitializerExtension;
	public bool Important { get; }
	public bool Running { get; private set; }
	public double ExecutionFrequency { get; private set; }
	private double _lastTickTime;
	protected Clock<double, StopwatchTickSupplier> ModuleClock { get; }

	internal event Action? FrequencyAltered;

	protected event Action? OnInitialize;
	protected event UpdateHandler? OnUpdate;
	protected event Action? OnDisposing;

	/// <param name="important">Determines if this module keeps the application running</param>
	/// <param name="frequency">The number of ticks per second. If <see cref="ExecutionFrequency"/> is <see cref="double.PositiveInfinity"/> (or any high enough number), there is no delay between ticks.</param>
	public ModuleBase( bool important, double frequency ) {
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero( frequency );
		this.InstanceProvider = InstanceManagement.CreateProvider();
		this._instanceUpdaterExtension = this.InstanceProvider.CreateUpdater();
		this._instanceInitializerExtension = this.InstanceProvider.CreateInitializer();
		this.Important = important;
		this.ExecutionFrequency = frequency;
		this.ModuleClock = Clock<double, StopwatchTickSupplier>.ReferenceClock;
		this.Running = true;
	}

	/// <param name="frequency">The number of ticks per second. If <see cref="TimeBetweenTicksMs"/> is 0, there is no delay between ticks.</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	protected void SetExecutionFrequency( double frequency ) {
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero( frequency );
		this.ExecutionFrequency = frequency;
		FrequencyAltered?.Invoke();
	}

	public void Stop() {
		if (!this.Running)
			return;
		this.Running = false;
		this.LogLine( "Shutdown was requested." );
	}

	internal void Initialize() {
		OnInitialize?.Invoke();
	}

	internal bool DoTick() {
		if (this.Running) {
			double currentTime = this.ModuleClock.Time;
			double timeSinceLastTick = currentTime - this._lastTickTime;
			this._lastTickTime = currentTime;
			this._instanceInitializerExtension.Update( currentTime, timeSinceLastTick );
			this._instanceUpdaterExtension.Update( currentTime, timeSinceLastTick );
			OnUpdate?.Invoke( currentTime, timeSinceLastTick );
		}
		return this.Running;
	}

	protected override bool InternalDispose() {
		OnDisposing?.Invoke();
		this.InstanceProvider.Dispose();
		return true;
	}
}
