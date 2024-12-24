using Engine.Standard.Render.Meshing;
using System;

namespace Sandbox.Logic.World.Tiles;

public sealed class CompositeTile( Icosphere icosphere, CompositeTile? containingTile, int indexA, int indexB, int indexC, uint layer ) : IContainedTile, IContainingTile {
	private readonly Icosphere _icosphere = icosphere;
	public int IndexA { get; } = indexA;
	public int IndexB { get; } = indexB;
	public int IndexC { get; } = indexC;
	public Vector3<double> VectorA => _icosphere.Vertices[ IndexA ];
	public Vector3<double> VectorB => _icosphere.Vertices[ IndexB ];
	public Vector3<double> VectorC => _icosphere.Vertices[ IndexC ];
	public uint RemainingLayers { get; private set; } = 0;
	public uint Layer { get; } = layer;
	public ITile? ContainingTile { get; } = containingTile;
	public IReadOnlyList<ITile> SubTiles { get; private set; } = [];

	public Vector4<double> Color => GetColor();

	private Vector4<double> GetColor() {
		Vector4<double> color = Vector4<double>.AdditiveIdentity;
		foreach (ITile subTile in SubTiles)
			color += subTile.Color;
		color /= SubTiles.Count;
		return color;
	}

	public void SetSubTiles( IContainedTile[] subTiles ) {
		SubTiles = subTiles;
		RemainingLayers = GetRemainingLayers( subTiles );
	}

	private uint GetRemainingLayers( IContainedTile[] subTiles ) {
		if (subTiles.Length == 0)
			return 0;
		uint max = 0;
		foreach (IContainedTile subTile in subTiles)
			max = Math.Max( max, subTile.RemainingLayers );
		return max + 1;
	}
}
