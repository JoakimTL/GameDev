using Engine.Rendering.Standard;
using Engine.Structure;

namespace Engine.Rendering.Services;

public class ShaderBundleManager : Identifiable {

	private readonly Dictionary<Type, ShaderBundle> _shaderBundlesByType;
	private readonly Dictionary<Guid, ShaderBundle> _shaderBundlesByGuid;

	public ShaderBundleManager() {
		this._shaderBundlesByType = new Dictionary<Type, ShaderBundle>();
		this._shaderBundlesByGuid = new Dictionary<Guid, ShaderBundle>();
	}

	internal void Initialize() {
		IEnumerable<Type> layoutTypes = TypeManager.GetAllTypes<ShaderBundle>( false, false );
		foreach ( Type type in layoutTypes ) {
			if ( type.GetCustomAttributes( typeof( IdentificationAttribute ), false ).FirstOrDefault() is not IdentificationAttribute attribute ) {
				Log.Warning( $"Found {type.Name}, but it has no identification guid and can't be used." );
				continue;
			}
			try {
				object? instance = Activator.CreateInstance( type );
				if ( instance is not ShaderBundle shaderBundle ) {
					Log.Warning( $"Internal error, instance created of {type} was not a {nameof( ShaderBundle )}!" );
					continue;
				}
				if ( this._shaderBundlesByType.ContainsKey( type ) ) {
					Log.Warning( $"A shader bundle of type {type} has already been added" );
					continue;
				}
				if ( this._shaderBundlesByGuid.TryGetValue( attribute.Guid, out ShaderBundle? sb ) ) {
					Log.Warning( $"The guid {attribute.Guid} has already been occupied by {sb.GetType()}, discarding {type}!" );
					continue;
				}
				this._shaderBundlesByType.Add( type, shaderBundle );
				this._shaderBundlesByGuid.Add( attribute.Guid, shaderBundle );
				this.LogLine( $"Found shader bundle with Guid {attribute.Guid}!", Log.Level.NORMAL, ConsoleColor.Green );
			} catch ( Exception ex ) {
				this.LogError( ex );
			}
		}
	}

	public ShaderBundle? Get( Guid guid ) {
		if ( this._shaderBundlesByGuid.TryGetValue( guid, out ShaderBundle? result ) )
			return result;
		this.LogWarning( $"No shader bundle with GUID {guid}" );
		return null;
	}

	public ShaderBundle? Get( Type type ) {
		if ( this._shaderBundlesByType.TryGetValue( type, out ShaderBundle? result ) )
			return result;
		return null;
	}

	public T Get<T>() where T : ShaderBundle {
		if ( this._shaderBundlesByType.TryGetValue( typeof( T ), out ShaderBundle? result ) && result is T outT )
			return outT;
		throw new NullReferenceException( $"Unable to find shader bundle {typeof( T ).Name}!" );
	}

}
