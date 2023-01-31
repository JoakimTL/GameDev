using Engine.Rendering.Objects;
using Engine.Rendering.Objects.Assets;
using Engine.Structure.Interfaces;
using System.Collections.Concurrent;

namespace Engine.Rendering.Services;

public sealed class ShaderAssetService : Identifiable, IContextService, IUpdateable
{

    private readonly ConcurrentDictionary<string, ShaderAsset> _assets;
    private readonly ConcurrentQueue<ShaderAsset> _newAssets;
    private readonly ShaderBundleService _shaderBundleService;

    public ShaderAssetService(ShaderBundleService shaderBundleService)
    {
        this._shaderBundleService = shaderBundleService;
        _assets = new();
        _newAssets = new();
    }

    public void Update(float time, float deltaTime)
    {
        while (_newAssets.TryDequeue(out var asset))
        {
            ShaderBundleBase? shaderBundle = _shaderBundleService.Get(asset.Identity);
            if (shaderBundle is not null)
            {
                asset.SetShaderBundle(shaderBundle);
            }
            else
            {
                this.LogWarning($"Unable to load shader for {asset}!");
            }
        }
    }

    public ShaderAsset? Get(string identity)
    {
        if (!_assets.TryGetValue(identity, out var asset))
        {
            asset = new(identity);
            if (_assets.TryAdd(identity, asset))
            {
                _newAssets.Enqueue(asset);
            }
            else
            {
                asset = _assets[identity];
            }
        }
        return asset;
    }

}
