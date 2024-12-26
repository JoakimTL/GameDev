using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Data.Boxing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects {
	public abstract class SceneObject<T> where T : SceneObjectData {

		#region ID
		private static uint GetNextID() {
			lock( openID )
				return openID.Value++;
		}
		private static readonly MutableSinglet<uint> openID = new MutableSinglet<uint>( 0 );
		#endregion

		public delegate void RenderableTransformChangeHandler( SceneObject<T> r );
		public virtual event RenderableTransformChangeHandler TransformChanged;

		public uint ID { get; }

		public bool Warn { get; set; } = true;

		public virtual bool Active { get; set; } = true;

		private readonly HashSet<Scene<T>> scenes;

		public IReadOnlyCollection<Scene<T>> Scenes { get => scenes; }

		private readonly MutableSinglet<RenderableSum> sum;
		public RenderableSum Sum { get => sum.Value; }

		protected readonly MutableSinglet<uint> _layer;
		public uint Layer { get { return _layer.Value; } set { _layer.Value = value; } }
		public event MutableSinglet<uint>.SingleValueChange LayerChanged;

		protected readonly MutableSinglet<Material> _material;
		public Material Material { get { return _material.Value; } set { _material.Value = value; } }
		public event MutableSinglet<Material>.SingleValueChange MaterialChanged;

		protected readonly MutableSinglet<Mesh> _mesh;
		public Mesh Mesh { get { return _mesh.Value; } set { _mesh.Value = value; } }
		public event MutableSinglet<Mesh>.SingleValueChange MeshChanged;

		protected readonly MutableSinglet<ShaderBundle> _shaderBundle;
		public ShaderBundle ShaderBundle { get { return _shaderBundle.Value; } set { _shaderBundle.Value = value; } }
		public event MutableSinglet<ShaderBundle>.SingleValueChange ShaderChanged;

		protected readonly MutableSinglet<RenderMethod<T>> _renderFunction;
		public RenderMethod<T> RenderFunction { get { return _renderFunction.Value; } set { _renderFunction.Value = value; } }
		public event MutableSinglet<RenderMethod<T>>.SingleValueChange RenderChanged;

		public T Data { get; private set; }

		public event Action BeforeRendering;
		public event Action AfterRendering;
		public event Action ValidationChanged;

		public SceneObject( T data, uint layer = 0, Material mat = null, Mesh mesh = null, ShaderBundle shader = null, RenderMethod<T> renderMethod = null ) {
			Data = data;
			Data.TransformObject.OnChangedEvent += TransformChangedInternal;
			ID = GetNextID();
			if( renderMethod is null ) {
				renderMethod = DefaultRenderMethod;
			}

			_layer = new MutableSinglet<uint>( layer );
			_layer.Changed += LayerChangedInternal;
			_layer.Changed += LayerChanged;
			_material = new MutableSinglet<Material>( mat );
			_material.Changed += ValueChanged;
			_material.Changed += MaterialChanged;
			_mesh = new MutableSinglet<Mesh>( mesh );
			_mesh.Changed += ValueChanged;
			_mesh.Changed += MeshChanged;
			_shaderBundle = new MutableSinglet<ShaderBundle>( shader );
			_shaderBundle.Changed += ValueChanged;
			_shaderBundle.Changed += ShaderChanged;
			_renderFunction = new MutableSinglet<RenderMethod<T>>( renderMethod );
			_renderFunction.Changed += ValueChanged;
			_renderFunction.Changed += RenderChanged;
			scenes = new HashSet<Scene<T>>();
			sum = new MutableSinglet<RenderableSum>( new RenderableSum( _material.Value, _mesh.Value, _shaderBundle.Value ) );
		}

		public void DefaultRenderMethod( SceneObject<T> so, Shader s, IView view ) {
			s.Set( "uMVP_mat", so.Data.TransformObject.Matrix * view.VPMatrix );
			s.Set( "uM_mat", so.Data.TransformObject.Matrix );
			s.Set( "uColor", so.Data.Color );
			so.Mesh.RenderMesh();
		}

		private void TransformChangedInternal() {
			TransformChanged?.Invoke( this );
		}

		internal void AddToScene( Scene<T> s ) {
			if( Warn && !Sum.IsValid() ) {
				Logging.Warning( $"Added [{this}] to scene[{s}] while being invalid." );
				if( Sum.MaterialID == 0 )
					Logging.Warning( $"--- Material is invalid/incomplete for [{ID}]." );
				if( Sum.ShaderID == 0 )
					Logging.Warning( $"--- Shader is invalid for [{ID}]." );
				if( Sum.MeshID == 0 )
					Logging.Warning( $"--- Mesh is invalid for [{ID}]." );
			}
			scenes.Add( s );
		}

		internal void RemoveFromScene( Scene<T> s ) {
			scenes.Remove( s );
		}

		public void AddObjectToScenes( SceneObject<T> so ) {
			if( so == this )
				return;
			foreach( Scene<T> l in scenes )
				l.Add( so );
		}

		private void LayerChangedInternal( uint oldValue ) {
			foreach( Scene<T> s in scenes ) {
				s.LayerChanged( oldValue );
				s.LayerChanged( Layer );
			}
		}

		private void ValueChanged( Material oldValue ) {
			ValueChanged();
		}

		private void ValueChanged( Mesh oldValue ) {
			ValueChanged();
		}

		private void ValueChanged( ShaderBundle oldValue ) {
			ValueChanged();
		}

		private void ValueChanged( RenderMethod<T> oldValue ) {
			ValueChanged();
		}

		private void ValueChanged() {
			bool wasValid = Sum.IsValid();
			sum.Value = new RenderableSum( _material.Value, _mesh.Value, _shaderBundle.Value );
			bool isValid = Sum.IsValid();
			if( isValid || wasValid != isValid ) {
				foreach( Scene<T> s in scenes )
					s.LayerChanged( Layer );
				ValidationChanged?.Invoke();
			}
		}

		public override bool Equals( object obj ) {
			if( !( obj is SceneObject<T> o ) )
				return false;
			return Equals( o );
		}

		public bool Equals( SceneObject<T> obj ) {
			return obj.ID == ID;
		}

		public override int GetHashCode() {
			return (int) ID;
		}

		public override string ToString() {
			return $"SceneObject[id:{ID}][valid:{Sum.IsValid()}][RS:{sum.Value} - [mat:{Material}][mesh:{Mesh}][shaders:{ShaderBundle}]][active:{Active}]";
		}

		internal void PreRender() {
			BeforeRendering?.Invoke();
		}

		internal void PostRender() {
			AfterRendering?.Invoke();
		}

		public void Dispose() {
			Data.Dispose();
			OnDispose();
		}

		protected abstract void OnDispose();

	}

	public struct RenderableSum {
		public uint MaterialID { get; private set; }
		public uint MeshID { get; private set; }
		public uint ShaderID { get; private set; }

		public RenderableSum( Material mat, Mesh mesh, ShaderBundle shader ) {
			if( mat is null ) {
				MaterialID = 0;
			} else
				MaterialID = mat.ID;
			if( mesh is null ) {
				MeshID = 0;
			} else
				MeshID = mesh.ID;
			if( shader is null ) {
				ShaderID = 0;
			} else
				ShaderID = shader.ID;
		}

		public RenderableSum( uint mat, uint mesh, uint shader ) {
			MaterialID = mat;
			MeshID = mesh;
			ShaderID = shader;
		}

		public bool IsValid() {
			return !( MaterialID == 0 || MeshID == 0 || ShaderID == 0 );
		}

		public override string ToString() {
			return $"RS[{MaterialID}:{MeshID}:{ShaderID}]";
		}
	}
}
