using Civs.Logic.World;
using Civs.Render.World.Shaders;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Standard.Render;

namespace Civs.Render.World;
public sealed class TileClusterRenderBehaviour : DependentRenderBehaviourBase<WorldClusterArchetype>, IInitializable {

	private TileGroupSceneInstance _sceneInstance = null!;
	private bool _needsMeshUpdate = false;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
	}

	public void Initialize() {
		_sceneInstance = RenderEntity.RequestSceneInstance<TileGroupSceneInstance>( "terrain", 0 );
		_sceneInstance.SetShaderBundle( RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<GlobeTerrainShaderBundle>() );
		_sceneInstance.SetVertexArrayObject( RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity3SceneData>() );
		_sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
		_needsMeshUpdate = true;
	}

	public override void Update( double time, double deltaTime ) {
		if (!RenderEntity.TryGetBehaviour( out ClusterVisibilityRenderBehaviour? visibilityBehaviour ))
			return;
		_sceneInstance.SetAllocated( visibilityBehaviour.IsVisible );
		if (_sceneInstance.Allocated && _needsMeshUpdate) {
			var clusterComponent = Archetype.ClusterComponent;
			var globe = clusterComponent.Globe;
			_sceneInstance.UpdateMesh( clusterComponent.Cluster.Faces, RenderEntity.ServiceAccess.MeshProvider );
			_sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
			_needsMeshUpdate = false;
		}
	}

	protected override bool InternalDispose() {
		return true;
	}

}
