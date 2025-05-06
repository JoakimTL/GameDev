using Engine;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

public sealed class GlobeVertex {
	private readonly UnmanagedStaticContainer<PackedNormal> _vectorContainer;
	private readonly uint _index;
	private readonly List<Face> _connectedFaces;

	public GlobeVertex( UnmanagedStaticContainer<PackedNormal> vectorContainer, uint index ) {
		_vectorContainer = vectorContainer;
		this._index = index;
		_connectedFaces = [];
	}

	public uint Index => _index;

	public Vector3<float> Vector => _vectorContainer.Get( _index ).Decode();

	public IReadOnlyList<Face> ConnectedFaces => _connectedFaces;

	internal void AddFace( Face face ) {
		if (_connectedFaces.Contains( face ))
			throw new InvalidOperationException( "Face already connected to this vertex." );
		_connectedFaces.Add( face );
	}

	public override bool Equals( object? obj ) => obj is GlobeVertex vertex && vertex._index == _index;

	public override int GetHashCode() => unchecked((int) _index);
}
