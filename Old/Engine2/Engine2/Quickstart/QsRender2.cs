using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Scenes;
using Engine.MemLib;
using Engine.Utilities.Time;

namespace Engine.QuickstartKit {
	public class QSRender2 {

		public QuickstartClient Client { get; private set; }
		public View2 Camera { get; set; }
		public SceneMeshMaterial SceneUI { get; set; }
		public SceneMeshMaterial SceneEntity { get; set; }
		/*public UIManager UIManager { get; set; }
		public PipelineDefault2 RenderingPipeline { get; set; }*/

		public QSRender2( QuickstartClient client, Clock32 uiClock ) {
			Client = client;
			Camera = new View2( client.Window, 1, 1, 2, 0 );
			SceneUI = new SceneMeshMaterial( "Scene 2D Entity", client.Window );
			SceneEntity = new SceneMeshMaterial( "Scene 2D Entity", client.Window );
			/*RenderingPipeline = new PipelineDefault2( Client.Window, SceneEntity, ParticlesStatic, ParticlesMoving, Camera );
			UIManager = new UIManager( SceneEntity, Client.Window, uiClock );*/
		}

	}
}