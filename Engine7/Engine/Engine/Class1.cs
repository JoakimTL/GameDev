using System.Numerics;

namespace Engine;

public class Class1 {

}

public interface IResizableSurface<TDimensionScalar, TAspectScalar>
	where TDimensionScalar : unmanaged, INumber<TDimensionScalar>
	where TAspectScalar : unmanaged, IFloatingPointIeee754<TAspectScalar> {

	event Action<IResizableSurface<TDimensionScalar, TAspectScalar>> OnResized;
	Vector2<TDimensionScalar> Size { get; }
	TAspectScalar AspectRatio { get; }
	Vector2<TAspectScalar> AspectRatioVector { get; }

}