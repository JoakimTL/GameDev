namespace Sandbox.Logic.OldWorld.Tiles;

public interface ITile {
	int IndexA { get; }
	int IndexB { get; }
	int IndexC { get; }

	Vector3<float> VectorA { get; }
	Vector3<float> VectorB { get; }
	Vector3<float> VectorC { get; }

	uint RemainingLayers { get; }
	uint Layer { get; }

	Vector4<float> Color { get; }
}
