using Engine.GlobalServices;
using Engine.Rendering.Objects;
using Engine.Rendering.Objects.Assets;
using Engine.Structure.Interfaces;
using Engine.Time;
using System.Collections.Concurrent;

namespace Engine.Rendering.Services;

public sealed class TextureAssetService : Identifiable, IContextService, IUpdateable
{
    public static string BaseDirectory => "assets/textures/";
    private readonly ConcurrentDictionary<string, TextureAsset> _assets;
    private readonly ConcurrentQueue<TextureAsset> _newAssets;
    private readonly ConcurrentQueue<TextureAsset> _dereferencedAssets;
    private readonly HashSet<TextureAsset> _oldAssets;
    private readonly List<TextureAsset> _removingAssets;
    private readonly TextureService _textureService;
    private readonly TextureIndexingService _textureIndexingService;
    private readonly ReferenceCountingService _referenceCountingService;

    public TextureAssetService(TextureService textureService, TextureIndexingService textureIndexingService, ReferenceCountingService referenceCountingService)
    {
        _assets = new();
        _newAssets = new();
        _dereferencedAssets = new();
        _oldAssets = new();
        _removingAssets = new();
        _textureService = textureService;
        _textureIndexingService = textureIndexingService;
        _referenceCountingService = referenceCountingService;
        _referenceCountingService.Unreferenced += Unreferenced;
    }

    private void Unreferenced(object obj)
    {
        if (obj is TextureAsset asset)
            _dereferencedAssets.Enqueue(asset);
    }

    public void Update(float time, float deltaTime)
    {
        while (_newAssets.TryDequeue(out var asset))
        {
            Texture? texture = _textureService.Get(asset.Path);
            if (texture is not null)
            {
                asset.SetTexture(texture);
                _textureIndexingService.SetIndex(asset);
            }
        }
        while (_dereferencedAssets.TryDequeue(out var asset))
            if (_oldAssets.Add(asset))
                asset.DeadlineTillDisposal = Clock32.StartupTime + 60;
        _removingAssets.AddRange(_oldAssets.Where(p => p.ShouldDispose));
        for (int i = 0; i < _removingAssets.Count; i++)
        {
            var asset = _removingAssets[i];
            asset.Dispose();
            _assets.TryRemove(asset.Path, out _);
            _oldAssets.Remove(asset);
        }
        _removingAssets.Clear();
    }

    //What if the refereer decides to swap assets, but never ends up gc-ed?

    public ReferenceContainer<TextureAsset>? Get(string assetPath)
    {
        string path = $"{BaseDirectory}{assetPath}";
        if (!File.Exists(path))
        {
            this.LogWarning($"File {path} not found!");
            return null;
        }
        if (!_assets.TryGetValue(path, out var asset))
        {
            asset = new(path);
            _assets.TryAdd(path, asset);
            _newAssets.Enqueue(asset);
        }
        if (asset is null)
            return null;
        var ret = new ReferenceContainer<TextureAsset>(asset);
        _referenceCountingService.Reference(ret, asset);
        return ret;
    }



    //Loads textures and reference counts them
    //Indexes the textures, such that they can be referenced using a ushort rather than ulong (2 bytes vs 8 bytes) (can be outsourced to separate service)

    //Create a FileLoaderService
}
