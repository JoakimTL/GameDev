using Engine.GlobalServices;
using Engine.Rendering.Contexts.Objects;
using Engine.Structure.Attributes;
using System.Reflection;

namespace Engine.Rendering.Contexts.Services;

public sealed class ShaderBundleService : Identifiable, IContextService {
	private readonly Dictionary<Type, ShaderBundleBase> _bundles;
	private readonly Dictionary<string, Type> _bundleTypeFromIdentity;
	private readonly IReadOnlyList<string> _allIndices;

	public ShaderBundleService( TypeService typeService, ShaderPipelineService pipelineService ) {
		_bundles = LoadBundles( typeService );
		_bundleTypeFromIdentity = LoadBundleIdentities();
		foreach ( var bundle in _bundles.Values )
			bundle.CreateBundle( pipelineService );
		_allIndices = _bundles.Values.SelectMany( p => p.AllIndices ).Distinct().ToList().AsReadOnly();
	}

	private static Dictionary<Type, ShaderBundleBase> LoadBundles( TypeService typeService )
		=> typeService.ImplementationTypes
			.Where( p => p.IsAssignableTo( typeof( ShaderBundleBase ) ) )
			.Select( p => p.GetInjectedInstance() )
			.OfType<ShaderBundleBase>()
			.ToDictionary( p => p.GetType(), p => p );


	private Dictionary<string, Type> LoadBundleIdentities() {
		Dictionary<string, Type> bundleTypes = new();
		foreach ( var type in Global.Get<TypeService>().DerivedTypes.Where( q => q.IsAssignableTo( typeof( ShaderBundleBase ) ) ) ) {
			IdentityAttribute? identity = type.GetCustomAttribute<IdentityAttribute>();
			if ( identity is null ) {
				this.LogWarning( $"{type} is missing an Identity!" );
				continue;
			}
			if ( bundleTypes.TryGetValue( identity.Identity, out Type? occupyingType ) )
				throw new InvalidDataException( $"{type.FullName}{Environment.NewLine}Identity \"{identity.Identity}\" already taken by:{Environment.NewLine}{occupyingType.FullName}" );
			bundleTypes.Add( identity.Identity, type );
		}
		return bundleTypes;
	}

	public ShaderBundleBase? Get( Type type ) => _bundles.TryGetValue( type, out var bundle ) ? bundle : null;
	public ShaderBundleBase? Get( string identity ) => _bundleTypeFromIdentity.TryGetValue( identity, out Type? type ) ? Get( type ) : null;
	public IReadOnlyList<string> AllShaderIndices => _allIndices;

}