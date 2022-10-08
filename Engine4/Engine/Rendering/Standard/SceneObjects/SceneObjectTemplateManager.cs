namespace Engine.Rendering.Standard.SceneObjects;

public class SceneObjectTemplateManager : Identifiable {

	private readonly Dictionary<string, SceneObjectTemplate> _templates;

	public SceneObjectTemplateManager() {
		this._templates = new();
		LoadAllTemplates();
	}

	private void LoadAllTemplates() {
		if ( !Directory.Exists( "res/templates" ) )
			Directory.CreateDirectory( "res/templates" );
		var files = Directory.GetFiles( "res/templates" );
		foreach ( var file in files ) {
			var sot = SceneObjectTemplate.Read( file );
			if ( sot is not null )
				this._templates.Add( sot.Name, sot );
		}
	}

	public SceneObjectTemplate? Get( string name ) {
		if ( name == SceneObjectTemplate.Square.Name )
			return SceneObjectTemplate.Square;
		if ( name == SceneObjectTemplate.Cube.Name )
			return SceneObjectTemplate.Cube;
		var ret = this._templates.GetValueOrDefault( name );
		if ( ret is null )
			this.LogWarning( $"Unable to get template {name}!" );
		return ret;
	}
}