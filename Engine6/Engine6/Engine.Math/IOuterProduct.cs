namespace Engine;

public interface IOuterProduct<TVector, TResult>
    where TVector :
        unmanaged, IOuterProduct<TVector, TResult>
    where TResult :
        unmanaged
{
    TResult Wedge(in TVector r);
}