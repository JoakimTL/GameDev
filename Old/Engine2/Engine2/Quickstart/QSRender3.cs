using Engine.Graphics.Objects.Default.Cameras.Projections;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Scenes;
using Engine.LMath;
using Engine.Utilities.Time;

namespace Engine.QuickstartKit {
	public class QSRender3 {

		public QuickstartClient Client { get; private set; }
		public View3 Camera { get; set; }
		public SceneMeshMaterial Scene { get; set; }
		/*public Light3Manager Lights { get; set; }
		public SceneCollection<Camera3View> Scenes { get; set; }
		public PipelineDefault3 RenderingPipeline { get; set; }*/

		public QSRender3( QuickstartClient client, Vector3 ambientColor, Clock32 clock, float fov, float zNear = 0.015625f, float zFar = 1024f ) {
			Client = client;
			Camera = new View3( new Perspective.Dynamic( "Camera Projection", Client.Window, fov, zNear, zFar ), new Graphics.Objects.Default.Transforms.Transform3() );
			Scene = new SceneMeshMaterial( "Scene3D Entity", Client.Window );

			/*Lights = new Light3Manager( ambientColor, Camera, Scenes, Client.Window );

			RenderingPipeline = new PipelineDefault3( Client.Window, Camera, Scenes, SceneWater, Particles, Lights, clock );
			*/
		}
	}
}