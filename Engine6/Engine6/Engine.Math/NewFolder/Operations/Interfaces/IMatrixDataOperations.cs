namespace Engine.Math.NewFolder.Operations.Interfaces;

public interface IMatrixDataOperations<T> where T : unmanaged
{
    static abstract bool TryFillRowMajor<TData>(in T m, Span<TData> resultStorage, uint destinationOffsetBytes = 0) where TData : unmanaged;
    static abstract bool TryFillColumnMajor<TData>(in T m, Span<TData> resultStorage, uint destinationOffsetBytes = 0) where TData : unmanaged;
}