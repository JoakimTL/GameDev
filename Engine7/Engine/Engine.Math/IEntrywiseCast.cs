using System.Numerics;

namespace Engine;

public interface IEntrywiseCast<TOriginalVector, TOriginalScalar>
	where TOriginalVector :
		unmanaged, IEntrywiseCast<TOriginalVector, TOriginalScalar>
	where TOriginalScalar :
		unmanaged, INumber<TOriginalScalar> {
	TResultVector EntrywiseCastChecked<TResultVector, TResultScalar>()
		where TResultVector :
			unmanaged
		where TResultScalar :
			unmanaged, INumber<TResultScalar>;
	TResultVector EntrywiseCastSaturating<TResultVector, TResultScalar>()
		where TResultVector :
			unmanaged
		where TResultScalar :
			unmanaged, INumber<TResultScalar>;
	TResultVector EntrywiseCastSaturatin2g<TResultVector, TResultScalar>()
		where TResultVector :
			unmanaged
		where TResultScalar :
			unmanaged, INumber<TResultScalar>;
}