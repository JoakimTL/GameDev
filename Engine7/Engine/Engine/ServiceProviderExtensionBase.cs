using System.Data;

namespace Engine;

public abstract class ServiceProviderExtensionBase<TProcess, TServiceType> : NamedIdentifiable {
	private readonly TypeDigraph<TProcess> _digraph;
	private readonly List<TServiceType> _sortedServices;
	private readonly IServiceProvider _serviceProvider;
	private bool _changed;

	protected ServiceProviderExtensionBase( IServiceProvider serviceProvider ) {
		this._serviceProvider = serviceProvider;
		this._digraph = new();
		this._sortedServices = [];
		this._serviceProvider.OnInstanceAdded += OnInstanceAdded;
	}

	private void OnInstanceAdded( object obj ) {
		if (obj is not TServiceType)
			return;
		this._changed |= this._digraph.Add( obj.GetType() );
	}

	protected void Remove( Type service ) 
		=> this._changed |= this._digraph.Remove( service );

	protected void RemoveAll(IEnumerable<Type> services ) {
		bool removedAny = false;
		foreach (Type service in services)
			removedAny |= this._digraph.Remove( service );
		this._changed |= removedAny;
	}

	protected void Clear() 
		=> this._changed |= this._digraph.Clear();

	protected IReadOnlyList<TServiceType> SortedServices {
		get {
			if (this._changed) {
				this._sortedServices.Clear();
				this._sortedServices.AddRange( this._digraph.GetTypes().Select( this._serviceProvider.Get ).OfType<TServiceType>() );
				this._changed = false;
			}
			return this._sortedServices;
		}
	}
}
