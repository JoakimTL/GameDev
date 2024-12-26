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
	public abstract class Scene {
		//render, but use nulls instead of 100 different render overloads
		//use mesh VAO to identify mesh change
		//No individual render handler
		//create equals overrides for shader, mesh and material, using their IDs to make it fast

		public string Name { get; private set; }

		private readonly HashSet<SceneObject> renderablesHashSet;
		private readonly Dictionary<uint, List<SceneObject>> layeredRenderables;
		private readonly HashSet<SceneObject> setupCalls;
		private readonly List<SceneObject> renderables;
		private readonly List<uint> layers;
		private readonly UniqueQueue<uint> changedLayers;

		private volatile bool layersChanged;
		private readonly AutoResetEvent sortInit;
		private readonly ManualResetEvent sortWait;

		public Scene( string name, GLWindow window ) {
			Name = name;
			renderablesHashSet = new HashSet<SceneObject>();
			layeredRenderables = new Dictionary<uint, List<SceneObject>>();
			renderables = new List<SceneObject>();
			setupCalls = new HashSet<SceneObject>();
			layers = new List<uint>();
			changedLayers = new UniqueQueue<uint>();
			sortInit = new AutoResetEvent( false );
			sortWait = new ManualResetEvent( true );
			layersChanged = false;
			window.SwapBufferEvent += InitSort;
			Mem.Threads.StartNew( SortingThread, $"Scene[{Name}] Sorter" );
		}

		internal void LayerChanged( uint layer ) {
			lock( changedLayers )
				changedLayers.Enqueue( layer );
		}

		public void Add( SceneObject r ) {
			lock( changedLayers )
				lock( layers )
					lock( layeredRenderables ) {
						if( renderablesHashSet.Add( r ) ) {
							if( !layeredRenderables.TryGetValue( r.Layer, out List<SceneObject> layer ) ) {
								layeredRenderables.Add( r.Layer, layer = new List<SceneObject>() );
								layers.Add( r.Layer );
								layersChanged = true;
							}
							layer.Add( r );
							if( !( r.RenderSetup is null ) )
								setupCalls.Add( r );
							changedLayers.Enqueue( r.Layer );
							r.AddToScene( this );
						}
					}
		}

		public void Remove( SceneObject r ) {
			lock( changedLayers )
				lock( layers )
					lock( layeredRenderables ) {
						if( renderablesHashSet.Remove( r ) ) {
							if( layeredRenderables.TryGetValue( r.Layer, out List<SceneObject> layer ) ) {
								layer.Remove( r );
								if( !( r.RenderSetup is null ) )
									setupCalls.Remove( r );
								r.RemoveFromScene( this );
								if( layer.Count == 0 ) {
									layeredRenderables.Remove( r.Layer );
									layersChanged = true;
								} else {
									changedLayers.Enqueue( r.Layer );
								}
							}
						}
					}
		}

		private delegate bool BindingHandler<T>( SceneObject r, ref T curShader );

		public void Render( Shader shaderOverride = null, Material materialOverride = null ) {
			foreach( SceneObject so in setupCalls )
				so.RenderSetup?.Invoke();

			Shader currentShader = shaderOverride;
			Material currentMaterial = materialOverride;
			Mesh currentMesh = null;

			BindingHandler<Shader> shaderBinder;
			if( shaderOverride is null )
				shaderBinder = BindShader;
			else
				shaderBinder = NoBindShader;

			BindingHandler<Material> materialBinder;
			if( materialOverride is null )
				materialBinder = BindMaterial;
			else
				materialBinder = NoBindMaterial;

			sortWait.WaitOne();
			lock( renderables ) {
				int count = renderables.Count;
				for( int i = 0; i < count; i++ ) {
					SceneObject r = renderables[ i ];
					if( !r.Active)
						continue;
					if( r.RenderFunction is null ) {
#if DEBUG
						if( r.Warn ) {
							Logging.Warning( $"Scene object {r.ID} has no set RenderFunction." );
							r.Warn = false;
						}
#endif
						continue;
					}

					bool shaderChanged = shaderBinder.Invoke( r, ref currentShader );
					bool materialChanged = materialBinder.Invoke( r, ref currentMaterial );
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
					r.RenderFunction.Invoke();
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

		internal void SetupChanged( bool setupActive, SceneObject obj ) {
			if( setupActive ) {
				setupCalls.Add( obj );
			} else
				setupCalls.Remove( obj );
		}

		private bool NoBindShader( SceneObject r, ref Shader c ) {
			return false;
		}

		private bool BindShader( SceneObject r, ref Shader c ) {
			if( r.Shader == c )
				return false;
			if( !( c is null ) )
				c.Unbind();
			c = r.Shader;
			c.Bind();
			return true;
		}

		private bool NoBindMaterial( SceneObject r, ref Material c ) {
			return false;
		}

		private bool BindMaterial( SceneObject r, ref Material c ) {
			if( r.Material == c )
				return false;
			if( !( c is null ) )
				c.Unbind();
			c = r.Material;
			c.Bind();
			return true;
		}

		private bool BindMesh( SceneObject r, ref Mesh c ) {
			if( r.Mesh == c )
				return false;
			if( !( c is null ) )
				c.Unbind();
			c = r.Mesh;
			c.PreBind();
			c.Bind();
			return true;
		}

		private void InitSort() {
			sortInit.Set();
		}

		private void SortingThread() {
			while( Mem.Threads.Running ) {
				sortInit.WaitOne();
				Sort();
			}
		}

		public void Sort() {
			sortWait.Reset();
			lock( layers )
				lock( layeredRenderables )
					lock( renderables ) {
						bool changed = layersChanged || changedLayers.Count > 0;
						if( layersChanged ) {
							layers.Sort();
							layersChanged = false;
						}

						while( changedLayers.TryDequeue( out uint layer ) ) 
							layeredRenderables[ layer ].Sort( Comparator );

						if( changed ) {
							renderables.Clear();
							foreach( uint layer in layers )
								foreach( SceneObject renderable in layeredRenderables[ layer ] )
									if( renderable.Sum.IsValid() )
										renderables.Add( renderable );
						}
					}
			sortWait.Set();
		}

		protected abstract int Comparator( SceneObject a, SceneObject b );

		public string GetRawString() {
			StringBuilder sb = new StringBuilder();

			sb.Append( "Scene Data:\r\n" );

			lock( layeredRenderables ) {
				foreach( var l in layeredRenderables ) {
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

			lock( renderables ) {
				foreach( SceneObject r in renderables ) {
					sb.Append( r.ToString() );
					sb.Append( "\r\n" );
				}
			}

			return sb.ToString();
		}
	}
}
