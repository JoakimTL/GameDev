namespace Sandbox.Logic.World.Tiles;

public sealed class TileRenderModel {
	private readonly Tile _tile;
	private readonly TriangleIndices _indices;

	public TileRenderModel( Tile tile, TriangleIndices indices, Vector4<float> color ) {
		this._tile = tile;
		this._indices = indices;
		this.Color = color;
	}

	public Vector3<float> VectorA => _tile.Globe.Vertices[ _indices.A ];
	public Vector3<float> VectorB => _tile.Globe.Vertices[ _indices.B ];
	public Vector3<float> VectorC => _tile.Globe.Vertices[ _indices.C ];
	public Vector3<float> Normal {
		get {
			var ab = VectorB - VectorA;
			var ac = VectorC - VectorA;
			return ab.Cross( ac ).Normalize<Vector3<float>, float>();
		}
	}
	public float Area {
		get {
			var ab = VectorB - VectorA;
			var ac = VectorC - VectorA;
			return ab.Cross( ac ).Magnitude<Vector3<float>, float>() / 2;
		}
	}

	public Vector4<float> Color { get; internal set; }

	public AABB<Vector3<float>> GetBounds()
		=> AABB.Create( [ VectorA, VectorB, VectorC ] );
}