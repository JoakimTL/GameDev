using Engine.Time;

namespace Engine.Modularity;

public abstract class ModuleBase : DisposableIdentifiable {

	public IInstanceProvider InstanceProvider { get; }
	private readonly InstanceUpdaterExtension _instanceUpdaterExtension;
	private readonly InstanceInitializerExtension _instanceInitializerExtension;
	public bool Important { get; }
	public bool Running { get; private set; }
	public double ExecutionFrequency { get; private set; }
	private double _lastTickTime;
	protected Clock<double, StopwatchTickSupplier> ModuleClock { get; }
	internal event Action? FrequencyAltered;

	/// <param name="important">Determines if this module keeps the application running</param>
	/// <param name="frequency">The number of ticks per second. If <see cref="TimeBetweenTicksMs"/> is 0, there is no delay between ticks.</param>
	public ModuleBase( bool important, double frequency ) {
		if (frequency <= 0)
			throw new ArgumentOutOfRangeException( "Execution frequency must be a non-zero positive number." );
		InstanceProvider = InstanceManagement.CreateProvider();
		_instanceUpdaterExtension = InstanceProvider.CreateUpdater();
		_instanceInitializerExtension = InstanceProvider.CreateInitializer();
		this.Important = important;
		ExecutionFrequency = frequency;
		ModuleClock = new( 1 );
		Running = true;
	}

	/// <param name="frequency">The number of ticks per second. If <see cref="TimeBetweenTicksMs"/> is 0, there is no delay between ticks.</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	protected void SetExecutionFrequency( double frequency ) {
		if (frequency <= 0)
			throw new ArgumentOutOfRangeException( "Execution frequency must be a non-zero positive number." );
		this.ExecutionFrequency = frequency;
		FrequencyAltered?.Invoke();
	}

	public void Stop() {
		Running = false;
		this.LogLine( "Shutdown was requested." );
	}

	internal bool DoTick() {
		if (Running) {
			var currentTime = ModuleClock.Time;
			var timeSinceLastTick = currentTime - _lastTickTime;
			_lastTickTime = currentTime;
			_instanceInitializerExtension.Update( currentTime, timeSinceLastTick );
			_instanceUpdaterExtension.Update( currentTime, timeSinceLastTick );
			Tick( currentTime, timeSinceLastTick );
		}
		return Running;
	}

	protected override bool InternalDispose() {
		InstanceProvider.Dispose();
		return true;
	}
	protected internal abstract void Tick( double time, double deltaTime );

}
