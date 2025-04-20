using Engine;
using Engine.Structures;

namespace Civs.World;

public sealed class Edge : IOcTreeLeaf<float> {

	public IcosphereVectorContainer Vectors { get; }
	public readonly EdgeIndices Indices;

	private readonly Tile[] _tiles;

	public IReadOnlyList<Tile> Tiles => _tiles;

	public Edge( IcosphereVectorContainer vectors, EdgeIndices indices ) {
		this.Vectors = vectors;
		this.Indices = indices;
		this._tiles = new Tile[ 2 ];
	}

	public Vector3<float> VectorA => Vectors.GetVector( Indices.A );
	public Vector3<float> VectorB => Vectors.GetVector( Indices.B );

	public AABB<Vector3<float>> Bounds => AABB.Create( [ VectorA, VectorB ] );

	internal void AddTile( Tile tile ) {
		if (_tiles[ 0 ] == null)
			_tiles[ 0 ] = tile;
		else if (_tiles[ 1 ] == null)
			_tiles[ 1 ] = tile;
		else
			throw new InvalidOperationException( "Edge cannot connect more than two tiles." );
	}
}