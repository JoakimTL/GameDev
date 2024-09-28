using Engine;
using Engine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UserTest.Pathfinding;
internal class Class1 {
}

public interface IPathNode<TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	ulong Id { get; }
	Vector2<TScalar> Center { get; }
	IReadOnlyCollection<IPathVertex<TScalar>> Vertices { get; }
	IReadOnlyCollection<IPathEdge<TScalar>> Edges { get; }
	IPathEdge<TScalar>? GetEdge( Vector2<TScalar> direction, double lowestAcceptedDotValue = 0 );
}

/// <summary>
/// A pathable edge between two nodes.
/// </summary>
/// <typeparam name="TScalar"></typeparam>
public interface IPathEdge<TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	IPathNode<TScalar> OwnerNode { get; }
	IPathNode<TScalar> OtherNode { get; }
	IPathVertex<TScalar> Start { get; }
	IPathVertex<TScalar> End { get; }
	TScalar Width { get; }
	Vector2<TScalar> NormalVector { get; }
}

public interface IPathVertex<TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	Vector2<TScalar> Translation { get; }
	IReadOnlyCollection<IPathNode<TScalar>> Nodes { get; }
}

public interface IPathManager<TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {

	/// <summary>
	/// Generates a series of vectors which represent the path between two points. If there are no valid paths, an empty enumerable is returned.
	/// </summary>
	/// <param name="start">Start location</param>
	/// <param name="end">Where the path should stop</param>
	/// <param name="agentRadius">How much space does the pathing agent use, in unit radii</param>
	/// <returns></returns>
	public IEnumerable<Vector2<TScalar>> FormPathBetweenPoints( Vector2<TScalar> start, Vector2<TScalar> end, TScalar agentRadius, IEnumerable<ulong> unknownAreas );
}

public sealed class DynamicPathManager : IPathManager<double> {

	private ulong _currentId;
	private readonly Dictionary<Vector2<double>, PathVertex> _vertices = new();
	private readonly Dictionary<ulong, PathNode> _nodes = new();

	public IEnumerable<Vector2<double>> FormPathBetweenPoints( Vector2<double> start, Vector2<double> end, double agentRadius, IEnumerable<ulong> unknownAreas ) {
		throw new NotImplementedException();
	}

	public bool TryCreateConvexNode( ReadOnlySpan<Vector2<double>> vertices, out IPathNode<double>? node ) {
		Vector2<double> center = 0;
		foreach (Vector2<double> vertex in vertices)
			center += vertex;
		center /= vertices.Length;
		PathNode pathnode = new( _currentId++, center );
		foreach (Vector2<double> vertex in vertices) {
			if (!_vertices.TryGetValue( vertex, out PathVertex? pathVertex ))
				_vertices.Add( vertex, pathVertex = new PathVertex( vertex ) );
			pathnode.AddVertex( pathVertex );
			pathVertex.AddNode( pathnode );
		}
		_nodes.Add( pathnode.Id, pathnode );
		pathnode.FormEdges();
		node = pathnode;
		return true;
	}

}

public sealed class PathVertex( Vector2<double> translation ) : IPathVertex<double> {
	public Vector2<double> Translation { get; } = translation;
	private readonly HashSet<PathNode> _nodes = [];

	internal void AddNode( PathNode node ) => _nodes.Add( node );

	public IReadOnlyCollection<IPathNode<double>> Nodes => _nodes;

	internal IPathNode<double>? GetCommonNode( IPathVertex<double> other ) {
		foreach (PathNode node in _nodes)
			if (other.Nodes.Contains( node ))
				return node;
		return null;
	}
}

public sealed class PathEdge( IPathNode<double> owner, IPathNode<double> other, IPathVertex<double> start, IPathVertex<double> end ) : IPathEdge<double> {
	public IPathNode<double> OwnerNode { get; } = owner;
	public IPathNode<double> OtherNode { get; } = other;
	public IPathVertex<double> Start { get; } = start;
	public IPathVertex<double> End { get; } = end;
	public double Width { get; } = (start.Translation - end.Translation).Magnitude<Vector2<double>, double>();
	public Vector2<double> NormalVector { get; } = (end.Translation - start.Translation).Normalize<Vector2<double>, double>() * Bivector2<double>.One;
}

public sealed class PathNode( ulong id, Vector2<double> center ) : IPathNode<double> {
	public ulong Id { get; } = id;
	public Vector2<double> Center { get; } = center;

	private readonly List<PathVertex> _vertices = [];
	private readonly List<PathEdge> _edges = [];

	public IReadOnlyCollection<IPathVertex<double>> Vertices => _vertices;
	public IReadOnlyCollection<IPathEdge<double>> Edges => _edges;

	public void AddVertex( PathVertex vertex ) {
		if (_vertices.Contains( vertex ))
			return;
		_vertices.Add( vertex );
	}

