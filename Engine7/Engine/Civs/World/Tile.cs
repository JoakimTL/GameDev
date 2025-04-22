using Engine;
using Engine.Structures;

namespace Civs.World;

public sealed class Tile : IOcTreeLeaf<float> {
	public IcosphereVectorContainer Vectors { get; }
	public double Area { get; }
	public uint Id { get; }

	public readonly TileIndices Indices;
	private readonly Edge[] _edges;

	public Tile( IcosphereVectorContainer vectors, double simulatedSurfaceArea, uint tileId, TileIndices indices ) {
		this.Id = tileId;
		this.Vectors = vectors;
		this.Indices = indices;
		_edges = new Edge[ 3 ];
		Area = GetArea( simulatedSurfaceArea );
		Terrain = TerrainTypes.GetTerrainType<OceanTerrain>();
	}

	public Vector3<float> VectorA => Vectors.GetVector( Indices.A );
	public Vector3<float> VectorB => Vectors.GetVector( Indices.B );
	public Vector3<float> VectorC => Vectors.GetVector( Indices.C );
	public Vector3<float> Normal {
		get {
			Vector3<float> ab = VectorB - VectorA;
			Vector3<float> ac = VectorC - VectorA;
			return ab.Cross( ac ).Normalize<Vector3<float>, float>();
		}
	}

	public IReadOnlyList<Edge> Edges => _edges;
	public AABB<Vector3<float>> Bounds => AABB.Create( [ VectorA, VectorB, VectorC ] );

	public Vector4<float> Color => Terrain.Color;

	public double Height { get; set; }

	public bool UnderOceanLevel => Height < 0;

	public TerrainTypeBase Terrain { get; internal set; }

	internal void AddEdge( Edge edge ) {
		for (int i = 0; i < 3; i++)
			if (_edges[ i ] == null) {
				_edges[ i ] = edge;
				return;
			}
	}

	public double GetArea( double simulatedSurfaceArea ) {
		Vector3<float> ab = VectorB - VectorA;
		Vector3<float> ac = VectorC - VectorA;
		return (ab.Cross( ac ).Magnitude<Vector3<float>, float>() / 2) * simulatedSurfaceArea / (4 * double.Pi);
	}
}