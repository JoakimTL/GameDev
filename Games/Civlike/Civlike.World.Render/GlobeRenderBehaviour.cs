using Civlike.World.Components;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Providers;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Civlike.World.TectonicGeneration;
public sealed class GlobeRenderBehaviour : DependentRenderBehaviourBase<GlobeArchetype>, IInitializable {

	private readonly List<BoundedRenderClusterRenderer> _clusterRenderers;

	public GlobeRenderBehaviour() {
		_clusterRenderers = [];
	}

	public void Initialize() {
		foreach ( var cluster in Archetype.GlobeComponent.Globe.Model.Clusters)
			_clusterRenderers.Add( new BoundedRenderClusterRenderer( cluster ) );
	}

	public override void Update( double time, double deltaTime ) {
		foreach ( var renderer in _clusterRenderers )
			renderer.Update( time, deltaTime );
	}

	protected override bool InternalDispose() {
		return true;
	}
}

public sealed class BoundedRenderClusterRenderer : DisposableIdentifiable {

	public BoundedRenderClusterRenderer( BoundedRenderCluster cluster ) {
		this.Cluster = cluster;
	}

	public BoundedRenderCluster Cluster { get; }

	public void Update( double time, double deltaTime ) {

	}

	protected override bool InternalDispose() {
		return true;
	}
}

public sealed class TileGroupSceneInstance() : SceneInstanceBase( typeof( Entity3SceneData ) ) {
	public void UpdateMesh( Globe globe, IReadOnlyList<ReadOnlyFace> faces, MeshProvider meshProvider, Vector4<float>? overrideColor = null ) {
		this.Mesh?.Dispose();
		SetMesh( CreateMesh( globe, faces, meshProvider, overrideColor ) );
	}

	private static IMesh CreateMesh( Globe globe, IReadOnlyList<ReadOnlyFace> faces, MeshProvider meshProvider, Vector4<float>? overrideColor ) {
		List<Vertex3> vertices = [];
		List<uint> indices = [];
		float globeRadius = (float) globe.Radius;
		for (int i = 0; i < faces.Count; i++) {
			ReadOnlyFace face = faces[ i ];
			ReadOnlyVertex vertexA = face.Vertices[ 0 ];
			ReadOnlyVertex vertexB = face.Vertices[ 1 ];
			ReadOnlyVertex vertexC = face.Vertices[ 2 ];
			Vector3<float> a = vertexA.Vector/* * ((vertexA.Height + globeRadius) / globeRadius)*/;
			Vector3<float> b = vertexB.Vector/* * ((vertexB.Height + globeRadius) / globeRadius)*/;
			Vector3<float> c = vertexC.Vector/* * ((vertexC.Height + globeRadius) / globeRadius)*/;
			Vector4<byte> color = new Vector4<float>( 255, 255, 255, 255 ) /* ((overrideColor ?? face.State /*.TerrainType* /.Color) * 255)*/
				.Clamp<Vector4<float>, float>( 0, 255 )
				.CastSaturating<float, byte>();

			//Create mesh
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( a, 0, 0, color ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( b, 0, 0, color ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( c, 0, 0, color ) );

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