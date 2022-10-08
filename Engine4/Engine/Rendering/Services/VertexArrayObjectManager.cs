using Engine.Structure;

namespace Engine.Rendering.Services;

[Structure.ProcessBefore( typeof( WindowService ), typeof( IDisposable ) )]
[Structure.ProcessAfter( typeof( WindowService ), typeof( IInitializable ) )]
public class VertexArrayObjectManager : ServiceProvider<VertexArrayObject> { }
