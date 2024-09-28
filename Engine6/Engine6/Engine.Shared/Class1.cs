namespace Engine.Shared;

public class Class1 {

}

public interface IVirtualSceneObject {
    /// <summary>
    /// Fired when the mesh or materials of the object have changed.
    /// </summary>
	event Action<IVirtualSceneObject> ChangedProperties;
    /// <summary>
    /// Fired when the transform of the object has changed.
    /// </summary>
	event Action<IVirtualSceneObject> ChangedTransform;
	//The mesh contains the vertex data of the model, but also the setup (Vertex Array Object) of the mesh.
	string MeshName { get; }
    string RenderSceneName { get; }
    //Materials contain texture, color and shader data. Using materials we can determine the shader, color and texture of the object
    IEnumerable<string> MaterialNames { get; }
    Matrix4x4<float> TransformationMatrix { get; }
}

public sealed class NewVirtualSceneObject {
    public string SceneName { get; }
    public IVirtualSceneObject VirtualSceneObject { get; }
}

public class Assets
{
    public string ShaderName;
    public string MeshName;
    public string MaterialNames;
}

public enum RenderedProperty {
    Shader,
    Mesh,
    Material,
    Color
    //VertexArrayObject Can be determined by the mesh
}