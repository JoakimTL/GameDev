using Engine.Rendering.Objects;
using System.Collections.Concurrent;

namespace Engine.Rendering.Services;

public sealed class ShaderBundleService : Identifiable, IContextService {

	private readonly Dictionary<string, ShaderBundle> _bundles;
	private readonly ShaderPipelineService _pipelineService;

	public ShaderBundleService( ShaderPipelineService pipelineService ) {
		_bundles = new();
		this._pipelineService = pipelineService;
	}

	public ShaderBundle Get( params (uint id, string pipelineIdentity)[] pipelines ) {
		string combined = string.Join( Environment.NewLine, pipelines );
		if ( _bundles.TryGetValue( combined, out ShaderBundle? bundle ) )
			return bundle;
		bundle = new ShaderBundle( pipelines.Select( p => (p.id, _pipelineService.GetOrFail( p.pipelineIdentity )) ) );
		_bundles.Add( combined, bundle );
		return bundle;
	}

}