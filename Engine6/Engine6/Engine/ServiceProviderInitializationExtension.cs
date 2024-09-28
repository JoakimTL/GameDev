namespace Engine;

public sealed class ServiceProviderInitializationExtension( IServiceProvider serviceProvider ) : HierarchicalServiceProviderExtension<IInitializable, IInitializable>( serviceProvider, true ), IUpdateable {
	public void Update( in double time, in double deltaTime ) {
		if (_tree.Update()) {
			for (int i = 0; i < _sortedServices.Count; i++) {
				Log.Line( $"Initializing {_sortedServices[ i ]}...", Log.Level.LOW, ConsoleColor.DarkCyan );
				_sortedServices[ i ].Initialize();
				Log.Line( $"Initialized {_sortedServices[ i ]}!", Log.Level.LOW, ConsoleColor.DarkCyan );
			}
			_tree.Clear();
		}
	}
}
