using Engine;

namespace Civlike.World;

public abstract class TerrainTypeBase( uint id, string name, Vector4<float> color, bool hasResources, bool isLand = true, bool claimable = true ) {

	public uint Id { get; } = id;

	public string Name { get; } = name;
	public Vector4<float> Color { get; } = color;

	public bool HasResources { get; } = hasResources;
	public bool IsLand { get; } = isLand;
	public bool Claimable { get; } = claimable;
}
