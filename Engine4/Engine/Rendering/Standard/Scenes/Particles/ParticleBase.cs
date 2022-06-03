namespace Engine.Rendering.Standard.Scenes.Particles;
public abstract class ParticleBase<SD> where SD : unmanaged {
	public SD Data { get; protected set; }
	public bool Alive { get; protected set; } = true;
	public void Update( float time, float deltaTime ) {
		if ( !this.Alive )
			return;
		this.Alive = DoUpdate( time, deltaTime );
	}
	protected abstract bool DoUpdate( float time, float deltaTime );
}
