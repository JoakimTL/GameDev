using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Pipelines;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual;
using Engine.Graphics.Objects.Default.Scenes;
using Engine.MemLib;
using Engine.Pipelines;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Time;
using System;

namespace Engine.QuickstartKit {
	public class QSRender2 {

		public QuickstartClient Client { get; private set; }
		public MutableSinglet<View2> Camera { get; protected set; }
		public SceneMeshMaterial<SceneObjectData2> SceneUI { get; protected set; }
		public SceneMeshMaterial<SceneObjectData2> SceneEntity { get; protected set; }
		public UIManager UIManager { get; protected set; }
		public InstanceSystemManager<SceneObjectData2> InstanceSystemManager { get; protected set; }
		public Pipeline PreRenderPipeline { get; protected set; }
		public RenderPipeline2 RenderingPipeline { get; protected set; }
		public TextLabel FrameTimeDisplay { get; protected set; }
		private string debug;

		public QSRender2( QuickstartClient client, Clock32 uiClock, bool frameTimer ) {
			Client = client;
			Camera = new MutableSinglet<View2>( new View2( client.Window, 1, 1, 2, 0 ), ( View2 n ) => { return !( n is null ); } );
			SceneEntity = new SceneMeshMaterial<SceneObjectData2>( "Scene 2D Entity", client.Window );
			SceneUI = new SceneMeshMaterial<SceneObjectData2>( "Scene 2D UI", client.Window );
			UIManager = new UIManager( SceneUI, Client.Window, uiClock );
			InstanceSystemManager = new InstanceSystemManager<SceneObjectData2>();
			PreRenderPipeline = new Pipeline( "Pre-Render Pipeline 2D" );
			PreRenderPipeline.InsertLast( new Junction( "Update Camera", delegate () { Camera.Value.UpdateMatrices(); } ) );
			PreRenderPipeline.InsertLast( new Junction( "Update UI", delegate () { UIManager.Update(); } ) );
			PreRenderPipeline.InsertLast( new Junction( "Update Instance Data", delegate () { InstanceSystemManager.Update(); } ) );
			RenderingPipeline = new RenderPipeline2( Client.Window, UIManager.UIView, Camera, SceneUI, SceneEntity ); //make views mutable?

			FrameTimeDisplay = new TextLabel() {
				LayerOffset = 2
			};
			FrameTimeDisplay.Constraints.Set(
				new ConstraintBundle(
					new ModTranslationSetAligmentHorizontal( HorizontalAlignment.RIGHT, true ),
					new ModTranslationSetAligmentVertical( VerticalAlignment.TOP, true ),
					new ModScalingSetPixel( 30 )
				)
			);
			FrameTimeDisplay.Attributes.SetAlignment( VerticalAlignment.BOTTOM ).SetAlignment( HorizontalAlignment.RIGHT ).SetMaxLength( 64f );
			FrameTimeDisplay.UpdatedThirdActive += UpdateFrameTimeLabel;
			if( frameTimer )
				UIManager.Add( FrameTimeDisplay, true );
		}

		public void SetDebugString(string debug) {
			this.debug = debug;
		}

		private void UpdateFrameTimeLabel( MouseInputEventData data ) {
			FrameTimeDisplay.Content = $"[{debug}]\n[{string.Format( "{0,6:F2}", Client.FrameTimeSampler.GetAverageMillis() )}]ms::[{string.Format( "{0,6:F2}", Client.FrameTimeSampler.GetAverageInverse() )}]FPS";
		}
	}
}