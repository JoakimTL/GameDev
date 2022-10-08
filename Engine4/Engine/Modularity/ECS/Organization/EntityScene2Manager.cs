using Engine.Modularity.Domains.Modules;
using Engine.Modularity.ECS.Components;
using Engine.Rendering.Shaders;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Modularity.ECS.Organization;

public class EntityScene2Manager : EntitySceneManager<Render2Component, Transform2Component, Vertex2, Entity2SceneData> {
	public EntityScene2Manager( RenderModule renderModule, EntityManager manager, Scene scene, VertexMesh<Vertex2> defaultMesh, ShaderBundle defaultShaderBundle ) : base( renderModule, manager, scene, defaultMesh, defaultShaderBundle ) { }
	//Entity2ShaderBundle

	protected override Entity2SceneData GetSceneData( Entity e ) {
		if ( e.TryGetComponent( out Transform2Component? transform ) && e.TryGetComponent(out Render2Component? render ) ) {
			return new Entity2SceneData {
				ModelMatrix = transform.Transform.Matrix,
				DiffuseTextureHandle = render.ResolveTextureToHandle( 0 ),
				Color = render.Color
			};
		}
		return default;
	}
}