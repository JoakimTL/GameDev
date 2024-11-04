using System.Data;
using Engine.Structures;

namespace Engine;

public abstract class InstanceProviderExtensionBase<TProcess, TInstanceType> : Identifiable {
	private readonly TypeDigraph<TProcess> _digraph;
	private readonly List<TInstanceType> _sortedInstances;
	private readonly IInstanceProvider _instanceProvider;
	private bool _changed;

	protected InstanceProviderExtensionBase( IInstanceProvider instanceProvider ) {
		this._instanceProvider = instanceProvider;
		this._digraph = new();
		this._sortedInstances = [];
		this._instanceProvider.OnInstanceAdded += OnInstanceAdded;
	}

	private void OnInstanceAdded( object obj ) {
		if (obj is not TInstanceType)
			return;
		this._changed |= this._digraph.Add( obj.GetType() );
	}

	protected void Remove( Type service ) 
		=> this._changed |= this._digraph.Remove( service );

	protected void RemoveAll(IEnumerable<Type> instanceTypes ) {
		bool removedAny = false;
		foreach (Type instanceType in instanceTypes)
			removedAny |= this._digraph.Remove( instanceType );
		this._changed |= removedAny;
	}

	protected void Clear() 
		=> this._changed |= this._digraph.Clear();

	protected IReadOnlyList<TInstanceType> SortedInstances {
		get {
			if (this._changed) {
				this._sortedInstances.Clear();
				this._sortedInstances.AddRange( this._digraph.GetTypes().Select( this._instanceProvider.Get ).OfType<TInstanceType>() );
				this._changed = false;
			}
			return this._sortedInstances;
		}
	}
}
