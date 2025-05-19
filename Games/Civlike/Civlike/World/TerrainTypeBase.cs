using Engine;

namespace Civlike.World;

public abstract class TerrainTypeBase {

	public uint Id { get; }

	public string Name { get; }
	public Vector4<float> Color { get; }

	public bool HasResources { get; }
	public bool IsLand { get; }
	public bool Claimable { get; }

	public TerrainTypeBase( uint id, string name, Vector4<float> color, bool hasResources, bool isLand = true, bool claimable = true ) {
		Id = id;
		Name = name;
		Color = color;
		HasResources = hasResources;
		IsLand = isLand;
		this.Claimable = claimable;
	}



}
