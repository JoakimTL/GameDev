using System.Collections.Concurrent;
using System.Reflection;

namespace Engine.Structure;

public class BidirectionalTreeStructureProvider : Identifiable {

	private readonly ConcurrentDictionary<Type, BidirectionalNode> _nodes;
	private bool _needsUpdate;
	public Type? ProcessType { get; }

	public event Action? TreeUpdated;

	public BidirectionalTreeStructureProvider( Type? processType ) {
		this.ProcessType = processType;
		this._nodes = new();
	}

	public void Add( Type t ) {
		if ( t is null )
			throw new ArgumentNullException( nameof( t ) );
		if ( this._nodes.TryAdd( t, new BidirectionalNode( t ) ) )
			this._needsUpdate = true;
	}

	public void Remove( Type t ) {
		if ( t is null )
			throw new ArgumentNullException( nameof( t ) );
		if ( this._nodes.TryRemove( t, out _ ) )
			this._needsUpdate = true;
	}

	public void Clear() => this._nodes.Clear();

	private void UpdateStructure() {
		foreach ( BidirectionalNode node in this._nodes.Values )
			node.Clear();

		foreach ( BidirectionalNode node in this._nodes.Values ) {
			Type t = node.Value;
			IEnumerable<Type> preceding = t.GetCustomAttributes<ProcessAfterAttribute>().Where( p => p.ProcessType == this.ProcessType ).Select( p => p.PrecedingType );
			IEnumerable<Type> following = t.GetCustomAttributes<ProcessBeforeAttribute>().Where( p => p.ProcessType == this.ProcessType ).Select( p => p.FollowingType );
			foreach ( Type pre in preceding ) {
				if ( this._nodes.TryGetValue( pre, out BidirectionalNode? preNode ) ) {
					node.AddParent( pre );
					preNode.AddChild( t );
				}
			}
			foreach ( Type fol in following ) {
				if ( this._nodes.TryGetValue( fol, out BidirectionalNode? folNode ) ) {
					node.AddChild( fol );
					folNode.AddParent( t );
				}
			}
		}
		this._needsUpdate = false;
		TreeUpdated?.Invoke();
	}

	public IEnumerable<Type> GetNodesSorted() {
		HashSet<Type> walked = new();
		Queue<BidirectionalNode> unprocessed = new( this._nodes.Values.Where( p => p.Parents.Count == 0 ) );

		while ( unprocessed.TryDequeue( out BidirectionalNode? node ) ) {
			if ( !node.Parents.All( p => walked.Contains( p ) ) )
				continue;

			if ( !walked.Add( node.Value ) )
				continue;

			foreach ( Type? childNode in node.Children )
				unprocessed.Enqueue( this._nodes[ childNode ] );
			yield return node.Value;
		}
	}

	public IEnumerable<Type> UpdateAndGetNodesSorted() {
		Update();
		return GetNodesSorted();
	}

	public bool Update() {
		if ( this._needsUpdate ) {
			UpdateStructure();
			return true;
		}
		return false;
	}

	public class BidirectionalNode {
		private readonly HashSet<Type> _children;
		private readonly HashSet<Type> _parents;

		public BidirectionalNode( Type value ) {
			this.Value = value;
			this._parents = new();
			this._children = new();
		}

		public void AddParent( Type parent ) => this._parents.Add( parent );
		public void AddChild( Type child ) => this._children.Add( child );

		public void Clear() {
			this._parents.Clear();
			this._children.Clear();
		}

		public Type Value { get; }
		public IReadOnlyCollection<Type> Parents => this._parents;
		public IReadOnlyCollection<Type> Children => this._children;

	}
}
