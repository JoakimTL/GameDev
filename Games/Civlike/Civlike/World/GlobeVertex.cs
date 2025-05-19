using Engine;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.World;

public sealed class GlobeVertex {
	private readonly PackedNormal _vector;
	private readonly List<Face> _connectedFaces;

	public GlobeVertex( PackedNormal vector ) {
		_vector = vector;
		_connectedFaces = [];
	}

	public Vector3<float> Vector => _vector.Decode();

	public IReadOnlyList<Face> ConnectedFaces => _connectedFaces;

	internal void AddFace( Face face ) {
		if (_connectedFaces.Contains( face ))
			throw new InvalidOperationException( "Face already connected to this vertex." );
		_connectedFaces.Add( face );
	}

	public override bool Equals( object? obj ) => obj is GlobeVertex vertex && vertex._vector.RawData == _vector.RawData;

	public override int GetHashCode() => unchecked((int) _vector.RawData);
}
