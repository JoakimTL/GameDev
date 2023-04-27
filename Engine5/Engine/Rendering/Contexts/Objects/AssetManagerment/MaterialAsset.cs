namespace Engine.Rendering.Contexts.Objects.AssetManagerment;

public class MaterialAsset : Identifiable
{
    public string MaterialAssetName { get; }
    public ShaderBundleBase ShaderBundle { get; private set; }
    private readonly List<IndexedTexture> _textures;
    public IReadOnlyList<IndexedTexture> Textures => _textures;

    public MaterialAsset(string materialAssetName, ShaderBundleBase shaderBundle, IEnumerable<IndexedTexture> textures) : base(materialAssetName)
    {
        MaterialAssetName = materialAssetName;
        ShaderBundle = shaderBundle;
        _textures = new(textures);
    }

}