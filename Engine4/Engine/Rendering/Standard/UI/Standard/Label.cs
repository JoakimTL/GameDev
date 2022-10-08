using Engine.Data.Datatypes;
using Engine.Rendering.Standard.UI.Standard.Text;

namespace Engine.Rendering.Standard.UI.Standard;
public class Label : UIElement {

	private readonly TextSceneObject _sceneObject;
	private readonly TextContainer _container;
	public TextContainer Container => this._container;

	private bool _transformChanged;
	private readonly Texture _fontTexture;

	public Label( Font font, string initialText, float textScale ) {
		this._fontTexture = Resources.Render.Textures.GetFromPath( font.FontTexturePath );
		this._container = new TextContainer( font, initialText, textScale, kerned: false );
		SetSceneObject( this._sceneObject = new TextSceneObject( this._fontTexture, (uint) initialText.Length ) );
		TransformsUpdated += OnUpdate;
		this.Transform.MatrixChanged += TransformChanged;
	}

	public string Text {
		get => this._container.Text;
		set => this._container.SetText( value ); //TODO: Individual character colors and rotations (and even scales?)
	}

	private void TransformChanged( IMatrixProvider obj ) => this._transformChanged = true;

	private void OnUpdate( UIElement e ) {
		bool textChanged = this._container.Update();
		if ( textChanged || this._transformChanged )
			this._sceneObject.UpdateSceneObjectData( this.Transform.Matrix, this._fontTexture.GetHandleDirect(), this._container.Glyphs );
		this._transformChanged = false;
	}
}

public class TextEdit : UIElement {

	public Label Label { get; }

	public TextEdit( Font font, string initialText, float textScale ) {
		this.Label = new( font, initialText, textScale );

	}
}
