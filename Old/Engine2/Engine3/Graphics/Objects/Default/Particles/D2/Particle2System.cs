using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.MemLib;
using Engine.Utilities.Time;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2 {
	public class Particle2System : InstanceHandlerDatabuffered<Particle2, InstancedMeshVBOArray, SceneObjectData2> {

		private Clock32 particleTime;
		public float DeltaTime { get; private set; }
		public float PreviousTime { get; private set; }
		public float Time { get; private set; }
		private HashSet<Particle2> particles;
		public ParticleMaterial ParticleMaterial { get; private set; }

		public Particle2System( ShaderBundle shaderBundle, ParticleMaterial material, IView view, int maxInstances ) : base( new SceneObjectData2(), new Particle2Mesh( maxInstances ), maxInstances, Particle2.SIZEBYTES, false ) {
			particleTime = new Clock32();
			ShaderBundle = shaderBundle;
			Material = material;
			ParticleMaterial = material;
			particles = new HashSet<Particle2>();
			RenderFunction = RenderMethod;
		}

		private void RenderMethod( SceneObject<SceneObjectData2> so, Shader s, IView view ) {
			s.Set( "uVP_mat", view.VPMatrix );
			so.Mesh.RenderMesh();
		}

		public Particle2 CreateParticle() {
			if( TryCreateInstance( out Particle2 o ) ) {
				return o;
			}
			return null;
		}

		protected override void CreationMethod( out Particle2 o ) {
			o = new Particle2( this );
		}

		protected override void PreUpdate() {
			PreviousTime = Time;
			Time = particleTime.Time;
			DeltaTime = Time - PreviousTime;
		}

		protected override void InstanceAdded( Particle2 t ) {
			particles.Add( t );
		}
		protected override void InstanceRemoved( Particle2 t ) {
			particles.Remove( t );
		}


		protected override void BufferData() {
			int i = 0;
			foreach( Particle2 p in particles ) {
				mesh.BufferData( (i++) * Particle2.SIZEBYTES, p.DataBytes );
			}
		}
	}
}
