using System.Numerics;

namespace Engine.Math;

public interface IVectorEntrywiseOperators<TSelf, T> where TSelf : unmanaged, IVectorEntrywiseOperators<TSelf, T> where T : unmanaged, INumberBase<T> {
	static abstract TSelf operator *( in TSelf l, in TSelf r );
	static abstract TSelf operator /( in TSelf l, in TSelf r );
}
