using Civlike.World;
using Engine;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace OldGen.WorldOld;

public sealed class GlobeVertex( PackedNormal vector ) {
	private readonly PackedNormal _vector = vector;
	private readonly List<Face> _connectedFaces = [];

	public Vector3<float> Vector => this._vector.Decode();

	public IReadOnlyList<Face> ConnectedFaces => this._connectedFaces;

	internal void AddFace( Face face ) {
		if (this._connectedFaces.Contains( face ))
			throw new InvalidOperationException( "Face already connected to this vertex." );
		this._connectedFaces.Add( face );
	}

	public override bool Equals( object? obj ) => obj is GlobeVertex vertex && vertex._vector.RawData == this._vector.RawData;

	public override int GetHashCode() => unchecked((int) this._vector.RawData);
}
