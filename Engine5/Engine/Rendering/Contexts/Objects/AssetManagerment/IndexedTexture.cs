namespace Engine.Rendering.Contexts.Objects.AssetManagerment;

public class IndexedTexture : Identifiable
{
    public Texture Texture { get; }
    public ushort Index { get; }

    public IndexedTexture(Texture texture, ushort index) : base(texture.IdentifiableName)
    {
        Texture = texture;
        Index = index;
    }
}
