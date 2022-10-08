namespace Engine.Structure;

public class ServiceProviderDisposalExtension<BASE> : HierarchicalServiceProviderExtension<BASE> where BASE : class {
	private readonly ServiceProvider<BASE> _serviceProvider;

	public ServiceProviderDisposalExtension( ServiceProvider<BASE> serviceProvider ) : base( typeof( IDisposable ) ) {
		this._serviceProvider = serviceProvider;
		this._serviceProvider.ServiceAdded += OnServiceAdded;
		this._serviceProvider.ServiceRemoved += OnServiceRemoved;
	}

	private void OnServiceAdded( object obj ) {
		if ( obj is IDisposable )
			this._tree.Add( obj.GetType() );
	}

	private void OnServiceRemoved( object obj ) {
		if ( obj is IDisposable disposable ) {
			this._tree.Remove( obj.GetType() );
			disposable.Dispose();
		}
	}

	protected override bool OnDispose() {
		this._serviceProvider.ExecuteOnServices( this._tree.UpdateAndGetNodesSorted(), p =>
		{
			if ( p is IDisposable disposable )
				disposable.Dispose();
		} );
		return true;
	}
}
