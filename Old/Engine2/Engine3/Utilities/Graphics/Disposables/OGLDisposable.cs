using Engine.Utilities.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.Graphics.Disposables {
	public abstract class OGLDisposable : IDisposable, IIdentifiable {

		private static readonly ConcurrentQueue<OGLDisposable> disposeQueue = new ConcurrentQueue<OGLDisposable>();
		/// <summary>
		/// Disposes a number of objects ready for disposal.
		/// </summary>
		/// <param name="num">The number of objects to dispose. Use a number below 0 to dispose all</param>
		public static void DisposeAmount( int num ) {
			if( num < 0 ) {
				while( disposeQueue.TryDequeue( out OGLDisposable oglDisp ) ) {
					oglDisp.Dispose();
				}
			} else {
				for( int i = 0; i < num; i++ ) {
					if( disposeQueue.TryDequeue( out OGLDisposable oglDisp ) ) {
						oglDisp.Dispose();
					} else
						break;
				}
			}

		}

		public string Name { get; private set; }

		public OGLDisposable( string name ) {
			Name = name;
			disposeQueue.Enqueue( this );
		}

		public abstract void Dispose();
	}
}
