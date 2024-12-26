using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual {
	public class ScrollableTextbox : UIElement {

		private TextData textData;
		public TextLabel Label { get; private set; }

		private bool updated;
		private int startLine, endLine;
		private float ratio = 0;

		public ScrollableTextbox() {
			Label = new TextLabel();
			Label.SetParent( this );
			textData = new TextData( Label.Attributes, "" );

			UpdatedSecondActive += OnUpdate;
			TransformChanged += OnTransformChanged;
		}

		private void OnTransformChanged( SceneObject<SceneObjectData2> r ) {
			ratio = TransformInterface.Scale.X / TransformInterface.Scale.Y;
			if( ratio * 2 != textData.Attributes.MaxLength )
				textData.Attributes.MaxLength = ratio * 2;
			updated = true;
		}

		private void OnUpdate( MouseInputEventData data ) {
			if( updated ) {
				textData.Update();
				Label.TextData.SublinesFrom( textData, startLine, endLine );
				updated = false;
			}
		}

		public void Reset( string content, Vector4i color ) {
			textData.Set( content, color );
			updated = true;
		}

		public void Append( string content, Vector4i color ) {
			textData.Append( content, color );
			updated = true;
		}

	}
}
