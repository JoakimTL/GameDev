using System.Data;
using Engine.Structures;

namespace Engine;

public abstract class InstanceProviderExtensionBase<TProcess, TInstanceType> : Identifiable {
	private readonly TypeDigraph<TProcess> _digraph;
	private readonly List<TInstanceType> _sortedInstances;
	private readonly Queue<Type> _incomingInstances;
	private readonly Queue<Type> _outgoingInstances;
	private readonly IInstanceProvider _instanceProvider;

	public bool HasChanges => _incomingInstances.Count > 0 || _outgoingInstances.Count > 0;

	protected InstanceProviderExtensionBase( IInstanceProvider instanceProvider ) {
		this._instanceProvider = instanceProvider;
		this._digraph = new();
		this._sortedInstances = [];
		this._incomingInstances = [];
		this._outgoingInstances = [];
		this._instanceProvider.OnInstanceAdded += OnInstanceAdded;
	}

	private void OnInstanceAdded( object obj ) {
		if (obj is not TInstanceType)
			return;
		_incomingInstances.Enqueue( obj.GetType() );
	}

	protected void Remove( Type service )
		=> _outgoingInstances.Enqueue( service );

	protected void RemoveAll( IEnumerable<Type> instanceTypes ) {
		foreach (Type instanceType in instanceTypes)
			_outgoingInstances.Enqueue( instanceType );
	}

	protected void Clear() {
		this._digraph.Clear();
		this._sortedInstances.Clear();
	}

	protected IReadOnlyList<TInstanceType> SortedInstances {
		get {
			if (this.HasChanges) {
				while (_incomingInstances.TryDequeue( out Type? instanceType ))
					this._digraph.Add( instanceType );

				while (_outgoingInstances.TryDequeue( out Type? instanceType )) 
					this._digraph.Remove( instanceType );

				this._sortedInstances.Clear();
				List<Type> types = this._digraph.GetTypes().ToList();
				List<object> instances = types.Select( this._instanceProvider.Get ).ToList();
				List<TInstanceType> filtered = instances.OfType<TInstanceType>().ToList();
				this._sortedInstances.AddRange( filtered );
			}
			return this._sortedInstances;
		}
	}
}
