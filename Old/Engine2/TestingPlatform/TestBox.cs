using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestingPlatform {
	class TestBox : TextureDisplay {

		public ModTranslationAdd translation;

		public TestBox( Texture t, Vector2 baseTranslation, Vector2 scale ) : base( t ) {
			Constraints.Set( new ConstraintBundle( new ModScalingSet( scale ), new ModTranslationSet( baseTranslation ), new ModColorSet(new Vector4(0.5f, 1f)), translation = new ModTranslationAdd( 0 ) ), 0 );
			UpdatedThirdActive += UpdatedPre;
			UpdatedFourthActive += UpdatedPost;
		}

		private void UpdatedPost( MouseInputEventData data ) {
			//var v = CheckCollisionToMouse( false );
			//CollisionResult<Vector2>. pair = CollisionData<Vector2>.GetDeepest( v );
			//Vector2 l = ( pair.B - pair.A );
			//if( l.X == l.Y && l.Y == 0 ) {
			//	translation.Translation = 0;
			//} else
			//	translation.Translation = l * 0.2f * -Math.Abs( Vector2.Dot( l.Normalized, new Vector2( 1, 5 ).Normalized ) );
			//UpdateTransform();
			//if( v.Collision == CollisionMode.COLLIDING )
			//	Data.Color = new Vector4( 0, 1 );
		}

		private void UpdatedPre( MouseInputEventData mouseInputEventData ) {
			translation.Translation = 0;
		}
	}
}