	internal void FormEdges() {
		Span<uint> remaining = stackalloc uint[ _vertices.Count ];
		ReadOnlySpan<Vector2<double>> vectors = stackalloc Vector2<double>[ _vertices.Count ];
		uint count = vectors.FormOuterEdges( remaining );
		for (int i = 0; i < count; i++) {
			PathVertex start = _vertices[ (int) remaining[ i ] ];
			PathVertex end = _vertices[ (int) remaining[ (i + 1) % (int) count ] ];
			IPathNode<double>? commonNode = start.GetCommonNode( end );
			if (commonNode is null)
				continue;
			_edges.Add( new PathEdge( this, commonNode, start, end ) );
		}
	}

	public IPathEdge<double>? GetEdge( Vector2<double> direction, double lowestAcceptedDotValue = 0 ) {
		double highestMatch = -2;
		PathEdge? bestEdge = null;
		Vector2<double> normalizedDirection = direction.Normalize<Vector2<double>, double>();
		foreach (PathEdge edge in _edges) {
			double dot = edge.NormalVector.Dot( normalizedDirection );
			if (dot > highestMatch && dot > lowestAcceptedDotValue) {
				highestMatch = dot;
				bestEdge = edge;
			}
		}
		return bestEdge;
	}
}

public static class GJK {

	/// <summary>
	/// This simplex is essentially a FIFO queue of vectors.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	private sealed class ExpandableSimplex<T>( uint initialSize )
		where T : unmanaged {

		public uint Count { get; private set; }

		private readonly UnmanagedFIFOData<T> _vectors = new( initialSize );

		public void Add( T vector ) {
			_vectors.Add( vector );
			Count++;
			if (Count > _vectors.MaxElements)
				Count = _vectors.MaxElements;
		}

		public void Set( T vector, uint elementIndex ) {
			if (elementIndex >= Count)
				throw new IndexOutOfRangeException();
			_vectors.Set( vector, elementIndex * (uint) Marshal.SizeOf<T>() );
		}

		public T Get( uint index ) {
			if (index >= Count)
				throw new IndexOutOfRangeException();
			return _vectors.Read<T>( index * (uint) Marshal.SizeOf<T>() );
		}

		public void Clear() => Count = 0;

	}

	public static double GetOverlap<TScalar>( ReadOnlySpan<Vector2<TScalar>> shapeA, ReadOnlySpan<Vector2<TScalar>> shapeB, uint maxIterations = 10 )
		where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		Span<(Vector2<TScalar> difference, int indexA, int indexB)> minkowski = stackalloc (Vector2<TScalar> difference, int indexA, int indexB)[ shapeA.Length * shapeB.Length ];

		for (int i = 0; i < shapeA.Length; i++)
			for (int j = 0; j < shapeB.Length; j++)
				minkowski[ (i * shapeB.Length) + j ] = (shapeA[ i ] - shapeB[ j ], i, j);

		ExpandableSimplex<(Vector2<TScalar> difference, int indexA, int indexB)> simplex = new( 3 );

		uint iteration = 0;
		while (true) {
			switch (simplex.Count) {
				case 0:
					simplex.Add( minkowski[ 0 ] );
					break;
				case 1:
					simplex.Add( Support<Vector2<TScalar>, TScalar>( minkowski, -simplex.Get( 0 ).difference ) );
					break;
				case 2:
					Vector2<TScalar> direction = (simplex.Get( 1 ).difference - simplex.Get( 0 ).difference) * Bivector2<TScalar>.One;
					if (direction.Dot( -simplex.Get( 1 ).difference ) < TScalar.Zero)
						direction = -direction;
					simplex.Add( Support<Vector2<TScalar>, TScalar>( minkowski, direction ) );
					break;
				case 3:
					Vector2<TScalar> ao = -simplex.Get( 2 ).difference;
					Vector2<TScalar> abPerp = (simplex.Get( 1 ).difference - simplex.Get( 0 ).difference) * Bivector2<TScalar>.One;

					TScalar clockwise = abPerp.DeterminantWithOrigin( simplex.Get( 0 ).difference, simplex.Get( 2 ).difference ) > TScalar.Zero ? TScalar.One : TScalar.NegativeOne;

					Vector2<TScalar> acPerp = (simplex.Get( 2 ).difference - simplex.Get( 0 ).difference) * Bivector2<TScalar>.One;

					Vector2<TScalar> abo = abPerp.Dot(ao);
					break;
			}
			iteration++;
			if (iteration >= maxIterations)
				break;
		}

		return 0;

	}

	public static (TVector difference, int indexA, int indexB) Support<TVector, TScalar>( in Span<(TVector difference, int indexA, int indexB)> set, TVector direction )
		where TVector :
			unmanaged, IInnerProduct<TVector, TScalar>
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar> {
		(TVector difference, int indexA, int indexB) highest = default;
		TScalar highestDot = TScalar.NegativeInfinity;
		for (int i = 0; i < set.Length; i++) {
			TScalar dot = set[ i ].difference.Dot( direction );
			if (dot > highestDot) {
				highestDot = dot;
				highest = set[ i ];
			}
		}
		return highest;
	}

}