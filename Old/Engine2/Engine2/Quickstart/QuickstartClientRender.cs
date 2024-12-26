using Engine.LMath;
using Engine.Pipelines;
using Engine.Pipelines.Default;
using Engine.Utilities.Time;

namespace Engine.QuickstartKit {
	public abstract class QuickstartClientRender : QuickstartClient {

		private readonly QS3Data render3Data;
		private readonly QS2Data render2Data;

		public QSRender3 Render3 { get; private set; }
		public QSRender2 Render2 { get; private set; }

		public Pipeline StandardPipeline { get; set; }
		protected Pipeline internalPipeline;

		public QuickstartClientRender( QSWinData windowData, QS3Data render3Data, QS2Data render2Data ) : base( delegate () { return MemLib.Mem.Windows.CreateWindow( windowData.windowName, windowData.resolution.X, windowData.resolution.Y ); }, windowData.vsyncLevel ) {
			this.render2Data = render2Data;
			this.render3Data = render3Data;
		}

		public override void Entry() {
			StandardPipeline = CreateStandardPipeline( out internalPipeline );

			if( !( render3Data is null ) ) {
				Render3 = new QSRender3( this, render3Data.ambientColor, render3Data.clock, render3Data.fov, render3Data.zNear, render3Data.zFar );
				//internalPipeline.InsertLast( new JunctionPipelineExecutor( "Render Scene3D", Render3.RenderingPipeline ) );
			}

			if( !( render2Data is null ) ) {
				Render2 = new QSRender2( this, render2Data.uiClock );
				//internalPipeline.InsertLast( new JunctionPipelineExecutor( "Render Scene2D", Render2.RenderingPipeline ) );
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
		public int maxStaticParticles = 256;
		public int maxMovingParticles = 256;
	}

	public class QS3Data {
		public Vector3 ambientColor = new Vector3( 0, 0, 0 );
		public Clock32 clock = Clock32.Standard;
		public float fov = 90;
		public float zNear = 0.015625f;
		public float zFar = 1024f;
	}

	public class QSWinData {
		public string windowName = "untitled window";
		public Vector2i resolution = new Vector2i( 800, 600 );
		public int vsyncLevel = 1;

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
