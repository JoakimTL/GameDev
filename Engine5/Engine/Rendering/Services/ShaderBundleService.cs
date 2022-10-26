using Engine.GlobalServices;
using Engine.Rendering.Objects;
using Engine.Structure.Attributes;
using Engine.Structure.ServiceProvider;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime;
using System.Security.Principal;

namespace Engine.Rendering.Services;

public sealed class ShaderBundleService : Identifiable, IContextService {

	private readonly RestrictedServiceProvider<ShaderBundleBase> _bundleProvider;
	private readonly Dictionary<string, Type> _bundleTypeFromIdentity;
	private readonly ShaderPipelineService _pipelineService;

	public ShaderBundleService( ShaderPipelineService pipelineService ) {
		_bundleProvider = new();
		this._pipelineService = pipelineService;
		_bundleProvider.AddConstant( _pipelineService );
		_bundleTypeFromIdentity = LoadBundles();
	}

	private Dictionary<string, Type> LoadBundles() {
		Dictionary<string, Type> bundleTypes = new();
		foreach ( var type in Global.Get<TypeService>().DerivedTypes.Where( q => q.IsAssignableTo( typeof( ShaderBundleBase ) ) ) ) {
			IdentityAttribute? identity = type.GetCustomAttribute<IdentityAttribute>();
			if (identity is null) {
				this.LogWarning( $"{type} is missing an Identity!" );
				continue;
			}
			if ( bundleTypes.TryGetValue( identity.Identity, out Type? occupyingType ) )
				throw new InvalidDataException( $"{type.FullName}{Environment.NewLine}Identity \"{identity.Identity}\" already taken by:{Environment.NewLine}{occupyingType.FullName}" );
			bundleTypes.Add( identity.Identity, type );
		}
		return bundleTypes;
	}

	public ShaderBundleBase? Get( Type type ) => _bundleProvider.Get( type ) as ShaderBundleBase;
	public ShaderBundleBase? Get( string identity ) => _bundleTypeFromIdentity.TryGetValue( identity, out Type? type ) ? Get( type ) : null;

}