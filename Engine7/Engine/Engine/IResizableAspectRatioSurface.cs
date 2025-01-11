using System.Numerics;

namespace Engine;

public interface IResizableAspectRatioSurface<TDimensionScalar, TAspectScalar> : IResizableSurface<TDimensionScalar>, IAspectRatioSurface<TDimensionScalar, TAspectScalar>
	where TDimensionScalar : unmanaged, INumber<TDimensionScalar>
	where TAspectScalar : unmanaged, IFloatingPointIeee754<TAspectScalar>;