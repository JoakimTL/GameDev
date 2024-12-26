using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Junctions;
using Engine.MemLib;
using Engine.Networking;
using Engine.Pipelines;
using Engine.Pipelines.Default;
using Engine.Utilities.Graphics.Disposables;
using Engine.Utilities.Graphics.Utilities;
using Engine.Utilities.Time;
using OpenGL;
using System.Globalization;
using System.Threading;

namespace Engine.QuickstartKit {
	public abstract class QuickstartClient : Quickstart {

		public delegate GLWindow WindowCreationHandler();
		private readonly WindowCreationHandler windowCreation;
		private readonly int vsyncLevel;
		public override bool Running { get; protected set; }
		public Sampler32 FrameTimeSampler { get; private set; }

		public GLWindow Window {
			get; protected set;
		}

		public QuickstartClient( WindowCreationHandler windowCreation, int vsyncLevel, int sampleCount = 60 ) {
			this.windowCreation = windowCreation;
			this.vsyncLevel = vsyncLevel;
			FrameTimeSampler = new Sampler32( new Watch32( Clock32.Standard ), sampleCount );
		}

		public override void Run() {

			Running = true;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			PacketManager.ScanPacketTypes();
			GLUtil.InitializeGL();

			Window = windowCreation.Invoke();

			Mem.PostGLInit();
			GLUtil.SetVSync( vsyncLevel );

			Entry();

			Mem.Dispose();
			GLUtil.Terminate();
			Running = false;

		}

		public abstract void EventsPolled();

		/// <summary>
		/// Creates a complete pipeline to execute in the while loop. The pipeline alone 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="internalPipeline"></param>
		/// <returns></returns>
		public Pipeline CreateStandardPipeline( out Pipeline preRenderPipeline, out Pipeline renderPipeline, out Pipeline postRenderPipeline ) {
			Pipeline p = new Pipeline( "OGL Pipeline" );
			preRenderPipeline = new Pipeline( "Pre-Rendering Pipeline" );
			renderPipeline = new Pipeline( "Rendering Pipeline" );
			postRenderPipeline = new Pipeline( "Post-Rendering Pipeline" );

			p.InsertLast( new Junction( "Polling Events", delegate () { GLUtil.PollEvents(); EventsPolled(); } ) );
			p.InsertLast( new Junction( "Disposing Unused Objects", delegate () { OGLDisposable.DisposeAmount( -1 ); } ) );
			p.InsertLast( new JunctionClearSceneBuffer( "Clear Buffer", ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit, new LinearAlgebra.Vector4( 0, 0, 0, 1 ) ) );
			p.InsertLast( new JunctionPipelineExecutor( "Execute Prerender Pipeline", preRenderPipeline ) );
			p.InsertLast( new JunctionPipelineExecutor( "Execute Rendering Pipeline", renderPipeline ) );
			p.InsertLast( new JunctionPipelineExecutor( "Execute Rendering Pipeline", postRenderPipeline ) );
			p.InsertLast( new Junction( "Check Errors", delegate () { GLUtil.CheckError( "Main Loop" ); } ) );
			p.InsertLast( new JunctionSwapBuffers( "Swap Buffers", Window ) );
			p.InsertLast( new Junction( "Register Frame Time", delegate () { FrameTimeSampler.Record(); } ) );

			return p;
		}

	}
}
