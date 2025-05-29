
using Engine;
using System.Numerics;

namespace Civlike.World.GenerationState;

public sealed class Face<TState> : FaceBase where TState : FaceStateBase, new() {

	private NeighbouringFace<TState>[]? _neighbours;

	public Face( uint id, GeneratingGlobeBase globe, Vertex[] vertices, Edge[] edges ) : base( id, globe, vertices, edges ) {
		State = new() {
			Face = this
		};
	}

	public IReadOnlyList<NeighbouringFace<TState>> Neighbours => this._neighbours ??= GetNeighbouringFaces();
	private NeighbouringFace<TState>[] GetNeighbouringFaces() {
		NeighbouringFace<TState>[] neighbours = [ .. this.Edges.Select( p => new NeighbouringFace<TState>( Center, GetNeighbour( p ), (float) Globe.TileLength ) ) ];

		InverseLengthNeighbourDirectionX = new Vector3( neighbours[ 0 ].InvLengthDirection.X, neighbours[ 1 ].InvLengthDirection.X, neighbours[ 2 ].InvLengthDirection.X );
		InverseLengthNeighbourDirectionY = new Vector3( neighbours[ 0 ].InvLengthDirection.Y, neighbours[ 1 ].InvLengthDirection.Y, neighbours[ 2 ].InvLengthDirection.Y );
		InverseLengthNeighbourDirectionZ = new Vector3( neighbours[ 0 ].InvLengthDirection.Z, neighbours[ 1 ].InvLengthDirection.Z, neighbours[ 2 ].InvLengthDirection.Z );

		return neighbours;
	}

	private Face<TState> GetNeighbour( Edge p )
		=> p.GetOther( this ) as Face<TState> ?? throw new InvalidCastException( $"Edge {p.Id} does not connect to a Face of type {typeof( TState )}." );

	public Vector3 InverseLengthNeighbourDirectionX { get; private set; }
	public Vector3 InverseLengthNeighbourDirectionY { get; private set; }
	public Vector3 InverseLengthNeighbourDirectionZ { get; private set; }

	public TState State { get; }
	public int LatitudeId { get; set; }

	public override void Apply( GameplayState.Face.Builder builder ) => State.Apply( builder );
}

public sealed class NeighbouringFace<TState> where TState : FaceStateBase, new() {
	public NeighbouringFace( Vector3 center, Face<TState> neighbour, float tileLength ) {
		NormalizedDirection = neighbour.Center - center;
		NormalizedDirection = NormalizedDirection - Vector3.Dot(NormalizedDirection, center ) * center;
		NormalizedDirection = Vector3.Normalize( NormalizedDirection );
		InvLengthDirection = NormalizedDirection / tileLength;
		this.Face = neighbour;
	}

	public Vector3 NormalizedDirection { get; }
	public Vector3 InvLengthDirection { get; }
	public Face<TState> Face { get; }
}