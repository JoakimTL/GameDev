namespace Engine.Math.NewFolder.Operations.Interfaces;

public interface IEntrywiseOperations<T> where T : unmanaged
{
    static abstract T MultiplyEntrywise(in T l, in T r);
    static abstract T DivideEntrywise(in T l, in T r);
}
