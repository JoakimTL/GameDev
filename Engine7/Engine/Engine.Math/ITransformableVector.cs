namespace Engine;

public interface ITransformableVector<TMatrix, TVector>
	where TMatrix :
		unmanaged
	where TVector :
		unmanaged, ITransformableVector<TMatrix, TVector> {
	/// <returns><c>null</c> if the transform can't be resolved.</returns>
	TVector? TransformWorld( in TMatrix l );
	TVector TransformNormal( in TMatrix l );
}