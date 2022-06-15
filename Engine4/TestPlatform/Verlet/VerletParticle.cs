using System.Numerics;
using Engine.Rendering.Colors;
using Engine.Rendering.Standard.Scenes.VerletParticles;
using Engine.Rendering.Standard.Scenes.VerletParticles.Systems;

namespace TestPlatform.Verlet;
public class VerletParticle : VerletParticleBase<VerletParticle3Data> {

	public Vector3 Position { get; set; }
	public Vector3 LastPosition { get; set; }
	public Vector3 Acceleration { get; set; }
	public float Radius { get; set; }
	public float Temperature { get; set; }

	public override VerletParticle3Data Data => new() { Translation = Position, Velocity = LastPosition - Position, Radius = Radius, Color = new Vector4( Temperature / 1400 - MathF.Max( ( Temperature - 2400 ) / 300, 0 ), ( Temperature - 900 ) / 1400, ( Temperature - 1500 ) / 1400, Temperature / 1000 ) };

	public void Reset( Vector3 position, float radius ) {
		Alive = true;
		Position = LastPosition = position;
		Radius = radius;
		Acceleration = new();
		Temperature = 0;
	}
	public void Kill() {
		Alive = false;
		Position = LastPosition = new();
		Radius = 0;
		Acceleration = new();
	}

}
