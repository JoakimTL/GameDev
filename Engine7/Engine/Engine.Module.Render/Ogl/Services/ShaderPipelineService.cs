using Engine.Module.Render.Ogl.OOP.Shaders;

namespace Engine.Module.Render.Ogl.Services;

public sealed class ShaderPipelineService : DisposableIdentifiable {

	private readonly IInstanceProvider _pipelineProvider;
	private readonly ShaderProgramService _shaderProgramService;

	public ShaderPipelineService( ShaderProgramService shaderProgramService ) {
		_shaderProgramService = shaderProgramService;
		_pipelineProvider = InstanceManagement.CreateProvider();
		_pipelineProvider.OnInstanceAdded += CreatePipeline;
	}

	private void CreatePipeline( object service ) {
		if (service is OglShaderPipelineBase pipeline)
			pipeline.CreatePipeline( _shaderProgramService );
	}

	public OglShaderPipelineBase Get<T>() where T : OglShaderPipelineBase => _pipelineProvider.Get<T>();
	public OglShaderPipelineBase? Get( Type type ) => _pipelineProvider.Get( type ) as OglShaderPipelineBase;

	protected override bool InternalDispose() {
		_pipelineProvider.Dispose();
		return true;
	}
}

public sealed class ShaderBundleService : Identifiable, IInitializable {
	private readonly ShaderPipelineService _pipelineService;
	private readonly Dictionary<Type, ShaderBundleBase> _bundles;
	private readonly Dictionary<string, Type> _bundleTypeFromIdentity;
	private readonly List<string> _allIndices;

	public ShaderBundleService( ShaderPipelineService pipelineService ) {
		this._pipelineService = pipelineService;
		_bundles = [];
		_allIndices = [];
		_bundleTypeFromIdentity = LoadBundleIdentities();
	}

	private void LoadBundles() {
		IEnumerable<ShaderBundleBase> bundles = TypeManager.Registry.ImplementationTypes
			.Where( p => p.IsAssignableTo( typeof( ShaderBundleBase ) ) )
			.Select( p => TypeManager.ResolveType( p ).CreateInstance( null ) )
			.OfType<ShaderBundleBase>();
		foreach (ShaderBundleBase shaderBundle in bundles)
			_bundles.Add( shaderBundle.GetType(), shaderBundle );
	}

	private Dictionary<string, Type> LoadBundleIdentities() {
		Dictionary<string, Type> bundleTypes = new();
		foreach (Type type in TypeManager.IdentityRegistry.TypesWithIdentity.Where( q => !q.IsAbstract && q.IsAssignableTo( typeof( ShaderBundleBase ) ) )) {
			IdentityAttribute identity = TypeManager.ResolveType( type ).GetAttribute<IdentityAttribute>();
			if (bundleTypes.TryGetValue( identity.Identity, out Type? occupyingType ))
				throw new InvalidDataException( $"{type.FullName}{Environment.NewLine}Identity \"{identity.Identity}\" already taken by:{Environment.NewLine}{occupyingType.FullName}" );
			bundleTypes.Add( identity.Identity, type );
		}
		return bundleTypes;
	}
	public void Initialize() {
		LoadBundles();
		foreach (ShaderBundleBase bundle in _bundles.Values)
			bundle.CreateBundle( _pipelineService );
		_allIndices.AddRange( _bundles.Values.SelectMany( p => p.AllIndices ).Distinct() );
	}

	public T? Get<T>() where T : ShaderBundleBase => _bundles.TryGetValue( typeof( T ), out ShaderBundleBase? bundle ) ? bundle as T : null;
	public ShaderBundleBase? Get( Type type ) => _bundles.TryGetValue( type, out ShaderBundleBase? bundle ) ? bundle : null;
	public ShaderBundleBase? Get( string identity ) => _bundleTypeFromIdentity.TryGetValue( identity, out Type? type ) ? Get( type ) : null;

	public IReadOnlyList<string> AllShaderIndices => _allIndices;

}