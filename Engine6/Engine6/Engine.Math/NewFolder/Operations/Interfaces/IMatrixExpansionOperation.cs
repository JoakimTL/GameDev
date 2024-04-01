namespace Engine.Math.NewFolder.Operations.Interfaces;

public interface IMatrixExpansionOperation<T1, T2> where T1 : unmanaged where T2 : unmanaged
{
    static abstract void Expand(in T1 m, out T2 result);
}
