using Engine.Graphics.Objects.Default.Cameras.Views;
using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.LinearAlgebra;
using Engine.MemLib;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Time;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D3 {
	public class Particle3System : InstanceHandlerDatabuffered<Particle3, InstancedMeshVBOArray, SceneObjectData3> {

		private Clock32 particleTime;
		public float Time { get; private set; }
		public float DeltaTime { get; private set; }
		public float PreviousTime { get; private set; }
		private List<Particle3> particlesSorted;
		public ParticleMaterial ParticleMaterial { get; private set; }
		private MutableSinglet<Camera3> camera;

		public int ActiveParticles { get => particlesSorted.Count; }

		public Particle3System( ShaderBundle shaderBundle, ParticleMaterial material, MutableSinglet<Camera3> camera, int maxInstances ) : base( new SceneObjectData3(), new Particle3Mesh( maxInstances ), maxInstances, Particle3.SIZEBYTES, false ) {
			this.camera = camera;
			particleTime = new Clock32();
			particlesSorted = new List<Particle3>();
			ShaderBundle = shaderBundle;
			Material = material;
			ParticleMaterial = material;
			RenderFunction = ( SceneObject<SceneObjectData3> so, Shader s, IView view ) => {
				s.Set( "zNear", camera.Value.PerspectiveProjection.ZNear );
				s.Set( "zFar", camera.Value.PerspectiveProjection.ZFar );
				s.Set( "uVP_mat", camera.Value.VPMatrix );
				s.Set( "uV_up", camera.Value.TranformInterface.GlobalRotation.Up );
				s.Set( "uV_right", camera.Value.TranformInterface.GlobalRotation.Right );
				so.Mesh.RenderMesh();
			};
			Mem.CollisionMolds.CubeUniform.MoldNew( Data.CollisionModel );
		}

		public Particle3 CreateParticle() {
			if( TryCreateInstance( out Particle3 o ) ) {
				return o;
			}
			return null;
		}

		protected override void PreUpdate() {
			PreviousTime = Time;
			Time = particleTime.Time;
			DeltaTime = Time - PreviousTime;
		}

		protected override void BufferData() {
			int count = particlesSorted.Count;
			Vector3 min, max;
			min = float.MaxValue;
			max = float.MinValue;
			for( int i = 0; i < count; i++ ) {
				Vector3 translation = particlesSorted[ i ].Data.Translation;
				float scale = particlesSorted[ i ].Data.Scale;
				particlesSorted[ i ].SetDistance( camera.Value );
				min = Vector3.Min( translation - scale, min );
				max = Vector3.Max( translation + scale, max );
			}
			Data.Transform.Translation = ( max + min ) / 2;
			Data.Transform.Scale = ( max - min ) / 2;
			particlesSorted.Sort( ParticleSorter );
			for( int i = 0; i < count; i++ ) {
				mesh.BufferData( i * Particle3.SIZEBYTES, particlesSorted[ i ].DataBytes );
			}
		}

		protected override void InstanceAdded( Particle3 t ) {
			particlesSorted.Add( t );
		}
		protected override void InstanceRemoved( Particle3 t ) {
			particlesSorted.Remove( t );
		}


		private int ParticleSorter( Particle3 x, Particle3 y ) {
			if( x.Distance > y.Distance )
				return -1;
			return 1;
		}

		protected override void CreationMethod( out Particle3 o ) {
			o = new Particle3( this );
		}
	}
}
