using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.MemLib;
using Engine.Utilities.Data;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Time;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Engine.Graphics.Objects {
	public abstract class SceneRenderer<T> where T : SceneObjectData {
		public abstract void Render( IView view, uint shaderUsecase = 0, RenderMethod<T> renderOverride = null, Material materialOverride = null );
	}

	public abstract class Scene<T> : SceneRenderer<T> where T : SceneObjectData {

		/*
		 * INSTANCE EVERY FUCKING THING
		 * 
		 * 1, 100, 1000000.
		 * DOESN'T MATTER
		 * INSTANCING
		 */

		public string Name { get; private set; }

		private readonly HashSet<SceneObject<T>> sceneObjectSet;
		private readonly Dictionary<uint, List<SceneObject<T>>> layeredSceneObjects;
		private readonly List<SceneObject<T>> sceneObjects;
		public IReadOnlyList<SceneObject<T>> SceneObjects { get => sceneObjects; }
		public int Count { get => sceneObjectSet.Count; }
		private readonly List<uint> layers;
		private readonly UniqueQueue<uint> changedLayers;
		private Sampler32 sortTimeSampler;

		private volatile bool layersChanged;
		private readonly AutoResetEvent sortInitWait;
		public WaitHandle SortInitWait { get => sortInitWait; }
		private readonly ManualResetEvent sortWait;
		public WaitHandle SortWait { get => sortWait; }

		public event Action SortedEvent;

		public Scene( string name, GLWindow window ) {
			Name = name;
			sceneObjectSet = new HashSet<SceneObject<T>>();
			layeredSceneObjects = new Dictionary<uint, List<SceneObject<T>>>();
			sceneObjects = new List<SceneObject<T>>();
			layers = new List<uint>();
			changedLayers = new UniqueQueue<uint>();
			sortInitWait = new AutoResetEvent( false );
			sortWait = new ManualResetEvent( true );
			layersChanged = false;
			sortTimeSampler = new Sampler32( new Watch32( Clock32.Standard ) );
			window.SwapBufferEvent += InitSort;
			Mem.Threads.StartNew( SortingThread, $"Scene[{Name}] Sorter" );
		}

		internal void LayerChanged( uint layer ) {
			lock( changedLayers )
				changedLayers.Enqueue( layer );
		}

		public void Add( SceneObject<T> r ) {
			if( r is null ) {
				Logging.Warning( "Tried to add a null member" );
				return;
			}
			lock( changedLayers )
				lock( layers )
					lock( layeredSceneObjects ) {
						if( sceneObjectSet.Add( r ) ) {
							if( !layeredSceneObjects.TryGetValue( r.Layer, out List<SceneObject<T>> layer ) ) {
								layeredSceneObjects.Add( r.Layer, layer = new List<SceneObject<T>>() );
								layers.Add( r.Layer );
								layersChanged = true;
							}
							layer.Add( r );
							changedLayers.Enqueue( r.Layer );
							r.AddToScene( this );
						}
					}
		}

		public void Remove( SceneObject<T> r ) {
			lock( changedLayers )
				lock( layers )
					lock( layeredSceneObjects ) {
						if( sceneObjectSet.Remove( r ) ) {
							if( layeredSceneObjects.TryGetValue( r.Layer, out List<SceneObject<T>> layer ) ) {
								layer.Remove( r );
								r.RemoveFromScene( this );
								if( layer.Count == 0 ) {
									layeredSceneObjects.Remove( r.Layer );
									layersChanged = true;
								} else {
									changedLayers.Enqueue( r.Layer );
								}
							}
						}
					}
		}

		public override void Render( IView view, uint shaderUsecase = 0, RenderMethod<T> renderOverride = null, Material materialOverride = null ) {
			ShaderBundle currentBundle = null;
			Shader currentShader = null;
			Material currentMaterial = materialOverride;
			Mesh currentMesh = null;

			bool isOverridingRendering = !( renderOverride is null );
			bool isOverridingMaterial = !( materialOverride is null );

			if( isOverridingMaterial )
				currentMaterial.Bind();

			sortWait.WaitOne();
			lock( sceneObjects ) {
				int count = sceneObjects.Count;
				for( int i = 0; i < count; i++ ) {
					SceneObject<T> r = sceneObjects[ i ];
					if( !r.Active ) //can be put into sorting method.
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

		private bool BindShader( SceneObject<T> r, ref ShaderBundle currentBundle, ref Shader c, uint use, out bool cancel ) {
			cancel = false;
			ShaderBundle nSb = r.ShaderBundle;
			if( nSb == currentBundle )
				return false;
			currentBundle = nSb;
			Shader nS = currentBundle.Get( use );
			if (nS is null ) {
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

		private bool BindMaterial( SceneObject<T> r, ref Material c ) {
			if( r.Material == c )
				return false;
			if( !( c is null ) )
				c.Unbind();
			c = r.Material;
			c.Bind();
			return true;
		}

		private bool BindMesh( SceneObject<T> r, ref Mesh c ) {
			if( r.Mesh == c )
				return false;
			if( !( c is null ) )
				c.Unbind();
			c = r.Mesh;
			c.Bind();
			return true;
		}

		private void InitSort() {
			sortInitWait.Set();
		}

		private void SortingThread() {
			while( Mem.Threads.Running ) {
				sortInitWait.WaitOne();
				Sort();
			}
		}

		public void Sort() {
			sortWait.Reset();
			lock( layers )
				lock( layeredSceneObjects )
					lock( sceneObjects ) {
						bool changed = layersChanged || changedLayers.Count > 0;
						sortTimeSampler.Watch.Zero();
						if( layersChanged ) {
							layers.Sort();
							layersChanged = false;
						}

						while( changedLayers.TryDequeue( out uint layer ) )
							layeredSceneObjects[ layer ].Sort( Comparator );

						if( changed ) {
							sceneObjects.Clear();
							foreach( uint layer in layers )
								foreach( SceneObject<T> so in layeredSceneObjects[ layer ] )
									if( so.Sum.IsValid() ) {
										if( !( so.RenderFunction is null ) ) {
											sceneObjects.Add( so );
										}
#if DEBUG
										else {
											if( so.Warn ) {
												Logging.Warning( $"Scene object {so.ID} has no set RenderFunction." );
												so.Warn = false;
											}
										}
#endif
									}
							sortTimeSampler.Record();
							SortedEvent?.Invoke();
						}
					}
			sortWait.Set();
		}

		protected abstract int Comparator( SceneObject<T> a, SceneObject<T> b );

		public string GetRawString() {
			StringBuilder sb = new StringBuilder();

			sb.Append( "Scene Data:\r\n" );

			lock( layeredSceneObjects ) {
				foreach( var l in layeredSceneObjects ) {
					foreach( var r in l.Value ) {
						sb.Append( r.ToString() );
						sb.Append( "\r\n" );
					}
				}
			}

			return sb.ToString();
		}

		public string GetSortedString() {
			StringBuilder sb = new StringBuilder();

			sb.Append( "Scene Data:\r\n" );

			lock( sceneObjects ) {
				foreach( SceneObject<T> r in sceneObjects ) {
					sb.Append( r.ToString() );
					sb.Append( "\r\n" );
				}
			}

			return sb.ToString();
		}
	}
}
