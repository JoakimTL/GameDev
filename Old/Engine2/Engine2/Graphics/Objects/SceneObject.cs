using Engine.LMath;
using Engine.MemLib;
using Engine.Utilities.Data.Boxing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects {
	public abstract class SceneObject {

		#region ID
		private static uint GetNextID() {
			lock( openID )
				return openID.Value++;
		}
		private static readonly MutableSinglet<uint> openID = new MutableSinglet<uint>( 0 );
		#endregion

		public uint ID { get; }

		public bool Warn { get; set; } = true;//Used to warn when ineligible added to scene

		public bool Active { get; set; } = true;


		private readonly HashSet<Scene> scenes;

		private readonly MutableSinglet<RenderableSum> sum;
		public RenderableSum Sum { get => sum.Value; }

		protected readonly MutableSinglet<uint> _layer;
		public uint Layer { get { return _layer.Value; } set { _layer.Value = value; } }

		protected readonly MutableSinglet<Material> _material;
		public Material Material { get { return _material.Value; } set { _material.Value = value; } }

		protected readonly MutableSinglet<Mesh> _mesh;
		public Mesh Mesh { get { return _mesh.Value; } set { _mesh.Value = value; } }

		protected readonly MutableSinglet<Shader> _shader;
		public Shader Shader { get { return _shader.Value; } set { _shader.Value = value; } }

		public Action RenderFunction { get; protected set; }

		protected readonly MutableSinglet<Action> _setup;
		public Action RenderSetup { get { return _setup.Value; } set { _setup.Value = value; } }

		public SceneObject( uint layer = 0, Material mat = null, Mesh mesh = null, Shader shader = null ) {
			ID = GetNextID();
			_layer = new MutableSinglet<uint>( layer );
			_layer.Changed += LayerChangedInternal;
			_material = new MutableSinglet<Material>( mat );
			_material.Changed += ValueChanged;
			_mesh = new MutableSinglet<Mesh>( mesh );
			_mesh.Changed += ValueChanged;
			_shader = new MutableSinglet<Shader>( shader );
			_shader.Changed += ValueChanged;
			_setup = new MutableSinglet<Action>( null );
			_setup.Changed += SetupChanged;
			scenes = new HashSet<Scene>();
			sum = new MutableSinglet<RenderableSum>( new RenderableSum( _material.Value, _mesh.Value, _shader.Value ) );
			RenderFunction = null;
		}

		internal void AddToScene( Scene s ) {
			if( Warn && !Sum.IsValid() )
				Logging.Warning( $"Added [{this}] to scene[{s}] while being invalid." );
			scenes.Add( s );
		}

		internal void RemoveFromScene( Scene s ) {
			scenes.Remove( s );
		}

		private void LayerChangedInternal( uint oldValue ) {
			foreach( Scene s in scenes ) {
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

		private void ValueChanged( Shader oldValue ) {
			ValueChanged();
		}

		private void ValueChanged() {
			bool wasValid = Sum.IsValid();
			sum.Value = new RenderableSum( _material.Value, _mesh.Value, _shader.Value );
			bool isValid = Sum.IsValid();
			if( isValid || wasValid != isValid )
				foreach( Scene s in scenes )
					s.LayerChanged( Layer );
		}

		private void SetupChanged( Action value ) {
			foreach( Scene s in scenes )
				s.SetupChanged( !( value is null ), this );
		}

		public override bool Equals( object obj ) {
			SceneObject o = obj as SceneObject;
			if( o is null )
				return false;
			return Equals( o );
		}

		public bool Equals( SceneObject obj ) {
			return obj.ID == ID;
		}

		public override int GetHashCode() {
			return (int) ID;
		}

		public override string ToString() {
			return $"Renderable[{ID}][{Sum.IsValid()}][{Material}][{Mesh}][{Shader}][{Active}]";
		}

		public virtual void PreRender() { }
		public virtual void PostRender() { }

	}

	public struct RenderableSum {
		public uint MaterialID { get; private set; }
		public uint MeshID { get; private set; }
		public uint ShaderID { get; private set; }

		public RenderableSum( Material mat, Mesh mesh, Shader shader ) {
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
