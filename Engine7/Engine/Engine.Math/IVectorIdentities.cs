using System.Numerics;

namespace Engine;

public interface IVectorIdentities<TVector> :
		IAdditiveIdentity<TVector, TVector>,
		IMultiplicativeIdentity<TVector, TVector>
	where TVector :
		unmanaged, IVectorIdentities<TVector> {
	static abstract TVector Two { get; }
	static abstract TVector One { get; }
	static abstract TVector Zero { get; }
}