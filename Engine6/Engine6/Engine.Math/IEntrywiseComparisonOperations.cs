namespace Engine;

public interface IEntrywiseComparisonOperations<TVector>
    where TVector :
        unmanaged, IEntrywiseProductOperations<TVector>
{
    /// <returns>True if all entries are greater than the corresponding entries in <c>other</c>.</returns>
    bool GreaterThanEntrywise(in TVector other);
    /// <returns>True if all entries are greater than or equal to the corresponding entries in <c>other</c>.</returns>
    bool GreaterThanOrEqualEntrywise(in TVector other);
}
