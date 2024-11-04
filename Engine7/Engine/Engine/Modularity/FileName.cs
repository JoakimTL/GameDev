namespace Engine.Modularity;
internal class FileName {
}

public class Startup {
	public static Startup Start() {
		ModuleManager.Initialize();
		return new();
	}

	public void WithModule<T>() where T : ModuleBase => ModuleManager.StartModule<T>();
}

internal static class ModuleManager {

	private static Thread _moduleStartupThread;
	private static Thread _moduleMonitoringThread;

	internal static void Initialize() {
		Log.Line( "Starting engine..." );
	}

	internal static void StartModule<T>() where T : ModuleBase {

	}
}

public abstract class ModuleBase : Identifiable {

	public IInstanceProvider InstanceProvider { get; } = InstanceManagement.CreateProvider();
	public bool Important { get; }
	public double ExecutionFrequency { get; private set; }
	public uint TimeBetweenTicksMs { get; private set; }
	internal event Action<ModuleBase>? FrequencyAltered;

	/// <param name="important">Determines if this module keeps the application running</param>
	/// <param name="frequency">The number of ticks per second. If <see cref="TimeBetweenTicksMs"/> is 0, there is no delay between ticks.</param>
	public ModuleBase( bool important, double frequency ) {
		this.Important = important;
		SetExecutionFrequency( frequency );
	}
	
	/// <param name="frequency">The number of ticks per second. If <see cref="TimeBetweenTicksMs"/> is 0, there is no delay between ticks.</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	protected void SetExecutionFrequency( double frequency ) {
		if (frequency <= 0)
			throw new ArgumentOutOfRangeException( "Execution frequency must be a non-zero positive number." );
		uint newDelay = frequency.ToPeriodMs();
		this.ExecutionFrequency = frequency;
		if (newDelay == this.TimeBetweenTicksMs)
			return;
		this.TimeBetweenTicksMs = newDelay;
		FrequencyAltered?.Invoke( this );
	}

	protected internal abstract void Tick();

}

public sealed class MessageBus : Identifiable {

}