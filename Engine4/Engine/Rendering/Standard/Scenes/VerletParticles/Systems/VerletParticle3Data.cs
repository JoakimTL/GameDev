using System.Numerics;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.Scenes.VerletParticles.Systems;

public struct VerletParticle3Data {
	public Vector3 Translation;
	public Vector3 Velocity;
	public float Radius;
	public Color16x4 Color;
}