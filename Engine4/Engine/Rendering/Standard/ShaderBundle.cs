namespace Engine.Rendering.Standard;

public abstract class ShaderBundle : Identifiable {
	private static readonly List<IReadOnlyList<uint>> _sequences = new() { new List<uint>() };

	public uint BundleID { get; private set; }
	private readonly Dictionary<uint, ShaderPipeline> _shaders;

	public abstract bool UsesTransparency { get; }

	public ShaderBundle( params (uint renderMode, ShaderPipeline shader)[] shadersCombos ) {
		this._shaders = new Dictionary<uint, ShaderPipeline>();
		if ( GetType().GetCustomAttributes( typeof( IdentificationAttribute ), false ).FirstOrDefault() is IdentificationAttribute attribute )
			SetGuid( attribute.Guid );

		for ( int i = 0; i < shadersCombos.Length; i++ ) {
			(uint renderMode, ShaderPipeline shader) = shadersCombos[ i ];
			this._shaders.Add( renderMode, shader );
		}

		List<uint> sortedUsecases = new( this._shaders.Keys );
		sortedUsecases.Sort();

		List<uint> shaderSequence = new();
		for ( int i = 0; i < sortedUsecases.Count; i++ ) {
			shaderSequence.Add( sortedUsecases[ i ] );
			shaderSequence.Add( this._shaders[ sortedUsecases[ i ] ].PipelineId );
		}

		this.BundleID = AssignID( shaderSequence );
	}

	public ShaderPipeline? Get( uint usecase ) => this._shaders.TryGetValue( usecase, out ShaderPipeline? r ) ? r : null;

	private static uint AssignID( List<uint> shaderSequence ) {
		lock ( _sequences ) {
			if ( shaderSequence.Count == 0 )
				return 0;

			for ( int i = 0; i < _sequences.Count; i++ )
				if ( _sequences[ i ].SequenceEqual( shaderSequence ) )
					return (uint) i;

			_sequences.Add( shaderSequence );
			return (uint) _sequences.Count - 1;
		}
	}
}
