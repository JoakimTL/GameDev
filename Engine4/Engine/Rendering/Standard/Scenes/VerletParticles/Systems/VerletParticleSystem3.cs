﻿using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Standard.Scenes.VerletParticles.Systems;
public class VerletParticleSystem3<P> : VerletParticleSystem<P, Vertex3, VerletParticle3Data> where P : VerletParticleBase<VerletParticle3Data>, new() {
	public VerletParticleSystem3( uint maxParticles, uint subcalculations ) : base( Resources.Render.Shader.Bundles.Get<VerletParticle3ShaderBundle>(), Resources.Render.Mesh3.Icosphere1, maxParticles, subcalculations ) { }

	public override void Bind() { }
}
