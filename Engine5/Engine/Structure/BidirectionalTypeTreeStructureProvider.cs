﻿using System.Collections.Concurrent;
using System.Reflection;
using Engine.Structure.Attributes;

namespace Engine.Structure;

public class BidirectionalTypeTree<T> : Identifiable {

	private readonly ConcurrentDictionary<Type, BidirectionalNode> _nodes;
	private bool _needsUpdate;

	public event Action? TreeUpdated;

	public BidirectionalTypeTree() {
		_nodes = new();
	}

	public void Add( Type t ) {
		if ( t is null )
			throw new ArgumentNullException( nameof( t ) );
		if ( _nodes.TryAdd( t, new BidirectionalNode( t ) ) )
			_needsUpdate = true;
	}

	public void Remove( Type t ) {
		if ( t is null )
			throw new ArgumentNullException( nameof( t ) );
		if ( _nodes.TryRemove( t, out _ ) )
			_needsUpdate = true;
	}

	public void Clear() => _nodes.Clear();

	private void UpdateStructure() {
		foreach ( BidirectionalNode node in _nodes.Values )
			node.Clear();

		foreach ( BidirectionalNode node in _nodes.Values ) {
			Type t = node.Value;
			IEnumerable<Type> preceding = t.GetCustomAttributes<ProcessAfterAttribute>().Select( p => p.PrecedingType );
			IEnumerable<Type> following = t.GetCustomAttributes<ProcessBeforeAttribute>().Select( p => p.FollowingType );
			foreach ( Type pre in preceding ) {
				if ( _nodes.TryGetValue( pre, out BidirectionalNode? preNode ) ) {
					node.AddParent( pre );
					preNode.AddChild( t );
				}
			}
			foreach ( Type fol in following ) {
				if ( _nodes.TryGetValue( fol, out BidirectionalNode? folNode ) ) {
					node.AddChild( fol );
					folNode.AddParent( t );
				}
			}
		}
		_needsUpdate = false;
		TreeUpdated?.Invoke();
	}

	public IEnumerable<Type> GetNodesSorted() {
		HashSet<Type> walked = new();
		Queue<BidirectionalNode> unprocessed = new( _nodes.Values.Where( p => p.Parents.Count == 0 ) );

		while ( unprocessed.TryDequeue( out BidirectionalNode? node ) ) {
			if ( !node.Parents.All( walked.Contains ) )
				continue;

			if ( !walked.Add( node.Value ) )
				continue;

			foreach ( Type? childNode in node.Children )
				unprocessed.Enqueue( _nodes[ childNode ] );
			yield return node.Value;
		}
	}

	public IEnumerable<Type> UpdateAndGetNodesSorted() {
		Update();
		return GetNodesSorted();
	}

	public bool Update() {
		if ( _needsUpdate ) {
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
			_parents = new();
			_children = new();
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