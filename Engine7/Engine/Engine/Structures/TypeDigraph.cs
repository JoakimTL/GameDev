namespace Engine.Structures;

public sealed partial class TypeDigraph<TProcessType> : Identifiable, ITypeDigraph {
	private readonly Type _processType = typeof( TProcessType );
	private readonly HashSet<Type> _types = [];
	private readonly List<ResolvedType> _unorderedTypes = [];
	private readonly List<Type> _orderedTypes = [];
	public event Action<Type>? OnTypeAdded;
	public event Action<Type>? OnTypeRemoved;
	public event Action? OnCleared;
	private bool _needsReorder = false;

	/// <summary>
	/// Returns a read-only list of the types in the graph sorted by directionality.
	/// </summary>
	/// <returns></returns>
	public IReadOnlyList<Type> GetTypes() {
		if (this._needsReorder)
			if (this._unorderedTypes.Count < 25)
				TypeDigraph.Sort.NoAlloc.Sort( this._processType, this._types, this._unorderedTypes, this._orderedTypes );
			else
				TypeDigraph.Sort.Alloc.Sort( this._processType, this._unorderedTypes, this._orderedTypes );
		this._needsReorder = false;
		return this._orderedTypes;
	}

	public void NoAllocSort() {
		TypeDigraph.Sort.NoAlloc.Sort( this._processType, this._types, this._unorderedTypes, this._orderedTypes );
		this._needsReorder = false;
	}

	public void AllocSort() {
		TypeDigraph.Sort.Alloc.Sort( this._processType, this._unorderedTypes, this._orderedTypes );
		this._needsReorder = false;
	}

	public bool Add( Type t ) {
		if (!this._types.Add( t ))
			return false;
		this._unorderedTypes.Add( TypeManager.ResolveType( t ) );
		this._needsReorder = this._unorderedTypes.Count > 1;
		if (this._unorderedTypes.Count == 1)
			this._orderedTypes.Add( t );
		OnTypeAdded?.Invoke( t );
		return true;
	}

	public bool Add<T>() => Add( typeof( T ) );

	public bool Remove( Type t ) {
		if (!this._types.Remove( t ))
			return false;
		this._unorderedTypes.Remove( TypeManager.ResolveType( t ) );
		this._orderedTypes.Remove( t );
		OnTypeRemoved?.Invoke( t );
		return true;
	}

	public bool Remove<T>() => Remove( typeof( T ) );

	public bool Clear() {
		if (this._types.Count == 0)
			return false;
		this._types.Clear();
		this._unorderedTypes.Clear();
		this._orderedTypes.Clear();
		this._needsReorder = false;
		OnCleared?.Invoke();
		return true;
	}
}
