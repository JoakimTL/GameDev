using System.Numerics;
using Engine.Rendering.Standard.Scenes.VerletParticles;
using Engine.Rendering.Standard.Scenes.VerletParticles.Systems;

namespace TestPlatform.Verlet;
public class VerletParticle : VerletParticleBase<VerletParticle3Data> {

	public Vector3 Position { get; set; }
	public Vector3 LastPosition { get; set; }
	public Vector3 Acceleration { get; set; }
	public float Radius { get; set; }
	public float Temperature { get; set; }

	public override VerletParticle3Data Data => new() { Translation = Position, Velocity = this.LastPosition - this.Position, Radius = Radius, Color = new Vector4( this.Temperature / 1400 - MathF.Max( ( this.Temperature - 2400 ) / 300, 0 ), ( this.Temperature - 900 ) / 1400, ( this.Temperature - 1500 ) / 1400, this.Temperature / 1000 ) };

	public void Reset( Vector3 position, float radius ) {
		this.Alive = true;
		this.Position = this.LastPosition = position;
		this.Radius = radius;
		this.Acceleration = new();
		this.Temperature = 0;
	}
	public void Kill() {
		this.Alive = false;
		this.Position = this.LastPosition = new();
		this.Radius = 0;
		this.Acceleration = new();
	}

}
