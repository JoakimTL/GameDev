﻿//using Engine.Rendering.Contexts.Objects.Assets;
//using Engine.Structure.Interfaces;
//using System.Collections.Concurrent;

//namespace Engine.Rendering.Contexts.Services;

//public sealed class MaterialAssetService : Identifiable, IContextService, IUpdateable
//{

//    private readonly ConcurrentDictionary<string, MaterialAsset> _assets;
//    private readonly ConcurrentQueue<MaterialAsset> _newAssets;
//    private readonly ShaderAssetService _shaderAssetService;
//    private readonly TextureAssetService _textureAssetService;

//    public MaterialAssetService(ShaderAssetService shaderAssetService, TextureAssetService textureAssetService)
//    {
//        _assets = new();
//        _newAssets = new();
//        _shaderAssetService = shaderAssetService;
//        _textureAssetService = textureAssetService;
//    }

//    public void Update(float time, float deltaTime)
//    {
//        while (_newAssets.TryDequeue(out var asset))
//        {
//            ShaderAsset? shaderAsset = _shaderAssetService.Get(asset.ShaderIdentity);
//            List<ReferenceContainer<TextureAsset>> textureAssets = asset.TexturePaths.Select(_textureAssetService.Get).OfType<ReferenceContainer<TextureAsset>>().ToList();
//            if (shaderAsset is null)
//            {
//                this.LogWarning($"Unable to load material for {asset}!");
//                continue;
//            }
//            asset.Set(shaderAsset, textureAssets);
//        }
//    }

//    public MaterialAsset Get(string identity)
//    {
//        if (!_assets.TryGetValue(identity, out var asset))
//        {
//            asset = new(identity);
//            if (_assets.TryAdd(identity, asset))
//            {
//                _newAssets.Enqueue(asset);
//            }
//            else
//            {
//                asset = _assets[identity];
//            }
//        }
//        return asset;
//    }

//}
