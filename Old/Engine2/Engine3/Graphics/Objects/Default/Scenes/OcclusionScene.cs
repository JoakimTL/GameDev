using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.MemLib;
using Engine.Utilities.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Engine.Graphics.Objects.Default.Scenes {
	public abstract class OcclusionScene<T, R, V> : SceneRenderer<V> where T : Transform where R : SceneObject<V> where V : SceneObjectData {
		//Scans the scene and when activated deactivates and activates renderables

		//Scans the scene and contains an essentially mini scene.

		//Generates a scene based on a collection of RENDERABLES, which utilize the Transform class, which allows for more efficient cpu usage.
		//Hook the occ scene onto a scene, which then adds and removes based on events from the hooked scene.

		public string Name { get; private set; }
		public bool Active { get; private set; }
		protected Scene<V> scene;
		protected HashSet<R> renderables;
		protected HashSet<R> occlusionSet;
		protected ConcurrentQueue<R> transformChangeQueue;
		protected T transform;
		private bool listChanged, occlusionChanged;
		private readonly AutoResetEvent occlusionWait;
		private readonly ManualResetEvent renderWait;
		private Sampler32 sampler = new Sampler32( new Watch32( Clock32.Standard ) );
		public double time { get => sampler.GetAverageMillis(); }

		public event Action Changed;

		public int InBound {
			get {
				lock( occlusionSet )
					return occlusionSet.Count;
			}
		}

		public OcclusionScene( string name, Scene<V> scene, T occlusionTransform ) {
			this.scene = scene;
			this.transform = occlusionTransform;
			scene.SortedEvent += SceneSorted;
			Name = name;
			Active = true;
			renderables = new HashSet<R>();
			occlusionSet = new HashSet<R>();
			transformChangeQueue = new ConcurrentQueue<R>();
			occlusionWait = new AutoResetEvent( false );
			renderWait = new ManualResetEvent( false );
			occlusionTransform.OnChangedEvent += OcclusionChanged;
			occlusionChanged = false;
			listChanged = false;
			Mem.Threads.StartNew( OcclusionThread, $"Occlusion Thread[{name}]" );
		}

		protected void RenderableChange( R r ) {
			transformChangeQueue.Enqueue( r );
			occlusionWait.Set();
		}

		private void OcclusionChanged() {
			occlusionChanged = true;
			occlusionWait.Set();
		}

		private void SceneSorted() {
			listChanged = true;
			occlusionWait.Set();
		}

		public override void Render( IView view, uint shaderUsecase = 0, RenderMethod<V> renderOverride = null, Material materialOverride = null ) {
			if( scene.Count == 0 )
				return;

			ShaderBundle currentBundle = null;
			Shader currentShader = null;
			Material currentMaterial = materialOverride;
			Mesh currentMesh = null;

			bool isOverridingRendering = !( renderOverride is null );
			bool isOverridingMaterial = !( materialOverride is null );

			if( isOverridingMaterial )
				currentMaterial.Bind();

			scene.SortWait.WaitOne();
			renderWait.WaitOne();
			lock( occlusionSet ) {
				foreach( R r in occlusionSet ) {
					if( !r.Active )
						continue;

					bool shaderChanged = BindShader( r, ref currentBundle, ref currentShader, shaderUsecase, out bool cancel );
					if( cancel ) 
						continue;

					bool materialChanged = false;
					if( !isOverridingMaterial )
						materialChanged = BindMaterial( r, ref currentMaterial );

					bool meshChanged = BindMesh( r, ref currentMesh );

					if( shaderChanged ) {
						currentMaterial.BindShader( currentShader );
						currentMesh.BindShader( currentShader );
					} else {
						if( meshChanged )
							currentMesh.BindShader( currentShader );
						if( materialChanged )
							currentMaterial.BindShader( currentShader );
					}

					r.PreRender();
					if( isOverridingRendering ) {
						renderOverride.Invoke( r, currentShader, view );
					} else {
						r.RenderFunction.Invoke( r, currentShader, view );
					}
					r.PostRender();
				}
			}

			if( !( currentMaterial is null ) )
				currentMaterial.Unbind();
			if( !( currentShader is null ) )
				currentShader.Unbind();
			if( !( currentMesh is null ) )
				currentMesh.Unbind();
		}

		/*public override void Render( uint shaderUsecase = 0, RenderMethod<V> renderOverride = null, Material materialOverride = null ) {
			if( scene.Count == 0 )
				return;

			ShaderBundle currentBundle = null;
			Shader currentShader = null;
			Material currentMaterial = materialOverride;
			Mesh currentMesh = null;

			bool isOverridingMaterial = !( materialOverride is null );

			if( isOverridingMaterial )
				currentMaterial.Bind();

			currentShader.Bind();

			scene.SortWait.WaitOne();
			renderWait.WaitOne();
			lock( occlusionSet ) {
				foreach( T r in occlusionSet ) {
					if( !r.Active )
						continue;

					bool meshChanged = BindMesh( r, ref currentMesh );

					bool materialChanged = false;
					if( !isOverridingMaterial )
						materialChanged = BindMaterial( r, ref currentMaterial );

					if( meshChanged )
						currentMesh.BindShader( currentShader );
					if( materialChanged )
						currentMaterial.BindShader( currentShader );

					r.PreRender();
					renderMethod.Invoke( r, currentShader );
					r.PostRender();
				}
			}

			if( !( currentMaterial is null ) )
				currentMaterial.Unbind();
			if( !( currentShader is null ) )
				currentShader.Unbind();
			if( !( currentMesh is null ) )
				currentMesh.Unbind();
		}*/

		private bool BindShader( SceneObject<V> r, ref ShaderBundle currentBundle, ref Shader c, uint use, out bool cancel ) {
			cancel = false;
			ShaderBundle nSb = r.ShaderBundle;
			if( nSb == currentBundle )
				return false;
			currentBundle = nSb;
			Shader nS = currentBundle.Get( use );
			if( nS is null ) {
				cancel = true;
				return false;
			}
			if( nS == c )
				return false;
			if( !( c is null ) )
				c.Unbind();
			c = nS;
			c.Bind();
			return true;
		}

		private bool BindMaterial( R r, ref Material c ) {
			if( r.Material == c )
				return false;
			if( !( c is null ) )
				c.Unbind();
			c = r.Material;
			c.Bind();
			return true;
		}

		private bool BindMesh( R r, ref Mesh c ) {
			if( r.Mesh == c )
				return false;
			if( !( c is null ) )
				c.Unbind();
			c = r.Mesh;
			c.Bind();
			return true;
		}

		private void OcclusionThread() {
			while( Mem.Threads.Running && Active ) {
				occlusionWait.WaitOne();
				renderWait.Reset();
				sampler.Watch.Zero();
				if( listChanged ) {
					occlusionChanged = false;
					listChanged = false;
					transformChangeQueue.Clear();
					ListChangeHandling();
					OccludeAll();
				} else if( occlusionChanged ) {
					occlusionChanged = false;
					transformChangeQueue.Clear();
					OccludeAll();
				} else {
					OccludeQueue();
				}
				sampler.Record();
				Changed?.Invoke();
				renderWait.Set();
			}
		}

		protected abstract bool CheckOcclusion( R r );
		protected abstract void RemoveTracking( R r );
		protected abstract void AddTracking( R r );


		private void OccludeQueue() {
			lock( occlusionSet ) {
				while( transformChangeQueue.TryDequeue( out R r ) ) {
					bool c = CheckOcclusion( r );
					if( occlusionSet.Contains( r ) ) {
						if( !c )
							occlusionSet.Remove( r );
					} else {
						if( c )
							occlusionSet.Add( r );
					}
				}
			}
		}

		private void OccludeAll() {
			lock( occlusionSet ) {
				occlusionSet.Clear();
				foreach( R r in renderables ) {
					if( CheckOcclusion( r ) )
						occlusionSet.Add( r );
				}
			}
		}

		public void ListChangeHandling() {
			listChanged = false;
			lock( renderables ) {
				foreach( R r in renderables )
					RemoveTracking( r );
				renderables.Clear();
				IReadOnlyList<SceneObject<V>> sos = scene.SceneObjects;
				for( int i = 0; i < sos.Count; i++ ) {
					if( sos[ i ] is R r ) {
						renderables.Add( r );
						AddTracking( r );
					}
				}
			}
		}

		public string GetString() {
			StringBuilder sb = new StringBuilder();

			sb.Append( "Occlusion Scene Data:\r\n" );
			sb.Append( "In List:\r\n" );

			lock( renderables ) {
				foreach( R r in renderables ) {
					sb.Append( r.ToString() );
					sb.Append( "\r\n" );
				}
			}

			sb.Append( "In Bound:\r\n" );
			lock( occlusionSet ) {
				foreach( R r in occlusionSet ) {
					sb.Append( r.ToString() );
					sb.Append( "\r\n" );
				}
			}

			return sb.ToString();
		}

	}
}
