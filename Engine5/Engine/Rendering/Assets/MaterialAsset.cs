namespace Engine.Rendering.Assets;
public class MaterialAsset
{
    //Textures
    //Shader


}

public class MeshAsset
{
    //VAO
    //Mesh vertex data


}


public class RenderableAsset
{
    public MaterialAsset Material { get; }
    public MeshAsset Mesh { get; }
}