using Engine.Rendering.Objects;
using Engine.Rendering.OGL;
using Engine.Structure;

namespace Engine.Rendering;

public sealed class Context : IUpdateable {
	private readonly RestrictedServiceProvider<IContextService> _serviceProvider;
	private readonly ServiceProviderDisposalExtension _serviceProviderDisposer;
	private readonly ServiceProviderUpdateExtension _serviceProviderUpdater;
	private readonly ServiceProviderInitializationExtension _serviceProviderInitializer;
	private readonly Window _window;

	public Context( Window window ) {
		_window = window;
		_serviceProvider = new();
		_serviceProvider.AddConstant( _window );
		_serviceProviderUpdater = new( _serviceProvider );
		_serviceProviderDisposer = new( _serviceProvider );
		_serviceProviderInitializer = new( _serviceProvider );
	}

	public void Bind() => ContextUtilities.MakeContextCurrent( _window.Pointer );
	public T? Service<T>() where T : IContextService => _serviceProvider.Get<T>();

	public void Update( float time, float deltaTime ) {
		_serviceProviderInitializer.Update( time, deltaTime );
		_serviceProviderUpdater.Update( time, deltaTime );
	}


	public void Dispose() => _serviceProviderDisposer.Dispose();
}
