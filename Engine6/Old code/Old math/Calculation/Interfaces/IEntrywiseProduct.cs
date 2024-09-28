namespace Engine.Math.NewFolder.Calculation.Interfaces;

public interface IEntrywiseProduct<T> where T : unmanaged
{
    static abstract T MultiplyEntrywise(in T l, in T r);
    static abstract T DivideEntrywise(in T l, in T r);
}