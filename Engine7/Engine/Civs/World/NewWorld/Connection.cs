//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World.NewWorld;

public sealed class Connection( Face faceA, Edge sharedEdge ) {

	private Face? _faceB;

	public Face FaceA { get; } = faceA;
	public Face FaceB => _faceB ?? throw new InvalidOperationException( "FaceB is not set." );
	public Edge SharedEdge { get; } = sharedEdge;

	internal void SetFaceB( Face face ) {
		if (_faceB is not null)
			throw new InvalidOperationException( "FaceB already set." );
		_faceB = face;
	}
}
