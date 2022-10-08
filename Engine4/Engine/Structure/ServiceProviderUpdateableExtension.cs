namespace Engine.Structure;

public class ServiceProviderUpdateableExtension<BASE> : HierarchicalServiceProviderExtension<BASE>, IUpdateable where BASE : class {
	private readonly ServiceProvider<BASE> _serviceProvider;
	private readonly List<IUpdateable> _updateables;
	public bool Active => true;

	public ServiceProviderUpdateableExtension( ServiceProvider<BASE> serviceProvider ) : base( typeof( IUpdateable ) ) {
		this._serviceProvider = serviceProvider;
		this._updateables = new();
		this._serviceProvider.ServiceAdded += OnServiceAdded;
		this._serviceProvider.ServiceRemoved += OnServiceRemoved;
	}

	private void OnServiceAdded( object obj ) {
		if ( obj is IUpdateable )
			this._tree.Add( obj.GetType() );
	}

	private void OnServiceRemoved( object obj ) {
		if ( obj is IUpdateable )
			this._tree.Remove( obj.GetType() );
	}

	public void Update( float time, float deltaTime ) {
		if ( this._tree.Update() ) {
			this._updateables.Clear();
			this._updateables.AddRange( this._serviceProvider.PeekAll( this._tree.GetNodesSorted() ).OfType<IUpdateable>() );
		}

		foreach ( IUpdateable updateable in this._updateables )
			if ( updateable.Active )
				updateable.Update( time, deltaTime );
	}

	protected override bool OnDispose() => true;
}
