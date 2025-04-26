using Engine;

namespace Civs.World;

public abstract class TerrainTypeBase {

	public uint Id { get; }

	public Vector4<float> Color { get; }

	public TerrainTypeBase( uint id, Vector4<float> color ) {
		Id = id;
		Color = color;
	}



}
