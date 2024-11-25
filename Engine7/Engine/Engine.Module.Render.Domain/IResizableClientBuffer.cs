using System.Numerics;

namespace Engine.Module.Render.Domain;

public interface IResizableClientBuffer<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	bool ResizeWrite( nint srcPtr, TScalar srcLengthBytes );
}
