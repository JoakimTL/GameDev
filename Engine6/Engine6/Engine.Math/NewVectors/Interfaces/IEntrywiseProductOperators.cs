namespace Engine.Math.NewVectors.Interfaces;

public interface IEntrywiseProductOperators<TVector>
    where TVector :
        unmanaged, IEntrywiseProductOperators<TVector>, IEntrywiseProductOperations<TVector>
{
    static abstract TVector operator *(in TVector l, in TVector r);
    static abstract TVector operator /(in TVector l, in TVector r);
}