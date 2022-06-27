using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Engine.Data.Datatypes;
using Engine.Rendering.Standard.UI.Standard.Text;

namespace Engine.Rendering.Standard.UI.Standard;
public class Label : UIElement {

	private readonly TextSceneObject _sceneObject;
	private readonly TextContainer _container;
	public TextContainer Container => _container;

	private bool _transformChanged;
	private readonly Texture _fontTexture;

	public Label( Font font, string initialText, float textScale ) {
		_fontTexture = Resources.Render.Textures.GetFromPath( font.FontTexturePath );
		_container = new TextContainer( font, initialText, textScale );
		SetSceneObject( _sceneObject = new TextSceneObject( _fontTexture, (uint) initialText.Length ) );
		TransformsUpdated += OnUpdate;
		this.Transform.MatrixChanged += TransformChanged;
	}

	public string Text {
		get => _container.Text;
		set => _container.SetText( value ); ////TODO: Individual character colors and rotations (and even scales?)
	}

	private void TransformChanged( IMatrixProvider obj ) => _transformChanged = true;

	private void OnUpdate( UIElement e ) {
		bool textChanged = _container.Update();
		if ( textChanged || _transformChanged )
			_sceneObject.UpdateSceneObjectData( Transform.Matrix, _fontTexture.GetHandleDirect(), _container.Glyphs );
		_transformChanged = false;
	}
}
