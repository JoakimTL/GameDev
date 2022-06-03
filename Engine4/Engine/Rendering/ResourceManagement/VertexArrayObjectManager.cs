using Engine.Structure;

namespace Engine.Rendering.ResourceManagement;

[Structure.ProcessBefore( typeof( Window ), typeof( IDisposable ) )]
public class VertexArrayObjectManager : SingletonProvider<VertexArrayObject> { }
