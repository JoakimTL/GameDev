using System.Numerics;

namespace Engine;

public interface ISurface<TDimensionScalar>
	where TDimensionScalar : unmanaged, INumber<TDimensionScalar> {

	Vector2<TDimensionScalar> Size { get; }
}
