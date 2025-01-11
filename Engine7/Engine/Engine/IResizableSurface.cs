using System.Numerics;

namespace Engine;

public interface IResizableSurface<TDimensionScalar> : ISurface<TDimensionScalar>
	where TDimensionScalar : unmanaged, INumber<TDimensionScalar> {

	event Action<IResizableSurface<TDimensionScalar>> OnResized;
}
