﻿namespace Sandbox.Logic.World.Tiles;

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
			Vector3<float> ab = VectorB - VectorA;
			Vector3<float> ac = VectorC - VectorA;
			return ab.Cross( ac ).Normalize<Vector3<float>, float>();
		}
	}
	public float Area {
		get {
			Vector3<float> ab = VectorB - VectorA;
			Vector3<float> ac = VectorC - VectorA;
			return ab.Cross( ac ).Magnitude<Vector3<float>, float>() / 2;
		}
	}

	public Vector4<float> Color { get; private set; }

	private Vector4<float> GetColor() {
		return 1;
	}

	public AABB<Vector3<float>> GetBounds()
		=> AABB.Create( [ VectorA, VectorB, VectorC ] );

	internal bool UpdateRenderingProperties() {
		Vector4<float> newColor = GetColor();
		if (newColor == Color)
			return false;
		Color = newColor;
		return true;
	}
}