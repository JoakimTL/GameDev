using Engine.Standard.Render.Meshing;

namespace Sandbox.Logic.OldWorld.Tiles;

public sealed class Region( Icosphere icosphere, CompositeTile containingTile, int indexA, int indexB, int indexC, uint layer ) : IContainedTile, IContainingTile {
	private readonly Icosphere _icosphere = icosphere;
	public int IndexA { get; } = indexA;
	public int IndexB { get; } = indexB;
	public int IndexC { get; } = indexC;
	public Vector3<float> VectorA => _icosphere.Vertices[ IndexA ];
	public Vector3<float> VectorB => _icosphere.Vertices[ IndexB ];
	public Vector3<float> VectorC => _icosphere.Vertices[ IndexC ];
	public uint RemainingLayers => 1;
	public uint Layer { get; } = layer;
	public Vector4<float> Color => GetColor();
	public ITile ContainingTile { get; } = containingTile;
	public IReadOnlyList<ITile> SubTiles { get; private set; } = [];

	private Vector4<float> GetColor() {
		Vector4<float> color = Vector4<float>.AdditiveIdentity;
		foreach (ITile subTile in SubTiles)
			color += subTile.Color;
		color /= SubTiles.Count;
		return color;
	}

	public void SetSubTiles( Tile[] subTiles ) {
		SubTiles = subTiles;
	}
}
