namespace Engine.Rendering.Objects.Assets;

/// <summary>
/// Assets are files that are loaded by an OGL context
/// </summary>
public abstract class LoadedAssetBase : Identifiable, IDisposable
{
    public string Path { get; }
    /// <summary>
    /// The time at which this asset will unload if no more references points to it.
    /// </summary>
    public float UnloadingTime { get; internal set; }

    public delegate void AssetEventHandler(LoadedAssetBase asset);
    public event AssetEventHandler? AssetDisposed;

    protected override string UniqueNameTag => Path;

    protected LoadedAssetBase(string path)
    {
        this.Path = path;
    }

#if DEBUG
    ~LoadedAssetBase()
    {
        System.Diagnostics.Debug.Fail($"{this} not disposed!");
    }
#endif

    protected abstract void OnDispose();
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        OnDispose();
        AssetDisposed?.Invoke(this);
    }
}
