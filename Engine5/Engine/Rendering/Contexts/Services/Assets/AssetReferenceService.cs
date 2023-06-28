namespace Engine.Rendering.Contexts.Services.Assets;

public abstract class AssetReferenceService<PathT> : Identifiable, IContextService where PathT : notnull
{
    private readonly Dictionary<PathT, uint> _numberOfReferencesByAsset;

    public AssetReferenceService()
    {
        _numberOfReferencesByAsset = new();
    }

    protected void AddReference(PathT t)
    {
        _numberOfReferencesByAsset.TryGetValue(t, out uint numRefs);
        _numberOfReferencesByAsset[t] = numRefs + 1;
    }

    /// <returns>True if the object has no more references</returns>
    protected bool RemoveReference(PathT t)
    {
        _numberOfReferencesByAsset.TryGetValue(t, out uint numRefs);
        if (numRefs > 2)
        {
            _numberOfReferencesByAsset[t] = numRefs - 1;
            return false;
        }
        _numberOfReferencesByAsset.Remove(t);
        return true;
    }

    protected void Clear() => _numberOfReferencesByAsset.Clear();
}
