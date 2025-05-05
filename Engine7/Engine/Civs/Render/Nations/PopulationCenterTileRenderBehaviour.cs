using Civs.Render.World.Shaders;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Standard.Render;
using Civs.Logic.Nations;
using Engine.Module.Entities.Container;
using Civs.World.NewWorld;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Providers;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render;

namespace Civs.Render.Nations;
public sealed class PopulationCenterTileRenderBehaviour : DependentRenderBehaviourBase<PopulationCenterArchetype>, IInitializable {

	private PopulationCenterTileGroupSceneInstance _sceneInstance = null!;
	private bool _needsMeshUpdate = false;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
	}

	protected override void OnArchetypeSet() {
		base.OnArchetypeSet();
		Archetype.TileOwnership.ComponentChanged += OnTileOwnershipChanged;
	}

	private void OnTileOwnershipChanged( ComponentBase component ) {
		_needsMeshUpdate = true;
	}

	public void Initialize() {
		_sceneInstance = RenderEntity.RequestSceneInstance<PopulationCenterTileGroupSceneInstance>( RenderConstants.GameObjectSceneName, 0 );
		_sceneInstance.SetShaderBundle( RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<GlobeTerrainShaderBundle>() );
		_sceneInstance.SetVertexArrayObject( RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity3SceneData>() );
		_sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
		_needsMeshUpdate = true;
	}

	public override void Update( double time, double deltaTime ) {
		//TODO: properly separate logic and render in all components. If an update happens which should be rendered occurs, it's the game logic's (ecs) responsibility to serialize and ship the new components to the render system. This means behaviours can in reality only depend on serializable components.
		if (_sceneInstance.Allocated && _needsMeshUpdate) {
			var parent = this.Archetype.Entity.Parent;
			_sceneInstance.UpdateMesh( [ .. this.Archetype.TileOwnership.OwnedFaces ], RenderEntity.ServiceAccess.MeshProvider, parent?.GetComponentOrDefault<PlayerComponent>()?.MapColor ?? 1 );
			_sceneInstance.Write( new Entity3SceneData( Matrix4x4<float>.MultiplicativeIdentity, ushort.MaxValue ) );
			_needsMeshUpdate = false;
		}
	}

	protected override bool InternalDispose() {
		Archetype.TileOwnership.ComponentChanged -= OnTileOwnershipChanged;
		return true;
	}
}

public sealed class PopulationCenterTileGroupSceneInstance() : SceneInstanceBase( typeof( Entity3SceneData ) ) {
	public void UpdateMesh( IReadOnlyList<Face> faces, MeshProvider meshProvider, Vector4<float>? overrideColor = null ) {
		Mesh?.Dispose();
		SetMesh( CreateMesh( faces, meshProvider, overrideColor ) );
	}

	private IMesh CreateMesh( IReadOnlyList<Face> faces, MeshProvider meshProvider, Vector4<float>? overrideColor ) {
		List<Vertex3> vertices = [];
		List<uint> indices = [];

		List<Face> borderFaces = [];
			Span<uint> vertexIndices = stackalloc uint[ 3 ];
		//Border faces are all faces where not all vertices are shared with other faces in the faces list.
		for (int i = 0; i < faces.Count; i++) {
			var face = faces[ i ];
			FaceIndices faceIndices = face.Blueprint.Indices;
			for (int j = 0; j < 3; j++) {
				face.Blueprint.Connections[i].GetOther(face).Blueprint.Indices.GetVertexIndex( faceIndices[ j ], out uint otherFaceIndex, out uint otherVertexIndex );
				vertexIndices[ j ] = faceIndices[ j ];
			}
		}

		for (int i = 0; i < faces.Count; i++) {
			var face = faces[ i ];
			Vector3<float> a = face.Blueprint.VertexA;
			Vector3<float> b = face.Blueprint.VertexB;
			Vector3<float> c = face.Blueprint.VertexC;
			Vector3<float> abCenter = (a + b) / 2f;
			Vector3<float> acCenter = (a + c) / 2f;
			Vector3<float> bcCenter = (b + c) / 2f;
			Vector3<float> center = (a + b + c) / 3f;
			Vector4<byte> color = ((overrideColor ?? face.State.TerrainType.Color) * 255)
				.Clamp<Vector4<float>, float>( 0, 255 )
				.CastSaturating<float, byte>();

			//Create mesh
			uint startIndex = (uint) vertices.Count;
			vertices.Add( new( a, 0, 0, color ) );
			vertices.Add( new( b, 0, 0, color ) );
			vertices.Add( new( c, 0, 0, color ) );
			vertices.Add( new( abCenter, 0, 0, (color.X, color.Y, color.Z, 25) ) );
			vertices.Add( new( center, 0, 0, (color.X, color.Y, color.Z, 25) ) );
			indices.Add( startIndex );
			indices.Add( startIndex + 1 );
			indices.Add( startIndex + 3 );
			indices.Add( startIndex + 1 );
			indices.Add( startIndex + 2 );
			indices.Add( startIndex + 3 );
			indices.Add( startIndex + 2 );
			indices.Add( startIndex );
			indices.Add( startIndex + 3 );

		}

		return meshProvider.CreateMesh( vertices.ToArray(), [ .. indices ] );
	}
	public new void SetAllocated( bool allocated ) => base.SetAllocated( allocated );
	public new void SetVertexArrayObject( OglVertexArrayObjectBase? vertexArrayObject ) => base.SetVertexArrayObject( vertexArrayObject );
	public new void SetShaderBundle( ShaderBundleBase? shaderBundle ) => base.SetShaderBundle( shaderBundle );

	public bool Write( Entity3SceneData data ) => Write<Entity3SceneData>( data );
	public bool TryRead( out Entity3SceneData data ) => TryRead<Entity3SceneData>( out data );

	protected override void Initialize() {

	}
}