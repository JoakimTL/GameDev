using Civlike.Client.Render.World.Shaders;
using Civlike.Logic.World;
using Civlike.World.GameplayState;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Standard.Render;

namespace Civlike.Client.Render.World;
public sealed class TileClusterRenderBehaviour : DependentRenderBehaviourBase<WorldClusterArchetype>, IInitializable {

	private TileGroupSceneInstance _sceneInstance = null!;
	private bool _needsMeshUpdate = false;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
	}

	public void Initialize() {
		this._sceneInstance = this.RenderEntity.RequestSceneInstance<TileGroupSceneInstance>( "terrain", 0 );
		this._sceneInstance.SetShaderBundle( this.RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<GlobeTerrainShaderBundle>() );
		this._sceneInstance.SetVertexArrayObject( this.RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity3SceneData>() );
		this._sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
		this._needsMeshUpdate = true;
	}

	public override void Update( double time, double deltaTime ) {
		if (!this.RenderEntity.TryGetBehaviour( out ClusterVisibilityRenderBehaviour? visibilityBehaviour ))
			return;
		this._sceneInstance.SetAllocated( visibilityBehaviour.IsVisible );
		if (this._sceneInstance.Allocated && this._needsMeshUpdate) {
			BoundedRenderClusterComponent clusterComponent = this.Archetype.ClusterComponent;
			this._sceneInstance.UpdateMesh( clusterComponent.Globe, clusterComponent.Cluster.Faces, this.RenderEntity.ServiceAccess.MeshProvider );
			this._sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
			this._needsMeshUpdate = false;
		}
	}

	protected override bool InternalDispose() {
		return true;
	}

}
