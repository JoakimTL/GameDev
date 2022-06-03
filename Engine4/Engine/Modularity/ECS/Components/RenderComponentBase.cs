using System.Reflection;
using Engine.Data;
using Engine.Rendering.Colors;

namespace Engine.Modularity.ECS.Components;

[OverrideType( typeof( RenderComponentBase ) )]
public abstract class RenderComponentBase : Component {

	private readonly string _defaultMeshName;
	private readonly Guid _defaultShaderBundleGuid;
	private string _meshName;
	private Guid _shaderBundleGuid;
	private Color16x4 _color;
	public Guid ShaderBundle { get => this._shaderBundleGuid; set => SetShaderBundle( value ); }
	public string Mesh { get => this._meshName; set => SetMesh( value ); }
	public Color16x4 Color { get => this._color; set => SetColor( value ); }

	public RenderComponentBase( string defaultMeshName, Type defaultShaderBundleType ) : this(
		defaultMeshName,
		defaultShaderBundleType.GetCustomAttribute<IdentificationAttribute>()?.Guid
			?? throw new ArgumentException( "Type does not have an Identification attribute!", nameof( defaultShaderBundleType ) ) ) { }

	public RenderComponentBase( string defaultMeshName, Guid defaultShaderBundleGuid ) {
		this._defaultMeshName = defaultMeshName ?? throw new ArgumentNullException( nameof( defaultMeshName ) );
		this._defaultShaderBundleGuid = defaultShaderBundleGuid;
		this._meshName = this._defaultMeshName;
		this._shaderBundleGuid = this._defaultShaderBundleGuid;
		this._color = Color16x4.White;
	}

	protected void SetColor( Color16x4 value ) {
		if ( value == this._color )
			return;
		this._color = value;
		TriggerChanged();
	}

	protected void SetMeshToDefault()
		=> SetMesh( this._defaultMeshName );

	protected void SetMeshOrDefault( string? mesh )
		=> SetMesh( mesh );

	protected void SetShaderBundle( Type value ) {
		Guid guid = this._defaultShaderBundleGuid;
		if ( value is not null ) {
			IdentificationAttribute? id = value.GetCustomAttribute<IdentificationAttribute>();
			if ( id is not null )
				guid = id.Guid;
		}
		if ( guid == this._shaderBundleGuid )
			return;
		this._shaderBundleGuid = guid;
		TriggerChanged();
	}

	protected void SetShaderBundle( Guid? value ) {
		if ( value is null )
			value = this._defaultShaderBundleGuid;
		if ( value == this._shaderBundleGuid )
			return;
		this._shaderBundleGuid = value ?? this._defaultShaderBundleGuid;
		TriggerChanged();
	}

	protected void SetMesh( string? value ) {
		if ( value is null )
			value = this._defaultMeshName;
		if ( value == this._meshName )
			return;
		this._meshName = value ?? this._defaultMeshName;
		TriggerChanged();
	}

	protected override byte[]? GetSerializedData() {
		if ( this._meshName is null )
			return null;
		return Segmentation.Segment( DataUtils.ToBytes( this._meshName ), this._shaderBundleGuid.ToByteArray() );
	}

	public override void SetFromSerializedData( byte[] data ) {
		byte[][]? segments = Segmentation.Parse( data );
		if ( segments is null || segments.Length != 2 )
			return;
		Guid? guid = DataUtils.ToUnmanaged<Guid>( segments[ 1 ] );
		if ( !guid.HasValue )
			return;
		SetMesh( DataUtils.ToStringUTF8( segments[ 0 ] ) );
		SetShaderBundle( new Guid( segments[ 1 ] ) );
	}
}
