namespace Engine.Rendering.Objects;

public sealed class ShaderBundle : Identifiable {

	private static readonly List<IReadOnlyList<uint>> _sequences = new() { new List<uint>() };

	public uint BundleID { get; private set; }
	private readonly Dictionary<uint, ShaderPipelineBase> _pipelines;

	internal ShaderBundle( IEnumerable<(uint usecaseId, ShaderPipelineBase pipeline)> usecases ) {
		_pipelines = new();
		foreach ( var usecase in usecases ) 
			_pipelines.Add( usecase.usecaseId, usecase.pipeline );

		List<uint> sortedUsecases = new( this._pipelines.Keys );
		sortedUsecases.Sort();

		List<uint> shaderSequence = new();
		for ( int i = 0; i < sortedUsecases.Count; i++ ) {
			shaderSequence.Add( sortedUsecases[ i ] );
			shaderSequence.Add( this._pipelines[ sortedUsecases[ i ] ].PipelineId );
		}

		this.BundleID = AssignID( shaderSequence );
	}

	public ShaderPipelineBase? Get( uint usecase ) => this._pipelines.TryGetValue( usecase, out ShaderPipelineBase? r ) ? r : null;

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

