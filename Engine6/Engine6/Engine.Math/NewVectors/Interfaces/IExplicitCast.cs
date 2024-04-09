namespace Engine.Math.NewVectors.Interfaces;

public interface IExplicitCast<TOriginal, TResult>
	where TOriginal :
		unmanaged, IExplicitCast<TOriginal, TResult>
	where TResult :
		unmanaged {
	static abstract explicit operator TResult( in TOriginal value );
}