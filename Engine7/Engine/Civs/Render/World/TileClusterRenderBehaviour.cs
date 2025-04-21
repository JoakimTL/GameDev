using Civs.Logic.World;
using Civs.Render.World.Shaders;
using Engine;
using Engine.Logging;
using Engine.Module.Render;
using Engine.Module.Render.Entities;
using Engine.Standard.Render;
using System;

namespace Civs.Render.World;
public sealed class TileClusterRenderBehaviour : DependentRenderBehaviourBase<WorldClusterArchetype>, IInitializable {

	private TileClusterSceneInstance _sceneInstance = null!;
	private bool _needsMeshUpdate = false;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
	}

	public void Initialize() {
		_sceneInstance = RenderEntity.RequestSceneInstance<TileClusterSceneInstance>( "terrain", 0 );
		_sceneInstance.SetShaderBundle( RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<GlobeTerrainShaderBundle>() );
		_sceneInstance.SetVertexArrayObject( RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity3SceneData>() );
		_sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
		_needsMeshUpdate = true;
	}

	public override void Update( double time, double deltaTime ) {
		if (!RenderEntity.TryGetBehaviour(out ClusterVisibilityRenderBehaviour? visibilityBehaviour ))
			return;
		_sceneInstance.SetActive( visibilityBehaviour.IsVisible );
		if (_sceneInstance.Active && _needsMeshUpdate) {
			_sceneInstance.UpdateMesh( this.Archetype.ClusterComponent.Cluster.Tiles, RenderEntity.ServiceAccess.MeshProvider );
			_sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
			_needsMeshUpdate = false;
		}
	}

	protected override bool InternalDispose() {
		return true;
	}

}

public sealed class ClusterVisibilityRenderBehaviour : DependentRenderBehaviourBase<WorldClusterArchetype> {
	public bool IsVisible { get; private set; } = false;

	public event Action? VisibilityChanged;

	public override void Update( double time, double deltaTime ) {
		CheckVisibilityAgainstCameraTranslation( RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation.Normalize<Vector3<float>, float>() );
	}

	public void SetVisibility( bool visible ) {
		if (IsVisible == visible)
			return;
		IsVisible = visible;
		VisibilityChanged?.Invoke();
	}

	public void CheckVisibilityAgainstCameraTranslation( Vector3<float> normalizedTranslation ) {
		bool shouldBeVisible = false;

		Vector3<float> min = Archetype.ClusterComponent.Cluster.Bounds.Minima;
		Vector3<float> max = Archetype.ClusterComponent.Cluster.Bounds.Maxima;

		Span<Vector3<float>> boundsCorners =
		[
			(min.X, min.Y, min.Z),
			(min.X, min.Y, max.Z),
			(min.X, max.Y, max.Z),
			(min.X, max.Y, min.Z),
			(max.X, min.Y, min.Z),
			(max.X, min.Y, max.Z),
			(max.X, max.Y, max.Z),
			(max.X, max.Y, min.Z)
		];

		for (int i = 0; i < boundsCorners.Length; i++)
			if (normalizedTranslation.Dot( boundsCorners[ i ] ) >= 0) {
				shouldBeVisible = true;
				break;
			}

		SetVisibility( shouldBeVisible );
	}
	protected override bool InternalDispose() {
		return true;
	}
}