namespace Civlike.World.GenerationState;

public sealed class Edge( int id, Vertex vertexA, Vertex vertexB ) {
	private FaceBase? _faceA;
	private FaceBase? _faceB;

	public int Id { get; } = id;
	public Vertex VertexA { get; } = vertexA;
	public Vertex VertexB { get; } = vertexB;

	public FaceBase FaceA => this._faceA ?? throw new InvalidOperationException( "No face A exists." );
	public FaceBase FaceB => this._faceB ?? throw new InvalidOperationException( "No face B exists." );

	internal void AddFace( FaceBase face ) {
		if (this._faceA is null) {
			this._faceA = face;
			return;
		}
		if (this._faceB is null) {
			this._faceB = face;
			return;
		}
		throw new InvalidOperationException( "Both faces are already assigned." );
	}

	public FaceBase GetOther( FaceBase face ) {
		if (face == this.FaceA)
			return this.FaceB;
		if (face == this.FaceB)
			return this.FaceA;
		throw new ArgumentException( "Face not connected to this edge." );
	}
}
