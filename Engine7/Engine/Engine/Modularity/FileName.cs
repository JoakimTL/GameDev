using Engine.Time;
using System.Reflection;

namespace Engine.Modularity;
internal class FileName {
}

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

internal static class ModuleManager {

	private static Thread? _moduleMonitoringThread;
	private static readonly List<ModuleOverseer> _moduleOverseers = [];

	internal static void Initialize() {
		if (_moduleOverseers.Count == 0)
			throw new InvalidOperationException( "No modules have been added." );
		Thread.CurrentThread.Name = "Entrypoint";
		Log.Line( "Starting engine..." );
		foreach (ModuleOverseer overseer in _moduleOverseers)
			overseer.Start();
		UpdatePeriod();
		_moduleMonitoringThread = new Thread( MonitorModules ) {
			Name = $"{nameof( ModuleManager )} Thread",
			IsBackground = true
		};
		_moduleMonitoringThread.Start();
	}

	private static void UpdatePeriod() {
		uint lowestNonZeroPeriod = _moduleOverseers.Where( p => p.PeriodMs > 0 ).Select(p => p.PeriodMs).Min();
		TimePeriod.Begin( (int) lowestNonZeroPeriod );
	}

	private static void MonitorModules() {
		IThreadBlocker threadBlocker = new ThreadBlocker();
		while (true) {
			for (int i = _moduleOverseers.Count - 1; i >= 0; i--) {
				if (!_moduleOverseers[ i ].Running) {
					_moduleOverseers[ i ].LogLine( "Shutdown registered." );
					_moduleOverseers[ i ].PeriodChanged -= UpdatePeriod;
					_moduleOverseers.RemoveAt( i );
				}
			}
			if (_moduleOverseers.Count == 0)
				break;
			threadBlocker.Block( 1000 );
		}
		Log.Line( "Engine is shuting down..." );
		threadBlocker.Block( 1000 );
		Log.Stop();
	}

	internal static void StartModule<T>() where T : ModuleBase, new() {
		ModuleOverseer overseer = new( new T() );
		_moduleOverseers.Add( overseer );
		overseer.PeriodChanged += UpdatePeriod;
	}
}

public sealed class MessageBus : Identifiable {

}