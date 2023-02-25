using Engine.GlobalServices;
using Engine.Rendering.Contexts.Objects;
using Engine.Rendering.Contexts.Objects.Assets;
using Engine.Structure.Interfaces;
using System.Collections.Concurrent;

namespace Engine.Rendering.Contexts.Services;

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
            else
            {
                this.LogWarning($"Unable to load texture for {asset}!");
            }
        }

        while (_dereferencedAssets.TryDequeue(out var asset))
            if (_oldAssets.Add(asset))
                asset.UnloadingTime = time + 60;
        _removingAssets.AddRange(_oldAssets.Where(p => p.UnloadingTime > time));
        for (int i = 0; i < _removingAssets.Count; i++)
        {
            var asset = _removingAssets[i];
            asset.Dispose();
            _assets.TryRemove(asset.Path, out _);
            _oldAssets.Remove(asset);
        }
        _removingAssets.Clear();
    }

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
            if (_assets.TryAdd(path, asset))
            {
                _newAssets.Enqueue(asset);
            }
            else
            {
                asset = _assets[path];
            }
        }
        if (asset is null)
            return null;
        var ret = new ReferenceContainer<TextureAsset>(asset);
        _referenceCountingService.Reference(ret, asset);
        return ret;
    }

}
