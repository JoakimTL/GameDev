//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.WorldOld;

public sealed class Connection( Face faceA, Edge sharedEdge ) {

	private Face? _faceB;

	public Face FaceA { get; } = faceA;
	public Face FaceB => this._faceB ?? throw new InvalidOperationException( "FaceB is not set." );
	public Edge SharedEdge { get; } = sharedEdge;

	public Face GetOther( Face face ) {
		if (face == this.FaceA)
			return this.FaceB;
		if (face == this.FaceB)
			return this.FaceA;
		throw new InvalidOperationException( "Current face is not part of this connection." );
	}

	internal void SetFaceB( Face face ) {
		if (this._faceB is not null)
			throw new InvalidOperationException( "FaceB already set." );
		this._faceB = face;
	}
}
