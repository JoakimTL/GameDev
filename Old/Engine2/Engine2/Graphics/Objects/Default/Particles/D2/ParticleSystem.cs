using Engine.Graphics.Objects.Default.Meshes.Instancing;
using Engine.Graphics.Objects.Default.SceneObjects;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.MemLib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2 {
	public class ParticleSystem : InstanceHandler<Particle2, InstancedMeshVBOArray> {
		public ParticleSystem( int maxInstances ) : base( new ParticleMesh( maxInstances ), maxInstances, Particle2.SIZEBYTES, Mem.Shaders.Get<Particle2Shader>() ) {
		}

		protected override bool CreationMethod( int index, out Particle2 o ) {
			throw new NotImplementedException();
		}
	}
}
