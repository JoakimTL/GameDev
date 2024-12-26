using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Tactile;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual;
using Engine.LinearAlgebra;
using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDevClient.Rendering.UI {
	class MenuButton : ButtonST {

		private Vector2 location;
		private ModTranslationAdd translation;
		private ModColorSet color;
		private TextLabel label;

		public MenuButton( Texture texture, Vector2 location, Vector2 scale ) : base( texture ) {
			UpdatedSecondActive += OnSecondUpdate;
			translation = new ModTranslationAdd( 0 );
			this.location = location;
			Constraints.Set(
				new ConstraintBundle(
					new ModTranslationSet( location ),
					translation = new ModTranslationAdd( 0 ),
					new ModScalingSet( scale ),
					color = new ModColorSet( 1 )
				)
			);
			Enter += OnEnter;
			Leave += OnLeave;
			Deactivated += OnDeactivation;
			TrackCollision = true;

			label = new TextLabel( Mem.Font.Get( "default" ) );
			label.Attributes.HorizontalAlignment = HorizontalAlignment.CENTER;
			label.Attributes.VerticalAlignment = VerticalAlignment.CENTER;
			label.Attributes.MaxLength = scale.X * 2 / scale.Y;
			label.SetParent( this );
		}

		private void OnDeactivation() {
			color.Color = 1;
		}

		public void SetText( string text, Vector4 color ) {
			label.TextData.Set(text);
			label.TextData.SetColor( ( color * 255 ).IntRounded, 0, text.Length );
		}

		private void OnEnter() {
			color.Color = (.7f, .7f, .7f, 1);
		}

		private void OnLeave() {
			color.Color = 1;
		}

		private void OnSecondUpdate( MouseInputEventData data ) {
			/*float rotation = TransformInterface.GlobalRotation;
			Vector2 rot = ((float) Math.Cos( rotation ), (float) Math.Sin( rotation ));
			Vector2 loc = location;
			if( !( Parent is null ) )
				loc = Vector2.Transform( location, Parent.TransformInterface.Matrix );
			float len = ( data.PositionNDCA - ( loc - rot * 0.5f ) ).Length;
			float dot = 0;
			if( len > 0 )
				dot = Math.Max( Vector2.Dot( rot, ( data.PositionNDCA - ( loc - rot * 0.5f ) ) / len ) * 0.5f, 0 );
			translation.Translation = rot * dot;*/
			translation.Translation = 0;
		}
	}
}
