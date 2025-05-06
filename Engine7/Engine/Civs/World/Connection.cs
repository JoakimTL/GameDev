//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

public sealed class Connection( Face faceA, Edge sharedEdge ) {

	private Face? _faceB;

	public Face FaceA { get; } = faceA;
	public Face FaceB => _faceB ?? throw new InvalidOperationException( "FaceB is not set." );
	public Edge SharedEdge { get; } = sharedEdge;

	public Face GetOther( Face face ) {
		if (face == FaceA)
			return FaceB;
		if (face == FaceB)
			return FaceA;
		throw new InvalidOperationException( "Current face is not part of this connection." );
	}

	internal void SetFaceB( Face face ) {
		if (_faceB is not null)
			throw new InvalidOperationException( "FaceB already set." );
		_faceB = face;
	}
}
