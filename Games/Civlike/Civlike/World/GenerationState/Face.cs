
using Engine;

namespace Civlike.World.GenerationState;

public sealed class Face<TState> : FaceBase where TState : FaceStateBase, new() {

	private NeighbouringFace<TState>[]? _neighbours;

	public Face( uint id, GeneratingGlobeBase globe, Vertex[] vertices, Edge[] edges ) : base( id, globe, vertices, edges ) {
		State = new() {
			Face = this
		};
	}

	public IReadOnlyList<NeighbouringFace<TState>> Neighbours => this._neighbours ??= GetNeighbouringFaces();
	private NeighbouringFace<TState>[] GetNeighbouringFaces() 
		=> [ .. this.Edges.Select( p => new NeighbouringFace<TState>( Center, GetNeighbour( p ), (float) Globe.TileLength ) ) ];
	private Face<TState> GetNeighbour( Edge p ) 
		=> p.GetOther( this ) as Face<TState> ?? throw new InvalidCastException( $"Edge {p.Id} does not connect to a Face of type {typeof( TState )}." );

	public TState State { get; }
	public int LatitudeId { get; set; }

	public override void Apply( GameplayState.Face.Builder builder ) => State.Apply( builder );
}

public sealed class NeighbouringFace<TState> where TState : FaceStateBase, new() {
	public NeighbouringFace( Vector3<float> center, Face<TState> neighbour, float tileLength ) {
		NormalizedDirection = neighbour.Center - center;
		NormalizedDirection = NormalizedDirection - NormalizedDirection.Dot( center ) * center;
		NormalizedDirection = NormalizedDirection.Normalize<Vector3<float>, float>();
		InvLengthDirection = NormalizedDirection / tileLength;
		this.Face = neighbour;
	}

	public Vector3<float> NormalizedDirection { get; }
	public Vector3<float> InvLengthDirection { get; }
	public Face<TState> Face { get; }
}