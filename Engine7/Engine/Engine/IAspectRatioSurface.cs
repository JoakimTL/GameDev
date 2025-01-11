using System.Numerics;

namespace Engine;

public interface IAspectRatioSurface<TDimensionScalar, TAspectScalar> : ISurface<TDimensionScalar>
	where TDimensionScalar : unmanaged, INumber<TDimensionScalar>
	where TAspectScalar : unmanaged, IFloatingPointIeee754<TAspectScalar> {
	TAspectScalar AspectRatio { get; }
	Vector2<TAspectScalar> AspectRatioVector { get; }
}
