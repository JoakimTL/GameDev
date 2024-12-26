using Engine.GLFrameWork;
using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.LinearAlgebra;
using Engine.MemLib;
using OpenGL;
using System;

namespace TicTacToe {
	internal class TicTile : UIElement {

		private TicTacToe client;

		private ModTranslationSet translation;
		private ModScalingSet scale;
		private ModColorSet color;

		private ClaimDisplay claimDisplay;

		private bool clicked;

		public Player Claimant { get; private set; }

		public TicTile( TicTacToe client, int x, int y ) {
			this.client = client;

			//Setting up the tic tac toe tile so that it renders
			ShaderBundle = Mem.ShaderBundles.UI;
			Material = new Material( "ticTile" ).AddTexture( TextureUnit.Texture0, Mem.Textures.BlankWhite );
			Mesh = Mem.Mesh2.Square;
			Mem.CollisionMolds.SquareUniform.MoldNew( Data.CollisionModel );

			//Placing the tile and coloring it however we want
			translation = new ModTranslationSet( new Vector2( x, y ) / TicTacToe.BOARDSIZE * 2 - 1 + 1f / TicTacToe.BOARDSIZE );
			scale = new ModScalingSet( 0.95f / TicTacToe.BOARDSIZE );
			color = new ModColorSet( Vector4.One );
			Constraints.Set( new ConstraintBundle( translation, scale, color ) );

			//Setting up user events, such as pressing a tile to capture it
			MouseButtonPressed += MousePress;
			MouseButtonReleased += MouseRelease;

			claimDisplay = new ClaimDisplay( null );
			claimDisplay.SetParent( this );
			Claimant = null;
		}

		internal void Claim( Player player ) {
			Claimant = player;
			color.Color = new Vector4( player.Color, 1 );
			if( claimDisplay.Mesh is null )
				claimDisplay.Mesh = player.ClaimantMesh;
			else
				claimDisplay.Mesh = null;//crashes.
		}

		private void MousePress( IntPtr winPtr, uint id, MouseButton button, MouseInputEventData data, ref bool triggered ) {
			if( button == MouseButton.Left ) {
				if( CheckCollisionToMouse().Colliding ) {
					clicked = true;
				}
			}
		}

		private void MouseRelease( IntPtr winPtr, uint id, MouseButton button, MouseInputEventData data, ref bool triggered ) {
			if( button == MouseButton.Left ) {
				if( clicked && CheckCollisionToMouse().Colliding ) {
					client.TrySelectTile( this );
				}
				clicked = false;
			}
		}

	}
}