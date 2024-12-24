using Engine.Standard.Render.Meshing;

namespace Sandbox.Logic.World.Tiles;

public sealed class Region( Icosphere icosphere, CompositeTile containingTile, int indexA, int indexB, int indexC, uint layer ) : IContainedTile, IContainingTile {
	private readonly Icosphere _icosphere = icosphere;
	public int IndexA { get; } = indexA;
	public int IndexB { get; } = indexB;
	public int IndexC { get; } = indexC;
	public Vector3<double> VectorA => _icosphere.Vertices[ IndexA ];
	public Vector3<double> VectorB => _icosphere.Vertices[ IndexB ];
	public Vector3<double> VectorC => _icosphere.Vertices[ IndexC ];
	public uint RemainingLayers => 1;
	public uint Layer { get; } = layer;
	public Vector4<double> Color => GetColor();
	public ITile ContainingTile { get; } = containingTile;
	public IReadOnlyList<ITile> SubTiles { get; private set; } = [];

	private Vector4<double> GetColor() {
		Vector4<double> color = Vector4<double>.AdditiveIdentity;
		foreach (ITile subTile in SubTiles)
			color += subTile.Color;
		color /= SubTiles.Count;
		return color;
	}

	public void SetSubTiles( Tile[] subTiles ) {
		SubTiles = subTiles;
	}
}
