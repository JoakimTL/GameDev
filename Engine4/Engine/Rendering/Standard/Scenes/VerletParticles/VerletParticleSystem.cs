using Engine.Rendering.Standard.Meshing;

namespace Engine.Rendering.Standard.Scenes.VerletParticles;
public abstract class VerletParticleSystem<P, V, SD> : ClosedSceneObject<V, SD> where P : VerletParticleBase<SD>, new() where V : unmanaged where SD : unmanaged {
	private readonly uint _subcalculations;
	private readonly SD[] _particleData;
	private readonly P[] _particles;
	public event Action<P[], uint, float, float>? Updated;
	public event Action<P[], uint, float, float>? SubUpdate;

	public VerletParticleSystem( ShaderBundle shaders, VertexMesh<V> mesh, uint maxParticles, uint subcalculations ) : base() {
		SetSceneData( new SceneInstanceData<SD>( maxParticles, 0 ) );
		SetMesh( mesh );
		SetShaders( shaders );
		this._particleData = new SD[ maxParticles ];
		this._particles = new P[ maxParticles ];
		for ( int i = 0; i < maxParticles; i++ )
			this._particles[ i ] = new();
		if ( subcalculations == 0 )
			throw new ArgumentOutOfRangeException( nameof( subcalculations ) );
		this._subcalculations = subcalculations;
	}

	public void SetActiveParticles( uint count ) {
		if ( count > this._particles.Length || this.SceneData is null )
			return;
		this.SceneData.SetActiveInstances( count );
	}

	public void Update( float time, float deltaTime ) {
		if ( deltaTime > 1f / 60 )
			deltaTime = 1f / 60;
		if ( this.SceneData is null )
			return;
		Updated?.Invoke( this._particles, this.SceneData.ActiveInstances, time, deltaTime );
		float deltaTimeSub = deltaTime / this._subcalculations;
		for ( int i = 0; i < this._subcalculations; i++ )
			SubUpdate?.Invoke( this._particles, this.SceneData.ActiveInstances, time, deltaTimeSub );
		for ( int i = 0; i < this.SceneData.ActiveInstances; i++ )
			this._particleData[ i ] = this._particles[ i ].Data;
		if ( this.SceneData.ActiveInstances > 0 )
			this.SceneData.SetInstances( 0, this._particleData.AsSpan()[ ..(int) this.SceneData.ActiveInstances ] );
	}
}
