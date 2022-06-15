namespace Engine.Rendering.Standard.Scenes.VerletParticles;

public abstract class VerletParticleBase<SD> where SD : unmanaged {
	public bool Alive { get; protected set; }
	public abstract SD Data { get; }
}
