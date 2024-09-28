using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

namespace Engine;

public class BidirectionalTypeTree<TProcessType>( bool addConstructorParameterTypesAsParents = false ) : Identifiable {

	private readonly ConcurrentDictionary<Type, BidirectionalNode> _nodes = new();
	private readonly Type _processType = typeof( TProcessType );
	private readonly bool _addConstructorParameterTypesAsParents = addConstructorParameterTypesAsParents;
	private bool _needsUpdate;

	public event Action? TreeUpdated;

	public void Add( Type t ) {
		ArgumentNullException.ThrowIfNull( t );
		AddInternal( t );
	}

	public void Add<T>() => AddInternal( typeof( T ) );

	private void AddInternal(Type t ) {
		if (_nodes.TryAdd( t, new BidirectionalNode( t ) ))
			_needsUpdate = true;
	}

	public void Remove( Type t ) {
		ArgumentNullException.ThrowIfNull( t );
		RemoveInternal( t );
	}
	public void Remove<T>() => RemoveInternal( typeof( T ) );

	private void RemoveInternal( Type t ) {
		if (_nodes.TryRemove( t, out _ ))
			_needsUpdate = true;
	}

	public void Clear() => _nodes.Clear();

	private void UpdateStructure() {
		foreach (BidirectionalNode node in _nodes.Values)
			node.Clear();

		foreach (BidirectionalNode node in _nodes.Values) {
			Type t = node.Value;
			HashSet<Type> preceding = t.GetCustomAttributes<Direction.AfterAttribute>().Where( p => p.ProcessType == _processType ).Select( p => p.PrecedingType ).ToHashSet();
			if (_addConstructorParameterTypesAsParents) {
				ConstructorInfo? ctor = t.GetConstructors().FirstOrDefault();
				if (ctor is not null) {
					foreach (Type? p in ctor.GetParameters().Select( p => p.ParameterType ))
						preceding.Add( p );
				}
			}
			HashSet<Type> following = t.GetCustomAttributes<Direction.BeforeAttribute>().Where( p => p.ProcessType == _processType ).Select( p => p.FollowingType ).ToHashSet();
			foreach (Type pre in preceding) {
				if (_nodes.TryGetValue( pre, out BidirectionalNode? preNode )) {
					node.AddParent( pre );
					preNode.AddChild( t );
				}
			}
			foreach (Type fol in following) {
				if (_nodes.TryGetValue( fol, out BidirectionalNode? folNode )) {
					node.AddChild( fol );
					folNode.AddParent( t );
				}
			}
		}
		_needsUpdate = false;
		TreeUpdated?.Invoke();
	}

	public IEnumerable<Type> GetNodesSorted() {
		HashSet<Type> walked = [];
		Queue<BidirectionalNode> unprocessed = new( _nodes.Values.Where( p => p.Parents.Count == 0 ) );

		while (unprocessed.TryDequeue( out BidirectionalNode? node )) {
			if (!node.Parents.All( walked.Contains ))
				continue;

			if (!walked.Add( node.Value ))
				continue;

			foreach (Type? childNode in node.Children)
				unprocessed.Enqueue( _nodes[ childNode ] );
			yield return node.Value;
		}
	}

	public IEnumerable<Type> UpdateAndGetNodesSorted() {
		Update();
		return GetNodesSorted();
	}

	public bool Update() {
		if (_needsUpdate) {
			UpdateStructure();
			return true;
		}
		return false;
	}

	public class BidirectionalNode {
		private readonly HashSet<Type> _children;
		private readonly HashSet<Type> _parents;

		public BidirectionalNode( Type value ) {
			Value = value;
			_parents = [];
			_children = [];
		}

		public void AddParent( Type parent ) => _parents.Add( parent );
		public void AddChild( Type child ) => _children.Add( child );

		public void Clear() {
			_parents.Clear();
			_children.Clear();
		}

		public Type Value { get; }
		public IReadOnlyCollection<Type> Parents => _parents;
		public IReadOnlyCollection<Type> Children => _children;

	}
}