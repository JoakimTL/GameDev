namespace Sandbox.Logic.World.Tiles;

public interface ITile {
	int IndexA { get; }
	int IndexB { get; }
	int IndexC { get; }

	Vector3<double> VectorA { get; }
	Vector3<double> VectorB { get; }
	Vector3<double> VectorC { get; }

	uint RemainingLayers { get; }
	uint Layer { get; }

	Vector4<double> Color { get; }
}
