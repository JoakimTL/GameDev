using Engine.Time;

namespace Engine.Modules;

public abstract class ModuleBase : DisposableIdentifiable {

	private readonly IModuleLoopTimer _loopTimer;

	public bool Alive { get; private set; } = true;

	internal event Action<object>? MessageEmitted;

	private Clock<double, StopwatchTickSupplier> Clock { get; }
	private double _lastTime;

	private readonly Dictionary<Type, Action<object>> _messageListeners;

	protected readonly IServiceProvider _serviceProvider;
	private readonly ServiceProviderDisposalExtension _serviceProviderDisposer;
	private readonly ServiceProviderUpdateExtension _serviceProviderUpdater;
	private readonly ServiceProviderInitializationExtension _serviceProviderInitializer;

	/// <param name="loopTimer">Decides how often <see cref="Run"/> will run per second.<br/>
	/// Standard loop timers include <see cref="NoDelayLoopTimer"/> and <see cref="FrequencyDelayedLoopTimer"/>.</param>
	/// <param name="serviceRegistry">A service registry allows for abstraction of implementation by injecting as interfaces rather than classes. The job of a service registry is to tell the service container which class implements an interface, as there might be more than one implementation.</param>
	/// <exception cref="ArgumentNullException"></exception>
	public ModuleBase( IModuleLoopTimer loopTimer, Clock<double, StopwatchTickSupplier> clock, IServiceRegistry? serviceRegistry ) {
		this._loopTimer = loopTimer ?? throw new ArgumentNullException( nameof( loopTimer ) );
		this.Clock = clock ?? throw new ArgumentNullException( nameof( clock ) );
		_lastTime = Clock.Time;
		_messageListeners = [];
		_serviceProvider = Services.CreateServiceProvider( serviceRegistry );
		_serviceProviderUpdater = new( _serviceProvider );
		_serviceProviderDisposer = new( _serviceProvider );
		_serviceProviderInitializer = new( _serviceProvider );
	}

	internal void AddServices( Type[] serviceTypes ) {
		foreach (Type serviceType in serviceTypes)
			_serviceProvider.GetService( serviceType );
	}

	internal void MessageReceived( object message ) {
		if (_messageListeners.TryGetValue( message.GetType(), out Action<object>? listener ))
			listener.Invoke( message );
	}

	protected void EmitMessage( object message ) => MessageEmitted?.Invoke( message );

	/// <summary>
	/// The module will start listening for messages of the specified type. Whenever a message of this type is received the messageReceptionResponse will trigger on the module's thread at the next update interval. 
	/// </summary>
	/// <typeparam name="T">The type to stop listening for.</typeparam>
	protected void ListenForMessages<T>( Action<T> messageReceptionResponse )
		=> _messageListeners[ typeof( T ) ] = ( object o ) => {
			if (o is T t)
				messageReceptionResponse( t );
			else
				this.LogWarning( "Attempted to receive message but got wrong type." );
		};


	/// <summary>
	/// The module will stop listening for messages of the specified type.
	/// </summary>
	/// <typeparam name="T">The type to stop listening for.</typeparam>
	protected void DeafenToMessages<T>()
		=> _messageListeners.Remove( typeof( T ) );

	internal void Run() {
		OnInitialize();
		while (Alive) {
			double currentTime = Clock.Time;

			Update( currentTime, currentTime - _lastTime );

			_lastTime = currentTime;
			if (!_loopTimer.Block())
				break;
		}
		Dispose();
	}

	private void Update( in double time, in double deltaTime ) {
		_serviceProviderInitializer.Update( time, deltaTime );
		_serviceProviderUpdater.Update( time, deltaTime );
		OnUpdate( time, deltaTime );
	}

	protected internal void Stop() {
		Alive = false;
		_loopTimer.Cancel();
	}

	protected override bool InternalDispose() {
		_serviceProviderDisposer.Dispose();
		OnDispose();
		return true;
	}

	protected abstract void OnInitialize();
	protected abstract void OnUpdate( in double time, in double deltaTime );
	protected abstract void OnDispose();
}
