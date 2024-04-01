namespace Engine.Math.NewVectors.Interfaces;

public interface IGeometricProduct<TRight, TLeft, TResult>
	where TRight :
		unmanaged, IGeometricProduct<TRight, TLeft, TResult>
	where TLeft :
		unmanaged
	where TResult :
		unmanaged {
	TResult Multiply( in TLeft l );
}
