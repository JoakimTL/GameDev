using Engine.Data;
using Engine.Rendering.Colors;
using Engine.Rendering.Standard.SceneObjects;

namespace Engine.Modularity.ECS.Components;

[OverrideType( typeof( RenderComponentBase ) )]
public abstract class RenderComponentBase : SerializableComponent {

	private readonly SceneObjectTemplate _defaultTemplate;

	private Color16x4 _color;
	private SceneObjectTemplate _template;
	public SceneObjectTemplate Template { get => this._template; set => SetTemplate( value ); }
	public Color16x4 Color { get => this._color; set => SetColor( value ); }

	public RenderComponentBase( SceneObjectTemplate defaultTemplate ) {
		this._defaultTemplate = defaultTemplate ?? throw new ArgumentNullException( nameof( defaultTemplate ) );
		this._template = defaultTemplate;
		this._color = Color16x4.White;
	}

	public void SetTemplate( SceneObjectTemplate? template ) {
		if ( template is null )
			template = this._defaultTemplate;
		if ( template == this._template )
			return;
		this._template = template;
		TriggerChanged();
	}

	public void SetTemplate( string templateName ) => SetTemplate( Resources.GlobalService<SceneObjectTemplateManager>().Get( templateName ) );

	public void SetColor( Color16x4 value ) {
		if ( value == this._color )
			return;
		this._color = value;
		TriggerChanged();
	}

	public ulong ResolveTextureToHandle( uint index ) {
		//TODO Fix, using assetRef
		if ( index >= this.Template.TexturePaths.Count )
			return Resources.Render.Textures.White1x1.GetHandleDirect();
		return Resources.Render.Textures.Get( this.Template.TexturePaths[ (int) index ] ).GetHandleDirect();
	}

	/*
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
	*/

	protected override byte[]? GetSerializedData() => Segmentation.Segment( DataUtils.ToBytes( this._template.Name ), DataUtils.ToBytes( this._color ) );

	public override void SetFromSerializedData( byte[] data ) {
		byte[][]? segments = Segmentation.Parse( data );
		if ( segments is null || segments.Length != 2 )
			return;
		var templateName = DataUtils.ToStringUTF8( segments[ 0 ] );
		if ( templateName is not null )
			SetTemplate( templateName );
		SetColor( DataUtils.ToUnmanaged<Color16x4>( segments[ 1 ] ) ?? Color16x4.White );
	}
}
