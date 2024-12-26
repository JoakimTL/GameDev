using Engine.LinearAlgebra;
using Engine.Pipelines;
using Engine.Pipelines.Default;
using Engine.Utilities.Time;

namespace Engine.QuickstartKit {
	public abstract class QuickstartClientRender : QuickstartClient {

		private readonly QS3Data render3Data;
		private readonly QS2Data render2Data;

		public QSRender3 Render3 { get; private set; }
		public QSRender2 Render2 { get; private set; }

		public Pipeline StandardPipeline { get; private set; }
		protected Pipeline preRenderPipeline;
		protected Pipeline renderingPipeline;
		protected Pipeline postRenderPipeline;

		public QuickstartClientRender( QSWinData windowData, QS3Data render3Data, QS2Data render2Data ) : base( delegate () { return MemLib.Mem.Windows.CreateWindow( windowData.windowName, windowData.resolution.X, windowData.resolution.Y ); }, windowData.vsyncLevel, windowData.sampleCount ) {
			this.render2Data = render2Data;
			this.render3Data = render3Data;
		}

		public override void Entry() {
			StandardPipeline = CreateStandardPipeline( out preRenderPipeline, out renderingPipeline, out postRenderPipeline );

			if( !( render3Data is null ) ) {
				Render3 = new QSRender3( this, render3Data.ambientColor, render3Data.clock, render3Data.fov, render3Data.zNear, render3Data.zFar );
				preRenderPipeline.InsertLast( new JunctionPipelineExecutor( "Pre-Render 3D", Render3.PreRenderPipeline ) );
				renderingPipeline.InsertLast( new JunctionPipelineExecutor( "Render Scene3D", Render3.RenderingPipeline ) );
			}

			if( !( render2Data is null ) ) {
				Render2 = new QSRender2( this, render2Data.uiClock, render2Data.frameTimer );
				preRenderPipeline.InsertLast( new JunctionPipelineExecutor( "Pre-Render 2D", Render2.PreRenderPipeline ) );
				renderingPipeline.InsertLast( new JunctionPipelineExecutor( "Render Scene2D", Render2.RenderingPipeline ) );
			}

			Initialization();

			while( ShouldRun() )
				InLoop();

			Exit();
		}

		protected virtual bool ShouldRun() {
			return !Window.ShouldClose && Running;
		}

		protected abstract void Initialization();
		protected abstract void InLoop();
		protected abstract void Exit();

	}

	public class QS2Data {
		public Clock32 uiClock = Clock32.Standard;
		public bool frameTimer = false;
	}

	public class QS3Data {
		public Vector3 ambientColor = new Vector3( 0, 0, 0 );
		public Clock32 clock = Clock32.Standard;
		public float fov = 90;
		public float zNear = 0.015625f;
		public float zFar = 256;
	}

	public class QSWinData {
		public string windowName = "untitled window";
		public Vector2i resolution = new Vector2i( 800, 600 );
		public int vsyncLevel = 1;
		public int sampleCount = 60;

		public QSWinData() { }

		public QSWinData( string windowName ) {
			this.windowName = windowName;
		}

		public QSWinData( string windowName, Vector2i resolution ) {
			this.windowName = windowName;
			this.resolution = resolution;
		}
	}
}
