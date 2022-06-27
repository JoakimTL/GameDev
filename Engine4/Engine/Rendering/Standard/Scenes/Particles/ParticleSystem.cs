using Engine.Rendering.Standard.Meshing;

namespace Engine.Rendering.Standard.Scenes.Particles;
public class ParticleSystem<V, SD> : ClosedSceneObject<V, SD> where V : unmanaged where SD : unmanaged {

	private readonly List<ParticleBase<SD>> _particles;

	public ParticleSystem( ShaderBundle shaders, VertexMesh<V> mesh, uint maxParticles ) : base() {
		SetSceneData( new SceneInstanceData<SD>( maxParticles, 0 ) );
		SetMesh( mesh );
		SetShaders( shaders );
		this._particles = new List<ParticleBase<SD>>();
	}

	public void Add( ParticleBase<SD> particle ) {
		if ( this.SceneData is not null && this._particles.Count < this.SceneData?.MaxInstances )
			this._particles.Add( particle );
	}

	public void Remove( ParticleBase<SD> particle ) => this._particles.Remove( particle );

	public void Update( float time, float deltaTime ) {
		for ( int i = this._particles.Count - 1; i >= 0; i-- ) {
			ParticleBase<SD> particle = this._particles[ i ];
			particle.Update( time, deltaTime );
			if ( !particle.Alive )
				this._particles.RemoveAt( i );
		}
		if ( this.SceneData is not null ) {
			for ( int i = 0; i < this._particles.Count; i++ )
				this.SceneData.SetInstance( (uint) i, this._particles[ i ].Data );
			this.SceneData.SetActiveInstances( (uint) this._particles.Count );
		}
	}

	public override void Bind() { }
}
