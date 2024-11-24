using System.Numerics;

namespace Engine;

public delegate void BufferDataChanged<TScalar>( TScalar offsetBytes, TScalar lengthBytes ) where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar>;