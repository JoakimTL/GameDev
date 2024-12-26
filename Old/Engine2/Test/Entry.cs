using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual;
using Engine.MemLib;
using Engine.QuickstartKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test {
	class Entry : Engine.QuickstartKit.QuickstartClientRender {

		TextureDisplay texDisp;
		Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints.ModRotationAdd rot;
		Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints.ModColorSet col;

		public Entry() : base( new QSWinData( "Test", (800, 600) ) { vsyncLevel = 2 }, null, new QS2Data() ) {
		}

		public override void EventsPolled() {

		}

		protected override void Exit() {

		}

		protected override void Initialization() {
			Render2.UIManager.Add( texDisp = new TextureDisplay( Mem.Textures.BlankWhite ), true );
			texDisp.Constraints.Set( new Engine.Graphics.Objects.Default.SceneObjects.UIs.ConstraintBundle( rot = new Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints.ModRotationAdd( 0 ), col = new Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints.ModColorSet( (1, 0, 0, 1) ) ) );
			Mem.CollisionMolds.SquareUniform.MoldNew( texDisp.Data.CollisionModel );
			texDisp.UpdatedSecondActive += Test2;
		}

		private void Test2( MouseInputEventData data ) {
			texDisp.CheckCollisionToMouse();
			col.Color = (1, 0, 0, 1);
			if( texDisp.CollisionResult.Colliding )
				col.Color = (0, 1, 1, 1);
		}

		protected override void InLoop() {

			rot.Rotation += 0.01f;

			this.StandardPipeline.Exectute();
		}
	}
}
