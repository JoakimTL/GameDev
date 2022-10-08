using Engine.Modularity.Domains.Modules;
using Engine.Modularity.ECS.Components;
using Engine.Rendering.Shaders;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Modularity.ECS.Organization;

public class EntityScene3Manager : EntitySceneManager<Render3Component, Transform3Component, Vertex3, Entity3SceneData> {
	public EntityScene3Manager( RenderModule renderModule, EntityManager manager, Scene scene, VertexMesh<Vertex3> defaultMesh, ShaderBundle defaultShaderBundle ) : base( renderModule, manager, scene, defaultMesh, defaultShaderBundle ) { }
	//Entity3ShaderBundle

	protected override Entity3SceneData GetSceneData( Entity e ) {
		if ( e.TryGetComponent( out Transform3Component? transform ) && e.TryGetComponent( out Render3Component? render ) ) {
			return new Entity3SceneData {
				ModelMatrix = transform.Transform.Matrix,
				DiffuseTextureHandle = render.ResolveTextureToHandle( 0 ),
				NormalTextureHandle = render.ResolveTextureToHandle( 1 ),
				GlowTextureHandle = render.ResolveTextureToHandle( 2 ),
				LightingTextureHandle = render.ResolveTextureToHandle( 3 ),
				NormalMapped = 1,
				Color = render.Color
			};
		}
		return default;
	}
}
