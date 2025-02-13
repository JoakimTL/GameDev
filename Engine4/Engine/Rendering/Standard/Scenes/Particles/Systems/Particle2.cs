﻿using System.Numerics;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Standard.Scenes.Particles.Systems;
public class Particle2 : ParticleBase<Particle2Data> {

	public float Time;
	public float Duration;
	public float Rotation;

	public Particle2( Particle2Data data, float time, float lifeTime ) {
		this.Data = data;
		this.Time = time;
		this.Duration = lifeTime;
		this.Rotation = 0;
	}

	protected override bool DoUpdate( float time, float deltaTime ) {
		Particle2Data newParticle = this.Data;
		this.Rotation += deltaTime;
		newParticle.RotationSineVector = new Vector2( MathF.Cos( this.Rotation ), MathF.Sin( this.Rotation ) );
		newParticle.Blend = ( time - this.Time ) / this.Duration;
		this.Data = newParticle;
		return time < this.Time + this.Duration;
	}
}
