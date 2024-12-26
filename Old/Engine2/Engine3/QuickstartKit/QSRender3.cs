using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Cameras.Projections;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Lights.D3;
using Engine.Graphics.Objects.Default.Pipelines;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Scenes;
using Engine.LinearAlgebra;
using Engine.Pipelines;
using Engine.Utilities.Time;

namespace Engine.QuickstartKit {
	public class QSRender3 {

		public QuickstartClient Client { get; private set; }
		public Camera3 Camera { get; private set; }
		public Scene3Collection Scenes { get; private set; }
		public InstanceSystemManager<SceneObjectData3> InstanceSystemManager { get; protected set; }
		public LightManager Lights { get; private set; }
		public Pipeline PreRenderPipeline { get; protected set; }
		public RenderPipeline3 RenderingPipeline { get; private set; }

		public QSRender3( QuickstartClient client, Vector3 ambientColor, Clock32 clock, float fov, float zNear = 0.015625f, float zFar = 1024f ) {
			Client = client;
			Camera = new Camera3( new Perspective.Dynamic( "Camera Projection", Client.Window, fov, zNear, zFar ), new Graphics.Objects.Default.Transforms.Transform3() );
			Scenes = new Scene3Collection( Client.Window );
			Lights = new LightManager( Scenes );

			InstanceSystemManager = new InstanceSystemManager<SceneObjectData3>();

			PreRenderPipeline = new Pipeline( "Pre-Render Pipeline 3D" );
			PreRenderPipeline.InsertLast( new Junction( "Update Camera", delegate () { Camera.UpdateMatrices(); } ) );
			PreRenderPipeline.InsertLast( new Junction( "Update Instance Data", delegate () { InstanceSystemManager.Update(); } ) );
			RenderingPipeline = new RenderPipeline3( Client.Window, Camera, Scenes, Lights );//( Client.Window, Camera, Scenes, SceneWater, Particles, Lights, clock );

		}
	}

	public class Scene3Collection {

		public SceneMeshMaterial<SceneObjectData3> Entity { get; private set; }
		public SceneMeshMaterial<SceneObjectData3> Transparency { get; private set; }
		public SceneMaterialMesh<SceneObjectData3> Particle { get; private set; }

		public Scene3Collection( GLWindow window ) {
			Entity = new SceneMeshMaterial<SceneObjectData3>( "Scene3D Entity", window );
			Transparency = new SceneMeshMaterial<SceneObjectData3>( "Scene3D Transparency", window );
			Particle = new SceneMaterialMesh<SceneObjectData3>( "Scene3D Particles", window );
		}
	}
}