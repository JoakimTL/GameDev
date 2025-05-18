using Engine;

namespace Civlike.World;

public abstract class TerrainTypeBase {

	public uint Id { get; }

	public Vector4<float> Color { get; }

	public bool HasResources { get; }
	public bool IsLand { get; }

	public TerrainTypeBase( uint id, Vector4<float> color, bool hasResources, bool isLand = true ) {
		Id = id;
		Color = color;
		HasResources = hasResources;
		IsLand = isLand;
	}



}
