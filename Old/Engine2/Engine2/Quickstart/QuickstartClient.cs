using Engine.Graphics.Objects;
using Engine.MemLib;
using Engine.Networking;
using Engine.Pipelines;
using Engine.Pipelines.Default;
using Engine.Pipelines.Default.Graphics;
using Engine.Utilities.Graphics.Disposables;
using Engine.Utilities.Graphics.Utilities;
using OpenGL;
using System.Globalization;
using System.Threading;

namespace Engine.QuickstartKit {
	public abstract class QuickstartClient : Quickstart {

		public delegate GLWindow WindowCreationHandler();
		private readonly WindowCreationHandler windowCreation;
		private readonly int vsyncLevel;
		public override bool Running { get; protected set; }

		public GLWindow Window {
			get; protected set;
		}

		public QuickstartClient( WindowCreationHandler windowCreation, int vsyncLevel ) {
			this.windowCreation = windowCreation;
			this.vsyncLevel = vsyncLevel;
		}

		public override void Run() {

			Running = true;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Packets();
			PacketType.ClearAndSetup();
			GLUtil.InitializeGL();

			Window = windowCreation.Invoke();

			Mem.PostGLInit();
			GLUtil.SetVSync( vsyncLevel );

			Entry();

			Mem.Dispose();
			GLUtil.Terminate();
			Running = false;

		}

		/// <summary>
		/// Creates a complete pipeline to execute in the while loop. The pipeline alone 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="internalPipeline"></param>
		/// <returns></returns>
		public Pipeline CreateStandardPipeline( out Pipeline internalPipeline ) {
			Pipeline p = new Pipeline( "OGL Pipeline" );
			internalPipeline = new Pipeline( "Internal Pipeline" );

			p.InsertLast( new Junction( "Polling Events", delegate () { GLUtil.PollEvents(); } ) );
			p.InsertLast( new Junction( "Disposing Unused Objects", delegate () { OGLDisposable.DisposeAmount( -1 ); } ) );
			p.InsertLast( new JunctionClearSceneBuffer( "Clear Buffer", ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, new LMath.Vector4(1, 0, 0, 1) ) );
			p.InsertLast( new JunctionPipelineExecutor( "Execute Internal Pipeline", internalPipeline ) );
			p.InsertLast( new Junction( "Updating Large Mesh", delegate () { Mem.Meshes.Update(); } ) );
			p.InsertLast( new JunctionSwapBuffers( "Swap Buffers", Window ) );

			return p;
		}

	}
}
