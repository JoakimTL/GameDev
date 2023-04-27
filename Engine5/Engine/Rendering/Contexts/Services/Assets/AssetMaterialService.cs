using Engine.Rendering.Contexts.Objects;
using Engine.Rendering.Contexts.Objects.AssetManagerment;

namespace Engine.Rendering.Contexts.Services.Assets;

public sealed class AssetMaterialService : ExternalAssetReferenceService<string, MaterialAsset>, IContextService
{

    private readonly Dictionary<string, MaterialAsset> _materials;
    private readonly ShaderBundleService _shaderBundleService;
    private readonly AssetIndexedTextureService _assetIndexedTextureService;

    public AssetMaterialService(ShaderBundleService shaderBundleService, AssetIndexedTextureService assetIndexedTextureService) : base("assets\\materials\\", ".mat")
    {
        _materials = new();
        _shaderBundleService = shaderBundleService;
        _assetIndexedTextureService = assetIndexedTextureService;
    }

    /// <summary>
    /// A requested texture should also be <see cref="Discarded(string)"/> when no longer in use.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    protected override MaterialAsset? OnRequest(string filePath)
    {
        if (_materials.TryGetValue(filePath, out MaterialAsset? material))
        {
            AddReference(filePath);
            return material;
        }

        if (!LoadFile(filePath, out string shaderIdentity, out string[] texturePaths))
            return null;

        ShaderBundleBase? shaderBundle = _shaderBundleService.Get(shaderIdentity);
        if (shaderBundle is null)
            return this.LogWarningThenReturnDefault<MaterialAsset>("Unable to load shaderbundle.");
        
        material = new(filePath, shaderBundle, texturePaths.Select(_assetIndexedTextureService.Request));
        _materials.Add(filePath, material);
        AddReference(filePath);
        return material;
    }

    protected override void OnDiscarded(string filePath)
    {
        if (!_materials.Remove(filePath, out MaterialAsset? material))
            return;
        if (RemoveReference(filePath))
            foreach (var t in material.Textures)
                if (t.Texture.FilePath is not null)
                    _assetIndexedTextureService.Discarded(t.Texture.FilePath);
    }

    private static bool LoadFile(string path, out string shaderIdentity, out string[] texturePaths)
    {
        shaderIdentity = string.Empty;
        texturePaths = Array.Empty<string>();
        if (!File.Exists(path))
            return Log.WarningThenReturn($"Could not find file {path}.", false);
        List<string> texturePathList = new();
        string[] fileData = File.ReadAllLines(path);
        for (int i = 0; i < fileData.Length; i++)
        {
            string line = fileData[i];
            string[] split = line.Split('=').Select(p => p.Trim()).ToArray();
            if (split.Length != 2)
            {
                Log.Warning($"{path}: Error on line {i + 1}.");
                continue;
            }
            if (split[0].StartsWith("shader"))
            {
                shaderIdentity = split[1];
            }
            else if (split[0].StartsWith("texture"))
            {
                texturePathList.Add(split[1]);
            }
        }
        texturePaths = texturePathList.ToArray();
        return true;
    }
}
