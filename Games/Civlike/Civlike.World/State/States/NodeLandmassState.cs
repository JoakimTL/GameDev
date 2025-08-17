using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civlike.World.State.States;
public sealed class NodeLandmassState : StateBase<Node> {
	public float Height { get; set; } = 0;
	public float HeightFactor => (float) ((StateContainer.Globe.RadiusMeters + Height) / StateContainer.Globe.RadiusMeters);
	public float SeismicActivity { get; set; }
	public float Ruggedness { get; set; }
}
public sealed class NodeWaterState : StateBase<Node> {
	public float WaterLevelMeters { get; set; }
	public float TotalHeightMeters => WaterLevelMeters + StateContainer.GetState<NodeLandmassState>().Height;
	public float WaterSalinity { get; set; }
	
	/// <summary>m² of water surface at the *current* stage (cached)</summary>
	public float SurfaceArea { get; set; }
	/// <summary>m³ stored when the surface just touches the outlet saddle.
	/// Lets you halve a lake, drain a reservoir, etc. without recomputing geometry.</summary>
	public float StorageVolumeAtOutlet { get; set; }

	public Node? DownstreamNode { get; set; }
	public float DownstreamSlope { get; set; }
	public float SpillSaddleMeters { get; set; }

	//TODO: calculate the volume of the node before spillover and the area of the node which can be used for volume post spillover heights.
}
/// <summary>
/// Immutable, byte-sized drainage pointer baked at world-gen.
/// Lives on *every* vertex so the hydrology loop never branches.
/// </summary>
public readonly struct DrainLink {
	public readonly int DownstreamIndex;   // -1 ⇒ ocean / interior sink
	public readonly float SpillSaddleM;      // sill height that must be overtopped

	public DrainLink( int downstreamIndex, float spillSaddleM ) {
		DownstreamIndex = downstreamIndex;
		SpillSaddleM = spillSaddleM;
	}
}

public sealed class TileColorState : StateBase<Tile> {
	public Vector4<float> Color { get; set; } = Vector4<float>.UnitW; //TODO: alpha>
}