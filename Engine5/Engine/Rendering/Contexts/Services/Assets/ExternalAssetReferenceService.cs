namespace Engine.Rendering.Contexts.Services.Assets;

public abstract class ExternalAssetReferenceService<PathT, AssetT> : AssetReferenceService<PathT> where PathT : notnull
{
    public string BaseDirectory { get; }
    public string AssetExtension { get; }

    public ExternalAssetReferenceService(string baseDirectory, string assetExtension)
    {
        BaseDirectory = baseDirectory;
        AssetExtension = assetExtension;
    }

    /// <summary>
    /// A requested texture should also be <see cref="Discarded(string)"/> when no longer in use.
    /// </summary>
    /// <param name="path">Relative path</param>
    public AssetT? Request(string assetPath) => OnRequest(GetProperAssetPath(assetPath).NotNull());

    public void Discarded(string assetPath) => OnDiscarded(GetProperAssetPath(assetPath).NotNull());

    public string? GetProperAssetPath(string? assetPath)
    {
        if (assetPath is null)
            return null;
        if (assetPath.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
            assetPath = assetPath[AppDomain.CurrentDomain.BaseDirectory.Length..];
        if (!assetPath.StartsWith(BaseDirectory))
            assetPath = BaseDirectory + assetPath;
        if (Path.GetExtension(assetPath) != AssetExtension)
            assetPath += AssetExtension;
        return assetPath;
    }

    protected abstract AssetT? OnRequest(string filePath);
    protected abstract void OnDiscarded(string filePath);
}
