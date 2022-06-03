using Engine;
using Engine.Modularity.Modules;
using Engine.Modularity.Modules.Submodules;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace TestPlatform;

[Engine.Structure.ProcessAfter( typeof( ContextUpdateSubmodule ), typeof( IUpdateable ) )]
public class TestHeightmapSubmodule : Submodule {

	public OpenSceneObject<Vertex3, Entity3SceneData> _map;

	public TestHeightmapSubmodule( ) : base( true ) {

	}

	protected override bool OnDispose() => true;
}
